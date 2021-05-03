using System.Collections.Generic;
using System.Threading.Tasks;
using FRF.Core.Models;
using FRF.Core.Response;

namespace FRF.Core.Services
{
    public interface IResourcesService
    {
        Task<ServiceResponse<List<Resource>>> GetAllAsync();
        Task<ServiceResponse<Resource>> GetAsync(int id);
        Task<ServiceResponse<Resource>> UpdateAsync(Resource resource);
        Task<ServiceResponse<Resource>> DeleteAsync(int id);
        Task<ServiceResponse<Resource>> SaveAsync(Resource resource);
    }
}