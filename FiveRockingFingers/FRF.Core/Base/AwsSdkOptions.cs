using System;
using System.Collections.Generic;
using System.Text;

namespace FRF.Core.Base
{
    public class AwsSdkOptions
    {
        public const string AwsSdk = "AwsSdk";

        public string AccessKeyId { get; set; }
        public string SecretAccessKey { get; set; }
    }
}
