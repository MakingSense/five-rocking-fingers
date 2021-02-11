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

            if(VolumenApiName.Equals(AwsEc2Descriptions.VolumenApiNameGp3Value))
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
            var price = -1m;

            if((PurchaseOption.Equals("All Upfront", StringComparison.InvariantCultureIgnoreCase) ||
                PurchaseOption.Equals("AllUpfront", StringComparison.InvariantCultureIgnoreCase)) &&
                (LeaseContractLength.Equals("1yr", StringComparison.InvariantCultureIgnoreCase) ||
                LeaseContractLength.Equals("1 yr", StringComparison.InvariantCultureIgnoreCase)))
            {
                price = GetPriceAllUpfront1Yr();
            }

            if ((PurchaseOption.Equals("All Upfront", StringComparison.InvariantCultureIgnoreCase) ||
                PurchaseOption.Equals("AllUpfront", StringComparison.InvariantCultureIgnoreCase)) &&
                (LeaseContractLength.Equals("3yr", StringComparison.InvariantCultureIgnoreCase) ||
                LeaseContractLength.Equals("3 yr", StringComparison.InvariantCultureIgnoreCase)))
            {
                price = GetPriceAllUpfront3Yr();
            }

            if ((PurchaseOption.Equals("Partial Upfront", StringComparison.InvariantCultureIgnoreCase) ||
                PurchaseOption.Equals("PartialUpfront", StringComparison.InvariantCultureIgnoreCase)) &&
                (LeaseContractLength.Equals("1yr", StringComparison.InvariantCultureIgnoreCase) ||
                LeaseContractLength.Equals("1 yr", StringComparison.InvariantCultureIgnoreCase)))
            {
                price = GetPricePartialUpfront1Yr();
            }

            if ((PurchaseOption.Equals("Partial Upfront", StringComparison.InvariantCultureIgnoreCase) ||
                PurchaseOption.Equals("PartialUpfront", StringComparison.InvariantCultureIgnoreCase)) &&
                (LeaseContractLength.Equals("3yr", StringComparison.InvariantCultureIgnoreCase) ||
                LeaseContractLength.Equals("3 yr", StringComparison.InvariantCultureIgnoreCase)))
            {
                price = GetPricePartialUpfront3Yr();
            }

            if (PurchaseOption.Equals("No Upfront", StringComparison.InvariantCultureIgnoreCase) ||
                PurchaseOption.Equals("NoUpfront", StringComparison.InvariantCultureIgnoreCase))
            {
                price = GetPriceNoUpfront();
            }

            return price;
        }

        private decimal GetPriceAllUpfront1Yr()
        {
            return InstancePricePerUnit1 / 12;
        }

        private decimal GetPriceAllUpfront3Yr()
        {
            return InstancePricePerUnit1 / 36;
        }

        private decimal GetPricePartialUpfront1Yr()
        {
            return InstancePricePerUnit0 * HoursUsedPerMonth + InstancePricePerUnit1 / 12;
        }

        private decimal GetPricePartialUpfront3Yr()
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

            InstancePricePerUnit0 = decimal.Parse(Settings.Element("product0")
                .Element("pricingDimensions")
                .Element("range0")
                .Element("pricePerUnit")
                .Value, CultureInfo.InvariantCulture);
            
            InstancePricePerUnit1 = decimal.Parse(Settings.Element("product0")
                .Element("pricingDimensions").Element("range1")
                .Element("pricePerUnit")
                .Value, CultureInfo.InvariantCulture);
            
            PurchaseOption = Settings.Element("product0").Element("purchaseOption").Value;
            
            LeaseContractLength = Settings.Element("product0").Element("leaseContractLength").Value;
        }

        private void GetEbsProperties()
        {
            VolumenApiName = Settings.Element("product1").Element("volumeApiName").Value;

            NumberOfGbStorageInEbs = int.Parse(Settings.Element("product1")
                .Element("numberOfGbStorageInEbs").Value);

            EbsPricePerUnit = decimal.Parse(Settings.Element("product1")
                .Element("pricingDimensions").Element("range0")
                .Element("pricePerUnit").Value, CultureInfo.InvariantCulture);
        }

        private void GetEbsSnapshotsProperties()
        {
            NumberOfSnapshotsPerMonth = decimal.Parse(Settings.Element("product2").
                Element("numberOfSnapshotsPerMonth").Value, CultureInfo.InvariantCulture);

            NumberOfGbChangedPerSnapshot = int.Parse(Settings.Element("product2").
                Element("numberOfGbChangedPerSnapshot").Value);

            SnapshotPricePerUnit = decimal.Parse(Settings.Element("product2").
                Element("pricingDimensions").
                Element("range0")
                .Element("pricePerUnit").Value, CultureInfo.InvariantCulture);
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
                if(decimal.TryParse(Settings.Element("product3")
                    .Element("pricingDimensions")
                    .Element("range0")
                    .Element("pricePerUnit").Value, out decimal iopsPricePerUnitParse))
                {
                    IopsPricePerUnit = iopsPricePerUnitParse;
                }                
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
                if(decimal.TryParse(Settings.Element("product4")
                    .Element("pricingDimensions")
                    .Element("range0")
                    .Element("pricePerUnit")
                    .Value, out decimal throughputPricePerUnitParse))
                {
                    ThroughputPricePerUnit = throughputPricePerUnitParse;
                }                
            }
        }

        private void GetDataTransferProperties()
        {
            NumberOfGbTransferIntraRegion = int.Parse(Settings.Element("product5").Element("numberOfGbTransferIntraRegion").Value);
            IntraTegionDataTransferPricePerUnit = decimal.Parse(Settings.Element("product5")
                .Element("pricingDimensions")
                .Element("range0")
                .Element("pricePerUnit")
                .Value, CultureInfo.InvariantCulture);
        }
    }
}
