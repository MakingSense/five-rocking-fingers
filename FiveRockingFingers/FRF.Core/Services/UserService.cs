using System;
using System.Threading.Tasks;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;

namespace FRF.Core.Services
{
    public class UserService : IUserService
    {
        /*private readonly SignInManager<CognitoUser> _signInManager;
        private readonly UserManager<CognitoUser> _userManager;

        public UserService(UserManager<CognitoUser> userManager, SignInManager<CognitoUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public async Task<string> GetFullname(string email)
        {
            if (string.IsNullOrEmpty(email)) return null;

            var fullName = "";
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                foreach (var (key, value) in user.Attributes)
                    fullName = key switch
                    {
                        "name" => value,
                        "family_name" => fullName + " " + value,
                        _ => fullName
                    };
            }
            catch (Exception e)
            {
                throw new ArgumentException("Failed to search user :" + e.Message);
            }

            return fullName;
        }

        public async Task Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
            }
            catch (Exception e)
            {
                throw new Exception("Logout fail: " + e.Message);
            }
        }

        public async Task<string> GetCurrentUserId()
        {
            try
            {
                var currentUser = SignInManager.Context.User;
                var result = await UserManager.GetUserAsync(currentUser).ConfigureAwait(false);
                return result.UserID;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<string> GetUserIdByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            return user == null ? null : user.UserID;
        }*/

        public async Task<string> GetEmailByUserId(string userId)
        {
            // TODO: AWS Credentials, Loggin bypassed.Uncomment after do:
            //var userEmail = await _userManager.FindByIdAsync(userId);
            //return userEmail == null ? null : userEmail.Username;

            // AND DELETE THIS:
            const string currentUserId = "c3c0b740-1c8f-49a0-a5d7-2354cb9b6eba";
            return userId.ToLower() == currentUserId ? "fiverockingfingers@making.com" : "cooluser@mock.org";
            //
        }
    }
}