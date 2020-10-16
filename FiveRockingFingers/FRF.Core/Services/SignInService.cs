using Amazon.Extensions.CognitoAuthentication;
using FRF.Core.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public class SignInService : ISignInService
    {
        private readonly SignInManager<CognitoUser> SignInManager;

        public SignInService(SignInManager<CognitoUser> signInManager)
        {
            SignInManager = signInManager;
        }

        public async Task<Tuple<bool, string>> SignIn(UserSignIn userSignIn)
        {
            var userEmail = userSignIn.Email.Trim();
            var userPassword = userSignIn.Password.Trim();

            try
            {
                var token = await SignInManager.UserManager.FindByEmailAsync(userEmail);
                if (token == null) return new Tuple<bool, string>(false, "");

                var result = await SignInManager.PasswordSignInAsync(token, userPassword,
                    userSignIn.RememberMe, lockoutOnFailure: false);
                return result.Succeeded
                    ? new Tuple<bool, string>(true, token.SessionTokens.IdToken)
                    : new Tuple<bool, string>(false, "");
            }
            catch (Exception e)
            {
                //throw message exception from Cognito User Pool
                throw new Exception(e.Message);
            }
        }
    }
}