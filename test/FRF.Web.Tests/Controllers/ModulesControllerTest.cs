using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Response;
using FRF.Core.Services;
using FRF.Web.Controllers;
using FRF.Web.Dtos.Modules;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace FRF.Web.Tests.Controllers
{
    public class ModulesControllerTest
    {
        private readonly ModulesController _classUnderTest;
        private readonly Mock<IModulesService> _resourcesServices;
        private readonly IMapper _mapper = MapperBuilder.Build();

        public ModulesControllerTest()
        {
            _resourcesServices = new Mock<IModulesService>();
            _classUnderTest = new ModulesController(_resourcesServices.Object,
                _mapper);
        }

        [Fact]
        public async Task GetAsync_ReturnsOkAndModule()
        {
            // Arrange
            const int ModuleId = 1;
            var resource = CreateModule(ModuleId);
            _resourcesServices
                .Setup(mock => mock.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<Module>(resource));

            // Act
            var response = await _classUnderTest.GetAsync(ModuleId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var returnValue = Assert.IsType<ModuleDTO>(okResult.Value);

            Assert.Equal(ModuleId, returnValue.Id);
            Assert.Equal(resource.Name, returnValue.Name);
            Assert.Equal(resource.Description, returnValue.Description);
            Assert.Equal(resource.SuggestedCost, returnValue.SuggestedCost);
            _resourcesServices.Verify(mock => mock.GetAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task GetAsync_WhenModuleIsNotFound_ReturnsNotFound()
        {
            // Arrange
            _resourcesServices
                .Setup(mock => mock.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<Module>(new Error(ErrorCodes.ModuleNotExist,
                    "Mock Not Found message.")));

            // Act
            var response = await _classUnderTest.GetAsync(It.IsAny<int>());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(response);
            var returnValue = Assert.IsType<Error>(notFoundResult.Value);

            Assert.Equal(ErrorCodes.ModuleNotExist, returnValue.Code);
            _resourcesServices.Verify(mock => mock.GetAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task SaveAsync_ReturnsOkAndSavedModule()
        {
            // Arrange
            const int ModuleId = 1;
            var resource = CreateModule(ModuleId);
            _resourcesServices
                .Setup(mock => mock.SaveAsync(It.IsAny<Module>()))
                .ReturnsAsync(new ServiceResponse<Module>(resource));

            // Act
            var response = await _classUnderTest.SaveAsync(It.IsAny<ModuleUpsertDTO>());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var returnValue = Assert.IsType<ModuleDTO>(okResult.Value);

            Assert.Equal(ModuleId, returnValue.Id);
            Assert.Equal(resource.Name, returnValue.Name);
            Assert.Equal(resource.Description, returnValue.Description);
            Assert.Equal(resource.SuggestedCost, returnValue.SuggestedCost);
            _resourcesServices.Verify(mock => mock.SaveAsync(It.IsAny<Module>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOkAndUpdatedModule()
        {
            // Arrange
            const int ModuleId = 1;
            var resource = CreateModule(ModuleId);
            var updatedModuleDTO = CreateModuleUpsertDto(resource);
            var updatedModule = _mapper.Map<Module>(updatedModuleDTO);
            updatedModule.Id = ModuleId;
            _resourcesServices
                .Setup(mock => mock.UpdateAsync(It.IsAny<Module>()))
                .ReturnsAsync(new ServiceResponse<Module>(updatedModule));

            // Act
            var response = await _classUnderTest.UpdateAsync(ModuleId, updatedModuleDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var returnValue = Assert.IsType<ModuleDTO>(okResult.Value);

            Assert.Equal(ModuleId, returnValue.Id);
            Assert.Equal(updatedModuleDTO.Name, returnValue.Name);
            Assert.Equal(updatedModuleDTO.Description, returnValue.Description);
            Assert.Equal(updatedModuleDTO.SuggestedCost, returnValue.SuggestedCost);
            _resourcesServices.Verify(mock => mock.UpdateAsync(It.IsAny<Module>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenModuleIsNotFound_ReturnsNotFound()
        {
            // Arrange
            const int ModuleId = 1;
            var resource = CreateModule(ModuleId);
            var updatedModuleDTO = CreateModuleUpsertDto(resource);
            _resourcesServices
                .Setup(mock => mock.UpdateAsync(It.IsAny<Module>()))
                .ReturnsAsync(new ServiceResponse<Module>(new Error(ErrorCodes.ModuleNotExist,
                    "Mock Not Found message.")));

            // Act
            var response = await _classUnderTest.UpdateAsync(ModuleId,updatedModuleDTO);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(response);
            var returnValue = Assert.IsType<Error>(notFoundResult.Value);

            Assert.Equal(ErrorCodes.ModuleNotExist, returnValue.Code);
            _resourcesServices.Verify(mock => mock.UpdateAsync(It.IsAny<Module>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenModuleIsNotFound_ReturnsNotFound()
        {
            // Arrange
            _resourcesServices
                .Setup(mock => mock.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<Module>(new Error(ErrorCodes.ModuleNotExist,
                    "Mock Not Found message.")));

            // Act
            var response = await _classUnderTest.DeleteAsync(It.IsAny<int>());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(response);
            var returnValue = Assert.IsType<Error>(notFoundResult.Value);

            Assert.Equal(ErrorCodes.ModuleNotExist, returnValue.Code);
            _resourcesServices.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNoContent()
        {
            // Arrange
            const int ModuleId = 1;
            var resource = CreateModule(ModuleId);
            _resourcesServices
                .Setup(mock => mock.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<Module>(resource));

            // Act
            var response = await _classUnderTest.DeleteAsync(It.IsAny<int>());

            // Assert
            var notFoundResult = Assert.IsType<NoContentResult>(response);

            _resourcesServices.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Once);
        }

        private Module CreateModule(int resourceId)
        {
            var module = new Module()
            {
                Id = resourceId,
                Name = "Mock module",
                Description = "Mock Description",
                SuggestedCost = 9
            };
            return module;
        }

        private ModuleUpsertDTO CreateModuleUpsertDto(Module resource)
        {
            var updateModule = new ModuleUpsertDTO
            {
                Description = resource.Description,
                Name = "Updated Role Name",
                SuggestedCost = resource.SuggestedCost,
            };
            return updateModule;
        }
    }
}