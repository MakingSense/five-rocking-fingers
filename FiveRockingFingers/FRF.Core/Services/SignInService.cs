using Amazon.Extensions.CognitoAuthentication;
using FRF.Core.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public class SignInService : ISignInService
    {
        private readonly SignInManager<CognitoUser> _signInManager;

        public SignInService(SignInManager<CognitoUser> signInManager)
        {
            _signInManager = signInManager;
        }
        /// <summary>
        /// Sign in user.
        /// </summary>
        /// <param name="userSignIn">User Object with email and password</param>
        /// <returns>If is a correct user, return true and user id otherwise return false</returns>
        public async Task<Tuple<bool, string>> SignIn(UserSignIn userSignIn)
        {
            var userEmail = userSignIn.Email.Trim();
            var userPassword = userSignIn.Password.Trim();

            try
            {
                //First Look if the email exist
                var token = await _signInManager.UserManager.FindByEmailAsync(userEmail);
                if (token == null) return new Tuple<bool, string>(false, "");

                var result = await _signInManager.PasswordSignInAsync(token, userPassword,
                    userSignIn.RememberMe, lockoutOnFailure: false);
                return result.Succeeded
                    ? new Tuple<bool, string>(true, token.UserID)
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