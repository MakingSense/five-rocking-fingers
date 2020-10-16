using System;
using FRF.Core.Models;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public interface ISignInService
    {
        Task<Tuple<bool, string>> SignIn(UserSignIn userSignIn);
    }
}