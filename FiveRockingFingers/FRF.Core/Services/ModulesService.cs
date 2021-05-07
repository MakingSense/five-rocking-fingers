using AutoMapper;
using FRF.Core.Response;
using FRF.DataAccess;
using FRF.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
                .ThenInclude(pr => pr.Project)
                .Include(m=>m.CategoryModules)
                .ThenInclude(cm=>cm.Category)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (response == null)
                return new ServiceResponse<Models.Module>(new Error(ErrorCodes.ModuleNotExist,
                    $"There is not Module with Id = {id}"));

            var modules = _mapper.Map<Models.Module>(response);
            return new ServiceResponse<Models.Module>(modules);
        }

        public async Task<ServiceResponse<IList<Models.Module>>> GetAllAsync()
        {
            var response = await _dataContext.Modules
                .Include(r => r.ProjectModules)
                .Include(m=>m.CategoryModules)
                .ThenInclude(cm=>cm.Category)
                .ToListAsync();

            var modules = _mapper.Map<IList<Models.Module>>(response);
            return new ServiceResponse<IList<Models.Module>>(modules);
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
                .Include(m=>m.Category)
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
            var mappedCategoryModules = await GetCategoryList(module.CategoryModules);
            if (mappedCategoryModules == null)
                return new ServiceResponse<Models.Module>(new Error(ErrorCodes.CategoryNotExists,
                    "At least one of the selected categories doesn't exist"));

            originalModules.CategoryModules = mappedCategoryModules;
            originalModules.Name = module.Name;
            originalModules.Description = module.Description;
            originalModules.SuggestedCost = module.SuggestedCost;
            await _dataContext.SaveChangesAsync();

            var updatedModules = _mapper.Map<Models.Module>(originalModules);
            return new ServiceResponse<Models.Module>(updatedModules);
        }

        public async Task<ServiceResponse<Models.Module>> DeleteAsync(int id)
        {
            var modulesToDelete = await _dataContext.Modules.FirstOrDefaultAsync(r => r.Id == id);
            if (modulesToDelete == null)
                return new ServiceResponse<Models.Module>(new Error(ErrorCodes.ModuleNotExist,
                    $"There is not module with Id = {id}"));
            _dataContext.Modules.Remove(modulesToDelete);
            await _dataContext.SaveChangesAsync();

            var deletedModules = _mapper.Map<Models.Module>(modulesToDelete);
            return new ServiceResponse<Models.Module>(deletedModules);
        }

        public async Task<ServiceResponse<Models.Module>> SaveAsync(Models.Module module)
        {
            var modulesToSave = _mapper.Map<Module>(module);
            if(module.CategoryModules.Any())
            {
                var mappedCategoryModules = await GetCategoryList(module.CategoryModules);
                if (mappedCategoryModules == null)
                    return new ServiceResponse<Models.Module>(new Error(ErrorCodes.CategoryNotExists,
                        "At least one of the selected categories doesn't exist"));

                modulesToSave.CategoryModules = mappedCategoryModules;
            }

            await _dataContext.Modules.AddAsync(modulesToSave);
            await _dataContext.SaveChangesAsync();

            var newModules = await _dataContext.Modules.Include(m => m.CategoryModules)
                .ThenInclude<Module, DataAccess.EntityModels.CategoryModule, Category>(cm => cm.Category)
                .FirstOrDefaultAsync(r => r.Id == modulesToSave.Id);
            return new ServiceResponse<Models.Module>(_mapper.Map<Models.Module>(newModules));
        }

        private async Task<List<DataAccess.EntityModels.CategoryModule>> GetCategoryList(IList<CategoryModule> categoryModule)
        {
            var categoryList = new List<Category>();
            foreach (var category in categoryModule)
            {
                var categoryToAdd =
                    await _dataContext.Categories.SingleOrDefaultAsync(c => c.Id == category.Category.Id);

                if (categoryToAdd == null)
                    return null;

                categoryList.Add(categoryToAdd);
            }

            var mappedCategoryModules = categoryList
                .Select(ct => new DataAccess.EntityModels.CategoryModule()
                {
                    Category = _mapper.Map<Category>(ct)
                }).ToList();

            return mappedCategoryModules;
        }
    }
}