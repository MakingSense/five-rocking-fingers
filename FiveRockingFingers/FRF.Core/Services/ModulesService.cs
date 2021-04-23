using System.Collections.Generic;
using AutoMapper;
using FRF.Core.Response;
using FRF.DataAccess;
using FRF.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using CategoryModule = FRF.Core.Models.CategoryModule;

namespace FRF.Core.Services
{
    public class ModuleService : IModulesService
    {
        private readonly DataAccessContext _dataContext;
        private readonly IMapper _mapper;

        public ModuleService(DataAccessContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<Models.Module>> GetAsync(int id)
        {
            var response = await _dataContext.Modules
                .Include(r => r.ProjectModules)
                .ThenInclude<Module, ProjectModule, Project>(pr => pr.Project)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (response == null)
                return new ServiceResponse<Models.Module>(new Error(ErrorCodes.ModuleNotExist,
                    $"There is not Module with Id = {id}"));

            var Modules = _mapper.Map<Models.Module>(response);
            return new ServiceResponse<Models.Module>(Modules);
        }

        public async Task<ServiceResponse<IList<Models.Module>>> GetAllByCategoryIdAsync(int id)
        {
            var category = await _dataContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
                return new ServiceResponse<IList<Models.Module>>(new Error(ErrorCodes.CategoryNotExists,
                    $"There is no category with Id = {id}"));

            var response = await _dataContext.CategoriesModules
                .Where(cm => cm.CategoryId == id)
                .Include(cm => cm.Module)
                .ThenInclude(m => m.ProjectModules)
                .ToListAsync();
            var categoryModules = _mapper.Map<IList<CategoryModule>>(response);
            var modules = new List<Models.Module>(categoryModules.Select(cm => cm.Module).ToList());
            return new ServiceResponse<IList<Models.Module>>(modules);
        }

        public async Task<ServiceResponse<Models.Module>> UpdateAsync(Models.Module module)
        {
            var originalModules = await _dataContext.Modules
                .FirstOrDefaultAsync(r => r.Id == module.Id);
            if (originalModules == null)
                return new ServiceResponse<Models.Module>(new Error(ErrorCodes.ModuleNotExist,
                    $"There is not module with Id = {module.Id}"));

            originalModules.Name = module.Name;
            originalModules.Description = module.Description;
            originalModules.SuggestedCost = module.SuggestedCost;
            await _dataContext.SaveChangesAsync();

            var updatedModules = _mapper.Map<Models.Module>(originalModules);
            return new ServiceResponse<Models.Module>(updatedModules);
        }

        public async Task<ServiceResponse<Models.Module>> DeleteAsync(int id)
        {
            var ModulesToDelete = await _dataContext.Modules.FirstOrDefaultAsync(r => r.Id == id);
            if (ModulesToDelete == null)
                return new ServiceResponse<Models.Module>(new Error(ErrorCodes.ModuleNotExist,
                    $"There is not module with Id = {id}"));
            _dataContext.Modules.Remove(ModulesToDelete);
            await _dataContext.SaveChangesAsync();

            var deletedModules = _mapper.Map<Models.Module>(ModulesToDelete);
            return new ServiceResponse<Models.Module>(deletedModules);
        }

        public async Task<ServiceResponse<Models.Module>> SaveAsync(Models.Module module)
        {
            var ModulesToSave = _mapper.Map<Module>(module);
            await _dataContext.Modules.AddAsync(ModulesToSave);
            await _dataContext.SaveChangesAsync();

            var newModules = await _dataContext.Modules.FirstOrDefaultAsync(r => r.Id == ModulesToSave.Id);
            return new ServiceResponse<Models.Module>(_mapper.Map<Models.Module>(newModules));
        }

        /*
        private async Task<bool> IsRoleNameRepeated(Models.Module module, bool isUpdate)
        {
            return isUpdate
                ? await _dataContext.module.AnyAsync(r => r.Id != module.Id && r.RoleName == module.RoleName)
                : await _dataContext.module.AnyAsync(r => r.RoleName == module.RoleName);
        }
    */
    }
}