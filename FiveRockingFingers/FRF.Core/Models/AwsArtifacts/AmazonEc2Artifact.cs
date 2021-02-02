using FRF.Core.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FRF.Core.Models.AwsArtifacts
{
    public class AmazonEc2Artifact : Artifact
    {
        public const string ServiceCode = "AmazonEC2";
        public string OperatingSystem { get; set; }
        public string Vcpu { get; set; }
        public string Memory { get; set; }
        public string NetworkPerformance { get; set; }
        public string TermType { get; set; }
        public string InstanceType { get; set; }
        public string CurrentGeneration { get; set; }
        public string PurchaseOption { get; set; }
        public string Tenancy { get; set; }
        public string Location { get; set; }
        public decimal PricePerHour { get; set; }
        public decimal Hours { get; set; }
        public PricingDimension Range0 { get; set; }
        public PricingDimension Range1 { get; set; }
        public string LeaseContractLength { get; set; }
        public decimal GetPrice()
        {
            var price = -1m;

            if(PurchaseOption.Equals("All Upfront") || PurchaseOption.Equals("AllUpfront") &&
                LeaseContractLength.Equals("1yr") || LeaseContractLength.Equals("1 yr"))
            {
                price = GetPriceAllUpfront1Yr();
            }

            if (PurchaseOption.Equals("All Upfront") || PurchaseOption.Equals("AllUpfront") &&
                LeaseContractLength.Equals("3yr") || LeaseContractLength.Equals("3 yr"))
            {
                price = GetPriceAllUpfront3Yr();
            }

            if (PurchaseOption.Equals("Partial Upfront") || PurchaseOption.Equals("PartialUpfront") &&
                LeaseContractLength.Equals("1yr") || LeaseContractLength.Equals("1 yr"))
            {
                price = GetPricePartialUpfront1Yr();
            }

            if (PurchaseOption.Equals("Partial Upfront") || PurchaseOption.Equals("PartialUpfront") &&
                LeaseContractLength.Equals("3yr") || LeaseContractLength.Equals("3 yr"))
            {
                price = GetPricePartialUpfront3Yr();
            }

            if (PurchaseOption.Equals("No Upfront") || PurchaseOption.Equals("NoUpfront"))
            {
                price = GetPriceNoUpfront();
            }

            return price;
        }

        private decimal GetPriceAllUpfront1Yr()
        {
            return (decimal)(Range1.PricePerUnit / 12);
        }

        private decimal GetPriceAllUpfront3Yr()
        {
            return (decimal)(Range1.PricePerUnit / 36);
        }

        private decimal GetPricePartialUpfront1Yr()
        {
            return (decimal)(Range0.PricePerUnit * 730 + Range1.PricePerUnit / 12);
        }

        private decimal GetPricePartialUpfront3Yr()
        {
            return (decimal)(Range0.PricePerUnit * 730 + Range1.PricePerUnit / 36);
        }

        private decimal GetPriceNoUpfront()
        {
            return (decimal)(Range0.PricePerUnit * 730);
        }
    }
}
