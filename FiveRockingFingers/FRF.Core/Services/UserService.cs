using Amazon.Extensions.CognitoAuthentication;
using FRF.Core.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

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

        public async Task Logout()
        {
           await _signInManager.SignOutAsync();
        }

        public async Task<Guid> GetCurrentUserIdAsync()
        {
            var currentUser = _signInManager.Context.User;
            var result = await _userManager.GetUserAsync(currentUser);
            if (result == null) return Guid.Empty;

            var userId = new Guid(result.UserID);
            return userId;
        }

        public async Task<UsersProfile> GetUserPublicProfileAsync(string email)
        {
            var response = await _userManager.FindByEmailAsync(email);
            if (response == null) return null;

            var user = new UsersProfile
            {
                Email = email,
                Fullname = $"{response.Attributes["name"]} {response.Attributes["family_name"]}",
                UserId = new Guid(response.UserID)
            };

            return user;
        }

        public async Task<UsersProfile> GetUserPublicProfileAsync(Guid userId)
        {
            var response = await _userManager.FindByIdAsync(userId.ToString());
            if (response == null) return null;

            var user = new UsersProfile
            {
                Email = response.Attributes["email"],
                Fullname = $"{response.Attributes["name"]} {response.Attributes["family_name"]}",
                UserId = new Guid(response.UserID)
            };

            return user;
        }
    }
}