using FRF.Core.Models;
using FRF.Core.Response;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public interface ISignInService
    {
        Task<ServiceResponse<string>> SignInAsync(UserSignIn userSignIn);
    }
}