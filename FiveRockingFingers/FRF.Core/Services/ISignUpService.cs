using FRF.Core.Models;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public interface ISignUpService
    {
        Task SignUp(User newUser);
    }
}