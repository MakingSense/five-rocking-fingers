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
        public IConfiguration Configuration { get; set; }
        public DataAccessContext DataContext { get; set; }
        private readonly IMapper Mapper;

        public ArtifactsService(IConfiguration configuration, DataAccessContext dataContext, IMapper mapper)
        {
            Configuration = configuration;
            DataContext = dataContext;
            Mapper = mapper;
        }

        public List<Artifact> GetAll()
        {
            try
            {
                var result = DataContext.Artifacts.Include(a => a.ArtifactType).Include(a => a.Project).ThenInclude(p => p.ProjectCategories).ThenInclude(pc => pc.Category).ToList();
                if (result == null)
                {
                    return null;
                }
                return Mapper.Map<List<Artifact>>(result);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Artifact Get(int id)
        {
            try
            {
                var artifact = DataContext.Artifacts.Include(a => a.ArtifactType).Include(a => a.Project).ThenInclude(p => p.ProjectCategories).ThenInclude(pc => pc.Category).SingleOrDefault(a => a.Id == id);
                if (artifact == null)
                {
                    return null;
                }
                return Mapper.Map<Artifact>(artifact);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Artifact Save(Artifact artifact)
        {
            if (String.IsNullOrWhiteSpace(artifact.Name))
            {
                throw new System.ArgumentException("The artifact needs a name", "artifact.Name");
            }

            try
            {
                // Maps the artifact into an EntityModel, deleting the Id if there was one, and setting the CreatedDate field
                var mappedArtifact = Mapper.Map<EntityModels.Artifact>(artifact);
                mappedArtifact.CreatedDate = DateTime.Now;
                mappedArtifact.ModifiedDate = null;

                // Adds the artifact to the database, generating a unique Id for it
                DataContext.Artifacts.Add(mappedArtifact);

                // Saves changes
                DataContext.SaveChanges();

                return Mapper.Map<Artifact>(mappedArtifact);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Artifact Update(Artifact artifact)
        {
            if (String.IsNullOrWhiteSpace(artifact.Name))
            {
                throw new System.ArgumentException("The artifact needs a name", "artifact.Name");
            }

            try
            {
                //Gets the artifact and the project associated to it from the database
                var result = DataContext.Artifacts.Include(a => a.ArtifactType).Include(a => a.Project).Single(a => a.Id == artifact.Id);
                var project = DataContext.Projects.Include(p => p.ProjectCategories).ThenInclude(pc => pc.Category).Single(p => p.Id == artifact.ProjectId);

                if (result == null)
                {
                    throw new System.ArgumentException("There is no artifact with Id = " + artifact.Id, "artifact,Id");
                }

                //Updates the artifact
                result.Name = artifact.Name;
                result.Provider = artifact.Provider;
                result.Settings = artifact.Settings;
                result.ModifiedDate = DateTime.Now;
                result.ProjectId = project.Id;
                result.Project = DataContext.Projects.Single(p => p.Id == project.Id);
                result.ArtifactTypeId = artifact.ArtifactTypeId;
                result.ArtifactType = DataContext.ArtifactType.Single(at => at.Id == artifact.ArtifactTypeId);

                //Saves the updated aritfact in the database
                DataContext.SaveChanges();

                return Mapper.Map<Artifact>(result);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Delete(int id)
        {
            try
            {
                var artifactToDelete = DataContext.Artifacts.Include(a => a.ArtifactType).SingleOrDefault(a => a.Id == id); ;
                DataContext.Artifacts.Remove(artifactToDelete);
                DataContext.SaveChanges();
                return;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}