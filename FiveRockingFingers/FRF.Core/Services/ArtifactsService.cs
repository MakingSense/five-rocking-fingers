using AutoMapper;
using FRF.Core.Models;
using FRF.DataAccess;
using EntityModels = FRF.DataAccess.EntityModels;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FRF.Core.Services
{
    public class ArtifactsService : IArtifactsService
    {
        private IConfiguration _configuration;
        private DataAccessContext _dataContext;
        private readonly IMapper _mapper;

        public ArtifactsService(IConfiguration configuration, DataAccessContext dataContext, IMapper mapper)
        {
            _configuration = configuration;
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public List<Artifact> GetAll()
        {
            var result = _dataContext.Artifacts.Include(a => a.ArtifactType).Include(a => a.Project).ThenInclude(p => p.ProjectCategories).ThenInclude(pc => pc.Category).ToList();
            if (result == null)
            {
                return null;
            }
            return _mapper.Map<List<Artifact>>(result);
        }

        public Artifact Get(int id)
        {
            var artifact = _dataContext.Artifacts.Include(a => a.ArtifactType).Include(a => a.Project).ThenInclude(p => p.ProjectCategories).ThenInclude(pc => pc.Category).SingleOrDefault(a => a.Id == id);
            if (artifact == null)
            {
                return null;
            }
            return _mapper.Map<Artifact>(artifact);
        }

        public Artifact Save(Artifact artifact)
        {
            if (String.IsNullOrWhiteSpace(artifact.Name))
            {
                throw new System.ArgumentException("The artifact needs a name", "artifact.Name");
            }

            //Gets the project associated to it from the database
            var project = _dataContext.Projects.Include(p => p.ProjectCategories).ThenInclude(pc => pc.Category).Single(p => p.Id == artifact.ProjectId);
            if (project == null)
            {
                throw new System.ArgumentException("There is no project with Id = " + artifact.ProjectId, "artifact.ProjectId");
            }

            //Gets the artifactType associated to it from the database
            var artifactType = _dataContext.ArtifactType.Single(at => at.Id == artifact.ArtifactTypeId);
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
            _dataContext.Artifacts.Add(mappedArtifact);

            // Saves changes
            _dataContext.SaveChanges();

            return _mapper.Map<Artifact>(mappedArtifact);
        }

        public Artifact Update(Artifact artifact)
        {
            if (String.IsNullOrWhiteSpace(artifact.Name))
            {
                throw new System.ArgumentException("The artifact needs a name", "artifact.Name");
            }

            //Gets the artifact associated to it from the database
            var result = _dataContext.Artifacts.Include(a => a.ArtifactType).Include(a => a.Project).Single(a => a.Id == artifact.Id);
            if (result == null)
            {
                throw new System.ArgumentException("There is no artifact with Id = " + artifact.Id, "artifact,Id");
            }

            //Gets the project associated to it from the database
            var project = _dataContext.Projects.Include(p => p.ProjectCategories).ThenInclude(pc => pc.Category).Single(p => p.Id == artifact.ProjectId);
            if (project == null)
            {
                throw new System.ArgumentException("There is no project with Id = " + artifact.ProjectId, "artifact.ProjectId");
            }

            //Gets the artifactType associated to it from the database
            var artifactType = _dataContext.ArtifactType.Single(at => at.Id == artifact.ArtifactTypeId);
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
            _dataContext.SaveChanges();

            return _mapper.Map<Artifact>(result);
        }

        public void Delete(int id)
        {
            var artifactToDelete = _dataContext.Artifacts.Include(a => a.ArtifactType).SingleOrDefault(a => a.Id == id); ;
            _dataContext.Artifacts.Remove(artifactToDelete);
            _dataContext.SaveChanges();
            return;
        }
    }
}