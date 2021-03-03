using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Models.AwsArtifacts;
using FRF.Core.Response;
using FRF.Core.XmlValidation;
using FRF.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using EntityModels = FRF.DataAccess.EntityModels;

namespace FRF.Core.Services
{
    public class ArtifactsService : IArtifactsService
    {
        private readonly DataAccessContext _dataContext;
        private readonly IMapper _mapper;
        private readonly ISettingsValidator _settingsValidator;

        public ArtifactsService(DataAccessContext dataContext, IMapper mapper, ISettingsValidator settingsValidator)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _settingsValidator = settingsValidator;
        }

        private async Task<bool> DoArtifactsExist (IList<ArtifactsRelation> artifactsRelations)
        {
            var dbArtifactsId = await _dataContext.Artifacts.Select(a => a.Id).ToListAsync();
            var artifactsRelationIds = artifactsRelations
                .Select(ar => ar.Artifact1Id)
                .Concat(artifactsRelations.Select(ar=>ar.Artifact2Id));

            return artifactsRelationIds.Except(dbArtifactsId).Any();
        }

        private Artifact MapArtifact(EntityModels.Artifact artifact)
        {
            switch (artifact.ArtifactType.Provider.Name)
            {
                case ArtifactTypes.Custom:
                    return _mapper.Map<CustomArtifact>(artifact);
                case ArtifactTypes.Aws:
                    return MapAwsArtifact(artifact);
                default:
                    return _mapper.Map<Artifact>(artifact);
            }
        }

        private Artifact MapAwsArtifact(EntityModels.Artifact artifact)
        {
            switch (artifact.ArtifactType.Name)
            {
                case AwsS3Descriptions.Service:
                    return _mapper.Map<AwsS3>(artifact);
                case AwsEc2Descriptions.ServiceValue:
                    return _mapper.Map<AwsEc2>(artifact);
                default:
                    return _mapper.Map<Artifact>(artifact);
            }
        }

        private List<Artifact> MapArtifacts(List<EntityModels.Artifact> artifacts)
        {
            var mappedArtifacts = new List<Artifact>();

            foreach (var artifact in artifacts)
            {
                mappedArtifacts.Add(MapArtifact(artifact));
            }

            return mappedArtifacts;
        }

        /// <summary>
        /// Check if a relation already exists in the database.
        /// </summary>
        /// <param name="dbArtifactRelations"></param>
        /// <param name="artifactsRelations"></param>
        /// <param name="isAnUpdate">True if is to update, False if is a set</param>
        /// <returns>True if At least one of the artifact relation provided already exist </returns>
        private bool IsAnyRelationRepeated(IList<EntityModels.ArtifactsRelation> dbArtifactRelations,
            IList<ArtifactsRelation> artifactsRelations, bool isAnUpdate)
        {
            if (isAnUpdate)
            {
                return artifactsRelations.Any(ar => dbArtifactRelations.Any(dbAr =>
                    (dbAr.Artifact1Id == ar.Artifact1Id && dbAr.Artifact2Id == ar.Artifact2Id &&
                     dbAr.Artifact1Property.Equals(ar.Artifact1Property, StringComparison.InvariantCultureIgnoreCase) &&
                     dbAr.Artifact2Property.Equals(ar.Artifact2Property, StringComparison.InvariantCultureIgnoreCase) &&
                     dbAr.RelationTypeId == ar.RelationTypeId)
                    ||
                    (dbAr.Artifact1Id == ar.Artifact2Id && dbAr.Artifact2Id == ar.Artifact1Id &&
                     dbAr.Artifact1Property.Equals(ar.Artifact2Property, StringComparison.InvariantCultureIgnoreCase) &&
                     dbAr.Artifact2Property.Equals(ar.Artifact1Property, StringComparison.InvariantCultureIgnoreCase) &&
                     dbAr.RelationTypeId == ar.RelationTypeId)));
            }

            return  artifactsRelations.Any(ar => dbArtifactRelations.Any(dbAr =>
                (dbAr.Artifact1Id == ar.Artifact1Id && dbAr.Artifact2Id == ar.Artifact2Id &&
                 dbAr.Artifact1Property.Equals(ar.Artifact1Property, StringComparison.InvariantCultureIgnoreCase) &&
                 dbAr.Artifact2Property.Equals(ar.Artifact2Property, StringComparison.InvariantCultureIgnoreCase))
                ||
                (dbAr.Artifact1Id == ar.Artifact2Id && dbAr.Artifact2Id == ar.Artifact1Id &&
                 dbAr.Artifact1Property.Equals(ar.Artifact2Property, StringComparison.InvariantCultureIgnoreCase) &&
                 dbAr.Artifact2Property.Equals(ar.Artifact1Property, StringComparison.InvariantCultureIgnoreCase))));
        }

        public async Task<ServiceResponse<List<Artifact>>> GetAll()
        {
            var artifacts = await _dataContext.Artifacts
                .Include(a => a.ArtifactType)
                    .ThenInclude(a => a.Provider)
                .Include(a => a.Project)
                    .ThenInclude(p => p.ProjectCategories)
                        .ThenInclude(pc => pc.Category)
                .ToListAsync();

            var mappedArtifacts = MapArtifacts(artifacts);
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
                    .ThenInclude(a => a.Provider)
                .Include(a => a.Project)
                    .ThenInclude(p => p.ProjectCategories)
                        .ThenInclude(pc => pc.Category)
                .Where(a => a.ProjectId == projectId)
                .ToListAsync();

            var mappedArtifacts = MapArtifacts(artifacts);

            return new ServiceResponse<List<Artifact>>(mappedArtifacts);
        }

        public async Task<ServiceResponse<Artifact>> Get(int id)
        {
            var artifact = await _dataContext.Artifacts
                .Include(a => a.ArtifactType)
                    .ThenInclude(a => a.Provider)
                .Include(a => a.Project)
                    .ThenInclude(p => p.ProjectCategories)
                        .ThenInclude(pc => pc.Category)
                .SingleOrDefaultAsync(a => a.Id == id);

            if (artifact == null)
            {
                return new ServiceResponse<Artifact>(new Error(ErrorCodes.ArtifactNotExists, $"There is no artifact with Id = {id}"));
            }

            var mappedArtifact = MapArtifact(artifact);
            return new ServiceResponse<Artifact>(mappedArtifact);
        }

        public async Task<ServiceResponse<Artifact>> Save(Artifact artifact)
        {
            if(! await _dataContext.Projects.AnyAsync(p => p.Id == artifact.ProjectId))
            {
                return new ServiceResponse<Artifact>(new Error(ErrorCodes.ProjectNotExists, $"There is no project with Id = {artifact.ProjectId}"));
            }

            var artifactType = await _dataContext.ArtifactType.Include(at => at.Provider).SingleOrDefaultAsync(at => at.Id == artifact.ArtifactTypeId);

            if (artifactType == null)
            {
                return new ServiceResponse<Artifact>(new Error(ErrorCodes.ArtifactTypeNotExists, $"There is no artifact type with Id = {artifact.ArtifactTypeId}"));
            }

            artifact.ArtifactType = _mapper.Map<ArtifactType>(artifactType);

            if (!_settingsValidator.ValidateSettings(artifact))
            {
                return new ServiceResponse<Artifact>(new Error(ErrorCodes.InvalidArtifactSettings, $"Settings are invalid"));
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

            var response = await _dataContext.Artifacts
                .Include(a => a.ArtifactType)
                    .ThenInclude(a => a.Provider)
                .Include(a => a.Project)
                    .ThenInclude(p => p.ProjectCategories)
                        .ThenInclude(pc => pc.Category)
                .SingleOrDefaultAsync(a => a.Id == mappedArtifact.Id);

            return new ServiceResponse<Artifact>(MapArtifact(response));
        }

        public async Task<ServiceResponse<Artifact>> Update(Artifact artifact)
        {
            //Gets the artifact associated to it from the database
            var result = await _dataContext.Artifacts
                .Include(a => a.ArtifactType)
                    .ThenInclude(at => at.Provider)
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

            var artifactType = await _dataContext.ArtifactType.Include(at => at.Provider).SingleOrDefaultAsync(at => at.Id == artifact.ArtifactTypeId);

            if (artifactType == null)
            {
                return new ServiceResponse<Artifact>(new Error(ErrorCodes.ArtifactTypeNotExists, $"There is no artifact type with Id = {artifact.ArtifactTypeId}"));
            }

            artifact.ArtifactType = _mapper.Map<ArtifactType>(artifactType);

            if (!_settingsValidator.ValidateSettings(artifact))
            {
                return new ServiceResponse<Artifact>(new Error(ErrorCodes.InvalidArtifactSettings, $"Settings are invalid"));
            }

            var artifactsRelationsIdsToDelete = await GetRelationsToDeleteOfUpdatedArtifact(artifact.Id, artifact.Settings, result.Settings);

            var artifactsRelationsToDelete = await _dataContext.ArtifactsRelation.Where(ar => artifactsRelationsIdsToDelete.Contains(ar.Id)).ToListAsync();

            _dataContext.ArtifactsRelation.RemoveRange(artifactsRelationsToDelete);

            //Updates the artifact
            result.Name = artifact.Name;
            result.Settings = artifact.Settings;
            result.ModifiedDate = DateTime.Now;
            result.ProjectId = artifact.ProjectId;
            result.ArtifactTypeId = artifact.ArtifactTypeId;

            //Saves the updated aritfact in the database
            await _dataContext.SaveChangesAsync();

            var response = await _dataContext.Artifacts
                .Include(a => a.ArtifactType)
                    .ThenInclude(a => a.Provider)
                .Include(a => a.Project)
                    .ThenInclude(p => p.ProjectCategories)
                        .ThenInclude(pc => pc.Category)
                .SingleOrDefaultAsync(a => a.Id == result.Id);

            var mappedArtifact = MapArtifact(response);
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

            var mappedArtifact = MapArtifact(artifactToDelete);
            return new ServiceResponse<Artifact>(mappedArtifact);
        }

        public async Task<ServiceResponse<IList<ArtifactsRelation>>> SetRelationAsync(IList<ArtifactsRelation> artifactRelations)
        {
            var artifactsExist = await DoArtifactsExist(artifactRelations);
            if (artifactsExist) return new ServiceResponse<IList<ArtifactsRelation>>(new Error(ErrorCodes.RelationNotValid, "At least one of the artifact Ids provided doesn't exist"));

            var dbArtifactRelations = await _dataContext.ArtifactsRelation.Where(ar =>
                    ar.Artifact1Id == artifactRelations[0].Artifact1Id ||
                    ar.Artifact2Id == artifactRelations[0].Artifact2Id ||
                    ar.Artifact2Id == artifactRelations[0].Artifact1Id ||
                    ar.Artifact1Id == artifactRelations[0].Artifact2Id)
                .ToListAsync();

            var relationsRepeated = IsAnyRelationRepeated(dbArtifactRelations, artifactRelations,isAnUpdate: false);
            if (relationsRepeated)
                return new ServiceResponse<IList<ArtifactsRelation>>(new Error(ErrorCodes.RelationAlreadyExisted, "At least one of the relations already existed"));

            var resultArtifactRelations = _mapper.Map<IList<EntityModels.ArtifactsRelation>>(artifactRelations);
            await _dataContext.ArtifactsRelation.AddRangeAsync(resultArtifactRelations);
            await _dataContext.SaveChangesAsync();

            return new ServiceResponse<IList<ArtifactsRelation>>(_mapper.Map<IList<ArtifactsRelation>>(resultArtifactRelations));
        }

        public async Task<ServiceResponse<IList<ArtifactsRelation>>> GetAllRelationsOfAnArtifactAsync(int artifactId)
        {
            var existArtifactId = await _dataContext.Artifacts.AnyAsync(a => a.Id == artifactId);
            if (!existArtifactId)
            {
                return new ServiceResponse<IList<ArtifactsRelation>>(new Error(ErrorCodes.ArtifactNotExists, $"There is no artifact with Id = {artifactId}"));
            }

            var result = await _dataContext.ArtifactsRelation.Include(ar => ar.Artifact1)
                .Include(ar => ar.Artifact2)
                .Where(ar => ar.Artifact1Id == artifactId || ar.Artifact2Id == artifactId)
                .ToListAsync();

            var resultArtifactRelations = _mapper.Map<List<ArtifactsRelation>>(result);
            return new ServiceResponse<IList<ArtifactsRelation>>(resultArtifactRelations);
        }

        public async Task<ServiceResponse<IList<ArtifactsRelation>>> GetAllRelationsByProjectIdAsync(int projectId)
        {
            var existProjectId = await _dataContext.Projects.AnyAsync(p => p.Id == projectId);
            if (!existProjectId)
            {
                return new ServiceResponse<IList<ArtifactsRelation>>(new Error(ErrorCodes.ProjectNotExists, $"There is no project with Id = {projectId}"));
            }

            var artifactsRelations = await _dataContext.ArtifactsRelation
                .Where(ar => ar.Artifact1.ProjectId == projectId || ar.Artifact2.ProjectId == projectId)
                .Include(ar => ar.Artifact1)
                .Include(ar => ar.Artifact2)
                .ToListAsync();
            var resultArtifactRelations = _mapper.Map<List<ArtifactsRelation>>(artifactsRelations);
            return new ServiceResponse<IList<ArtifactsRelation>>(resultArtifactRelations);
        }

        public async Task<ServiceResponse<ArtifactsRelation>> DeleteRelationAsync(Guid artifactRelationId)
        {
            var artifactsRelation = await _dataContext.ArtifactsRelation
                .FirstOrDefaultAsync(ar => ar.Id.Equals(artifactRelationId));
            if (artifactsRelation == null)
            {
                return new ServiceResponse<ArtifactsRelation>(new Error(ErrorCodes.RelationNotExists, $"There is no relation with Id={artifactRelationId}"));
            }
            _dataContext.ArtifactsRelation.Remove(artifactsRelation);
            await _dataContext.SaveChangesAsync();

            return new ServiceResponse<ArtifactsRelation>(_mapper.Map<ArtifactsRelation>(artifactsRelation));
        }

        public async Task<ServiceResponse<IList<ArtifactsRelation>>> DeleteRelationsAsync(IList<Guid> artifactRelationIds)
        {
            var relationsIdFromDb = await _dataContext.ArtifactsRelation.Select(ar => ar.Id).ToListAsync();
            var relationsThatDoNotExist = artifactRelationIds.Except(relationsIdFromDb).ToList();
            if (relationsThatDoNotExist.Any())
            {
                return new ServiceResponse<IList<ArtifactsRelation>>(new Error(ErrorCodes.RelationNotExists,
                    $"There is no relation with Id={string.Join(", Id=", relationsThatDoNotExist)}"));
            }

            var artifactsRelations = await _dataContext.ArtifactsRelation.Where(ar => artifactRelationIds.Contains(ar.Id)).ToListAsync();

            _dataContext.ArtifactsRelation.RemoveRange(artifactsRelations);

            await _dataContext.SaveChangesAsync();

            return new ServiceResponse<IList<ArtifactsRelation>>(_mapper.Map<IList<ArtifactsRelation>>(artifactsRelations));
        }

        public async Task<ServiceResponse<IList<ArtifactsRelation>>> UpdateRelationAsync(int artifactId,
            IList<ArtifactsRelation> artifactsRelationsNew)
        {
            var existArtifactId = await _dataContext.Artifacts.AnyAsync(a => a.Id == artifactId);
            if (!existArtifactId)
                return new ServiceResponse<IList<ArtifactsRelation>>(new Error(ErrorCodes.ArtifactNotExists, $"There is no artifact with Id = {artifactId}"));

            var artifactsExist = await DoArtifactsExist(artifactsRelationsNew);
            if (artifactsExist)
                return new ServiceResponse<IList<ArtifactsRelation>>(new Error(ErrorCodes.ArtifactNotExists,
                    "At least one of the artifact Ids provided doesn't exist"));

            var relationsOriginal = await _dataContext.ArtifactsRelation
                .Where(ar => ar.Artifact1Id == artifactId || ar.Artifact2Id == artifactId)
                .Include(ar => ar.Artifact1)
                .Include(ar => ar.Artifact2)
                .ToListAsync();

            var relationsWithOriginalRepeated = IsAnyRelationRepeated(relationsOriginal, artifactsRelationsNew,isAnUpdate: true);
            if (relationsWithOriginalRepeated)
                return new ServiceResponse<IList<ArtifactsRelation>>(new Error(ErrorCodes.RelationNotValid,
                    "At least one of the artifact relation provided already exist"));

            var relationInNewListRepeated = artifactsRelationsNew.GroupBy(ar => ar)
                .Where(groupAr => groupAr.Count() > 1)
                .Select(ar => ar.Key)
                .Any();
            if (relationInNewListRepeated)
                return new ServiceResponse<IList<ArtifactsRelation>>(new Error(ErrorCodes.RelationNotValid,
                    "At least one of the artifact relation provided is repeated"));

            foreach (var relationOriginal in relationsOriginal)
            {
                foreach (var relationNew in artifactsRelationsNew)
                {
                    if (relationOriginal.Id != relationNew.Id) continue;

                    relationOriginal.Artifact1Id = relationNew.Artifact1Id;
                    relationOriginal.Artifact2Id = relationNew.Artifact2Id;
                    relationOriginal.Artifact1Property = relationNew.Artifact1Property;
                    relationOriginal.Artifact2Property = relationNew.Artifact2Property;
                    relationOriginal.RelationTypeId = relationNew.RelationTypeId;
                }
            }

            await _dataContext.SaveChangesAsync();

            return new ServiceResponse<IList<ArtifactsRelation>>(artifactsRelationsNew);
        }

        private async Task<List<Guid>> GetRelationsToDeleteOfUpdatedArtifact(int artifactId, XElement updatedSettings, XElement originalSettings)
        {
            var changedSettingsName = FindSettingsNamesChanged(updatedSettings, originalSettings);
            var artifactsRelationsIdsToDelete = await FindRelationsToDeleteOfUpdatedArtifact(artifactId, changedSettingsName);
            return artifactsRelationsIdsToDelete;
        }

        private List<string> FindSettingsNamesChanged(XElement updatedSettings, XElement originalSettings)
        {
            var changedSettingsName = new List<string>();

            foreach(var setting in originalSettings.Elements())
            {
                if(!setting.HasElements && !updatedSettings.Elements(setting.Name).Any())
                {
                    changedSettingsName.Add(setting.Name.ToString());
                }
            }

            return changedSettingsName;
        }

        private async Task<List<Guid>> FindRelationsToDeleteOfUpdatedArtifact(int artifactId, List<string> changedSettingsName)
        {
            var relationsResponse = await GetAllRelationsOfAnArtifactAsync(artifactId);
            var relations = relationsResponse.Value;

            var artifactsRelationsIdsToDelete = new List<Guid>();

            foreach (var relation in relations)
            {
                if(changedSettingsName.Contains(relation.Artifact1Property) || changedSettingsName.Contains(relation.Artifact2Property))
                {
                    artifactsRelationsIdsToDelete.Add(relation.Id);
                }
            }

            return artifactsRelationsIdsToDelete;
        }
    }
}
