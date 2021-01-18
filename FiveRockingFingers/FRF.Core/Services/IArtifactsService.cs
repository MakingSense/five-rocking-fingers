using FRF.Core.Models;
using FRF.Core.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public interface IArtifactsService
    {
        Task<ServiceResponse<List<Artifact>>> GetAll();
        Task<ServiceResponse<List<Artifact>>> GetAllByProjectId(int projectId);
        Task<ServiceResponse<Artifact>> Get(int id);
        Task<ServiceResponse<Artifact>> Update(Artifact artifact);
        Task<ServiceResponse<Artifact>> Delete(int id);
        Task<ServiceResponse<Artifact>> Save(Artifact artifact);
        Task<ServiceResponse<IList<ArtifactsRelation>>> SetRelationAsync(IList<ArtifactsRelation> artifactRelations);
        Task<IList<ArtifactsRelation>> GetRelationsAsync(int artifactId);
        Task<ServiceResponse<IList<ArtifactsRelation>>> GetAllRelationsByProjectIdAsync(int projectId);
        Task<ServiceResponse<ArtifactsRelation>> DeleteRelationAsync(int artifact1Id, int artifact2Id);
        Task<ServiceResponse<IList<ArtifactsRelation>>> UpdateRelationAsync(int artifact1Id,
            IList<ArtifactsRelation> artifactsRelationsNew);
    }
}
