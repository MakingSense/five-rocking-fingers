using System.Globalization;

namespace FRF.Core.Models.AwsArtifacts
{
    public class AwsS3Artifact : Artifact
    {
        public decimal StoragePrice { get; set; }
        public int StorageUsed { get; set; }
        public decimal WriteRequestsPrice { get; set; }
        public int WriteRequestsUsed { get; set; }
        public decimal RetrieveRequestsPrice { get; set; }
        public int RetrieveRequestsUsed { get; set; }
        public int? InfrequentAccessMultiplier { get; set; }

        public override decimal GetPrice()
        {
            return InfrequentAccessMultiplier != 0 && InfrequentAccessMultiplier != null ? GetIntelligentPrice() : GetStandardPrice();
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
            if (!int.TryParse(
                    Settings.Element("product1").Element("pricingDimensions").Element("range2").Element("endRange")
                        .Value,
                    out var endRange1) ||
                !int.TryParse(
                    Settings.Element("product1").Element("pricingDimensions").Element("range1").Element("endRange")
                        .Value,
                    out var endRange2) ||
                !int.TryParse(
                    Settings.Element("product1").Element("pricingDimensions").Element("range0").Element("beginRange")
                        .Value,
                    out var beginRangeLast)) return 0;

            decimal storageFrequentAccessCost;
            const decimal InfrequentAccessPrice = 0.0125m;

            var storageInfrequentAccessMultiplier = (int) InfrequentAccessMultiplier / 100;
            var storageFrequentAccessMultiplier = 1 - storageInfrequentAccessMultiplier;
            var storageFrequentAccessUsed = StorageUsed * storageFrequentAccessMultiplier;
            if (storageFrequentAccessUsed <= endRange1)
            {
                storageFrequentAccessCost = storageFrequentAccessUsed * decimal.Parse(
                    Settings.Element("product1").Element("pricingDimensions").Element("range2").Element("pricePerUnit")
                        .Value, CultureInfo.InvariantCulture);
            }
            else if (storageFrequentAccessUsed >= endRange2 && storageFrequentAccessUsed <= beginRangeLast)
            {
                storageFrequentAccessCost = storageFrequentAccessUsed * decimal.Parse(
                    Settings.Element("product1").Element("pricingDimensions").Element("range1").Element("pricePerUnit")
                        .Value, CultureInfo.InvariantCulture);
            }
            else
            {
                storageFrequentAccessCost = storageFrequentAccessUsed * decimal.Parse(
                    Settings.Element("product1").Element("pricingDimensions").Element("range0").Element("pricePerUnit")
                        .Value, CultureInfo.InvariantCulture);
            }

            var storageInfrequentAccessCost = storageInfrequentAccessMultiplier * StorageUsed * InfrequentAccessPrice;
            var writeRequestsCost = WriteRequestsUsed * WriteRequestsPrice;
            var retrieveRequestsCost = RetrieveRequestsUsed * RetrieveRequestsPrice;
            return storageFrequentAccessCost + storageInfrequentAccessCost + writeRequestsCost + retrieveRequestsCost;
        }
    }
}