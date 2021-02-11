using System;
using System.Collections.Generic;
using System.Text;

namespace FRF.Core
{
    public class AwsEc2Descriptions
    {
        //General
        public const string Service = "AmazonEC2";
        public const string LocationType = "AWS Region";

        //Compute instance
        public const string ProductFamilyComputeInstance = "Compute Instance";


        //EBS storage
        public const string ProductFamilyEbsStorage = "Storage";
        public const string VolumenApiNameGp3 = "gp3";
        public const string VolumenApiNameIo1 = "io1";
        public const string VolumenApiNameIo2 = "io2";


        //EBS snapshots
        public const string ProductFamilyEbsSnapshots = "Storage Snapshot";


        //EBS Iops
        public const string ProductFamilyEbsIops = "System Operation";


        //Provisioned Throughput
        public const string ProductFamilyEbsThroughput = "Provisioned Throughput";


        //Data transfer
        public const string ProductFamilyDataTransfer = "Data Transfer";


        public const decimal FreeTierGp3Iops = 3000m;
        public const decimal FreeTierGp3Throughput = 125m;
    }
}
