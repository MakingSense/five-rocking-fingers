using FRF.Core.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FRF.Core.Models.AwsArtifacts
{
    public class AmazonEc2Artifact : Artifact
    {
        public string PurchaseOption { get; set; }
        public decimal PricePerUnit0 { get; set; }
        public decimal PricePerUnit1 { get; set; }
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
            return PricePerUnit1 / 12;
        }

        private decimal GetPriceAllUpfront3Yr()
        {
            return PricePerUnit1 / 36;
        }

        private decimal GetPricePartialUpfront1Yr()
        {
            return PricePerUnit0 * 730 + PricePerUnit1 / 12;
        }

        private decimal GetPricePartialUpfront3Yr()
        {
            return PricePerUnit0 * 730 + PricePerUnit1 / 36;
        }

        private decimal GetPriceNoUpfront()
        {
            return PricePerUnit0 * 730;
        }
    }
}
