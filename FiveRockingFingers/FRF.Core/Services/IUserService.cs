using FRF.Core.Models;
using FRF.Core.Response;
using System;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public interface IUserService
    {
        
        Task Logout();
        Task<ServiceResponse<Guid>> GetCurrentUserIdAsync();
        Task<ServiceResponse<UsersProfile>> GetUserPublicProfileAsync(string email);
        Task<ServiceResponse<UsersProfile>> GetUserPublicProfileAsync(Guid userId);
    }
}
