using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using FRF.Core.Base;
using FRF.Core.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public class SignUpService : ISignUpService
    {
        private readonly CognitoConfigurationBase CognitoBase;
        private readonly SignInManager<CognitoUser> SignInManager;
        private readonly IConfigurationService ConfigurationService;

        public SignUpService(SignInManager<CognitoUser> signInManager, IConfigurationService configurationService)
        {
            SignInManager = signInManager;
            ConfigurationService = configurationService;
            CognitoBase = configurationService.GetConfigurationSettings();
        }

        public async Task SignUp(User newUser)
        {
            try
            {
                var provider = new AmazonCognitoIdentityProviderClient(CognitoBase.AccessKeyId,
                    CognitoBase.SecretAccKey, RegionEndpoint.USWest2);
                var response = await provider.SignUpAsync(new SignUpRequest
                {
                    ClientId = CognitoBase.ClientId,
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
                    UserPoolId = CognitoBase.UserPoolId
                });

                await provider.AdminConfirmSignUpAsync(new AdminConfirmSignUpRequest
                {
                    Username = response.UserSub,
                    UserPoolId = CognitoBase.UserPoolId
                });


                var result = await SignInManager.PasswordSignInAsync(newUser.Email, newUser.Password,
                    false, lockoutOnFailure: false);

                if (!result.Succeeded)
                {
                    throw new Exception("Account created but sign in failed. Contact your system administrator");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Sign up failed: " + e.Message);
            }
        }
    }
}