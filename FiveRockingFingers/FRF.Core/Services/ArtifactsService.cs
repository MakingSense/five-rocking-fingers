using AutoMapper;
using FRF.Core.Models;
using FRF.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityModels = FRF.DataAccess.EntityModels;

namespace FRF.Core.Services
{
    public class ArtifactsService : IArtifactsService
    {
        private readonly DataAccessContext _dataContext;
        private readonly IMapper _mapper;

        public ArtifactsService(DataAccessContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<List<Artifact>> GetAll()
        {
            var result = await _dataContext.Artifacts.Include(a => a.ArtifactType).Include(a => a.Project).ThenInclude(p => p.ProjectCategories).ThenInclude(pc => pc.Category).ToListAsync();
            return _mapper.Map<List<Artifact>>(result);
        }

        public async Task<List<Artifact>> GetAllByProjectId(int projectId)
        {
            if (!await _dataContext.Projects.AnyAsync(p => p.Id == projectId))
            {
                throw new System.ArgumentException("There is no project with Id = " + projectId, "projectId");
            }

            var result = await _dataContext.Artifacts.Include(a => a.ArtifactType).Include(a => a.Project).ThenInclude(p => p.ProjectCategories).ThenInclude(pc => pc.Category).Where(a => a.ProjectId == projectId).ToListAsync();
            return _mapper.Map<List<Artifact>>(result);
        }

        public async Task<Artifact> Get(int id)
        {
            var artifact = await _dataContext.Artifacts.Include(a => a.ArtifactType).Include(a => a.Project).ThenInclude(p => p.ProjectCategories).ThenInclude(pc => pc.Category).SingleOrDefaultAsync(a => a.Id == id);
            if (artifact == null)
            {
                return null;
            }
            return _mapper.Map<Artifact>(artifact);
        }

        public async Task<Artifact> Save(Artifact artifact)
        {
            if(! await _dataContext.Projects.AnyAsync(p => p.Id == artifact.ProjectId))
            {
                throw new System.ArgumentException("There is no project with Id = " + artifact.ProjectId, "artifact.ProjectId");
            }

            if(! await _dataContext.ArtifactType.AnyAsync(at => at.Id == artifact.ArtifactTypeId))
            {
                throw new System.ArgumentException("There is no ArtifactType with Id = " + artifact.ArtifactTypeId, "artifact.ArtifactTypeId");
            }

            // Maps the artifact into an EntityModel, deleting the Id if there was one, and setting the CreatedDate field
            var mappedArtifact = _mapper.Map<EntityModels.Artifact>(artifact);
            mappedArtifact.CreatedDate = DateTime.Now;
            mappedArtifact.ModifiedDate = null;
            mappedArtifact.ProjectId = artifact.ProjectId;
            mappedArtifact.ArtifactTypeId = artifact.ArtifactTypeId;

            // Adds the artifact to the database, generating a unique Id for it
            await _dataContext.Artifacts.AddAsync(mappedArtifact);

            // Saves changes
            await _dataContext.SaveChangesAsync();

            return _mapper.Map<Artifact>(mappedArtifact);
        }

        public async Task<Artifact> Update(Artifact artifact)
        {
            //Gets the artifact associated to it from the database
            var result = await _dataContext.Artifacts.Include(a => a.ArtifactType).Include(a => a.Project).SingleOrDefaultAsync(a => a.Id == artifact.Id);
            if (result == null)
            {
                throw new System.ArgumentException("There is no artifact with Id = " + artifact.Id, "artifact,Id");
            }

            if (! await _dataContext.Projects.AnyAsync(p => p.Id == artifact.ProjectId))
            {
                throw new System.ArgumentException("There is no project with Id = " + artifact.ProjectId, "artifact.ProjectId");
            }

            if (!await _dataContext.ArtifactType.AnyAsync(at => at.Id == artifact.ArtifactTypeId))
            {
                throw new System.ArgumentException("There is no ArtifactType with Id = " + artifact.ArtifactTypeId, "artifact.ArtifactTypeId");
            }

            //Updates the artifact
            result.Name = artifact.Name;
            result.Provider = artifact.Provider;
            result.Settings = artifact.Settings;
            result.ModifiedDate = DateTime.Now;
            result.ProjectId = artifact.ProjectId;
            result.ArtifactTypeId = artifact.ArtifactTypeId;

            //Saves the updated aritfact in the database
            await _dataContext.SaveChangesAsync();

            return _mapper.Map<Artifact>(result);
        }

        public async Task<Artifact> Delete(int id)
        {
            var artifactToDelete = await _dataContext.Artifacts.SingleOrDefaultAsync(a => a.Id == id);
            if (artifactToDelete == null)
            {
                return null;
            }
            _dataContext.Artifacts.Remove(artifactToDelete);
            await _dataContext.SaveChangesAsync();
            return _mapper.Map<Artifact>(artifactToDelete);
        }

        public async Task<IList<ArtifactsRelation>> SetRelationAsync(IList<ArtifactsRelation> artifactRelations)
        {
            var resultArtifactRelations = new List<ArtifactsRelation>();
            var dbArtifactsId = await _dataContext.Artifacts.Select(a => a.Id).ToListAsync();
            var artifactsRelationIds = artifactRelations
                .Select(ar => ar.Artifact1Id)
                .Concat(artifactRelations.Select(ar=>ar.Artifact2Id));

            var isAnyArtifactExcept= artifactsRelationIds.Except(dbArtifactsId).Any();
            if (isAnyArtifactExcept) return null;

            var dbArtifactRelations = await _dataContext.ArtifactsRelation.ToListAsync();
            var isAnyArtifactRepeated = artifactRelations
                .Any(ar => dbArtifactRelations
                    .Any(dbAr => 
                        dbAr.Artifact1Id == ar.Artifact1Id
                        && dbAr.Artifact2Id == ar.Artifact2Id
                        && dbAr.Artifact1Property.Equals(ar.Artifact1Property, StringComparison.InvariantCultureIgnoreCase)
                        && dbAr.Artifact2Property.Equals(ar.Artifact2Property, StringComparison.InvariantCultureIgnoreCase)
                        )
                );
            if (isAnyArtifactRepeated) return null;

            foreach (var artifactRelation in artifactRelations)
            { 
                var mappedArtifactRelation = _mapper.Map<EntityModels.ArtifactsRelation>(artifactRelation);
                await _dataContext.ArtifactsRelation.AddAsync(mappedArtifactRelation);
                resultArtifactRelations.Add(artifactRelation);
            }

            await _dataContext.SaveChangesAsync();
            return resultArtifactRelations;
        }
    }
}