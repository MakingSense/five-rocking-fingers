using Amazon.AspNetCore.Identity.Cognito;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using FRF.Core.Models;
using FRF.Core.Response;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public class SignUpService : ISignUpService
    {
        private readonly SignInManager<CognitoUser> _signInManager;
        private readonly UserManager<CognitoUser> _userManager;
        private readonly IAmazonCognitoIdentityProvider _cognitoIdentity;
        private readonly CognitoUserPool _pool;

        public SignUpService(SignInManager<CognitoUser> signInManager, IAmazonCognitoIdentityProvider cognitoIdentity,
            UserManager<CognitoUser> userManager, CognitoUserPool pool)
        {
            _signInManager = signInManager;
            _cognitoIdentity = cognitoIdentity;
            _userManager = userManager;
            _pool = pool;
        }

        public async Task<ServiceResponse<string>> SignUpAsync(User newUser)
        {
            try
            {
                //Sign up the new user
                var newCognitoUser = _pool.GetUser(newUser.Email);
                newCognitoUser.Attributes.Add(CognitoAttribute.Email.AttributeName, newUser.Email);
                newCognitoUser.Attributes.Add(CognitoAttribute.FamilyName.AttributeName, newUser.FamilyName);
                newCognitoUser.Attributes.Add(CognitoAttribute.Name.AttributeName, newUser.Name);
                var response = await _userManager.CreateAsync(newCognitoUser, newUser.Password);

                if (!response.Succeeded)
                    return new ServiceResponse<string>(new Error(ErrorCodes.InvalidCredentials, "There was an error with your email and/or password"));

                //Auto validate the new user. They will not be included in the production build
                await _cognitoIdentity.AdminUpdateUserAttributesAsync(new AdminUpdateUserAttributesRequest
                {
                    UserAttributes = new List<AttributeType>
                    {
                        new AttributeType {Name = "email_verified", Value = "true"}
                    },
                    Username = newUser.Email,
                    UserPoolId = _pool.PoolID
                });

                await _cognitoIdentity.AdminConfirmSignUpAsync(new AdminConfirmSignUpRequest
                {
                    Username = newUser.Email,
                    UserPoolId = _pool.PoolID
                });

                var token = await _signInManager.UserManager.FindByEmailAsync(newUser.Email);
                if (token == null)
                    return new ServiceResponse<string>(new Error(ErrorCodes.AuthenticationServerCurrentlyUnavailable, "There was an error with the Authentication service"));

                var result =
                    await _signInManager.PasswordSignInAsync(token, newUser.Password, false, false);
                return result.Succeeded
                    ? new ServiceResponse<string>(token.SessionTokens.IdToken)
                    : new ServiceResponse<string>(new Error(ErrorCodes.AuthenticationServerCurrentlyUnavailable, "There was an error with the Authentication service"));
            }
            catch
            {
                return new ServiceResponse<string>(new Error(ErrorCodes.AuthenticationServerCurrentlyUnavailable, "There was an error with the Authentication service"));

            }
        }
    }
}