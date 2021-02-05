using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace FRF.Core.Models.AwsArtifacts
{
    public class AwsS3 : Artifact
    {
        public decimal StoragePrice { get; set; }
        public int StorageUsed { get; set; }
        public decimal WriteRequestsPrice { get; set; }
        public int WriteRequestsUsed { get; set; }
        public decimal RetrieveRequestsPrice { get; set; }
        public int RetrieveRequestsUsed { get; set; }
        public int? InfrequentAccessMultiplier { get; set; }
        public Dictionary<string,PricingDimension> PricingDimensions { get; private set; }

        public override decimal GetPrice()
        {
            PricingDimensions = new Dictionary<string, PricingDimension>();
            var doc = new XmlDocument();
            if (Settings.Element("product1") == null) return 0;
            doc.LoadXml(Settings.Element("product0").Element("pricingDimension").ToString());
            var priceDimensionsJson = JsonConvert.SerializeXmlNode(doc);
            var pricingDimensionsJToken = JObject.Parse(priceDimensionsJson).SelectToken("pricingDimension").ToObject<JObject>().Properties().ToList(); ;
            foreach (var jtoken in pricingDimensionsJToken)
            {
                var name = jtoken.Name;
                var price = jtoken.Value.ToObject<PricingDimension>();
                PricingDimensions.Add(name,price);
            }
          
            return InfrequentAccessMultiplier != 0 && InfrequentAccessMultiplier != null ?
                GetIntelligentPrice() : GetStandardPrice();
        }

        private decimal GetStandardPrice()
        {
            var storageCost = StoragePrice * StorageUsed;
            var writeRequestCost = WriteRequestsPrice * WriteRequestsUsed;
            var retrieveRequestCost = RetrieveRequestsPrice * RetrieveRequestsUsed;
            var totalCost = storageCost + writeRequestCost + retrieveRequestCost;
            return totalCost;
        }

        private decimal GetIntelligentPrice()
        {
            var endRange1 = (decimal)PricingDimensions["range0"].EndRange;
            var endRange2 = (decimal)PricingDimensions["range2"].EndRange;
            var beginRangeLast = (decimal)PricingDimensions["range1"].BeginRange;

            if (endRange1 == 0 || endRange2 == 0 || beginRangeLast == 0) return 0;

            decimal storageFrequentAccessCost;
            const decimal InfrequentAccessPrice = 0.0125m;

            var storageInfrequentAccessMultiplier = (decimal) InfrequentAccessMultiplier / 100;
            var storageFrequentAccessMultiplier = 1 - storageInfrequentAccessMultiplier;
            var storageFrequentAccessUsed = StorageUsed * storageFrequentAccessMultiplier;
            if (storageFrequentAccessUsed <= endRange1)
                storageFrequentAccessCost = storageFrequentAccessUsed *
                                            PricingDimensions["range2"].PricePerUnit;
            else if (storageFrequentAccessUsed >= endRange2 && storageFrequentAccessUsed <= beginRangeLast)
                storageFrequentAccessCost = storageFrequentAccessUsed *
                                            PricingDimensions["range1"].PricePerUnit;
            else
                storageFrequentAccessCost = storageFrequentAccessUsed *
                                            PricingDimensions["range0"].PricePerUnit;

            var storageInfrequentAccessCost = storageInfrequentAccessMultiplier * StorageUsed * InfrequentAccessPrice;
            var writeRequestsCost = WriteRequestsUsed * WriteRequestsPrice;
            var retrieveRequestsCost = RetrieveRequestsUsed * RetrieveRequestsPrice;
            return storageFrequentAccessCost + storageInfrequentAccessCost + writeRequestsCost + retrieveRequestsCost;
        }
    }
}