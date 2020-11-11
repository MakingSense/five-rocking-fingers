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

namespace FRF.Core.Services
{
    public class ProjectsService : IProjectsService
    {
        
        private readonly DataAccessContext _dataContext;
        private readonly IMapper _mapper;

        public ProjectsService(DataAccessContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<List<Project>> GetAllAsync(string userId)
        {
            var result = await _dataContext.UsersByProject
                .Where(up => up.UserId == userId)
                .Include(pr => pr.Project)
                .ThenInclude(c => c.ProjectCategories)
                .ThenInclude(ca => ca.Category)
                .Include(pp => pp.Project)
                .Select(pro => pro.Project).ToListAsync();

            return _mapper.Map<List<Project>>(result);
        }

        public async Task<Project> SaveAsync(Project project)
        {
            if (String.IsNullOrWhiteSpace(project.Name))
            {
                throw new System.ArgumentException("The project needs a name", "project.Name");
            }

            // Gets the categories from the database on a list
            var categoryList = new List<EntityModels.Category>();
            foreach (var category in project.ProjectCategories)
            {
                categoryList.Add(await _dataContext.Categories.SingleAsync(c => c.Id == category.Category.Id));
            }

            // Maps the project into an EntityModel, deleting the Id if there was one, and the list projectCategories
            var mappedProject = _mapper.Map<EntityModels.Project>(project);
            mappedProject.ProjectCategories = new List<EntityModels.ProjectCategory>();
            mappedProject.CreatedDate = DateTime.Now;
            mappedProject.ModifiedDate = null;
            mappedProject.UsersByProject = new List<EntityModels.UsersByProject>();

            // Adds the project to the database, generating a unique Id for it
            _dataContext.Projects.Add(mappedProject);

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

            // Generates the relationships between the users and the project using the project Id
            foreach (var userId in project.UsersByProject)
            {
                var up = new EntityModels.UsersByProject();
                up.UserId = userId.UserId;
                up.Project = mappedProject;
                up.ProjectId = mappedProject.Id;
                mappedProject.UsersByProject.Add(up);
            }

            // Saves changes
            await _dataContext.SaveChangesAsync();

            return _mapper.Map<Project>(mappedProject);
        }

        public async Task<Project> GetAsync(int id)
        {
            /* TODO:Pending AWS Credentials. Login is bypassed![FIVE-6] */
            /*Uncomment this after do.*/
            /*
            var userId = _userService.GetCurrentUserId();
            var project = await _dataContext.UsersByProject
                .Where(up => up.UserId == userId)
                .Include(pr => pr.Project)
                .ThenInclude(c => c.ProjectCategories)
                .ThenInclude(ca => ca.Category)
                .Include(pp => pp.Project)
                .SingleOrDefaultAsync(p => p.ProjectId == id);
            */
            /*Then delete this*/
            var project =await _dataContext.Projects
                .Include(p => p.ProjectCategories)
                .ThenInclude(pc => pc.Category)
                .Include(up=>up.UsersByProject)
                .SingleOrDefaultAsync(p => p.Id == id);
            //

            return _mapper.Map<Project>(project);
        }

        public async Task<Project> UpdateAsync(Project project)
        {
            var categoryList = new List<EntityModels.Category>();
            foreach (var category in project.ProjectCategories)
            {
                categoryList.Add(await _dataContext.Categories.SingleAsync(c => c.Id == category.Category.Id));
            }

            /* TODO:Pending AWS Credentials. Login is bypassed![FIVE-6] */
            /*Uncomment this after do.*/
            /* var userId = _userService.GetCurrentUserId();
            var result = await _dataContext.UsersByProject
                .Where(up => up.UserId == userId)
                .Include(pr => pr.Project)
                .ThenInclude(c => c.ProjectCategories)
                .SingleOrDefaultAsync(p => p.Id == id);
             */
            /*Then delete this*/
            var result = await _dataContext.Projects
                .Include(p => p.ProjectCategories)
                .ThenInclude(pc => pc.Category)
                .SingleOrDefaultAsync(p => p.Id == project.Id);
            //
            if (result == null) return null;

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

            await _dataContext.SaveChangesAsync();
            return _mapper.Map<Project>(result);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            /* TODO:Pending AWS Credentials. Login is bypassed![FIVE-6] */
            /*Uncomment this after do.*/
            /* var userId = _userService.GetCurrentUserId();
            var projectToDelete = await _dataContext.UsersByProject
                .Where(up => up.UserId == userId)
                .Include(pr => pr.Project)
                .ThenInclude(c => c.ProjectCategories)
                .SingleOrDefaultAsync(p => p.Id == id);
             */

            var projectToDelete = await _dataContext.Projects
                .Include(p => p.ProjectCategories)
                .SingleOrDefaultAsync(p => p.Id == id);

            if (projectToDelete == null) return false;

            _dataContext.Projects.Remove(projectToDelete);
            await _dataContext.SaveChangesAsync();
            return true;
        }
    }
}