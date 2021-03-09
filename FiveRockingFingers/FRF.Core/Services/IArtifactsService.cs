using FRF.Core.Models;
using FRF.Core.Response;
using System;
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
        Task<ServiceResponse<IList<ArtifactsRelation>>> GetAllRelationsOfAnArtifactAsync(int artifactId);
        Task<ServiceResponse<IList<ArtifactsRelation>>> GetAllRelationsByProjectIdAsync(int projectId);
        Task<ServiceResponse<ArtifactsRelation>> DeleteRelationAsync(Guid artifactRelationId);
        Task<ServiceResponse<IList<ArtifactsRelation>>> DeleteRelationsAsync(IList<Guid> artifactRelationIds);
        Task<ServiceResponse<IList<ArtifactsRelation>>> UpdateRelationAsync(int artifact1Id,
            IList<ArtifactsRelation> artifactsRelationsNew);
    }
}