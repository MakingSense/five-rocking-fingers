using FRF.Core.Models;
using FRF.Core.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public interface IArtifactTypesService
    {
        Task<ServiceResponse<List<ArtifactType>>> GetAll();
        Task<ServiceResponse<List<ArtifactType>>> GetAllByProvider(string providerName);
    }
}
