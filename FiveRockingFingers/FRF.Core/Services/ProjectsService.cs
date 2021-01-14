using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Response;
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

        public async Task<ServiceResponse<List<Project>>> GetAllAsync(Guid userId)
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
                var userPublicProfile =await _userService.GetUserPublicProfileAsync(user.UserId);
                if (userPublicProfile == null) continue;
                user.Fullname = userPublicProfile.Fullname;
                user.Email = userPublicProfile.Email;
            }

            return new ServiceResponse<List<Project>>(projects);
        }

        public async Task<ServiceResponse<Project>> SaveAsync(Project project)
        {
            // Gets the categories from the database on a list
            var categoryList = new List<EntityModels.Category>();
            foreach (var category in project.ProjectCategories)
            {
                var categoryToAdd =
                    await _dataContext.Categories.SingleOrDefaultAsync(c => c.Id == category.Category.Id);

                if (categoryToAdd == null) 
                    return new ServiceResponse<Project>(
                        new Error(ErrorCodes.CategoryNotExists, "At least one of the categories selected doesn't exist")
                        );

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

            return new ServiceResponse<Project>(_mapper.Map<Project>(mappedProject));
        }

        public async Task<ServiceResponse<Project>> GetAsync(int id)
        {
            var userId =await _userService.GetCurrentUserIdAsync();
            var result = await _dataContext.UsersByProject
                .Where(up => up.UserId == userId)
                .Include(pr=>pr.Project)
                .ThenInclude(c=>c.ProjectCategories)
                .ThenInclude(ca=>ca.Category).Include(pp=>pp.Project)
                .ThenInclude(upp=>upp.UsersByProject)
                .SingleOrDefaultAsync(p=>p.ProjectId==id);

            if (result == null)
                return new ServiceResponse<Project>(
                    new Error(ErrorCodes.ProjectNotExists, $"There is no project with Id = {id}")
                    );

            var project = _mapper.Map<Project>(result.Project);
            foreach (var user in project.UsersByProject)
            {
                var userPublicProfile = await _userService.GetUserPublicProfileAsync(user.UserId);
                if (userPublicProfile == null) continue;
                user.Fullname = userPublicProfile.Fullname;
                user.Email = userPublicProfile.Email;
            }

            return new ServiceResponse<Project>(project);
        }

        public async Task<ServiceResponse<Project>> UpdateAsync(Project project)
        {
            var categoryList = new List<EntityModels.Category>();
            foreach (var category in project.ProjectCategories)
            {
                var categoryToAdd =
                    await _dataContext.Categories.SingleOrDefaultAsync(c => c.Id == category.Category.Id);
                if (categoryToAdd == null)
                    return new ServiceResponse<Project>(
                        new Error(ErrorCodes.CategoryNotExists, "At least one of the categories selected doesn't exist")
                        );

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

            if (result == null)
                return new ServiceResponse<Project>(
                    new Error(ErrorCodes.ProjectNotExists, $"There is no project with Id = {project.Id}")
                    );

            result.Name = project.Name;
            result.Owner = project.Owner;
            result.Client = project.Client;
            result.Budget = project.Budget;
            result.ModifiedDate = DateTime.Now;
            result.ProjectCategories = mappedProjectCategory;
            result.UsersByProject = mappedUBP;

            await _dataContext.SaveChangesAsync();
            return new ServiceResponse<Project>(_mapper.Map<Project>(result));
        }

        public async Task<ServiceResponse<Project>> DeleteAsync(int id)
        {
            var userId = await _userService.GetCurrentUserIdAsync();
            var projectToDelete = await _dataContext.UsersByProject
                .Where(ubp => ubp.UserId == userId)
                .Include(pr => pr.Project)
                .Select(ubp=> ubp.Project)
                .SingleOrDefaultAsync(p => p.Id == id);

            if (projectToDelete == null)
                return new ServiceResponse<Project>(
                    new Error(ErrorCodes.ProjectNotExists, $"There is no project with Id = {id}")
                    );

            _dataContext.Projects.Remove(projectToDelete);
            await _dataContext.SaveChangesAsync();

            var mappedProject = _mapper.Map<Project>(projectToDelete);
            return new ServiceResponse<Project>(mappedProject);
        }
    }
}