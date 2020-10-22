using System;
using System.Threading.Tasks;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;

namespace FRF.Core.Services
{
    public class UserService : IUserService
    {
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
                throw new Exception("Failed to search user :" + e.Message);
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
    }
}