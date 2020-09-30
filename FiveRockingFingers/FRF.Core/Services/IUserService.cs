using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public interface IUserService
    {
        Task<string> GetFullname(string email);
        Task Logout();
    }
}
