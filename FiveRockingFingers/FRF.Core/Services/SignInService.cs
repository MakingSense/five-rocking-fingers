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

        public async Task SignIn(UserSignIn userSignIn)
        {
            var userEmail = userSignIn.Email.Trim();
            var userPassword = userSignIn.Password.Trim();

            try
            {
                var result = await SignInManager.PasswordSignInAsync(userEmail, userPassword,
                    userSignIn.RememberMe, lockoutOnFailure: false);
                if (!result.Succeeded)
                {
                    throw new Exception("Login failed. Contact your system administrator");
                }
            }

            catch (Exception e)
            {
                throw new Exception("Login failed:" + e.Message + "\n");
            }
        }
    }
}