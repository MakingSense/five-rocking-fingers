using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Services;
using FRF.Web.Controllers;
using FRF.Web.Dtos.Categories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace FRF.Web.Tests.Controllers
{
    public class CategoriesControllerTests
    {
        private readonly IMapper _mapper = MapperBuilder.Build();
        private readonly Mock<ICategoriesService> _categoriesService;

        private readonly CategoriesController _classUnderTest;

        public CategoriesControllerTests()
        {
            _categoriesService = new Mock<ICategoriesService>();

            _classUnderTest = new CategoriesController(_categoriesService.Object, _mapper);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOk()
        {
            // Arrange
            var categories = new List<Category>()
            {
                new Category()
                {
                    Id = 1,
                    Name = "[Mock] Category Name 1",
                    Description = "[Mock] Category description 1"
                },
                new Category()
                {
                    Id = 2,
                    Name = "[Mock] Category Name 2",
                    Description = "[Mock] Category description 2"
                },
                new Category()
                {
                    Id = 3,
                    Name = "[Mock] Category Name 3",
                    Description = "[Mock] Category description 3"
                }
            };

            _categoriesService
                .Setup(mock => mock.GetAllAsync())
                .ReturnsAsync(categories);

            // Act
            var result = await _classUnderTest.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<CategoryDTO>>(okResult.Value);

            Assert.Equal(categories.Count, returnValue.Count);
            for (int i = 0; i < categories.Count; i++)
            {
                Assert.Equal(categories[i].Id, returnValue[i].Id);
                Assert.Equal(categories[i].Name, returnValue[i].Name);
                Assert.Equal(categories[i].Description, returnValue[i].Description);
            }
        }

        [Fact]
        public async Task GetAsync_ReturnsOk()
        {
            // Arrange
            var categoryId = 900;
            var category = new Category
            {
                Id = categoryId,
                Name = "Some cool category",
                Description = "Testing description"
            };

            _categoriesService
                .Setup(mock => mock.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(category);

            // Act
            var result = await _classUnderTest.GetAsync(categoryId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<CategoryDTO>(okResult.Value);

            Assert.Equal(category.Id, returnValue.Id);
            Assert.Equal(category.Name, returnValue.Name);
            Assert.Equal(category.Description, returnValue.Description);

            _categoriesService.Verify(mock => mock.GetAsync(categoryId), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ReturnsNotFound()
        {
            // Arrange
            var categoryId = 900;

            // Act
            var result = await _classUnderTest.GetAsync(categoryId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            _categoriesService.Verify(mock => mock.GetAsync(categoryId), Times.Once);
        }

        [Fact]
        public async Task SaveAsync_ReturnsOk()
        {
            // Arrange
            var categoryToSave = new CategoryUpsertDTO()
            {
                Name = "CategoryName to save",
                Description = "CategoryDescription to save"
            };

            _categoriesService
                .Setup(mock => mock.SaveAsync(It.IsAny<Category>()))
                .ReturnsAsync(new Category() 
                { 
                    Id = 1,
                    Name = categoryToSave.Name,
                    Description = categoryToSave.Description,
                    ProjectCategories = new List<ProjectCategory>()
                });

            // Act
            var result = await _classUnderTest.SaveAsync(categoryToSave);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<CategoryDTO>(okResult.Value);

            Assert.Equal(categoryToSave.Name, returnValue.Name);
            Assert.Equal(categoryToSave.Description, returnValue.Description);

            _categoriesService.Verify(mock => mock.SaveAsync(It.IsAny<Category>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOk()
        {
            // Arrange
            var updatedCategory = new CategoryUpsertDTO()
            {
                Name = "CategoryName to update",
                Description = "CategoryDescription to update"
            };

            var oldCategory = new Category()
            {
                Id = 1,
                Name = "CategoryName original",
                Description = "CategoryDescription original",
                ProjectCategories = new List<ProjectCategory>()
            };

            _categoriesService
                .Setup(mock => mock.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(oldCategory);

            _categoriesService
                .Setup(mock => mock.UpdateAsync(It.IsAny<Category>()))
                .ReturnsAsync(new Category()
                {
                    Id = oldCategory.Id,
                    Name = updatedCategory.Name,
                    Description = updatedCategory.Description,
                    ProjectCategories = oldCategory.ProjectCategories
                });

            // Act
            var result = await _classUnderTest.UpdateAsync(oldCategory.Id, updatedCategory);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<CategoryDTO>(okResult.Value);

            Assert.Equal(oldCategory.Id, returnValue.Id);
            Assert.Equal(updatedCategory.Name, returnValue.Name);
            Assert.Equal(updatedCategory.Description, returnValue.Description);
            Assert.Equal(oldCategory.ProjectCategories, oldCategory.ProjectCategories);

            _categoriesService.Verify(mock => mock.GetAsync(It.IsAny<int>()), Times.Once);
            _categoriesService.Verify(mock => mock.UpdateAsync(It.IsAny<Category>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNotFound()
        {
            // Arrange
            var updatedCategory = new CategoryUpsertDTO()
            {
                Name = "CategoryName to update",
                Description = "CategoryDescription to update"
            };

            // Act
            var result = await _classUnderTest.UpdateAsync(1, updatedCategory);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _categoriesService.Verify(mock => mock.GetAsync(It.IsAny<int>()), Times.Once);
            _categoriesService.Verify(mock => mock.UpdateAsync(It.IsAny<Category>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNoContent()
        {
            // Arrange
            var idToDelete = 1;

            _categoriesService
                .Setup(mock => mock.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new Category());

            // Act
            var result = await _classUnderTest.DeleteAsync(idToDelete);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _categoriesService.Verify(mock => mock.GetAsync(It.IsAny<int>()), Times.Once);
            _categoriesService.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound()
        {
            // Act
            var result = await _classUnderTest.DeleteAsync(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _categoriesService.Verify(mock => mock.GetAsync(It.IsAny<int>()), Times.Once);
            _categoriesService.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}
