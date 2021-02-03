namespace FRF.Core
{
    public static class ArtifactTypes
    {
        public const string Custom = "Custom";
        public const string Aws = "AWS";
    }

    public static class AwsS3Constants
    {
        public const string Service = "AmazonS3";
        public const string WriteFrequentGroup = "S3-API-Tier1";
        public const string RetrieveFrequentGroup = "S3-API-Tier2";

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

    }
}
