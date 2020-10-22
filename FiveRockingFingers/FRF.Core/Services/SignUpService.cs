using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using FRF.Core.Base;
using FRF.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace FRF.Core.Services
{
    public class SignUpService : ISignUpService
    {
        private readonly CognitoConfigurationBase _cognitoBase;
        private readonly IConfigurationService _configurationService;
        private readonly SignInManager<CognitoUser> _signInManager;

        public SignUpService(SignInManager<CognitoUser> signInManager, IConfigurationService configurationService)
        {
            _signInManager = signInManager;
            _configurationService = configurationService;
            _cognitoBase = configurationService.GetConfigurationSettings();
        }

        public async Task<Tuple<bool, string>> SignUp(User newUser)
        {
            try
            {
                var provider = new AmazonCognitoIdentityProviderClient(_cognitoBase.AccessKeyId,
                    _cognitoBase.SecretAccKey, RegionEndpoint.USWest2);

                //Sign up the new user
                var response = await provider.SignUpAsync(new SignUpRequest
                {
                    ClientId = _cognitoBase.ClientId,
                    Username = newUser.Email,
                    Password = newUser.Password,
                    UserAttributes = new List<AttributeType>
                    {
                        new AttributeType {Name = "name", Value = newUser.Name},
                        new AttributeType {Name = "family_name", Value = newUser.FamilyName}
                    }
                });

                if (response.HttpStatusCode != HttpStatusCode.OK)
                    return new Tuple<bool, string>(false, "");

                //Auto validate the new user. They will not be included in the production build
                await provider.AdminUpdateUserAttributesAsync(new AdminUpdateUserAttributesRequest
                {
                    UserAttributes = new List<AttributeType>
                    {
                        new AttributeType {Name = "email_verified", Value = "true"}
                    },
                    Username = response.UserSub,
                    UserPoolId = _cognitoBase.UserPoolId
                });

                await provider.AdminConfirmSignUpAsync(new AdminConfirmSignUpRequest
                {
                    Username = response.UserSub,
                    UserPoolId = _cognitoBase.UserPoolId
                });

                var token = await _signInManager.UserManager.FindByEmailAsync(newUser.Email);
                if (token == null) return new Tuple<bool, string>(false, "");

                var result =
                    await _signInManager.PasswordSignInAsync(token, newUser.Password, false, false);
                return result.Succeeded
                    ? new Tuple<bool, string>(true, token.UserID)
                    : new Tuple<bool, string>(false, "");
            }
            catch (Exception e)
            {
                //throw exception from cognito user pool
                throw new Exception("Sign up failed: " + e.Message);
            }
        }
    }
}