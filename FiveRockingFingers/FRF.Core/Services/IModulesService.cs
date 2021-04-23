using System.Collections.Generic;
using System.Threading.Tasks;
using FRF.Core.Models;
using FRF.Core.Response;

namespace FRF.Core.Services
{
    public interface IModulesService
    {
        Task<ServiceResponse<Module>> GetAsync(int id);
        Task<ServiceResponse<IList<Module>>> GetAllByCategoryIdAsync(int id);
        Task<ServiceResponse<Module>> UpdateAsync(Module module);
        Task<ServiceResponse<Module>> DeleteAsync(int id);
        Task<ServiceResponse<Module>> SaveAsync(Module module);
    }
}