using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using FRF.Core.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using FRF.Core.Base;

namespace FRF.Core.Services
{
    public class SignInService : ISignInService
    {
        public IConfiguration Configuration { get; set; }
        private readonly UserBase UserBase;

        public SignInService(IConfiguration configuration)
        {
            UserBase = new UserBase(configuration);
        }

        public async Task<string> SignIn(UserSignIn userSignIn)
        {
            var userEmail = userSignIn.Email.Trim();
            var userPassword = userSignIn.Password.Trim();

            var provider = new AmazonCognitoIdentityProviderClient(UserBase.AccessKeyId, UserBase.SecretAccKey,
                RegionEndpoint.USWest2);

            var userPool = new CognitoUserPool(UserBase.UserPoolId, UserBase.ClientId, provider);

            var user = new CognitoUser(userEmail, UserBase.ClientId, userPool,
                provider);

            var authRequest = new InitiateSrpAuthRequest() {Password = userPassword};

            try
            {
                var authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);
                //Here method authResponse to User//
                return authResponse.AuthenticationResult.AccessToken;
            }

            catch (Exception e)
            {
                throw new Exception("Login failed:" + e.Message + "\n");
            }
        }
    }
}