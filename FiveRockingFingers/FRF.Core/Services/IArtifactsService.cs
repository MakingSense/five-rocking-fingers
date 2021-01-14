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
        Task<IList<ArtifactsRelation>> SetRelationAsync(IList<ArtifactsRelation> artifactRelations);
        Task<IList<ArtifactsRelation>> GetRelationsAsync(int artifactId);
        Task<IList<ArtifactsRelation>> GetAllRelationsByProjectIdAsync(int projectId);
        Task<ArtifactsRelation> DeleteRelationAsync(int artifact1Id, int artifact2Id);
        Task<IList<ArtifactsRelation>> UpdateRelationAsync(int artifact1Id, IList<ArtifactsRelation> artifactsRelationsNew);
    }
}
