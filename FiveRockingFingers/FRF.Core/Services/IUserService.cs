using FRF.Core.Models;
using System;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public interface IUserService
    {
        
        Task Logout();
        Task<Guid> GetCurrentUserIdAsync();
        Task<UsersProfile> GetUserPublicProfileAsync(string email);
        Task<UsersProfile> GetUserPublicProfileAsync(Guid userId);
    }
}
