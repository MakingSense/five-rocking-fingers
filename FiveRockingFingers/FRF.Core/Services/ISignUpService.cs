using System;
using FRF.Core.Models;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public interface ISignUpService
    {
        Task<Tuple<bool, string>> SignUp(User newUser);
    }
}