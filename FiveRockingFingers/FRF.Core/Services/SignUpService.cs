using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using FRF.Core.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FRF.Core.Base;

namespace FRF.Core.Services
{
    public class SignUpService : ISignUpService
    {
        public IConfiguration Configuration { get; set; }
        private readonly UserBase UserBase;

        public SignUpService(IConfiguration configuration)
        {
            UserBase = new UserBase(configuration);
        }

        public async Task<string> SignUp(User newUser)
        {
            try
            {
                var provider = new AmazonCognitoIdentityProviderClient(UserBase.AccessKeyId,
                    UserBase.SecretAccKey, RegionEndpoint.USWest2);
                var response = await provider.SignUpAsync(new SignUpRequest
                {
                    ClientId = UserBase.ClientId,
                    Username = newUser.Email,
                    Password = newUser.Password,
                    UserAttributes = new List<AttributeType>
                    {
                        new AttributeType {Name = "name", Value = newUser.Name},
                        new AttributeType {Name = "family_name", Value = newUser.FamilyName}
                    }
                });

                await provider.AdminUpdateUserAttributesAsync(new AdminUpdateUserAttributesRequest
                {
                    UserAttributes = new List<AttributeType>
                    {
                        new AttributeType {Name = "email_verified", Value = "true"}
                    },
                    Username = response.UserSub,
                    UserPoolId = UserBase.UserPoolId
                });

                await provider.AdminConfirmSignUpAsync(new AdminConfirmSignUpRequest
                {
                    Username = response.UserSub,
                    UserPoolId = UserBase.UserPoolId
                });
                return response.UserSub;
            }
            catch (Exception e)
            {
                throw new Exception("Sign up fail: " + e.Message);
            }
        }
    }
}