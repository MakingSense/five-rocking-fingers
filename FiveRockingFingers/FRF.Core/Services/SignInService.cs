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
        private readonly UserManager<CognitoUser> UserManager;

        public SignInService(SignInManager<CognitoUser> signInManager, UserManager<CognitoUser> userManager)
        {
            SignInManager = signInManager;
            UserManager = userManager;
        }

        public async Task<string> SignIn(UserSignIn userSignIn)
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
                var user = await UserManager.FindByEmailAsync(userEmail);
                return user.ClientID;
            }

            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}