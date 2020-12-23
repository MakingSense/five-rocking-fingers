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
        private readonly IUserService _userService;

        public ProjectsService(DataAccessContext dataContext, IMapper mapper, IUserService userService)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _userService = userService;
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

            var projects = _mapper.Map<List<Project>>(result);

            foreach (var user in projects.SelectMany(project => project.UsersByProject))
            {
                var userPublicProfile =await _userService.GetUserPublicProfile(user.UserId);
                if (userPublicProfile == null) continue;
                user.Fullname = userPublicProfile.Fullname;
                user.Email = userPublicProfile.Email;
            }

            return projects;
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

            // Separate all the non duplicated User Id in a list of UsersProfile
            var noDuplicatedUsersId = project.UsersByProject
                .Select(ubp =>
                    ubp.UserId)
                .Distinct()
                .Select(ubp =>
                    new UsersProfile()
                    {
                        UserId = ubp
                    })
                .ToList();

            // Map all non duplicated users list
            var mappedUBP =
                _mapper.Map<IList<EntityModels.UsersByProject>>(noDuplicatedUsersId);

            // Map all the categories from the categories list in to a ProjectCategory
            var mappedCat = categoryList
                .Select(ct => new EntityModels.ProjectCategory
                {
                    Category = _mapper.Map<EntityModels.Category>(ct)
                }).ToList();

            // Add the mapped UsersProfile and ProjectCategory
            mappedProject.UsersByProject = mappedUBP;
            mappedProject.ProjectCategories = mappedCat;

            // Save the changes
            await _dataContext.Projects.AddAsync(mappedProject);
            await _dataContext.SaveChangesAsync();

            return _mapper.Map<Project>(mappedProject);
        }

        public async Task<Project> GetAsync(int id)
        {
            var userId =await _userService.GetCurrentUserId();
            var result = await _dataContext.UsersByProject
                .Where(up => up.UserId == userId)
                .Include(pr=>pr.Project)
                .ThenInclude(c=>c.ProjectCategories)
                .ThenInclude(ca=>ca.Category).Include(pp=>pp.Project)
                .ThenInclude(upp=>upp.UsersByProject)
                .SingleOrDefaultAsync(p=>p.ProjectId==id);

            if (result == null) return null;

            var project = _mapper.Map<Project>(result.Project);
            foreach (var user in project.UsersByProject)
            {
                var userPublicProfile = await _userService.GetUserPublicProfile(user.UserId);
                if (userPublicProfile == null) continue;
                user.Fullname = userPublicProfile.Fullname;
                user.Email = userPublicProfile.Email;
            }

            return project;
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

            var noDuplicatedUsersId = project.UsersByProject
                .Select(ubp =>
                    ubp.UserId)
                .Distinct()
                .Select(ubp =>
                    new UsersProfile()
                    {
                        UserId = ubp
                    })
                .ToList();

            var mappedUBP = _mapper.Map<IList<EntityModels.UsersByProject>>(noDuplicatedUsersId);

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
            var userId = await _userService.GetCurrentUserId();
            var projectToDelete = await _dataContext.UsersByProject
                .Where(up => up.UserId == userId)
                .Include(pr => pr.Project)
                .SingleOrDefaultAsync(p => p.ProjectId == id);

            if (projectToDelete == null) return false;

            _dataContext.Projects.Remove(projectToDelete.Project);
            await _dataContext.SaveChangesAsync();
            return true;
        }
    }
}