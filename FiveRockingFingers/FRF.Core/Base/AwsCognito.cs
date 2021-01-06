namespace FRF.Core.Base
{
    public class AwsCognito
    {
        public const string AwsCognitoCredential = "AWSCognitoCredentials";

        public string AccessKeyId { get; set; }
        public string ClientId { get; set; }
        public string SecretAccKey { get; set; }
        public string UserPoolId { get; set; }
    }
}