using AutoMapper;
using FRF.Core.Models;
using FRF.DataAccess;
using Microsoft.EntityFrameworkCore;
using EntityModels = FRF.DataAccess.EntityModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly IConfiguration _configuration;
        private readonly DataAccessContext _dataContext;
        private readonly IMapper _mapper;
        
        public CategoriesService(IConfiguration configuration, DataAccessContext dataContext, IMapper mapper)
        {
            _configuration = configuration;
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            var result = await _dataContext.Categories
                .Include(c => c.ProjectCategories)
                    .ThenInclude(pc => pc.Project)
                .ToListAsync();
            return _mapper.Map<List<Category>>(result);
        }

        public async Task<Category> GetAsync(int id)
        {
            var category = await _dataContext
                .Categories.Include(c => c.ProjectCategories)
                    .ThenInclude(pc => pc.Project)
                .SingleOrDefaultAsync(c => c.Id == id);
            return _mapper.Map<Category>(category);
        }

        public async Task<Category> SaveAsync(Category category)
        {
            // Maps the category into an EntityModel, deleting the Id if there was one.
            var mappedCategory = _mapper.Map<EntityModels.Category>(category);            

            // Adds the category to the database, generating a unique Id for it
            await _dataContext.Categories.AddAsync(mappedCategory);

            // Saves changes
            await _dataContext.SaveChangesAsync();

            return _mapper.Map<Category>(mappedCategory);
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            var result = await _dataContext.Categories
                .Include(c => c.ProjectCategories)
                .SingleAsync(c => c.Id == category.Id);
            result.Name = category.Name;
            result.Description = category.Description;

            await _dataContext.SaveChangesAsync();

            return _mapper.Map<Category>(result);
        }

        public async Task DeleteAsync(int id)
        {
            var categoryToDelete = await _dataContext.Categories
                .Include(c => c.ProjectCategories)
                .SingleAsync(c => c.Id == id);
            _dataContext.Categories.Remove(categoryToDelete);
            await _dataContext.SaveChangesAsync();
            return;
        }
    }
}