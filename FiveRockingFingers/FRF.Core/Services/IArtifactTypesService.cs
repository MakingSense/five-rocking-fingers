using FRF.Core.Models;
using FRF.Core.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public interface IArtifactTypesService
    {
        Task<ServiceResponse<List<ArtifactType>>> GetAllAsync();
        Task<ServiceResponse<List<ArtifactType>>> GetAllByProviderAsync(string providerName);
    }
}
