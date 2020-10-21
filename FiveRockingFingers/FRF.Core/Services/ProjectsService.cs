using AutoMapper;
using FRF.Core.Models;
using FRF.DataAccess;
using EntityModels = FRF.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ValidationContext = AutoMapper.ValidationContext;

namespace FRF.Core.Services
{
    public class ProjectsService : IProjectsService
    {
        public IConfiguration Configuration { get; set; }
        public DataAccessContext DataContext { get; set; }
        private readonly IMapper Mapper;

        public ProjectsService(IConfiguration configuration, DataAccessContext dataContext, IMapper mapper)
        {
            Configuration = configuration;
            DataContext = dataContext;
            Mapper = mapper;
        }

        public List<Project> GetAll()
        {
            try
            {
                var result = DataContext.Projects.Include(p => p.ProjectCategories).ThenInclude(pc => pc.Category)
                    .ToList();
                if (result == null)
                {
                    return null;
                }

                return Mapper.Map<List<Project>>(result);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<Project>> GetAllByUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            try
            {
                var result =await DataContext.Projects.Where(p=>p.UserId == userId).Include(p => p.ProjectCategories).ThenInclude(pc => pc.Category).ToListAsync();
                if (result == null)
                {
                    return null;
                }

                return Mapper.Map<List<Project>>(result);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Project Save(Project project)
        {
            if (String.IsNullOrWhiteSpace(project.Name))
            {
                throw new System.ArgumentException("The project needs a name", "project.Name");
            }

            try
            {
                // Gets the categories from the database on a list
                var categoryList = new List<EntityModels.Category>();
                foreach (var category in project.ProjectCategories)
                {
                    categoryList.Add(DataContext.Categories.Single(c => c.Id == category.Category.Id));
                }

                // Maps the project into an EntityModel, deleting the Id if there was one, and the list projectCategories
                var mappedProject = Mapper.Map<EntityModels.Project>(project);
                mappedProject.ProjectCategories = new List<EntityModels.ProjectCategory>();
                mappedProject.CreatedDate = DateTime.Now;
                mappedProject.ModifiedDate = null;

                // Adds the project to the database, generating a unique Id for it
                DataContext.Projects.Add(mappedProject);

                // Generates the relationships between the categories and the project using the project Id
                foreach (var category in categoryList)
                {
                    var pc = new EntityModels.ProjectCategory();
                    pc.Category = category;
                    pc.CategoryID = category.Id;
                    pc.Project = mappedProject;
                    pc.ProjectId = mappedProject.Id;
                    mappedProject.ProjectCategories.Add(pc);
                }

                // Saves changes
                DataContext.SaveChanges();

                return Mapper.Map<Project>(mappedProject);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Project Get(int id)
        {
            try
            {
                var project = DataContext.Projects.Include(p => p.ProjectCategories).ThenInclude(pc => pc.Category)
                    .SingleOrDefault(p => p.Id == id);
                if (project == null)
                {
                    return null;
                }

                return Mapper.Map<Project>(project);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Project Update(Project project)
        {
            if (String.IsNullOrWhiteSpace(project.Name))
            {
                throw new System.ArgumentException("The project needs a name", "project.Name");
            }

            try
            {
                var categoryList = new List<EntityModels.Category>();
                foreach (var category in project.ProjectCategories)
                {
                    categoryList.Add(DataContext.Categories.Single(c => c.Id == category.Category.Id));
                }

                var result = DataContext.Projects.Include(p => p.ProjectCategories).ThenInclude(pc => pc.Category)
                    .Single(p => p.Id == project.Id);

                if (result == null)
                {
                    throw new System.ArgumentException("There is no project with Id = " + project.Id, "project,Id");
                }

                result.Name = project.Name;
                result.Owner = project.Owner;
                result.Client = project.Client;
                result.Budget = project.Budget;
                result.ModifiedDate = DateTime.Now;

                result.ProjectCategories.Clear();

                foreach (var category in categoryList)
                {
                    var pc = new EntityModels.ProjectCategory();
                    pc.Category = category;
                    pc.CategoryID = category.Id;
                    pc.Project = result;
                    pc.ProjectId = result.Id;
                    result.ProjectCategories.Add(pc);
                }

                DataContext.SaveChanges();
                return Mapper.Map<Project>(result);
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
                var projectToDelete = DataContext.Projects.Include(p => p.ProjectCategories).Single(p => p.Id == id);
                DataContext.Projects.Remove(projectToDelete);
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