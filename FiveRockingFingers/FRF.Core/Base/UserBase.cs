using Microsoft.Extensions.Configuration;

namespace FRF.Core.Base
{
    public class UserBase
    {
        public IConfiguration Configuration { get; }
        public string UserPoolId { get; }
        public string ClientId { get; }
        public string SecretAccKey { get; }
        public string AccessKeyId { get; }

        public UserBase(IConfiguration configuration)
        {
            Configuration = configuration;
            SecretAccKey = Configuration.GetValue<string>("AWS:AwsKey");
            ClientId = Configuration.GetValue<string>("AWS:ClientId");
            UserPoolId = Configuration.GetValue<string>("AWS:UserPoolId");
            AccessKeyId = Configuration.GetValue<string>("AWS:AwsId");
        }

        //TODO method from AuthResponse to User
    }
}