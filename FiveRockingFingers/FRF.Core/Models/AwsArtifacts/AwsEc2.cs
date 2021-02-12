using FRF.Core.Response;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace FRF.Core.Models.AwsArtifacts
{
    public class AwsEc2 : Artifact
    {
        public const decimal PartialStorageDiscount = 0.5m;
        public const int PriceError = -1;

        //Compute instance properties
        public int HoursUsedPerMonth { get; set; }
        public string PurchaseOption { get; set; }
        public decimal InstancePricePerUnit0 { get; set; }
        public decimal InstancePricePerUnit1 { get; set; }
        public string LeaseContractLength { get; set; }

        //EBS Storage properties
        public string VolumenApiName { get; set; }

        public int NumberOfGbStorageInEbs { get; set; }
        public decimal EbsPricePerUnit { get; set; }

        //EBS Snapshots properties
        public int NumberOfGbChangedPerSnapshot {get; set; }
        public decimal NumberOfSnapshotsPerMonth { get; set; }
        public decimal SnapshotPricePerUnit { get; set; }

        //EBS IOPS properties
        public int NumberOfIops { get; set; }
        public decimal IopsPricePerUnit { get; set; }

        //EBS Throughput properties
        public int NumberOfMbpsThroughput { get; set; }
        public decimal ThroughputPricePerUnit { get; set; }

        //Data Transfer properties
        public int NumberOfGbTransferIntraRegion { get; set; }
        public decimal IntraTegionDataTransferPricePerUnit { get; set; }

        override public decimal GetPrice()
        {
            GetAllProperties();

            if(!ArePricesCorrect())
            {
                return PriceError;
            }

            var totalPrice = GetInstancePrice() + GetEbsPrice() + GetSnapshotsPrice()
                + GetIntraRegionDataTransferPrice() + GetIopsPrice() + GetThroughputPrice();

            return totalPrice;
        }

        private decimal GetIntraRegionDataTransferPrice()
        {
            var intraRegionDataTransferPrice = 2 * NumberOfGbTransferIntraRegion * IntraTegionDataTransferPricePerUnit;

            return intraRegionDataTransferPrice;
        }

        private decimal GetIopsPrice()
        {
            var price = 0m;

            if (VolumenApiName.Equals(AwsEc2Descriptions.VolumenApiNameGp3Value))
            {
                var numberOfIopsBillable = NumberOfIops - AwsEc2Descriptions.FreeTierGp3Iops;

                if(numberOfIopsBillable > 0)
                {
                    price = numberOfIopsBillable * IopsPricePerUnit;
                }

                return price;
            }

            if(VolumenApiName.Equals(AwsEc2Descriptions.VolumenApiNameIo1Value) || VolumenApiName.Equals(AwsEc2Descriptions.VolumenApiNameIo2Value))
            {
                price = NumberOfIops * IopsPricePerUnit;

                return price;
            }

            return price;
        }

        private decimal GetThroughputPrice()
        {
            var price = 0m;

            if (VolumenApiName.Equals(AwsEc2Descriptions.VolumenApiNameGp3Value))
            {
                var numberOfMbpsThroughputBillable = NumberOfMbpsThroughput - AwsEc2Descriptions.FreeTierGp3Throughput;

                var numberOfGbpsThroughputBillable = numberOfMbpsThroughputBillable / 1024;

                if (numberOfMbpsThroughputBillable > 0)
                {
                    price = numberOfGbpsThroughputBillable * ThroughputPricePerUnit;
                }

                return price;
            }

            if (VolumenApiName.Equals(AwsEc2Descriptions.VolumenApiNameIo1Value) || VolumenApiName.Equals(AwsEc2Descriptions.VolumenApiNameIo2Value))
            {
                price = NumberOfMbpsThroughput * ThroughputPricePerUnit;

                return price;
            }

            return price;
        }

        private decimal GetSnapshotsPrice()
        {
            var initialSnapshotPrice = NumberOfGbStorageInEbs * SnapshotPricePerUnit;

            var incrementaSnapshotsPrice = NumberOfSnapshotsPerMonth * SnapshotPricePerUnit * PartialStorageDiscount * NumberOfGbChangedPerSnapshot;

            var totalShanpshotPrice = initialSnapshotPrice + incrementaSnapshotsPrice;

            return totalShanpshotPrice;
        }

        private decimal GetEbsPrice()
        {
            return NumberOfGbStorageInEbs * EbsPricePerUnit;
        }

        private decimal GetInstancePrice()
        {
            var price = 0m;

            if ((PurchaseOption.Equals("All Upfront", StringComparison.InvariantCultureIgnoreCase) ||
                PurchaseOption.Equals("AllUpfront", StringComparison.InvariantCultureIgnoreCase)) &&
                (LeaseContractLength.Equals("1yr", StringComparison.InvariantCultureIgnoreCase) ||
                LeaseContractLength.Equals("1 yr", StringComparison.InvariantCultureIgnoreCase)))
            {
                price = GetPriceAllUpfront1Year();
            }

            if ((PurchaseOption.Equals("All Upfront", StringComparison.InvariantCultureIgnoreCase) ||
                PurchaseOption.Equals("AllUpfront", StringComparison.InvariantCultureIgnoreCase)) &&
                (LeaseContractLength.Equals("3yr", StringComparison.InvariantCultureIgnoreCase) ||
                LeaseContractLength.Equals("3 yr", StringComparison.InvariantCultureIgnoreCase)))
            {
                price = GetPriceAllUpfront3Year();
            }

            if ((PurchaseOption.Equals("Partial Upfront", StringComparison.InvariantCultureIgnoreCase) ||
                PurchaseOption.Equals("PartialUpfront", StringComparison.InvariantCultureIgnoreCase)) &&
                (LeaseContractLength.Equals("1yr", StringComparison.InvariantCultureIgnoreCase) ||
                LeaseContractLength.Equals("1 yr", StringComparison.InvariantCultureIgnoreCase)))
            {
                price = GetPricePartialUpfront1Year();
            }

            if ((PurchaseOption.Equals("Partial Upfront", StringComparison.InvariantCultureIgnoreCase) ||
                PurchaseOption.Equals("PartialUpfront", StringComparison.InvariantCultureIgnoreCase)) &&
                (LeaseContractLength.Equals("3yr", StringComparison.InvariantCultureIgnoreCase) ||
                LeaseContractLength.Equals("3 yr", StringComparison.InvariantCultureIgnoreCase)))
            {
                price = GetPricePartialUpfront3Year();
            }

            if (PurchaseOption.Equals("No Upfront", StringComparison.InvariantCultureIgnoreCase) ||
                PurchaseOption.Equals("NoUpfront", StringComparison.InvariantCultureIgnoreCase))
            {
                price = GetPriceNoUpfront();
            }

            return price;
        }

        private decimal GetPriceAllUpfront1Year()
        {
            return InstancePricePerUnit1 / 12;
        }

        private decimal GetPriceAllUpfront3Year()
        {
            return InstancePricePerUnit1 / 36;
        }

        private decimal GetPricePartialUpfront1Year()
        {
            return InstancePricePerUnit0 * HoursUsedPerMonth + InstancePricePerUnit1 / 12;
        }

        private decimal GetPricePartialUpfront3Year()
        {
            return InstancePricePerUnit0 * HoursUsedPerMonth + InstancePricePerUnit1 / 36;
        }

        private decimal GetPriceNoUpfront()
        {
            return InstancePricePerUnit0 * HoursUsedPerMonth;
        }

        private void GetAllProperties()
        {
            GetComputeInstaceProperties();
            GetEbsProperties();
            GetEbsSnapshotsProperties();
            GetEbsIopsProperties();
            GetEbsThroughputProperties();
            GetDataTransferProperties();
        }

        private void GetComputeInstaceProperties()
        {
            HoursUsedPerMonth = int.Parse(Settings.Element("product0").Element("hoursUsedPerMonth").Value);

            InstancePricePerUnit0 = GetDecimalPrice(Settings.Element("product0")
                .Element("pricingDimensions")
                .Element("range0")
                .Element("pricePerUnit")
                .Value);
            
            InstancePricePerUnit1 = GetDecimalPrice(Settings.Element("product0")
                .Element("pricingDimensions").Element("range1")
                .Element("pricePerUnit")
                .Value);

            PurchaseOption = Settings.Element("product0").Element("purchaseOption").Value;
            
            LeaseContractLength = Settings.Element("product0").Element("leaseContractLength").Value;
        }

        private void GetEbsProperties()
        {
            VolumenApiName = Settings.Element("product1").Element("volumeApiName").Value;

            NumberOfGbStorageInEbs = int.Parse(Settings.Element("product1")
                .Element("numberOfGbStorageInEbs").Value);

            EbsPricePerUnit = GetDecimalPrice(Settings.Element("product1")
                .Element("pricingDimensions").Element("range0")
                .Element("pricePerUnit").Value);
        }

        private void GetEbsSnapshotsProperties()
        {
            NumberOfSnapshotsPerMonth = GetDecimalPrice(Settings.Element("product2").
                Element("numberOfSnapshotsPerMonth").Value);

            NumberOfGbChangedPerSnapshot = int.Parse(Settings.Element("product2").
                Element("numberOfGbChangedPerSnapshot").Value);

            SnapshotPricePerUnit = GetDecimalPrice(Settings.Element("product2")
                .Element("pricingDimensions")
                .Element("range0")
                .Element("pricePerUnit").Value);
        }

        private void GetEbsIopsProperties()
        {
            if(Settings.Element("product3") != null &&
                Settings.Element("product3")
                .Element("numberOfIopsPerMonth") != null)
            {
                if(int.TryParse(Settings.Element("product3")
                    .Element("numberOfIopsPerMonth").Value, out int numberOfIopsParse))
                {
                    NumberOfIops = numberOfIopsParse;
                }                
            }

            if(Settings.Element("product3") != null &&
                Settings.Element("product3")
                .Element("pricingDimensions")
                .Element("range0")
                .Element("pricePerUnit") != null)
            {
                IopsPricePerUnit = GetDecimalPrice(Settings.Element("product3")
                    .Element("pricingDimensions")
                    .Element("range0")
                    .Element("pricePerUnit").Value);
            }
        }

        private void GetEbsThroughputProperties()
        {
            if(Settings.Element("product4") != null &&
                Settings.Element("product4")
                .Element("numberOfMbpsThroughput") != null)
            {
                if(int.TryParse(Settings.Element("product4").Element("numberOfMbpsThroughput").Value, out int numberOfMbpsThroughputParse))
                {
                    NumberOfMbpsThroughput = numberOfMbpsThroughputParse;
                }                
            }

            if(Settings.Element("product4") != null &&
                Settings.Element("product4")
                .Element("pricingDimensions")
                .Element("range0")
                .Element("pricePerUnit") != null)
            {
                ThroughputPricePerUnit = GetDecimalPrice(Settings.Element("product4")
                    .Element("pricingDimensions")
                    .Element("range0")
                    .Element("pricePerUnit")
                    .Value);
            }
        }

        private void GetDataTransferProperties()
        {
            NumberOfGbTransferIntraRegion = int.Parse(Settings.Element("product5").Element("numberOfGbTransferIntraRegion").Value);
            IntraTegionDataTransferPricePerUnit = GetDecimalPrice(Settings.Element("product5")
                .Element("pricingDimensions")
                .Element("range0")
                .Element("pricePerUnit")
                .Value);
        }

        private bool ArePricesCorrect()
        {
            return IntraTegionDataTransferPricePerUnit != PriceError &&
                ThroughputPricePerUnit != PriceError &&
                IopsPricePerUnit != PriceError &&
                SnapshotPricePerUnit != PriceError &&
                EbsPricePerUnit != PriceError &&
                InstancePricePerUnit0 != PriceError &&
                InstancePricePerUnit1 != PriceError;
        }

        private decimal GetDecimalPrice(string pricePerUnit)
        {
            if (decimal.TryParse(pricePerUnit, NumberStyles.AllowExponent, CultureInfo.InvariantCulture,
                out decimal decimalPrice)) return decimalPrice;

            if (decimal.TryParse(pricePerUnit, NumberStyles.Currency, CultureInfo.InvariantCulture,
                out decimalPrice)) return decimalPrice;

            return -1;
        }
    }
}
