using AutoMapper;
using FRF.Core.Response;
using FRF.Core.Services;
using FRF.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace FRF.Core.Tests.Services
{
    public class CategoriesServiceTests
    {
        private readonly Mock<IConfiguration> _configuration;
        private readonly IMapper _mapper = MapperBuilder.Build();
        private readonly DataAccessContextForTest _dataAccess;
        private readonly CategoriesService _classUnderTest;

        public CategoriesServiceTests()
        {
            _configuration = new Mock<IConfiguration>();

            _dataAccess = new DataAccessContextForTest(Guid.NewGuid(), _configuration.Object);

            _dataAccess.Database.EnsureDeleted();
            _dataAccess.Database.EnsureCreated();

            _classUnderTest = new CategoriesService(_configuration.Object, _dataAccess, _mapper);
        }

        private Category CreateCategory()
        {
            var category = new Category()
            {
                Name = "[Mock] Category name",
                Description = "[Mock] Category description",
                ProjectCategories = new List<ProjectCategory>()
            };
            _dataAccess.Categories.Add(category);
            _dataAccess.SaveChanges();

            return category;
        }

        private Project CreateProject(Category category)
        {
            var project = new Project()
            {
                Name = "[Mock] Project name",
                Budget = 100,
                ProjectCategories = new List<ProjectCategory>()
                {
                    new ProjectCategory()
                    {
                        CategoryID = category.Id
                    }
                }
            };

            _dataAccess.Projects.Add(project);
            _dataAccess.SaveChanges();

            return project;
        }

        [Fact]
        public async Task GetAllAsync_ReturnsList()
        {
            // Arrange
            var category = CreateCategory();
            var project = CreateProject(category);

            // Act
            var result = await _classUnderTest.GetAllAsync();

            // Assert
            Assert.IsType<ServiceResponse<List<Models.Category>>>(result);
            Assert.True(result.Success);
            var resultValue = Assert.Single(result.Value);

            Assert.Equal(category.Name, resultValue.Name);
            Assert.Equal(category.Description, resultValue.Description);
            Assert.Equal(category.Id, resultValue.Id);
            Assert.Single(resultValue.ProjectCategories);

            Assert.Equal(resultValue.ProjectCategories[0].Project.Name, project.Name);
            Assert.Equal(resultValue.ProjectCategories[0].Project.Budget, project.Budget);
            Assert.Equal(resultValue.ProjectCategories[0].Project.Id, project.Id);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList()
        {
            // Act
            var result = await _classUnderTest.GetAllAsync();

            // Assert
            Assert.IsType<ServiceResponse<List<Models.Category>>>(result);
            Assert.True(result.Success);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetAsync_ReturnsCategory()
        {
            // Arrange
            var category = CreateCategory();
            var project = CreateProject(category);

            // Act
            var result = await _classUnderTest.GetAsync(category.Id);

            // Assert
            Assert.IsType<ServiceResponse<Models.Category>>(result);
            Assert.True(result.Success);
            var resultValue = result.Value;

            Assert.Equal(category.Name, resultValue.Name);
            Assert.Equal(category.Description, resultValue.Description);
            Assert.Equal(category.Id, resultValue.Id);
            Assert.Single(resultValue.ProjectCategories);

            Assert.Equal(resultValue.ProjectCategories[0].Project.Name, project.Name);
            Assert.Equal(resultValue.ProjectCategories[0].Project.Budget, project.Budget);
            Assert.Equal(resultValue.ProjectCategories[0].Project.Id, project.Id);
        }

        [Fact]
        public async Task GetAsync_ReturnsNull()
        {
            // Arrange
            var categoryId = 0;

            // Act
            var result = await _classUnderTest.GetAsync(categoryId);

            // Assert
            Assert.IsType<ServiceResponse<Models.Category>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(result.Error.Code, ErrorCodes.CategoryNotExists);
        }

        [Fact]
        public async Task SaveAsync_ReturnsCategory()
        {
            // Arrange
            var categoryToSave = new Models.Category()
            {
                Name = "Category name to save",
                Description = "Category description to save",
                ProjectCategories = new List<Models.ProjectCategory>()
            };

            // Act
            var result = await _classUnderTest.SaveAsync(categoryToSave);

            // Assert
            Assert.IsType<ServiceResponse<Models.Category>>(result);
            Assert.True(result.Success);
            var resultValue = result.Value;

            Assert.Equal(resultValue.Name, categoryToSave.Name);
            Assert.Equal(resultValue.Description, categoryToSave.Description);
            Assert.Empty(resultValue.ProjectCategories);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsCategory()
        {
            // Arrange
            var category = CreateCategory();
            var project = CreateProject(category);

            var updatedCategory = new Models.Category()
            {
                Id = category.Id,
                Name = "Updated name",
                Description = "Updated description",
            };

            // Act
            var result = await _classUnderTest.UpdateAsync(updatedCategory);

            // Assert
            Assert.IsType<ServiceResponse<Models.Category>>(result);
            Assert.True(result.Success);
            var resultValue = result.Value;

            Assert.Equal(resultValue.Id, category.Id);
            Assert.Equal(resultValue.Id, updatedCategory.Id);

            Assert.Equal(resultValue.Name, category.Name);
            Assert.Equal(resultValue.Name, updatedCategory.Name);

            Assert.Equal(resultValue.Description, category.Description);
            Assert.Equal(resultValue.Description, updatedCategory.Description);

            Assert.Single(resultValue.ProjectCategories);
            Assert.Equal(resultValue.ProjectCategories[0].Project.Name, project.Name);
            Assert.Equal(resultValue.ProjectCategories[0].Project.Budget, project.Budget);
            Assert.Equal(resultValue.ProjectCategories[0].Project.Id, project.Id);
        }

        [Fact]
        public async Task DeleteAsync_Returns()
        {
            // Arrange
            var category = CreateCategory();
            var project = CreateProject(category);

            // Act
            var result = await _classUnderTest.DeleteAsync(category.Id);

            // Assert
            Assert.IsType<ServiceResponse<Models.Category>>(result);
            Assert.True(result.Success);
            var resultValue = result.Value;

            Assert.Equal(category.Name, resultValue.Name);
            Assert.Equal(category.Description, resultValue.Description);
            Assert.Equal(category.Id, resultValue.Id);
            Assert.Single(resultValue.ProjectCategories);

            Assert.Equal(resultValue.ProjectCategories[0].Project.Name, project.Name);
            Assert.Equal(resultValue.ProjectCategories[0].Project.Budget, project.Budget);
            Assert.Equal(resultValue.ProjectCategories[0].Project.Id, project.Id);
        }
    }
}