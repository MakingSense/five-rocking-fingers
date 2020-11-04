using System;
using System.Threading.Tasks;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;

namespace FRF.Core.Services
{
    public class UserService : IUserService
    {
        /*Uncomment this after do.*/
       /*
       private readonly SignInManager<CognitoUser> _signInManager;
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
    }
}