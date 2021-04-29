using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Response;
using FRF.Core.Services;
using FRF.Web.Controllers;
using FRF.Web.Dtos.Modules;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FRF.Web.Tests.Controllers
{
    public class ModulesControllerTest
    {
        private readonly ModulesController _classUnderTest;
        private readonly Mock<IModulesService> _modulesServices;
        private readonly IMapper _mapper = MapperBuilder.Build();

        public ModulesControllerTest()
        {
            _modulesServices = new Mock<IModulesService>();
            _classUnderTest = new ModulesController(_modulesServices.Object,
                _mapper);
        }

        [Fact]
        public async Task GetAsync_ReturnsOkAndModule()
        {
            // Arrange
            const int ModuleId = 1;
            var module = CreateModule(ModuleId);
            _modulesServices
                .Setup(mock => mock.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<Module>(module));

            // Act
            var response = await _classUnderTest.GetAsync(ModuleId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var returnValue = Assert.IsType<ModuleDTO>(okResult.Value);

            Assert.Equal(ModuleId, returnValue.Id);
            Assert.Equal(module.Name, returnValue.Name);
            Assert.Equal(module.Description, returnValue.Description);
            Assert.Equal(module.SuggestedCost, returnValue.SuggestedCost);
            _modulesServices.Verify(mock => mock.GetAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task GetAsync_WhenModuleIsNotFound_ReturnsNotFound()
        {
            // Arrange
            _modulesServices
                .Setup(mock => mock.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<Module>(new Error(ErrorCodes.ModuleNotExist,
                    "Mock Not Found message.")));

            // Act
            var response = await _classUnderTest.GetAsync(It.IsAny<int>());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(response);
            var returnValue = Assert.IsType<Error>(notFoundResult.Value);

            Assert.Equal(ErrorCodes.ModuleNotExist, returnValue.Code);
            _modulesServices.Verify(mock => mock.GetAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOk()
        {
            // Arrange
            var modules = new List<Module>
            {
                CreateModule(new Random().Next()),
                CreateModule(new Random().Next()),
                CreateModule(new Random().Next())
            };

            _modulesServices
                .Setup(mock => mock.GetAllAsync())
                .ReturnsAsync(new ServiceResponse<IList<Module>>(modules));

            // Act
            var response = await _classUnderTest.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var returnValue = Assert.IsType<List<ModuleDTO>>(okResult.Value);
            _modulesServices.Verify(mock => mock.GetAllAsync(), Times.Once);
            Assert.Equal(modules.Count, returnValue.Count);
            for (var i = 0; i < modules.Count; i++)
            {
                Assert.Equal(modules[i].Id, returnValue[i].Id);
                Assert.Equal(modules[i].Name, returnValue[i].Name);
                Assert.Equal(modules[i].Description, returnValue[i].Description);
            }
        }

        [Fact]
        public async Task GetAllByCategoryIdAsync_ReturnsOk()
        {
            // Arrange
            const int CategoryId = 9;
            var modules = new List<Module>
            {
                CreateModule(new Random().Next(), CategoryId),
                CreateModule(new Random().Next(), CategoryId),
                CreateModule(new Random().Next(), CategoryId)
            };

            _modulesServices
                .Setup(mock => mock.GetAllByCategoryIdAsync(CategoryId))
                .ReturnsAsync(new ServiceResponse<IList<Module>>(modules));

            // Act
            var response = await _classUnderTest.GetAllByCategoryIdAsync(CategoryId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var returnValue = Assert.IsType<List<ModuleDTO>>(okResult.Value);
            _modulesServices.Verify(mock => mock.GetAllByCategoryIdAsync(It.IsAny<int>()), Times.Once);
            Assert.Equal(modules.Count, returnValue.Count);
            for (var i = 0; i < modules.Count; i++)
            {
                Assert.Equal(modules[i].Id, returnValue[i].Id);
                Assert.Equal(modules[i].Name, returnValue[i].Name);
                Assert.Equal(modules[i].Description, returnValue[i].Description);
                Assert.Equal(CategoryId, returnValue[i].CategoryModules[0].Category.Id);
            }
        }

        [Fact]
        public async Task GetAllByCategoryIdAsync_WhenCategoryNotExist_ReturnsNotFound()
        {
            // Arrange
            const int CategoryId = 9;

            _modulesServices
                .Setup(mock => mock.GetAllByCategoryIdAsync(CategoryId))
                .ReturnsAsync(new ServiceResponse<IList<Module>>(new Error(ErrorCodes.CategoryNotExists,
                    "Mock Not Found message.")));

            // Act
            var response = await _classUnderTest.GetAllByCategoryIdAsync(CategoryId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(response);
            var returnValue = Assert.IsType<Error>(notFoundResult.Value);

            Assert.Equal(ErrorCodes.CategoryNotExists, returnValue.Code);
            _modulesServices.Verify(mock => mock.GetAllByCategoryIdAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task SaveAsync_ReturnsOkAndSavedModule()
        {
            // Arrange
            const int ModuleId = 1;
            var module = CreateModule(ModuleId);
            _modulesServices
                .Setup(mock => mock.SaveAsync(It.IsAny<Module>()))
                .ReturnsAsync(new ServiceResponse<Module>(module));

            // Act
            var response = await _classUnderTest.SaveAsync(It.IsAny<ModuleUpsertDTO>());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var returnValue = Assert.IsType<ModuleDTO>(okResult.Value);

            Assert.Equal(ModuleId, returnValue.Id);
            Assert.Equal(module.Name, returnValue.Name);
            Assert.Equal(module.Description, returnValue.Description);
            Assert.Equal(module.SuggestedCost, returnValue.SuggestedCost);
            _modulesServices.Verify(mock => mock.SaveAsync(It.IsAny<Module>()), Times.Once);
        }

        [Fact]
        public async Task SaveAsync_ReturnsBadRequest()
        {
            // Arrange
            _modulesServices
                .Setup(mock => mock.SaveAsync(It.IsAny<Module>()))
                .ReturnsAsync(new ServiceResponse<Module>(new Error(ErrorCodes.CategoryNotExists,
                    "Mock Not Found message.")));

            // Act
            var response = await _classUnderTest.SaveAsync(It.IsAny<ModuleUpsertDTO>());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);
            var returnValue = Assert.IsType<Error>(badRequestResult.Value);

            Assert.Equal(ErrorCodes.CategoryNotExists, returnValue.Code);
            _modulesServices.Verify(mock => mock.SaveAsync(It.IsAny<Module>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOkAndUpdatedModule()
        {
            // Arrange
            const int ModuleId = 1;
            var module = CreateModule(ModuleId);
            var updatedModuleDTO = CreateModuleUpsertDto(module);
            var updatedModule = _mapper.Map<Module>(updatedModuleDTO);
            updatedModule.Id = ModuleId;
            _modulesServices
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
            _modulesServices.Verify(mock => mock.UpdateAsync(It.IsAny<Module>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenModuleIsNotFound_ReturnsNotFound()
        {
            // Arrange
            const int ModuleId = 1;
            var module = CreateModule(ModuleId);
            var updatedModuleDTO = CreateModuleUpsertDto(module);
            _modulesServices
                .Setup(mock => mock.UpdateAsync(It.IsAny<Module>()))
                .ReturnsAsync(new ServiceResponse<Module>(new Error(ErrorCodes.ModuleNotExist,
                    "Mock Not Found message.")));

            // Act
            var response = await _classUnderTest.UpdateAsync(ModuleId, updatedModuleDTO);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(response);
            var returnValue = Assert.IsType<Error>(notFoundResult.Value);

            Assert.Equal(ErrorCodes.ModuleNotExist, returnValue.Code);
            _modulesServices.Verify(mock => mock.UpdateAsync(It.IsAny<Module>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenModuleIsNotFound_ReturnsNotFound()
        {
            // Arrange
            _modulesServices
                .Setup(mock => mock.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<Module>(new Error(ErrorCodes.ModuleNotExist,
                    "Mock Not Found message.")));

            // Act
            var response = await _classUnderTest.DeleteAsync(It.IsAny<int>());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(response);
            var returnValue = Assert.IsType<Error>(notFoundResult.Value);

            Assert.Equal(ErrorCodes.ModuleNotExist, returnValue.Code);
            _modulesServices.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNoContent()
        {
            // Arrange
            const int ModuleId = 1;
            var module = CreateModule(ModuleId);
            _modulesServices
                .Setup(mock => mock.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<Module>(module));

            // Act
            var response = await _classUnderTest.DeleteAsync(It.IsAny<int>());

            // Assert
            var notFoundResult = Assert.IsType<NoContentResult>(response);

            _modulesServices.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Once);
        }

        private Module CreateModule(int moduleId)
        {
            var module = new Module
            {
                Id = moduleId,
                Name = "Mock module",
                Description = "Mock Description",
                SuggestedCost = 9
            };
            return module;
        }

        private Module CreateModule(int moduleId, int categoryId)
        {
            var module = new Module
            {
                Id = moduleId,
                Name = "Mock module",
                Description = "Mock Description",
                SuggestedCost = 9,
                CategoryModules = new List<CategoryModule>
                {
                    new CategoryModule
                    {
                        Category = new Category
                        {
                            Id = categoryId,
                            Name = "[Mock] Category Name 1",
                            Description = "[Mock] Category description 1"
                        }
                    }
                }
            };
            return module;
        }

        private ModuleUpsertDTO CreateModuleUpsertDto(Module module)
        {
            var updateModule = new ModuleUpsertDTO
            {
                Description = module.Description,
                Name = "Updated Role Name",
                SuggestedCost = module.SuggestedCost
            };
            return updateModule;
        }
    }
}