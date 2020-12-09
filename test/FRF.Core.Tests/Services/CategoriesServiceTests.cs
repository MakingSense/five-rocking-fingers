using AutoMapper;
using FRF.Core.Services;
using FRF.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
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
        private readonly DbContextOptions<DataAccessContextForTest> contextOptions;

        public CategoriesServiceTests()
        {
            _configuration = new Mock<IConfiguration>();

            contextOptions = new DbContextOptionsBuilder<DataAccessContextForTest>()
                .UseInMemoryDatabase(databaseName: "Test")
                .Options;
            _dataAccess = new DataAccessContextForTest(contextOptions, _configuration.Object);

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
            Assert.IsType<List<Models.Category>>(result);
            Assert.Single(result);

            Assert.Equal(category.Name, result[0].Name);
            Assert.Equal(category.Description, result[0].Description);
            Assert.Equal(category.Id, result[0].Id);
            Assert.Single(result[0].ProjectCategories);

            Assert.Equal(result[0].ProjectCategories[0].Project.Name, project.Name);
            Assert.Equal(result[0].ProjectCategories[0].Project.Budget, project.Budget);
            Assert.Equal(result[0].ProjectCategories[0].Project.Id, project.Id);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList()
        {
            // Act
            var result = await _classUnderTest.GetAllAsync();

            // Assert
            Assert.IsType<List<Models.Category>>(result);
            Assert.Empty(result);
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
            Assert.IsType<Models.Category>(result);

            Assert.Equal(category.Name, result.Name);
            Assert.Equal(category.Description, result.Description);
            Assert.Equal(category.Id, result.Id);
            Assert.Single(result.ProjectCategories);

            Assert.Equal(result.ProjectCategories[0].Project.Name, project.Name);
            Assert.Equal(result.ProjectCategories[0].Project.Budget, project.Budget);
            Assert.Equal(result.ProjectCategories[0].Project.Id, project.Id);
        }

        [Fact]
        public async Task GetAsync_ReturnsNull()
        {
            // Arrange
            var categoryId = 99;

            // Act
            var result = await _classUnderTest.GetAsync(categoryId);

            // Assert
            Assert.Null(result);
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
            Assert.IsType<Models.Category>(result);

            Assert.Equal(result.Name, categoryToSave.Name);
            Assert.Equal(result.Description, categoryToSave.Description);
            Assert.Empty(result.ProjectCategories);
        }
        // The case of saving a category with an empty name is solved in the controller (name can't be null)

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
            Assert.IsType<Models.Category>(result);

            Assert.Equal(result.Id, category.Id);
            Assert.Equal(result.Id, updatedCategory.Id);

            Assert.Equal(result.Name, category.Name);
            Assert.Equal(result.Name, updatedCategory.Name);

            Assert.Equal(result.Description, category.Description);
            Assert.Equal(result.Description, updatedCategory.Description);

            Assert.Single(result.ProjectCategories);
            Assert.Equal(result.ProjectCategories[0].Project.Name, project.Name);
            Assert.Equal(result.ProjectCategories[0].Project.Budget, project.Budget);
            Assert.Equal(result.ProjectCategories[0].Project.Id, project.Id);
        }
        // The case of updating a non existent category is solved in the controller (an Id that doesn't exist)
        // The case of updating a category to have an empty name is solved in the controller (name can't be null)

        [Fact]
        public async Task DeleteAsync_Returns()
        {
            // Arrange
            var category = CreateCategory();

            // Act
            await _classUnderTest.DeleteAsync(category.Id);

            // Assert
            Assert.Null(await _dataAccess.Categories.FirstOrDefaultAsync(c => c.Id == category.Id));
        }
    }
}