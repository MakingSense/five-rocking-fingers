using Amazon.Extensions.CognitoAuthentication;
using FRF.Core.Models;
using FRF.Core.Response;
using Microsoft.AspNetCore.Identity;
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
        public async Task<ServiceResponse<string>> SignInAsync(UserSignIn userSignIn)
        {
            var userEmail = userSignIn.Email.Trim();
            var userPassword = userSignIn.Password.Trim();

            try
            {
                //First Look if the email exist
                var token = await _signInManager.UserManager.FindByEmailAsync(userEmail);
                if (token == null) return new ServiceResponse<string>(new Error(ErrorCodes.InvalidCredentials, ""));

                var result = await _signInManager.PasswordSignInAsync(token, userPassword,
                    userSignIn.RememberMe, lockoutOnFailure: false);
                return result.Succeeded
                    ? new ServiceResponse<string>(token.UserID)
                    : new ServiceResponse<string>(new Error(ErrorCodes.InvalidCredentials, ""));
            }
            catch
            {
                //throw message exception from AWS Cognito User Pool
                return new ServiceResponse<string>(new Error(ErrorCodes.AuthenticationServerCurrentlyUnavailable, ""));
            }
        }
    }
}