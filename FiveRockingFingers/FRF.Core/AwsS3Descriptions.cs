namespace FRF.Core
{
    public class AwsS3Descriptions
    {
        public const string Service = "AmazonS3";
        public const string WriteFrequentGroup = "S3-API-Tier1";
        public const string RetrieveFrequentGroup = "S3-API-Tier2";
        public const string Location = "location";
        public const string StorageClass = "storageClass";
        public const string VolumeType = "volumeType";

        public const string StandardInfrequentAccessProduct = "Standard - Infrequent Access";
        public const string IntelligentInfrequentAccessProduct = "Intelligent-Tiering Infrequent Access";
        public const string IntelligentFrequentAccessProduct = "Intelligent-Tiering Frequent Access";
        public const string WriteInfrequentGroup = "S3-API-SIA-Tier1";
        public const string RetrieveInfrequentGroup = "S3-API-SIA-Tier2";

        public const string InfrequentAccessProduct = "One Zone - Infrequent Access";
        public const string WriteOneZoneInfrequentGroup = "S3-API-ZIA-Tier1";
        public const string RetrieveOneZoneInfrequentGroup = "S3-API-ZIA-Tier2";

        public const string AutomationObjectCountFee = "S3-Monitoring and Automation-ObjectCount";
        public const string IntelligentTieringProduct = "Intelligent-Tiering";

        //Products
        public const string Storage = "Storage";
        public const string WriteRequests = "Write Requests";
        public const string RetriveRequests = "Retrive Requests";
        public const string InfrequentAccessStorge = "Infrequent Access Storge";
        public const string AutomaticMonitoring = "Automatic Monitoring";

        //XML nodes names
        public const string Product0 = "product0";
        public const string Product1 = "product1";
        public const string Product2 = "product2";
        public const string Product3 = "product3";
        public const string Range0 = "range0";
        public const string Range1 = "range1";
        public const string Range2 = "range2";
        public const string PricePerUnit = "pricePerUnit";
        public const string PricingDimensions = "pricingDimensions";
    }
}