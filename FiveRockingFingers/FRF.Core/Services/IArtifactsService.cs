using FRF.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public interface IArtifactsService
    {
        Task<List<Artifact>> GetAll();
        Task<Artifact> Get(int id);
        Task<Artifact> Update(Artifact artifact);
        void Delete(int id);
        Task<Artifact> Save(Artifact artifact);
    }
}
