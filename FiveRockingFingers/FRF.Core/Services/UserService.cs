using Amazon.Extensions.CognitoAuthentication;
using FRF.Core.Models;
using FRF.Core.Response;
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

        public async Task<ServiceResponse<Guid>> GetCurrentUserIdAsync()
        {
            var currentUser = _signInManager.Context.User;
            var result = await _userManager.GetUserAsync(currentUser);
            if (result == null)
                return new ServiceResponse<Guid>(new Error(ErrorCodes.UserNotExists, ""));

            var userId = new Guid(result.UserID);
            return new ServiceResponse<Guid>(userId);
        }

        public async Task<ServiceResponse<UsersProfile>> GetUserPublicProfileAsync(string email)
        {
            var response = await _userManager.FindByEmailAsync(email);
            if (response == null)
                return new ServiceResponse<UsersProfile>(new Error(ErrorCodes.UserNotExists, ""));

            var user = new UsersProfile
            {
                Email = email,
                Fullname = $"{response.Attributes["name"]} {response.Attributes["family_name"]}",
                UserId = new Guid(response.UserID)
            };

            return new ServiceResponse<UsersProfile>(user);
        }

        public async Task<ServiceResponse<UsersProfile>> GetUserPublicProfileAsync(Guid userId)
        {
            var response = await _userManager.FindByIdAsync(userId.ToString());
            if (response == null)
                return new ServiceResponse<UsersProfile>(new Error(ErrorCodes.UserNotExists, ""));

            var user = new UsersProfile
            {
                Email = response.Attributes["email"],
                Fullname = $"{response.Attributes["name"]} {response.Attributes["family_name"]}",
                UserId = new Guid(response.UserID)
            };

            return new ServiceResponse<UsersProfile>(user);
        }
    }
}