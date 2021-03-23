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
            if (Settings.Element("product0") == null) return 0;

            doc.LoadXml(Settings.Element("product0").Element("pricingDimensions").ToString());
            var priceDimensionsJson = JsonConvert.SerializeXmlNode(doc);
            var pricingDimensionsJToken = JObject.Parse(priceDimensionsJson).SelectToken("pricingDimensions").ToObject<JObject>().Properties().ToList(); ;
            foreach (var jtoken in pricingDimensionsJToken)
            {
                var name = jtoken.Name;
                var a = jtoken.First.SelectToken("pricePerUnit").Value<string>();
                if (!string.IsNullOrWhiteSpace(a))
                {
                    var price = jtoken.Value.ToObject<PricingDimension>();
                    PricingDimensions.Add(name, price);
                }
                else return 0;
            }
          
            if(InfrequentAccessMultiplier != 0 && InfrequentAccessMultiplier != null)
            {
                WriteRequestsPrice = GetDecimalPrice("product1");
                RetrieveRequestsPrice = GetDecimalPrice("product2");
                InfrequentAccessStoragePrice = GetDecimalPrice("product3");
                return GetIntelligentPrice();
            }
            else
            {
                WriteRequestsPrice = GetDecimalPrice("product1");
                RetrieveRequestsPrice = GetDecimalPrice("product2");
                return GetStandardPrice();
            }
        }

        private decimal GetDecimalPrice(string product)
        {
            if (product == null) return -1;

            var pricePerUnit = Settings.Element($"{product}").Element("pricingDimensions").Element("range0")
                .Element("pricePerUnit")
                .Value;
            if (decimal.TryParse(pricePerUnit, NumberStyles.AllowExponent, CultureInfo.InvariantCulture,
                out decimal decimalPrice)) return decimalPrice;

            if (decimal.TryParse(pricePerUnit, NumberStyles.Currency, CultureInfo.InvariantCulture, out decimalPrice))
                return decimalPrice;

            return -1;
        }

        private decimal GetStandardPrice()
        {
            var endRange1 = (decimal) PricingDimensions["range0"].EndRange;
            var endRange2 = (decimal) PricingDimensions["range2"].EndRange;
            if (endRange1 == 0 || endRange2 == 0) return 0;

            decimal storageCost;
            if (StorageUsed <= endRange1)
                storageCost = PricingDimensions["range0"].PricePerUnit * StorageUsed;

            else if (StorageUsed >= endRange1 && StorageUsed <= endRange2)
                storageCost = PricingDimensions["range2"].PricePerUnit * StorageUsed;
            else
                storageCost = PricingDimensions["range1"].PricePerUnit * StorageUsed;

            var writeRequestCost = WriteRequestsPrice * WriteRequestsUsed;
            var retrieveRequestCost = RetrieveRequestsPrice * RetrieveRequestsUsed;
            var totalCost = storageCost + writeRequestCost + retrieveRequestCost;
            return totalCost;
        }

        private decimal GetIntelligentPrice()
        {
            var endRange0 = (decimal)PricingDimensions["range0"].EndRange;
            var endRange2 = (decimal)PricingDimensions["range2"].EndRange;

            if (endRange0 == 0 || endRange2 == 0) return 0;

            decimal storageFrequentAccessCost;
            const decimal InfrequentAccessPrice = 0.0125m;

            var storageInfrequentAccessMultiplier = (decimal) InfrequentAccessMultiplier / 100;
            var storageFrequentAccessMultiplier = 1 - storageInfrequentAccessMultiplier;
            var storageFrequentAccessUsed = StorageUsed * storageFrequentAccessMultiplier;
            if (storageFrequentAccessUsed <= endRange0)
                storageFrequentAccessCost = storageFrequentAccessUsed *
                                            PricingDimensions["range0"].PricePerUnit;
            else if (storageFrequentAccessUsed >= endRange0 && storageFrequentAccessUsed <= endRange2)
                storageFrequentAccessCost = storageFrequentAccessUsed *
                                            PricingDimensions["range2"].PricePerUnit;
            else
                storageFrequentAccessCost = storageFrequentAccessUsed *
                                            PricingDimensions["range1"].PricePerUnit;

            var storageInfrequentAccessCost = storageInfrequentAccessMultiplier * StorageUsed * InfrequentAccessPrice;
            var writeRequestsCost = WriteRequestsUsed * WriteRequestsPrice;
            var retrieveRequestsCost = RetrieveRequestsUsed * RetrieveRequestsPrice;
            
            var totalCost = storageFrequentAccessCost + storageInfrequentAccessCost + writeRequestsCost + retrieveRequestsCost;
            return totalCost;
        }
    }
}