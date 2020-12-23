using FRF.Core.Models;
using System;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public interface IUserService
    {
        
        Task Logout();
        Task<Guid> GetCurrentUserId();
        Task<UsersProfile> GetUserPublicProfile(string email);
        Task<UsersProfile> GetUserPublicProfile(Guid userId);
    }
}
