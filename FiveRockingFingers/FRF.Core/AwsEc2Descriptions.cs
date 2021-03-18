using System;
using System.Collections.Generic;
using System.Text;

namespace FRF.Core
{
    public class AwsEc2Descriptions
    {
        //Atributtes
        public const string Location = "location";
        public const string PreInstalledSw = "preInstalledSw";
        public const string OperatingSystem = "operatingSystem";
        public const string InstanceType = "instanceType";
        public const string Vcpu = "vcpu";
        public const string Memory = "memory";
        public const string NetworkPerformance = "networkPerformance";
        public const string Storage = "storage";
        public const string VolumeApiName = "volumeApiName";
        public const string TransferType = "transferType";
        public const string FromLocation = "fromLocation";
        public const string ToLocation = "toLocation";
        public const string ServiceCode = "servicecode";
        public const string LocationType = "locationType";
        public const string ToLocationType = "toLocationType";
        public const string FromLocationType = "fromLocationType";
        public const string ProductFamily = "productFamily";
        public const string HoursUsedPerMonth = "hoursUsedPerMonth";
        public const string NumberOfGbStorageInEbs = "numberOfGbStorageInEbs";
        public const string NumberOfSnapshotsPerMonth = "numberOfSnapshotsPerMonth";
        public const string NumberOfGbChangedPerSnapshot = "numberOfGbChangedPerSnapshot";
        public const string NumberOfIopsPerMonth = "numberOfIopsPerMonth";
        public const string NumberOfMbpsThroughput = "numberOfMbpsThroughput";

        public const string TransferType1 = "transferType1";
        public const string FromLocation1 = "fromLocation1";
        public const string ToLocation1 = "toLocation1";

        public const string TransferType2 = "transferType2";
        public const string FromLocation2 = "fromLocation2";
        public const string ToLocation2 = "toLocation2";

        public const string TransferType3 = "transferType3";
        public const string FromLocation3 = "fromLocation3";
        public const string ToLocation3 = "toLocation3";

        public const string PurchaseOption = "PurchaseOption";
        public const string TermType = "termType";
        public const string OfferingClass = "OfferingClass";
        public const string LeaseContractLength = "LeaseContractLength";

        public const string OnDemandTermType = "OnDemand";


        //General
        public const string ServiceValue = "AmazonEC2";
        public const string LocationTypeValue = "AWS Region";

        //Compute instance
        public const string ProductFamilyComputeInstanceValue = "Compute Instance";


        //EBS storage
        public const string ProductFamilyEbsStorageValue = "Storage";
        public const string VolumenApiNameGp3Value = "gp3";
        public const string VolumenApiNameIo1Value = "io1";
        public const string VolumenApiNameIo2Value = "io2";


        //EBS snapshots
        public const string ProductFamilyEbsSnapshotsValue = "Storage Snapshot";


        //EBS Iops
        public const string ProductFamilyEbsIopsValue = "System Operation";


        //Provisioned Throughput
        public const string ProductFamilyEbsThroughputValue = "Provisioned Throughput";


        //Data transfer
        public const string ProductFamilyDataTransferValue = "Data Transfer";
        public const string IntraRegionDataTransfer = "IntraRegion";

        //Free tier
        public const decimal FreeTierGp3Iops = 3000m;
        public const decimal FreeTierGp3Throughput = 125m;

        //Term attributes constants
        public const string AllUpfront = "AllUpfront";
        public const string PartialUpfront = "PartialUpfront";
        public const string NoUpfront = "NoUpfront";
        public const string OneYear = "1yr";
        public const string ThreeYears = "3yr";
        public const string PurchaseOption = "purchaseOption";
        public const string LeaseContractLength = "leaseContractLength";


        //XML nodes names
        public const string Product0 = "product0";
        public const string Product1 = "product1";
        public const string Product2 = "product2";
        public const string Product3 = "product3";
        public const string Product4 = "product4";
        public const string Range0 = "range0";
        public const string Range1 = "range1";
        public const string PricePerUnit = "pricePerUnit";
        public const string PricingDimensions = "pricingDimensions";
    }
}
