using FRF.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public interface IArtifactsService
    {
        Task<List<Artifact>> GetAll();
        Task<List<Artifact>> GetAllByProjectId(int projectId);
        Task<Artifact> Get(int id);
        Task<Artifact> Update(Artifact artifact);
        Task<Artifact> Delete(int id);
        Task<Artifact> Save(Artifact artifact);
    }
}
