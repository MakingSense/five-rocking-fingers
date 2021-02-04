using FRF.Core.Models;
using FRF.Core.Response;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public interface ISignUpService
    {
        Task<ServiceResponse<string>> SignUpAsync(User newUser);
    }
}