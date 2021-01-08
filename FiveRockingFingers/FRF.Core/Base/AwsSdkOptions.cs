using System;
using System.Collections.Generic;
using System.Text;

namespace FRF.Core.Base
{
    public class AwsSdkOptions
    {
        public const string AwsSDK = "AwsSDK";

        public string AccessKeyId { get; set; }
        public string SecretAccessKey { get; set; }
    }
}
