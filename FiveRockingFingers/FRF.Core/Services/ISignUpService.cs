using System;
using System.Threading.Tasks;
using FRF.Core.Models;

namespace FRF.Core.Services
{
    public interface ISignUpService
    {
        Task<Tuple<bool, string>> SignUpAsync(User newUser);
    }
}