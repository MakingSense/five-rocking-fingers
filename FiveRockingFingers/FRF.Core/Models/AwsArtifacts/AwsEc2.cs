using FRF.Core.Response;
using System;
using System.Collections.Generic;
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
            var totalPrice = GetInstancePrice() + GetEbsPrice() + GetSnapshotsPrice()
                + GetIntraRegionDataTransferPrice() + GetIopsPrice() + GetThroughputPrice();

            return totalPrice;
        }

        public decimal GetIntraRegionDataTransferPrice()
        {
            var intraRegionDataTransferPrice = 2 * NumberOfGbTransferIntraRegion * IntraTegionDataTransferPricePerUnit;

            return intraRegionDataTransferPrice;
        }

        public decimal GetIopsPrice()
        {
            var price = 0m;

            if(VolumenApiName.Equals(AwsEc2Descriptions.VolumenApiNameGp3))
            {
                var numberOfIopsBillable = NumberOfIops - AwsEc2Descriptions.FreeTierGp3Iops;

                if(numberOfIopsBillable > 0)
                {
                    price = numberOfIopsBillable * IopsPricePerUnit;
                }

                return price;
            }

            if(VolumenApiName.Equals(AwsEc2Descriptions.VolumenApiNameIo1) || VolumenApiName.Equals(AwsEc2Descriptions.VolumenApiNameIo2))
            {
                price = NumberOfIops * IopsPricePerUnit;

                return price;
            }

            return price;
        }

        public decimal GetThroughputPrice()
        {
            var price = 0m;

            if (VolumenApiName.Equals(AwsEc2Descriptions.VolumenApiNameGp3))
            {
                var numberOfMbpsThroughputBillable = NumberOfMbpsThroughput - AwsEc2Descriptions.FreeTierGp3Throughput;

                if (numberOfMbpsThroughputBillable > 0)
                {
                    price = numberOfMbpsThroughputBillable * ThroughputPricePerUnit;
                }

                return price;
            }

            if (VolumenApiName.Equals(AwsEc2Descriptions.VolumenApiNameIo1) || VolumenApiName.Equals(AwsEc2Descriptions.VolumenApiNameIo2))
            {
                price = NumberOfMbpsThroughput * ThroughputPricePerUnit;

                return price;
            }

            return price;
        }

        public decimal GetSnapshotsPrice()
        {
            var initialSnapshotPrice = NumberOfGbStorageInEbs * SnapshotPricePerUnit;

            var incrementaSnapshotsPrice = NumberOfSnapshotsPerMonth * SnapshotPricePerUnit * PartialStorageDiscount * NumberOfGbChangedPerSnapshot;

            var totalShanpshotPrice = initialSnapshotPrice + incrementaSnapshotsPrice;

            return totalShanpshotPrice;
        }

        public decimal GetEbsPrice()
        {
            return NumberOfGbStorageInEbs * EbsPricePerUnit;
        }

        public decimal GetInstancePrice()
        {
            var price = -1m;

            if((PurchaseOption.Equals("All Upfront") || PurchaseOption.Equals("AllUpfront")) &&
                (LeaseContractLength.Equals("1yr") || LeaseContractLength.Equals("1 yr")))
            {
                price = GetPriceAllUpfront1Yr();
            }

            if ((PurchaseOption.Equals("All Upfront") || PurchaseOption.Equals("AllUpfront")) &&
                (LeaseContractLength.Equals("3yr") || LeaseContractLength.Equals("3 yr")))
            {
                price = GetPriceAllUpfront3Yr();
            }

            if ((PurchaseOption.Equals("Partial Upfront") || PurchaseOption.Equals("PartialUpfront")) &&
                (LeaseContractLength.Equals("1yr") || LeaseContractLength.Equals("1 yr")))
            {
                price = GetPricePartialUpfront1Yr();
            }

            if ((PurchaseOption.Equals("Partial Upfront") || PurchaseOption.Equals("PartialUpfront")) &&
                (LeaseContractLength.Equals("3yr") || LeaseContractLength.Equals("3 yr")))
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
            return InstancePricePerUnit1 / 12;
        }

        private decimal GetPriceAllUpfront3Yr()
        {
            return InstancePricePerUnit1 / 36;
        }

        private decimal GetPricePartialUpfront1Yr()
        {
            return InstancePricePerUnit0 * 730 + InstancePricePerUnit1 / 12;
        }

        private decimal GetPricePartialUpfront3Yr()
        {
            return InstancePricePerUnit0 * 730 + InstancePricePerUnit1 / 36;
        }

        private decimal GetPriceNoUpfront()
        {
            return InstancePricePerUnit0 * 730;
        }
    }
}
