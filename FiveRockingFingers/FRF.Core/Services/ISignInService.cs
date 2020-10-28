using System;
using System.Threading.Tasks;
using FRF.Core.Models;

namespace FRF.Core.Services
{
    public interface ISignInService
    {
        Task<Tuple<bool, string>> SignInAsync(UserSignIn userSignIn);
    }
}