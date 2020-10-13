using FRF.Core.Models;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public interface ISignInService
    {
        Task<string> SignIn(UserSignIn userSignIn);
    }
}