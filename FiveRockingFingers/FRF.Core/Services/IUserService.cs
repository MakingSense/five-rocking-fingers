using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public interface IUserService
    {
        Task<string> GetFullnameByEmail(string email);
        Task<string> GetFullnameByID(string userId);
        Task Logout();
    }
}
