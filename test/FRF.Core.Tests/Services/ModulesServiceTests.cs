using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Response;
using FRF.Core.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using CategoryModule = FRF.DataAccess.EntityModels.CategoryModule;
using ProjectCategory = FRF.DataAccess.EntityModels.ProjectCategory;

namespace FRF.Core.Tests.Services
{
    public class ModulesServiceTests
    {
        private readonly Mock<IConfiguration> _configuration;
        private readonly IMapper _mapper = MapperBuilder.Build();
        private readonly DataAccessContextForTest _dataAccess;
        private readonly ModuleService _classUnderTest;

        public ModulesServiceTests()
        {
            _configuration = new Mock<IConfiguration>();
            _dataAccess = new DataAccessContextForTest(Guid.NewGuid(), _configuration.Object);
            _dataAccess.Database.EnsureDeleted();
            _dataAccess.Database.EnsureCreated();
            _classUnderTest = new ModuleService(_dataAccess, _mapper);
        }

        [Fact]
        public async Task GetAsync_ReturnsModule()
        {
            // Arrange
            var resource = CreateAndSaveModule();

            // Act
            var result = await _classUnderTest.GetAsync(resource.Id);

            // Assert
            Assert.IsType<ServiceResponse<Module>>(result);
            Assert.True(result.Success);
            var resultValue = result.Value;

            Assert.Equal(resource.Name, resultValue.Name);
            Assert.Equal(resource.Description, resultValue.Description);
            Assert.Equal(resource.SuggestedCost, resultValue.SuggestedCost);
            Assert.Equal(resource.Id, resultValue.Id);
        }

        [Fact]
        public async Task GetAsync_WhenModuleNotExist_ReturnsNotExistError()
        {
            // Act
            var result = await _classUnderTest.GetAsync(It.IsAny<int>());

            // Assert
            Assert.IsType<ServiceResponse<Module>>(result);
            Assert.False(result.Success);
            Assert.Equal(ErrorCodes.ModuleNotExist, result.Error.Code);
        }

        [Fact]
        public async Task GetAllByCategoryIdAsync_ReturnEmptyList()
        {
            // Arange
            var category = CreateCategory();
            var module = CreateAndSaveModule();


            // Act
            var result = await _classUnderTest.GetAllByCategoryIdAsync(category.Id);

            // Assert
            Assert.IsType<ServiceResponse<IList<Module>>>(result);
            Assert.True(result.Success);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetAllByCategoryIdAsync_WhenCategoryNotExists_ReturnCategoryNotExistsError()
        {
            // Arange
            var CategoryId = 1;
            // Act
            var result = await _classUnderTest.GetAllByCategoryIdAsync(CategoryId);

            // Assert
            Assert.IsType<ServiceResponse<IList<Module>>>(result);
            Assert.False(result.Success);
            Assert.Equal(ErrorCodes.CategoryNotExists, result.Error.Code);
        }

        [Fact]
        public async Task GetAllByCategoryIdAsync_ReturnList()
        {
            // Arange
            var category = CreateCategory();
            var module = CreateAndSaveModule(category);

            // Act
            var result = await _classUnderTest.GetAllByCategoryIdAsync(category.Id);

            // Assert
            Assert.IsType<ServiceResponse<IList<Module>>>(result);
            Assert.True(result.Success);
            var resultValue = Assert.Single(result.Value);

            Assert.Equal(module.Name, resultValue.Name);
            Assert.Equal(module.Description, resultValue.Description);
            Assert.Equal(module.Id, resultValue.Id);
            Assert.Equal(module.SuggestedCost, resultValue.SuggestedCost);

            Assert.Equal(category.Name, resultValue.CategoryModules[0].Category.Name);
            Assert.Equal(category.Id, resultValue.CategoryModules[0].Category.Id);
            Assert.Equal(category.Description, resultValue.CategoryModules[0].Category.Description);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsUpdatedModule()
        {
            // Arrange
            var baseModule = CreateAndSaveModule();
            var updatedModule = CreateUpdateModule(baseModule);

            // Act
            var result = await _classUnderTest.UpdateAsync(updatedModule);

            // Assert
            Assert.IsType<ServiceResponse<Module>>(result);
            Assert.True(result.Success);
            var resultValue = result.Value;

            Assert.Equal(baseModule.Name, resultValue.Name);
            Assert.Equal(baseModule.Description, resultValue.Description);
            Assert.Equal(updatedModule.SuggestedCost, resultValue.SuggestedCost);
            Assert.Equal(baseModule.Id, resultValue.Id);
        }

        [Fact]
        public async Task UpdateAsync_WhenModuleNotExist_ReturnsNotExistError()
        {
            // Arrange
            var resource = new Module
            {
                Description = "Mock description",
                Id = 1,
                Name = "Mock name",
                SuggestedCost = 999,
            };
            // Act
            var result = await _classUnderTest.UpdateAsync(resource);

            // Assert
            Assert.IsType<ServiceResponse<Module>>(result);
            Assert.False(result.Success);
            Assert.Equal(ErrorCodes.ModuleNotExist, result.Error.Code);
        }

        [Fact]
        public async Task DeleteAsync_WhenModuleNotExist_ReturnsNotExistError()
        {
            // Act
            var result = await _classUnderTest.DeleteAsync(It.IsAny<int>());

            // Assert
            Assert.IsType<ServiceResponse<Module>>(result);
            Assert.False(result.Success);
            Assert.Equal(ErrorCodes.ModuleNotExist, result.Error.Code);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsDeletedModule()
        {
            // Arrange
            var baseModule = CreateAndSaveModule();

            // Act
            var result = await _classUnderTest.DeleteAsync(baseModule.Id);

            // Assert
            Assert.IsType<ServiceResponse<Module>>(result);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task SaveAsync_ReturnsSavedModule()
        {
            // Arrange
            var resource = new Module
            {
                Description = "Mock Description",
                Name = "Mock role name",
                SuggestedCost = 1000000,
            };

            // Act
            var result = await _classUnderTest.SaveAsync(resource);

            // Assert
            Assert.IsType<ServiceResponse<Module>>(result);
            Assert.True(result.Success);
            var resultValue = result.Value;

            Assert.Equal(resource.Name, resultValue.Name);
            Assert.Equal(resource.Description, resultValue.Description);
            Assert.Equal(resource.SuggestedCost, resultValue.SuggestedCost);
        }

        private Module CreateAndSaveModule()
        {
            var rnd = new Random();
            var id = rnd.Next();
            var resource = new DataAccess.EntityModels.Module()
            {
                Id = id,
                Name = $"Mock Role name {id}",
                Description = "Mock Description",
                SuggestedCost = 99999,
            };
            _dataAccess.Modules.Add(resource);
            _dataAccess.SaveChanges();

            return _mapper.Map<Module>(resource);
            ;
        }

        private Module CreateAndSaveModule(Category category)
        {
            var rnd = new Random();
            var id = rnd.Next();
            var resource = new DataAccess.EntityModels.Module()
            {
                Id = id,
                Name = $"Mock Role name {id}",
                Description = "Mock Description",
                SuggestedCost = 99999,
                CategoryModules = new List<CategoryModule>
                {
                    new CategoryModule
                    {
                        CategoryId = category.Id,
                        ModuleId = id
                    }
                }
            };
            _dataAccess.Modules.Add(resource);
            _dataAccess.SaveChanges();

            return _mapper.Map<Module>(resource);
            ;
        }

        private Module CreateUpdateModule(Module resource)
        {
            return new Module()
            {
                Id = resource.Id,
                Name = resource.Name,
                Description = resource.Description,
                SuggestedCost = 1000000,
            };
        }

        private Category CreateCategory()
        {
            var category = new DataAccess.EntityModels.Category
            {
                Description = "Description Mock",
                Name = "Category Mock",
                CategoryModules = new List<CategoryModule>(),
                ProjectCategories = new List<ProjectCategory>()
            };
            _dataAccess.Categories.Add(category);
            _dataAccess.SaveChanges();

            return _mapper.Map<Category>(category);
        }
    }
}