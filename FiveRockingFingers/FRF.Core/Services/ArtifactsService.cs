using AutoMapper;
using FRF.Core.Models;
using FRF.DataAccess;
using EntityModels = FRF.DataAccess.EntityModels;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public class ArtifactsService : IArtifactsService
    {
        private readonly IConfiguration _configuration;
        private readonly DataAccessContext _dataContext;
        private readonly IMapper _mapper;

        public ArtifactsService(IConfiguration configuration, DataAccessContext dataContext, IMapper mapper)
        {
            _configuration = configuration;
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<List<Artifact>> GetAll()
        {
            var result = await _dataContext.Artifacts.Include(a => a.ArtifactType).Include(a => a.Project).ThenInclude(p => p.ProjectCategories).ThenInclude(pc => pc.Category).ToListAsync();
            if (result == null)
            {
                return null;
            }
            return _mapper.Map<List<Artifact>>(result);
        }

        public async Task<List<Artifact>> GetAllByProjectId(int projectId)
        {
            var result = await _dataContext.Artifacts.Include(a => a.ArtifactType).Include(a => a.Project).ThenInclude(p => p.ProjectCategories).ThenInclude(pc => pc.Category).Where(a => a.ProjectId == projectId).ToListAsync();
            if (result == null)
            {
                return null;
            }
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
            //Gets the project associated to it from the database
            var project = await _dataContext.Projects.Include(p => p.ProjectCategories).ThenInclude(pc => pc.Category).SingleAsync(p => p.Id == artifact.ProjectId);
            if (project == null)
            {
                throw new System.ArgumentException("There is no project with Id = " + artifact.ProjectId, "artifact.ProjectId");
            }

            //Gets the artifactType associated to it from the database
            var artifactType = await _dataContext.ArtifactType.SingleAsync(at => at.Id == artifact.ArtifactTypeId);
            if (artifactType == null)
            {
                throw new System.ArgumentException("There is no ArtifactType with Id = " + artifact.ArtifactTypeId, "artifact.ArtifactTypeId");
            }

            // Maps the artifact into an EntityModel, deleting the Id if there was one, and setting the CreatedDate field
            var mappedArtifact = _mapper.Map<EntityModels.Artifact>(artifact);
            mappedArtifact.CreatedDate = DateTime.Now;
            mappedArtifact.ModifiedDate = null;
            mappedArtifact.ProjectId = artifact.ProjectId;
            mappedArtifact.Project = project;
            mappedArtifact.ArtifactTypeId = artifact.ArtifactTypeId;
            mappedArtifact.ArtifactType = artifactType;


            // Adds the artifact to the database, generating a unique Id for it
            await _dataContext.Artifacts.AddAsync(mappedArtifact);

            // Saves changes
            await _dataContext.SaveChangesAsync();

            return _mapper.Map<Artifact>(mappedArtifact);
        }

        public async Task<Artifact> Update(Artifact artifact)
        {
            //Gets the artifact associated to it from the database
            var result = await _dataContext.Artifacts.Include(a => a.ArtifactType).Include(a => a.Project).SingleAsync(a => a.Id == artifact.Id);
            if (result == null)
            {
                throw new System.ArgumentException("There is no artifact with Id = " + artifact.Id, "artifact,Id");
            }

            //Gets the project associated to it from the database
            var project = await _dataContext.Projects.Include(p => p.ProjectCategories).ThenInclude(pc => pc.Category).SingleAsync(p => p.Id == artifact.ProjectId);
            if (project == null)
            {
                throw new System.ArgumentException("There is no project with Id = " + artifact.ProjectId, "artifact.ProjectId");
            }

            //Gets the artifactType associated to it from the database
            var artifactType = await _dataContext.ArtifactType.SingleAsync(at => at.Id == artifact.ArtifactTypeId);
            if (artifactType == null)
            {
                throw new System.ArgumentException("There is no ArtifactType with Id = " + artifact.ArtifactTypeId, "artifact.ArtifactTypeId");
            }

            //Updates the artifact
            result.Name = artifact.Name;
            result.Provider = artifact.Provider;
            result.Settings = artifact.Settings;
            result.ModifiedDate = DateTime.Now;
            result.ProjectId = project.Id;
            result.Project = project;
            result.ArtifactTypeId = artifact.ArtifactTypeId;
            result.ArtifactType = artifactType;

            //Saves the updated aritfact in the database
            await _dataContext.SaveChangesAsync();

            return _mapper.Map<Artifact>(result);
        }

        public async Task Delete(int id)
        {
            var artifactToDelete = await _dataContext.Artifacts.Include(a => a.ArtifactType).SingleOrDefaultAsync(a => a.Id == id); ;
            _dataContext.Artifacts.Remove(artifactToDelete);
            await _dataContext.SaveChangesAsync();
            return;
        }
    }
}