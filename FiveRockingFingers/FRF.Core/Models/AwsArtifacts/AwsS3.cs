using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace FRF.Core.Models.AwsArtifacts
{
    public class AwsS3 : Artifact
    {
        public int StorageUsed { get; set; }
        public decimal WriteRequestsPrice { get; set; }
        public int WriteRequestsUsed { get; set; }
        public decimal RetrieveRequestsPrice { get; set; }
        public int RetrieveRequestsUsed { get; set; }
        public int? InfrequentAccessMultiplier { get; set; }
        public decimal InfrequentAccessStoragePrice { get; set; }
        public Dictionary<string, PricingDimension> PricingDimensions { get; private set; }

        public AwsS3()
        {
            RelationalFields = new Dictionary<string, string>();
            RelationalFields.Add("writeRequestsUsed", SettingTypes.NaturalNumber);
            RelationalFields.Add("retrieveRequestsUsed", SettingTypes.NaturalNumber);
            RelationalFields.Add("storageUsed", SettingTypes.Decimal);
            RelationalFields.Add("infrequentAccessMultiplier", SettingTypes.Decimal);
        }

        public override decimal GetPrice()
        {
            PricingDimensions = new Dictionary<string, PricingDimension>();
            var doc = new XmlDocument();
            if (Settings.Element(AwsS3Descriptions.Product0) == null) return 0;

            doc.LoadXml(Settings.Element(AwsS3Descriptions.Product0).Element(AwsS3Descriptions.PricingDimensions).ToString());
            var priceDimensionsJson = JsonConvert.SerializeXmlNode(doc);
            var pricingDimensionsJToken = JObject.Parse(priceDimensionsJson).SelectToken(AwsS3Descriptions.PricingDimensions).ToObject<JObject>().Properties().ToList();
            foreach (var jtoken in pricingDimensionsJToken)
            {
                var name = jtoken.Name;
                var a = jtoken.First.SelectToken(AwsS3Descriptions.PricePerUnit).Value<string>();
                if (!string.IsNullOrWhiteSpace(a))
                {
                    var price = jtoken.Value.ToObject<PricingDimension>();
                    PricingDimensions.Add(name, price);
                }
                else return 0;
            }
          
            if(InfrequentAccessMultiplier != 0 && InfrequentAccessMultiplier != null)
            {
                WriteRequestsPrice = GetDecimalPrice(AwsS3Descriptions.Product1);
                RetrieveRequestsPrice = GetDecimalPrice(AwsS3Descriptions.Product2);
                InfrequentAccessStoragePrice = GetDecimalPrice(AwsS3Descriptions.Product3);
                return GetIntelligentPrice();
            }
            else
            {
                WriteRequestsPrice = GetDecimalPrice(AwsS3Descriptions.Product1);
                RetrieveRequestsPrice = GetDecimalPrice(AwsS3Descriptions.Product2);
                return GetStandardPrice();
            }
        }

        private decimal GetDecimalPrice(string product)
        {
            if (product == null) return -1;

            var pricePerUnit = Settings.Element($"{product}").Element(AwsS3Descriptions.PricingDimensions).Element(AwsS3Descriptions.Range0)
                .Element(AwsS3Descriptions.PricePerUnit)
                .Value;
            if (decimal.TryParse(pricePerUnit, NumberStyles.AllowExponent, CultureInfo.InvariantCulture,
                out decimal decimalPrice)) return decimalPrice;

            if (decimal.TryParse(pricePerUnit, NumberStyles.Currency, CultureInfo.InvariantCulture, out decimalPrice))
                return decimalPrice;

            return -1;
        }

        private decimal GetStandardPrice()
        {
            var endRange2 = 0m;
            var endRange1 = (decimal) PricingDimensions[AwsS3Descriptions.Range0].EndRange;
            var isEndRange2 = PricingDimensions.ContainsKey(AwsS3Descriptions.Range2);
            if (isEndRange2)
                endRange2 = (decimal) PricingDimensions[AwsS3Descriptions.Range2].EndRange;

            if (endRange1 == 0) return 0;

            decimal storageCost;
            if (StorageUsed <= endRange1 || endRange1 == -1)
                storageCost = PricingDimensions[AwsS3Descriptions.Range0].PricePerUnit * StorageUsed;

            else if (StorageUsed >= endRange1 && StorageUsed <= endRange2)
                storageCost = PricingDimensions[AwsS3Descriptions.Range2].PricePerUnit * StorageUsed;
            else
                storageCost = PricingDimensions[AwsS3Descriptions.Range1].PricePerUnit * StorageUsed;

            var writeRequestCost = WriteRequestsPrice * WriteRequestsUsed;
            var retrieveRequestCost = RetrieveRequestsPrice * RetrieveRequestsUsed;
            var totalCost = storageCost + writeRequestCost + retrieveRequestCost;
            return totalCost;
        }

        private decimal GetIntelligentPrice()
        {
            var endRange0 = (decimal)PricingDimensions[AwsS3Descriptions.Range0].EndRange;
            var endRange2 = (decimal)PricingDimensions[AwsS3Descriptions.Range2].EndRange;

            if (endRange0 == 0 || endRange2 == 0) return 0;

            decimal storageFrequentAccessCost;
            const decimal InfrequentAccessPrice = 0.0125m;

            var storageInfrequentAccessMultiplier = (decimal) InfrequentAccessMultiplier / 100;
            var storageFrequentAccessMultiplier = 1 - storageInfrequentAccessMultiplier;
            var storageFrequentAccessUsed = StorageUsed * storageFrequentAccessMultiplier;
            if (storageFrequentAccessUsed <= endRange0)
                storageFrequentAccessCost = storageFrequentAccessUsed *
                                            PricingDimensions[AwsS3Descriptions.Range0].PricePerUnit;
            else if (storageFrequentAccessUsed >= endRange0 && storageFrequentAccessUsed <= endRange2)
                storageFrequentAccessCost = storageFrequentAccessUsed *
                                            PricingDimensions[AwsS3Descriptions.Range2].PricePerUnit;
            else
                storageFrequentAccessCost = storageFrequentAccessUsed *
                                            PricingDimensions[AwsS3Descriptions.Range1].PricePerUnit;

            var storageInfrequentAccessCost = storageInfrequentAccessMultiplier * StorageUsed * InfrequentAccessPrice;
            var writeRequestsCost = WriteRequestsUsed * WriteRequestsPrice;
            var retrieveRequestsCost = RetrieveRequestsUsed * RetrieveRequestsPrice;
            
            var totalCost = storageFrequentAccessCost + storageInfrequentAccessCost + writeRequestsCost + retrieveRequestsCost;
            return totalCost;
        }
    }
}