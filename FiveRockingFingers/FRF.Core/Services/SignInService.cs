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

        public async Task<string> SignIn(UserSignIn userSignIn)
        {
            var userEmail = userSignIn.Email.Trim();
            var userPassword = userSignIn.Password.Trim();

            try
            {
                var token = await SignInManager.UserManager.FindByEmailAsync(userEmail);
                if (token==null)
                {
                    throw new Exception("The email account that you tried to reach does not exist");
                }
                var result = await SignInManager.PasswordSignInAsync(token, userPassword,
                    userSignIn.RememberMe, lockoutOnFailure: false);
                if (!result.Succeeded)
                {
                    throw new Exception("Login failed. Contact your system administrator");
                }

                return token.SessionTokens.IdToken;
            }

            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}