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
            var relations = new List<ArtifactsRelation>(artifactsRelations);
            var relation = new ArtifactsRelation();

            var oldRelations = _mapper.Map<IEnumerable<ArtifactsRelation>>(dbArtifactRelations);

            if (isAnUpdate) oldRelations = oldRelations.Where(rel => artifactsRelations.All(newRelation => newRelation.Id != rel.Id));

            relations.AddRange(oldRelations);

            while (relations.Skip(1).Any())
            {
                relation = relations[0];
                relations.RemoveAt(0);
                if (relations.FindAll(rel =>
                        rel.Artifact1Id == relation.Artifact1Id &&
                        rel.Artifact2Id == relation.Artifact2Id &&
                        rel.Artifact1Property.Equals(relation.Artifact1Property, StringComparison.InvariantCultureIgnoreCase) &&
                        rel.Artifact2Property.Equals(relation.Artifact2Property, StringComparison.InvariantCultureIgnoreCase) 
                        ||
                        rel.Artifact1Id == relation.Artifact2Id &&
                        rel.Artifact2Id == relation.Artifact1Id &&
                        rel.Artifact1Property.Equals(relation.Artifact2Property, StringComparison.InvariantCultureIgnoreCase) &&
                        rel.Artifact2Property.Equals(relation.Artifact1Property, StringComparison.InvariantCultureIgnoreCase)
                        ).Any())
                    return true;
            }

            return false;
        }
        /// <summary>
        /// Check if any relation is duplicate in the submitted list.
        /// </summary>
        /// <param name="artifactsRelations"></param>
        /// <returns>True if At least one of the artifact relation is duplicate </returns>
        private bool IsAnyRelationRepeated(IList<ArtifactsRelation> artifactsRelations)
        {
            return artifactsRelations.GroupBy(ar => new { ar.Artifact1Id, ar.Artifact2Id, ar.Artifact1Property, ar.Artifact2Property })
                .Where(groupAr => groupAr.Skip(1).Any())
                .Select(ar => ar.Key)
                .Any();
        }

        private IList<ArtifactsRelation> ExcludeDuplicates(IList<ArtifactsRelation> artifactsRelations)
        {
            IList<ArtifactsRelation> nonDuplicatesArtifactsRelations = new List<ArtifactsRelation>();

            nonDuplicatesArtifactsRelations = artifactsRelations.Distinct().ToList();

            return nonDuplicatesArtifactsRelations;
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

            var settingsWithValueUpdated = GetSettingsWithValueUpdated(artifact.Settings, result.Settings);

            foreach(var setting in settingsWithValueUpdated)
            {
                if(await IsSettingIsAtEndOfAnyRelation(artifact.Id, setting))
                {
                    return new ServiceResponse<Artifact>(new Error(ErrorCodes.InvalidArtifactSettings, $"You are trying to modify a setting at the of a relation"));
                }
            }

            //Updates the artifact
            result.Name = artifact.Name;
            result.Settings = artifact.Settings;
            result.ModifiedDate = DateTime.Now;
            result.ProjectId = artifact.ProjectId;
            result.ArtifactTypeId = artifact.ArtifactTypeId;

            foreach (var settingWithValueUpdated in settingsWithValueUpdated)
            {
                var relationsToUpdateResponse = await GetRelationsToUpdateAsync(artifact.Id, settingWithValueUpdated);
                var relationsToUpdate = relationsToUpdateResponse.Value;

                foreach (var relationToUpdate in relationsToUpdate)
                {
                    var success = await UpdateArtifactOfRelation(relationToUpdate);

                    if(!success)
                    {
                        return new ServiceResponse<Artifact> (new Error(ErrorCodes.InvalidArtifactSettings, "At least one setting is a string"));
                    }
                }
            }

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

        private async Task<ServiceResponse<Artifact>> AuxiliarUpdateArtifact(Artifact artifact)
        {
            //Gets the artifact associated to it from the database
            var result = await _dataContext.Artifacts
                .Include(a => a.ArtifactType)
                    .ThenInclude(at => at.Provider)
                .Include(a => a.Project)
                .SingleOrDefaultAsync(a => a.Id == artifact.Id);

            result.Settings = artifact.Settings;
            result.ModifiedDate = DateTime.Now;

            var mappedArtifact = MapArtifact(result);

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

            //Aca se deben actualizar los valores de las settings
            foreach (var artifactRelation in artifactRelations)
            {
                var success = await UpdateArtifactOfRelation(artifactRelation);

                if(!success)
                {
                    return new ServiceResponse<IList<ArtifactsRelation>>(new Error(ErrorCodes.InvalidArtifactSettings, "At least one setting is a string"));
                }
            }

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

            var success = await UpdateArtifactOfRelation(_mapper.Map<ArtifactsRelation>(artifactsRelation));

            if(!success)
            {
                return new ServiceResponse<ArtifactsRelation>(new Error(ErrorCodes.InvalidArtifactSettings, "At least one setting is a string"));
            }

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

            var relationInNewListRepeated = IsAnyRelationRepeated(artifactsRelationsNew);
            if (relationInNewListRepeated)
                return new ServiceResponse<IList<ArtifactsRelation>>(new Error(ErrorCodes.RelationNotValid,
                    "At least one of the artifact relation provided is repeat"));

            var relationsWithOriginalRepeated = IsAnyRelationRepeated(relationsOriginal, artifactsRelationsNew, isAnUpdate: true);
            if (relationsWithOriginalRepeated)
                return new ServiceResponse<IList<ArtifactsRelation>>(new Error(ErrorCodes.RelationNotValid,
                    "At least one of the artifact relation provided already exist"));

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

            foreach (var relationOriginal in relationsOriginal)
            {
                foreach (var relationNew in artifactsRelationsNew)
                {
                    if (relationOriginal.Id != relationNew.Id) continue;

                    var success = await UpdateArtifactOfRelation(relationNew);

                    if(!success)
                    {
                        return new ServiceResponse<IList<ArtifactsRelation>>(new Error(ErrorCodes.InvalidArtifactSettings, "At least one setting is a string"));
                    }
                }
            }

            await _dataContext.SaveChangesAsync();

            return new ServiceResponse<IList<ArtifactsRelation>>(artifactsRelationsNew);
        }

        private async Task<List<Guid>> GetRelationsToDeleteOfUpdatedArtifact(int artifactId, XElement updatedSettings, XElement originalSettings)
        {
            var changedSettingsName = FindSettingsWithNamesChanged(updatedSettings, originalSettings);
            var artifactsRelationsIdsToDelete = await FindRelationsToDeleteOfUpdatedArtifact(artifactId, changedSettingsName);
            return artifactsRelationsIdsToDelete;
        }

        private List<string> FindSettingsWithNamesChanged(XElement updatedSettings, XElement originalSettings)
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
        private async Task<ServiceResponse<IList<ArtifactsRelation>>> GetRelationsToUpdateAsync(int artifactId, string propertyName)
        {
            var existArtifactId = await _dataContext.Artifacts.AnyAsync(a => a.Id == artifactId);
            if (!existArtifactId)
            {
                return new ServiceResponse<IList<ArtifactsRelation>>(new Error(ErrorCodes.ArtifactNotExists, $"There is no artifact with Id = {artifactId}"));
            }

            var result = await _dataContext.ArtifactsRelation.Include(ar => ar.Artifact1)
                .Include(ar => ar.Artifact2)
                .Where(ar => ((ar.Artifact1Id == artifactId && ar.Artifact1Property.Equals(propertyName)) || ((ar.Artifact2Id == artifactId) &&
                ar.Artifact2Property.Equals(propertyName))))
                .ToListAsync();

            var resultModel = _mapper.Map<List<ArtifactsRelation>>(result);

            var relationsToUpdate = new List<ArtifactsRelation>();

            foreach(var relation in resultModel)
            {
                if(IsASettingAtBeginningOfRelation(propertyName, relation))
                {
                    relationsToUpdate.Add(relation);
                }
            }
            
            return new ServiceResponse<IList<ArtifactsRelation>>(relationsToUpdate);
        }

        private async Task<ServiceResponse<IList<ArtifactsRelation>>> GetRelationsForUpdateAsync(int artifactId, string propertyName, bool isAManipulationOfRelation)
        {
            var existArtifactId = await _dataContext.Artifacts.AnyAsync(a => a.Id == artifactId);
            if (!existArtifactId)
            {
                return new ServiceResponse<IList<ArtifactsRelation>>(new Error(ErrorCodes.ArtifactNotExists, $"There is no artifact with Id = {artifactId}"));
            }

            var result = new List<EntityModels.ArtifactsRelation>();

            if (isAManipulationOfRelation)
            {
                result = _dataContext.ArtifactsRelation.Local
                    .Where(ar => ((ar.Artifact1Id == artifactId && ar.Artifact1Property.Equals(propertyName)) || ((ar.Artifact2Id == artifactId) &&
                    ar.Artifact2Property.Equals(propertyName)))).ToList();
            }
            else
            {
                result = await _dataContext.ArtifactsRelation.Include(ar => ar.Artifact1)
                    .Include(ar => ar.Artifact2)
                    .Where(ar => ((ar.Artifact1Id == artifactId && ar.Artifact1Property.Equals(propertyName)) || ((ar.Artifact2Id == artifactId) &&
                    ar.Artifact2Property.Equals(propertyName)))).ToListAsync();
            }

            var resultModel = _mapper.Map<List<ArtifactsRelation>>(result);

            var relationsToUpdate = new List<ArtifactsRelation>();

            foreach (var relation in resultModel)
            {
                if (IsASettingAtEndOfRelation(propertyName, relation))
                {
                    relationsToUpdate.Add(relation);
                }
            }

            return new ServiceResponse<IList<ArtifactsRelation>>(relationsToUpdate);
        }

        private async Task<ServiceResponse<IList<ArtifactsRelation>>> GetRelationsByArtifactAndSetting(int artifactId, string propertyName)
        {
            var existArtifactId = await _dataContext.Artifacts.AnyAsync(a => a.Id == artifactId);
            if (!existArtifactId)
            {
                return new ServiceResponse<IList<ArtifactsRelation>>(new Error(ErrorCodes.ArtifactNotExists, $"There is no artifact with Id = {artifactId}"));
            }

            var result = await _dataContext.ArtifactsRelation.Include(ar => ar.Artifact1)
                .Include(ar => ar.Artifact2)
                .Where(ar => ((ar.Artifact1Id == artifactId && ar.Artifact1Property.Equals(propertyName)) || ((ar.Artifact2Id == artifactId) &&
                ar.Artifact2Property.Equals(propertyName))))
                .ToListAsync();

            var resultModel = _mapper.Map<List<ArtifactsRelation>>(result);

            return new ServiceResponse<IList<ArtifactsRelation>>(resultModel);
        }

        private async Task<bool> UpdateValueOfSettingRelated(int artifactToUpdateId, string settingToUpdateName, bool isAManipulationOfRelation)
        {
            var artifactToUpdateResponse = await Get(artifactToUpdateId);
            var artifactToUpdate = artifactToUpdateResponse.Value;

            var relationsForUpdateResponse = await GetRelationsForUpdateAsync(artifactToUpdateId, settingToUpdateName, isAManipulationOfRelation);
            var relationsForUpdate = relationsForUpdateResponse.Value;

            var finalValueOfSetting = 0f;

            var allValuesFromSettingsAreFloat = true;

            foreach (var relationForUpdate in relationsForUpdate)
            {
                var artifactAtTheBegin = await GetArtifactAtBeginOfRelationAsync(relationForUpdate);
                var settingAtTheBegining = GetSettingAtBeginOfRelationAsync(relationForUpdate);

                if (float.TryParse(artifactAtTheBegin.Settings.Element(settingAtTheBegining).Value, out float valueToUpdate))
                {
                    finalValueOfSetting += valueToUpdate;
                }
                else
                {
                    allValuesFromSettingsAreFloat = false;
                }
            }

            if (allValuesFromSettingsAreFloat)
            {
                artifactToUpdate.Settings.Element(settingToUpdateName).Value = finalValueOfSetting.ToString();
            }
            else
            {
                return false;
            }

            var artifactUpdatedResponse = await AuxiliarUpdateArtifact(artifactToUpdate);
            var artifactUpdated = artifactUpdatedResponse.Value;

            var relationsWithSettingsToUpdatedResponse = await GetRelationsToUpdateAsync(artifactUpdated.Id, settingToUpdateName);
            var relationsWithSettingsToUpdated = relationsWithSettingsToUpdatedResponse.Value;

            foreach (var relationToUpdate in relationsWithSettingsToUpdated)
            {
                foreach (var relationWithSettingsToUpdated in relationsWithSettingsToUpdated)
                {
                    var success = await UpdateArtifactOfRelation(relationWithSettingsToUpdated);

                    if(!success)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public async Task<bool> UpdateArtifactOfRelation(ArtifactsRelation artifactRelation)
        {
            var artifactAtTheEnd = await GetArtifactAtEndOfRelationAsync(artifactRelation);
            var settingAtTheEnd = GetSettingAtEndOfRelationAsync(artifactRelation);
            var success = await UpdateValueOfSettingRelated(artifactAtTheEnd.Id, settingAtTheEnd, false);

            return success;
        }

        public bool IsASettingAtBeginningOfRelation(string propertyName, ArtifactsRelation relation)
        {
            return (relation.RelationTypeId == 0 && relation.Artifact1Property == propertyName) || (relation.RelationTypeId == 1 && relation.Artifact2Property == propertyName);
        }

        public bool IsASettingAtEndOfRelation(string propertyName, ArtifactsRelation relation)
        {
            return (relation.RelationTypeId == 0 && relation.Artifact2Property == propertyName) || (relation.RelationTypeId == 1 && relation.Artifact1Property == propertyName);
        }

        public async Task<Artifact> GetArtifactAtBeginOfRelationAsync(ArtifactsRelation relation)
        {
            if(relation.RelationTypeId == 0)
            {
                var artifactAtBeginOfRelationResponse = await Get(relation.Artifact1Id);
                var artifactAtBeginOfRelation = artifactAtBeginOfRelationResponse.Value;
                return artifactAtBeginOfRelation;
            }
            else
            {
                var artifactAtBeginOfRelationResponse = await Get(relation.Artifact2Id);
                var artifactAtBeginOfRelation = artifactAtBeginOfRelationResponse.Value;
                return artifactAtBeginOfRelation;
            }
        }

        public async Task<Artifact> GetArtifactAtEndOfRelationAsync(ArtifactsRelation relation)
        {
            if (relation.RelationTypeId == 1)
            {
                var artifactAtBeginOfRelationResponse = await Get(relation.Artifact1Id);
                var artifactAtBeginOfRelation = artifactAtBeginOfRelationResponse.Value;
                return artifactAtBeginOfRelation;
            }
            else
            {
                var artifactAtBeginOfRelationResponse = await Get(relation.Artifact2Id);
                var artifactAtBeginOfRelation = artifactAtBeginOfRelationResponse.Value;
                return artifactAtBeginOfRelation;
            }
        }

        public string GetSettingAtBeginOfRelationAsync(ArtifactsRelation relation)
        {
            if (relation.RelationTypeId == 0)
            {
                return relation.Artifact1Property;
            }
            else
            {
                return relation.Artifact2Property;
            }
        }

        public string GetSettingAtEndOfRelationAsync(ArtifactsRelation relation)
        {
            if (relation.RelationTypeId == 1)
            {
                return relation.Artifact1Property;
            }
            else
            {
                return relation.Artifact2Property;
            }
        }

        private List<string> GetSettingsWithValueUpdated(XElement updatedSettings, XElement originalSettings)
        {
            var settingsWithValueUpdated = new List<string>();

            foreach (var setting in originalSettings.Elements())
            {
                if (!setting.HasElements && updatedSettings.Element(setting.Name) != null && !setting.Value.Equals(updatedSettings.Element(setting.Name).Value))
                {                    
                    settingsWithValueUpdated.Add(setting.Name.ToString());
                }
            }

            return settingsWithValueUpdated;
        }

        public async Task<bool> IsSettingIsAtEndOfAnyRelation(int artifactId, string settingName)
        {
            var relationsResponse = await GetRelationsByArtifactAndSetting(artifactId, settingName);
            var relations = relationsResponse.Value;

            var flag = false;
            var i = 0;

            while(i < relations.Count && !flag)
            {
                if(IsASettingAtEndOfRelation(settingName, relations[i]))
                {
                    flag = true;
                }

                i++;
            }

            return flag;
        }
    }
}
