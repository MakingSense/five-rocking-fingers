using Amazon.Extensions.CognitoAuthentication;
using FRF.Core.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public class SignInService : ISignInService
    {
        /* TODO:Pending AWS Credentials. Login is bypassed![FIVE-6] */
        /*Uncomment this after do.*/
        /*
        private readonly SignInManager<CognitoUser> _signInManager;

        public SignInService(SignInManager<CognitoUser> signInManager)
        {
            _signInManager = signInManager;
        }
        */

        /// <summary>
        /// Sign in user.
        /// </summary>
        /// <param name="userSignIn">User Object with email and password</param>
        /// <returns>If is a correct user, return true and user id otherwise return false</returns>
        public async Task<Tuple<bool, string>> SignInAsync(UserSignIn userSignIn)
        {
            var userEmail = userSignIn.Email.Trim();
            var userPassword = userSignIn.Password.Trim();

            /* TODO:Pending AWS Credentials. Login is bypassed![FIVE-6] */
            /*Clear this after do.*/
            if (userEmail == "fiverockingfingers@making.com" && userPassword == "Frfadmin123")
                return new Tuple<bool, string>(true, "c3c0b740-1c8f-49a0-a5d7-2354cb9b6eba");
            else return new Tuple<bool, string>(false, "");
            /**/

            /*Uncomment this after do.*/
            /*
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
                //throw message exception from AWS Cognito User Pool
                throw new Exception(e.Message);
            }*/
        }
    }
}