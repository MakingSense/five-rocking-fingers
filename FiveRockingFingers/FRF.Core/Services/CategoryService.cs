using AutoMapper;
using FRF.Core.Models;
using FRF.DataAccess;
using Microsoft.EntityFrameworkCore;
using EntityModels = FRF.DataAccess.EntityModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace FRF.Core.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IConfiguration _configuration;
        private readonly DataAccessContext _dataContext;
        private readonly IMapper _mapper;
        public CategoryService(IConfiguration configuration, DataAccessContext dataContext, IMapper mapper)
        {
            _configuration = configuration;
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public List<Category> GetAll()
        {
            var result = _dataContext.Categories.Include(c => c.ProjectCategories).ThenInclude(pc => pc.Project).ToList();
            if (result == null)
            {
                return null;
            }
            return _mapper.Map<List<Category>>(result);
        }

        public Category Get(int id)
        {
            var category = _dataContext.Categories.Include(c => c.ProjectCategories).ThenInclude(pc => pc.Project).SingleOrDefault(c => c.Id == id);
            if (category == null)
            {
                return null;
            }
            return _mapper.Map<Category>(category);
        }

        public Category Save(Category category)
        {
            // Maps the project into an EntityModel, deleting the Id if there was one, and the list projectCategories
            var mappedCategory = _mapper.Map<EntityModels.Category>(category);            

            // Adds the project to the database, generating a unique Id for it
            _dataContext.Categories.Add(mappedCategory);

            // Saves changes
            _dataContext.SaveChanges();

            return _mapper.Map<Category>(mappedCategory);
        }

        public Category Update(Category category)
        {
            if (String.IsNullOrWhiteSpace(category.Name))
            {
                throw new System.ArgumentException("The category needs a name", "category.Name");
            }

            var result = _dataContext.Categories.Include(c => c.ProjectCategories).Single(c => c.Id == category.Id);

            if (result == null)
            {
                throw new System.ArgumentException("There is no category with Id = " + category.Id, "category.Id");
            }

            result.Name = category.Name;
            result.Description = category.Description;

            _dataContext.SaveChanges();

            return _mapper.Map<Category>(result);
        }

        public void Delete(int id)
        {
            var categoryToDelete = _dataContext.Categories.Include(c => c.ProjectCategories).Single(c => c.Id == id);
            _dataContext.Categories.Remove(categoryToDelete);
            _dataContext.SaveChanges();
            return;
        }

        

        

        

        
    }
}
