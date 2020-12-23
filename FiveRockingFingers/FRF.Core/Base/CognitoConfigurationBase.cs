using FRF.Core.Services;

namespace FRF.Core.Base
{
    public class CognitoConfigurationBase
    {
        public const string AwsPricingOptions = "AWSCognitoCredentials";
        public string UserPoolId { get; set; }
        public string ClientId { get; set; }
        public string SecretAccKey { get; set; }
        public string AccessKeyId { get; set; }

    }
}