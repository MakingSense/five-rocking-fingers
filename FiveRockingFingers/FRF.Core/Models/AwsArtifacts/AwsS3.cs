using Newtonsoft.Json;
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
        public PricingDimensionPocos PricingDimensions { get; private set; }

        public override decimal GetPrice()
        {
            var doc = new XmlDocument();
            if (Settings.Element("product1") == null) return 0;
            
            doc.LoadXml(Settings.Element("product1").Element("pricingDimensions").ToString());
            var priceDimensionsJson = JsonConvert.SerializeXmlNode(doc);
            PricingDimensions = JsonConvert.DeserializeObject<PricingDimensionPocos>(priceDimensionsJson);

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
            var endRange1 = PricingDimensions.PricingDimensions["range1"].EndRange;
            var endRange2 = PricingDimensions.PricingDimensions["range2"].EndRange;
            var beginRangeLast = PricingDimensions.PricingDimensions["range0"].BeginRange;
            
            if (endRange1 == 0 || endRange2 == 0 || beginRangeLast == 0) return 0;

            decimal storageFrequentAccessCost;
            const decimal InfrequentAccessPrice = 0.0125m;

            var storageInfrequentAccessMultiplier = (int) InfrequentAccessMultiplier / 100;
            var storageFrequentAccessMultiplier = 1 - storageInfrequentAccessMultiplier;
            var storageFrequentAccessUsed = StorageUsed * storageFrequentAccessMultiplier;
            if (storageFrequentAccessUsed <= endRange1)
                storageFrequentAccessCost = storageFrequentAccessUsed *
                                            PricingDimensions.PricingDimensions["range2"].PricePerUnit;
            else if (storageFrequentAccessUsed >= endRange2 && storageFrequentAccessUsed <= beginRangeLast)
                storageFrequentAccessCost = storageFrequentAccessUsed *
                                            PricingDimensions.PricingDimensions["range1"].PricePerUnit;
            else
                storageFrequentAccessCost = storageFrequentAccessUsed *
                                            PricingDimensions.PricingDimensions["range0"].PricePerUnit;

            var storageInfrequentAccessCost = storageInfrequentAccessMultiplier * StorageUsed * InfrequentAccessPrice;
            var writeRequestsCost = WriteRequestsUsed * WriteRequestsPrice;
            var retrieveRequestsCost = RetrieveRequestsUsed * RetrieveRequestsPrice;
            return storageFrequentAccessCost + storageInfrequentAccessCost + writeRequestsCost + retrieveRequestsCost;
        }
    }
}