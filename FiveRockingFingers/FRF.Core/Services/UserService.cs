using System;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<CognitoUser> UserManager;
        private readonly SignInManager<CognitoUser> SignInManager;

        public UserService(UserManager<CognitoUser> userManager, SignInManager<CognitoUser> signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public async Task<string> GetFullname(string email)
        {
            if (string.IsNullOrEmpty(email)) return null;

            var fullName = "";
            try
            {
                var user = await UserManager.FindByEmailAsync(email);
                foreach (var (key, value) in user.Attributes)
                {
                    fullName = key switch
                    {
                        "name" => value,
                        "family_name" => fullName + " " + value,
                        _ => fullName
                    };
                }
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
                await SignInManager.SignOutAsync();
            }
            catch (Exception e)
            {
                throw new Exception("Logout fail: " + e.Message);
            }
        }
    }
}