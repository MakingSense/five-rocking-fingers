using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Services;
using FRF.Web.Controllers;
using FRF.Web.Dtos.Categories;
using Microsoft.AspNetCore.Mvc;
using Moq;
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
    }
}
