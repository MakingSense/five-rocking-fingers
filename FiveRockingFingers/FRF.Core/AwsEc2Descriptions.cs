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
    }
}
