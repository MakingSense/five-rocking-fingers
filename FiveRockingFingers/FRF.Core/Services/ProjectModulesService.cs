using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Response;
using FRF.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public class ProjectModulesService : IProjectModulesService
    {
        private readonly DataAccessContext _dataContext;
        private readonly IMapper _mapper;

        public ProjectModulesService(DataAccessContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<ProjectModule>> GetAsync(int id)
        {
            var projectModule = await _dataContext.ProjectModules.FirstOrDefaultAsync(pm => pm.Id == id);

            if (projectModule == null)
                return new ServiceResponse<ProjectModule>(new Error(ErrorCodes.ProjectModuleNotExists, $"There is no project resource with Id = {id}"));

            var mappedProjectModule = _mapper.Map<ProjectModule>(projectModule);
            return new ServiceResponse<ProjectModule>(mappedProjectModule);
        }

        public async Task<ServiceResponse<List<ProjectModule>>> GetByProjectIdAsync(int projectId)
        {
            if (!await _dataContext.Projects.AnyAsync(p => p.Id == projectId))
                return new ServiceResponse<List<ProjectModule>>(new Error(ErrorCodes.ProjectNotExists, $"There is no project with Id = {projectId}"));

            var projectModules = await _dataContext.ProjectModules
                .Where(pm => pm.ProjectId == projectId)
                .ToListAsync();

            var mappedProjectModules = _mapper.Map<List<ProjectModule>>(projectModules);

            return new ServiceResponse<List<ProjectModule>>(mappedProjectModules);
        }

        public async Task<ServiceResponse<ProjectModule>> SaveAsync(ProjectModule projectModule)
        {
            var project = await _dataContext.Projects.FirstOrDefaultAsync(p => p.Id == projectModule.ProjectId);

            if (project == null)
                return new ServiceResponse<ProjectModule>(new Error(ErrorCodes.ProjectNotExists, $"There is no project with Id = {projectModule.ProjectId}"));

            if (!await _dataContext.Modules.AnyAsync(m => m.Id == projectModule.ModuleId))
                return new ServiceResponse<ProjectModule>(new Error(ErrorCodes.ModuleNotExists, $"There is no module with Id = {projectModule.ModuleId}"));

            if (projectModule.Cost < 0)
                return new ServiceResponse<ProjectModule>(new Error(ErrorCodes.ModuleCostInvalid, $"The cost of the module must be grater or equal to zero"));

            // Maps the category into an EntityModel, deleting the Id if there was one.
            var mappedProjectModule = _mapper.Map<DataAccess.EntityModels.ProjectModule>(projectModule);

            // Adds the category to the database, generating a unique Id for it
            await _dataContext.ProjectModules.AddAsync(mappedProjectModule);

            // Saves changes
            await _dataContext.SaveChangesAsync();

            return new ServiceResponse<ProjectModule>(_mapper.Map<ProjectModule>(mappedProjectModule));
        }

        public async Task<ServiceResponse<ProjectModule>> UpdateAsync(ProjectModule projectModule)
        {
            var projectModuleResponse = await GetAsync(projectModule.Id);

            if (!projectModuleResponse.Success)
                return new ServiceResponse<ProjectModule>(new Error(ErrorCodes.ProjectModuleNotExists, $"There is no project module with Id = {projectModule.ProjectId}"));

            var project = await _dataContext.Projects.SingleOrDefaultAsync(p => p.Id == projectModule.ProjectId);

            if (project == null)
                return new ServiceResponse<ProjectModule>(new Error(ErrorCodes.ProjectNotExists, $"There is no project with Id = {projectModule.ProjectId}"));

            if (!await _dataContext.Modules.AnyAsync(m => m.Id == projectModule.ModuleId))
                return new ServiceResponse<ProjectModule>(new Error(ErrorCodes.ModuleNotExists, $"There is no module with Id = {projectModule.ModuleId}"));

            if (projectModule.Cost < 0)
                return new ServiceResponse<ProjectModule>(new Error(ErrorCodes.ModuleCostInvalid, $"The cost of the module must be grater or equal to zero"));

            var result = await _dataContext.ProjectModules.SingleAsync(pm => pm.Id == projectModule.Id);
            result.ModuleId = projectModule.ModuleId;
            result.Alias = projectModule.Alias;
            result.Cost = projectModule.Cost;

            await _dataContext.SaveChangesAsync();

            var mappedProjectModule = _mapper.Map<ProjectModule>(result);
            return new ServiceResponse<ProjectModule>(mappedProjectModule);
        }

        public async Task<ServiceResponse<ProjectModule>> DeleteAsync(int id)
        {
            var projectModuleToDelete = await _dataContext.ProjectModules.SingleOrDefaultAsync(pm => pm.Id == id);

            if (projectModuleToDelete == null)
                return new ServiceResponse<ProjectModule>(new Error(ErrorCodes.ProjectModuleNotExists, $"There is no project module with Id = {id}"));

            _dataContext.ProjectModules.Remove(projectModuleToDelete);
            await _dataContext.SaveChangesAsync();

            var mappedProjectModule = _mapper.Map<ProjectModule>(projectModuleToDelete);
            return new ServiceResponse<ProjectModule>(mappedProjectModule);
        }
    }
}
