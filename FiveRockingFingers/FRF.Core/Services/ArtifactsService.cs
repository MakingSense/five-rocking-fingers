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
            if (artifactsExist) return new ServiceResponse<IList<ArtifactsRelation>>(new Error(ErrorCodes.ArtifactNotExists, "At least one of the artifact Ids provided doesn't exist"));

            var dbArtifactRelations = await _dataContext.ArtifactsRelation.Where(ar =>
                    ar.Artifact1Id == artifactRelations[0].Artifact1Id ||
                    ar.Artifact2Id == artifactRelations[0].Artifact2Id ||
                    ar.Artifact2Id == artifactRelations[0].Artifact1Id ||
                    ar.Artifact1Id == artifactRelations[0].Artifact2Id)
                .ToListAsync();

            var relationsRepeated = IsAnyRelationRepeated(dbArtifactRelations, artifactRelations, isAnUpdate: false);
            if (relationsRepeated)
                return new ServiceResponse<IList<ArtifactsRelation>>(new Error(ErrorCodes.RelationAlreadyExisted, "At least one of the relations already existed"));

            var cycleDetected = await IsCircularReferenceAsync(artifactRelations, false);
            if (cycleDetected)
                return new ServiceResponse<IList<ArtifactsRelation>>(new Error(ErrorCodes.RelationCycleDetected, "These relations would generate at least one cycle"));

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

            var relationsWithOriginalRepeated = IsAnyRelationRepeated(relationsOriginal, artifactsRelationsNew, isAnUpdate: true);
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

            var cycleDetected = await IsCircularReferenceAsync(artifactsRelationsNew, true);
            if (cycleDetected)
                return new ServiceResponse<IList<ArtifactsRelation>>(new Error(ErrorCodes.RelationCycleDetected, "These relations would generate at least one cycle"));

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

        private async Task<bool> IsCircularReferenceAsync(IList<ArtifactsRelation> newRelations, bool isUpdate)
        {
            // Detecting cycles between relations of a project, using the pair "artifactId - setting name" as the vertices of a graph,
            // and the relations as the links. To search for the cycle, we're using depth first search (DFS)

            // If there are no new relations there can't new cycles introduced
            if (newRelations.Count == 0)
            {
                return false;
            }

            // Getting all the project relations. This might be modified in the future, to pass them via parameter.
            // If this fails, there is no way to confirm there won't be any cycles, so the function returns true to avoid problems.
            var projectId = _dataContext.Artifacts.Single(art => art.Id == newRelations[0].Artifact1Id).Project.Id;
            var dbRelations = await GetAllRelationsByProjectIdAsync(projectId);
            if (!dbRelations.Success)
            {
                return true;
            }
            var oldRelations = _mapper.Map<IEnumerable<ArtifactsRelation>>(dbRelations.Value);

            // If it's an update, the old relations (the modified ones) are removed from the list
            // so that there are no dupplicates once we add the new ones.
            if (isUpdate)
            {
                oldRelations = oldRelations.Where(rel => !newRelations.Where(updatedRelation => updatedRelation.Id == rel.Id).Any());
            }

            // Creating one list with all the relations
            var relations = new List<ArtifactsRelation>(oldRelations);
            relations.AddRange(newRelations);

            var vertices = new List<Tuple<int, string, int>>(); // ArtifactId, setting name, index
            var visited = new List<bool>(); // Using the index from the tuple, each position tells if the vertex has been visited
            var exploring = new List<bool>(); // Using the index from the tuple, each position tells if the vertex is currently being explored

            // Creating every vertex from the ones in the relation list. We need every pair "artifactId - setting name" from the relations.
            foreach (var relation in relations)
            {
                if (!vertices.Any(s => s.Item1 == relation.Artifact1Id && s.Item2 == relation.Artifact1Property))
                {
                    vertices.Add(new Tuple<int, string, int>(relation.Artifact1Id, relation.Artifact1Property, vertices.Count));
                    visited.Add(false);
                    exploring.Add(false);
                }
                if (!vertices.Any(s => s.Item1 == relation.Artifact2Id && s.Item2 == relation.Artifact2Property))
                {
                    vertices.Add(new Tuple<int, string, int>(relation.Artifact2Id, relation.Artifact2Property, vertices.Count));
                    visited.Add(false);
                    exploring.Add(false);
                }
            }

            // Cycle through the vertices until they are all visited.
            foreach (var vertex in vertices)
            {
                if (!visited[vertex.Item3])
                {
                    var cicleDetected = CycleDetect(vertices, vertex, visited, exploring, relations);
                    if (cicleDetected) return true;
                }
            }

            return false;
        }

        private bool CycleDetect(IList<Tuple<int, string, int>> vertices, Tuple<int, string, int> vertex, IList<bool> visited, IList<bool> exploring, IList<ArtifactsRelation> relations)
        {
            // If the the vertex was already visited, and all the paths from it were completed without cycles, return false, there's no need to check them again.
            if (visited[vertex.Item3]) return false;
            // If the current vertex is already being explored, that means we're in a cycle.
            if (exploring[vertex.Item3]) return true;

            // Set current vertex as being explored.
            exploring[vertex.Item3] = true;

            // Create a list of the next vertices (the ones that this vertex can reach).
            // If it's a RelationTypeId == 0 and this vertex is in the first position, it can reach the second one.
            // If it's a RelationTypeId == 2 and this vertex is in the second position, it can reach the first one.
            var selectedRelations = relations.Where(rel => (rel.Artifact1Id == vertices[vertex.Item3].Item1 && rel.Artifact1Property == vertices[vertex.Item3].Item2 && rel.RelationTypeId == 0) ||
                                                           (rel.Artifact2Id == vertices[vertex.Item3].Item1 && rel.Artifact2Property == vertices[vertex.Item3].Item2 && rel.RelationTypeId == 1));

            var nextVertices = new List<Tuple<int, string, int>>();
            foreach (var relation in selectedRelations)
            {
                if (relation.RelationTypeId == 0)
                {
                    nextVertices.Add(vertices.First(v => v.Item1 == relation.Artifact2Id && v.Item2 == relation.Artifact2Property));
                }
                else
                {
                    nextVertices.Add(vertices.First(v => v.Item1 == relation.Artifact1Id && v.Item2 == relation.Artifact1Property));
                }
            }

            // Cycle through the next vertices (the ones that this current vertex can reach)
            foreach (var nextVertex in nextVertices)
            {
                if (!visited[nextVertex.Item3])
                {
                    var cicleDetected = CycleDetect(vertices, nextVertex, visited, exploring, relations);
                    if (cicleDetected) return true;
                }
            }

            // Once every path starting in this node is completed, the current vertex can be set as visited, and finish it's exploration.
            visited[vertex.Item3] = true;
            exploring[vertex.Item3] = false;

            return false;
        }
    }
}
