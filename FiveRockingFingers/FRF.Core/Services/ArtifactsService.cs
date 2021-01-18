using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Response;
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

        public async Task<ServiceResponse<List<Artifact>>> GetAll()
        {
            var artifacts = await _dataContext.Artifacts
                .Include(a => a.ArtifactType)
                .Include(a => a.Project)
                    .ThenInclude(p => p.ProjectCategories)
                        .ThenInclude(pc => pc.Category)
                .ToListAsync();

            var mappedArtifacts = _mapper.Map<List<Artifact>>(artifacts);
            return new ServiceResponse<List<Artifact>>(mappedArtifacts);
        }

        public async Task<ServiceResponse<List<Artifact>>> GetAllByProjectId(int projectId)
        {
            if (!await _dataContext.Projects.AnyAsync(p => p.Id == projectId))
            {
                return new ServiceResponse<List<Artifact>>(new Error(ErrorCodes.ProjectNotExists, $"There is no project with Id = {projectId}"));
            }

            var artifacts = await _dataContext.Artifacts
                .Include(a => a.ArtifactType)
                .Include(a => a.Project)
                    .ThenInclude(p => p.ProjectCategories)
                        .ThenInclude(pc => pc.Category)
                .Where(a => a.ProjectId == projectId)
                .ToListAsync();

            var mappedArtifacts = _mapper.Map<List<Artifact>>(artifacts);
            return new ServiceResponse<List<Artifact>>(mappedArtifacts);
        }

        public async Task<ServiceResponse<Artifact>> Get(int id)
        {
            var artifact = await _dataContext.Artifacts
                .Include(a => a.ArtifactType)
                .Include(a => a.Project)
                    .ThenInclude(p => p.ProjectCategories)
                        .ThenInclude(pc => pc.Category)
                .SingleOrDefaultAsync(a => a.Id == id);

            if (artifact == null)
            {
                return new ServiceResponse<Artifact>(new Error(ErrorCodes.ArtifactNotExists, $"There is no artifact with Id = {id}"));
            }

            var mappedArtifact = _mapper.Map<Artifact>(artifact);
            return new ServiceResponse<Artifact>(mappedArtifact);
        }

        public async Task<ServiceResponse<Artifact>> Save(Artifact artifact)
        {
            if(! await _dataContext.Projects.AnyAsync(p => p.Id == artifact.ProjectId))
            {
                return new ServiceResponse<Artifact>(new Error(ErrorCodes.ProjectNotExists, $"There is no project with Id = {artifact.ProjectId}"));
            }

            if(! await _dataContext.ArtifactType.AnyAsync(at => at.Id == artifact.ArtifactTypeId))
            {
                return new ServiceResponse<Artifact>(new Error(ErrorCodes.ArtifactTypeNotExists, $"There is no artifact type with Id = {artifact.ArtifactTypeId}"));
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

            return new ServiceResponse<Artifact>(_mapper.Map<Artifact>(mappedArtifact));
        }

        public async Task<ServiceResponse<Artifact>> Update(Artifact artifact)
        {
            //Gets the artifact associated to it from the database
            var result = await _dataContext.Artifacts
                .Include(a => a.ArtifactType)
                .Include(a => a.Project)
                .SingleOrDefaultAsync(a => a.Id == artifact.Id);

            if (result == null)
            {
                return new ServiceResponse<Artifact>(new Error(ErrorCodes.ArtifactNotExists, $"There is no artifact with Id = {artifact.Id}"));
            }

            if (! await _dataContext.Projects.AnyAsync(p => p.Id == artifact.ProjectId))
            {
                return new ServiceResponse<Artifact>(new Error(ErrorCodes.ProjectNotExists, $"There is no project with Id = {artifact.ProjectId}"));
            }

            if (!await _dataContext.ArtifactType.AnyAsync(at => at.Id == artifact.ArtifactTypeId))
            {
                return new ServiceResponse<Artifact>(new Error(ErrorCodes.ArtifactTypeNotExists, $"There is no artifact type with Id = {artifact.ArtifactTypeId}"));
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

            var mappedArtifact = _mapper.Map<Artifact>(result);
            return new ServiceResponse<Artifact>(mappedArtifact);
        }

        public async Task<ServiceResponse<Artifact>> Delete(int id)
        {
            var artifactToDelete = await _dataContext.Artifacts.SingleOrDefaultAsync(a => a.Id == id);
            if (artifactToDelete == null)
            {
                return new ServiceResponse<Artifact>(new Error(ErrorCodes.ArtifactNotExists, $"There is no artifact with Id = {id}"));
            }

            _dataContext.Artifacts.Remove(artifactToDelete);
            await _dataContext.SaveChangesAsync();

            var mappedArtifact = _mapper.Map<Artifact>(artifactToDelete);
            return new ServiceResponse<Artifact>(mappedArtifact);
        }

        public async Task<ServiceResponse<IList<ArtifactsRelation>>> SetRelationAsync(IList<ArtifactsRelation> artifactRelations)
        {
            var resultArtifactRelations = new List<ArtifactsRelation>();
            var dbArtifactsId = await _dataContext.Artifacts.Select(a => a.Id).ToListAsync();
            var artifactsRelationIds = artifactRelations
                .Select(ar => ar.Artifact1Id)
                .Concat(artifactRelations.Select(ar=>ar.Artifact2Id));

            var isAnyArtifactExcept= artifactsRelationIds.Except(dbArtifactsId).Any();
            if (isAnyArtifactExcept) 
                return new ServiceResponse<IList<ArtifactsRelation>>(new Error(ErrorCodes.RelationNotValid, "At least one of the artifact Ids provided doesn't exist"));

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
            if (isAnyArtifactRepeated)
                return new ServiceResponse<IList<ArtifactsRelation>>(new Error(ErrorCodes.RelationAlreadyExisted, "At least one of the relations already existed"));

            foreach (var artifactRelation in artifactRelations)
            { 
                var mappedArtifactRelation = _mapper.Map<EntityModels.ArtifactsRelation>(artifactRelation);
                await _dataContext.ArtifactsRelation.AddAsync(mappedArtifactRelation);
                resultArtifactRelations.Add(artifactRelation);
            }

            await _dataContext.SaveChangesAsync();
            return new ServiceResponse<IList<ArtifactsRelation>>(resultArtifactRelations);
        }
    }
}