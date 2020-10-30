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

        public async Task<List<Category>> GetAll()
        {
            var result = await _dataContext.Categories
                .Include(c => c.ProjectCategories)
                    .ThenInclude(pc => pc.Project)
                .ToListAsync();
            if (result == null)
            {
                return null;
            }
            return _mapper.Map<List<Category>>(result);
        }

        public async Task<Category> Get(int id)
        {
            var category = await _dataContext
                .Categories.Include(c => c.ProjectCategories)
                    .ThenInclude(pc => pc.Project)
                .SingleOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return null;
            }
            return _mapper.Map<Category>(category);
        }

        public async Task<Category> Save(Category category)
        {
            // Maps the category into an EntityModel, deleting the Id if there was one.
            var mappedCategory = _mapper.Map<EntityModels.Category>(category);            

            // Adds the category to the database, generating a unique Id for it
            await _dataContext.Categories.AddAsync(mappedCategory);

            // Saves changes
            await _dataContext.SaveChangesAsync();

            return _mapper.Map<Category>(mappedCategory);
        }

        public async Task<Category> Update(Category category)
        {
            var result = await _dataContext.Categories
                .Include(c => c.ProjectCategories)
                .SingleAsync(c => c.Id == category.Id);

            if (result == null)
            {
                throw new System.ArgumentException("There is no category with Id = " + category.Id, "category.Id");
            }

            result.Name = category.Name;
            result.Description = category.Description;

            await _dataContext.SaveChangesAsync();

            return _mapper.Map<Category>(result);
        }

        public async Task Delete(int id)
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