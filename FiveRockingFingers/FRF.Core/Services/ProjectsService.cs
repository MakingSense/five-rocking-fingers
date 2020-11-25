using AutoMapper;
using FRF.Core.Models;
using FRF.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityModels = FRF.DataAccess.EntityModels;

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

        public async Task<List<Project>> GetAllAsync(Guid userId)
        {
            var result = await _dataContext.UsersByProject
                .Where(up => up.UserId == userId)
                .Include(pr => pr.Project)
                .ThenInclude(c => c.ProjectCategories)
                .ThenInclude(ca => ca.Category)
                .Include(pp => pp.Project)
                .ThenInclude(upp => upp.UsersByProject)
                .Select(pro => pro.Project).ToListAsync();

            return _mapper.Map<List<Project>>(result);
        }

        public async Task<Project> SaveAsync(Project project)
        {
            // Gets the categories from the database on a list
            var categoryList = new List<EntityModels.Category>();
            foreach (var category in project.ProjectCategories)
            {
                var categoryToAdd =
                    await _dataContext.Categories.SingleOrDefaultAsync(c => c.Id == category.Category.Id);
                
                if (categoryToAdd == null) return null;

                categoryList.Add(categoryToAdd);
            }

            // Map the new project
            var mappedProject = _mapper.Map<EntityModels.Project>(project);
            mappedProject.CreatedDate = DateTime.Now;
            mappedProject.ModifiedDate = null;

            // Map all the User from the Project 
            var mappedUBP = _mapper.Map<IList<EntityModels.UsersByProject>>(project.UsersByProject);

            // Map all the categories from the categories list in to a ProjectCategory
            var mappedCat = categoryList
                .Select(ct => new EntityModels.ProjectCategory
                {
                    Category = _mapper.Map<EntityModels.Category>(ct)
                }).ToList();

            // Add the mapped UsersByProject and ProjectCategory
            mappedProject.UsersByProject = mappedUBP;
            mappedProject.ProjectCategories = mappedCat;

            // Save the changes
            await _dataContext.Projects.AddAsync(mappedProject);
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
                .ThenInclude(upp=>upp.UsersByProject)
                .SingleOrDefaultAsync(p => p.ProjectId == id);
            */
            /*Then delete this*/
            var project = await _dataContext.Projects
                .Include(p => p.ProjectCategories)
                .ThenInclude(pc => pc.Category)
                .Include(up => up.UsersByProject)
                .SingleOrDefaultAsync(p => p.Id == id);
            //

            return _mapper.Map<Project>(project);
        }

        public async Task<Project> UpdateAsync(Project project)
        {
            var categoryList = new List<EntityModels.Category>();
            foreach (var category in project.ProjectCategories)
            {
                var categoryToAdd =
                    await _dataContext.Categories.SingleOrDefaultAsync(c => c.Id == category.Category.Id);
                if (categoryToAdd == null) return null;

                categoryList.Add(categoryToAdd);
            }

            var mappedProjectCategory = categoryList
                .Select(ct => new EntityModels.ProjectCategory
                {
                    Category = _mapper.Map<EntityModels.Category>(ct)
                }).ToList();

            var userList = project.UsersByProject
                .Select(ubp => ubp.UserId)
                .Distinct()
                .ToList();
            
            var mappedUBP = _mapper.Map<IList<EntityModels.UsersByProject>>(userList);

            var result = await _dataContext.Projects
                .Include(p => p.ProjectCategories)
                .ThenInclude(pc => pc.Category)
                .Include(up => up.UsersByProject)
                .SingleOrDefaultAsync(p => p.Id == project.Id);

            if (result == null) return null;

            result.Name = project.Name;
            result.Owner = project.Owner;
            result.Client = project.Client;
            result.Budget = project.Budget;
            result.ModifiedDate = DateTime.Now;
            result.ProjectCategories = mappedProjectCategory;
            result.UsersByProject = mappedUBP;

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
                .ThenInclude(upp=>upp.UsersByProject)
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