using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Response;
using FRF.Core.Services;
using FRF.Web.Controllers;
using FRF.Web.Dtos.ProjectModules;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FRF.Web.Tests.Controllers
{
    public class ProjectModulesControllerTests
    {
        private readonly IMapper _mapper = MapperBuilder.Build();
        private readonly Mock<IProjectModulesService> _projectModulesService;
        private readonly ProjectModulesController _classUnderTest;

        public ProjectModulesControllerTests()
        {
            _projectModulesService = new Mock<IProjectModulesService>();

            _classUnderTest = new ProjectModulesController(_projectModulesService.Object, _mapper);
        }

        [Fact]
        public async Task GetByProjectIdAsync_ReturnsOk()
        {
            // Arrange
            var projectModuleId = 1;
            var projectId = 1;
            var moduleId = 1;
            var projectModuleAlias = "[Mock] Alias";
            var projectModuleCost = 10;

            var projectModules = new List<ProjectModule>()
            {
                new ProjectModule()
                {
                    Id = projectModuleId,
                    ProjectId = projectId,
                    ModuleId = moduleId,
                    Alias = projectModuleAlias,
                    Cost = projectModuleCost
                }
            };

            _projectModulesService
                .Setup(mock => mock.GetByProjectIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<List<ProjectModule>>(projectModules));

            // Act
            var result = await _classUnderTest.GetByProjectIdAsync(projectId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultValue = Assert.IsType<List<ProjectModuleDTO>>(okResult.Value);

            Assert.Equal(projectModules.Count, resultValue.Count);

            CompareProjectModules(_mapper.Map<ProjectModuleDTO>(projectModules[0]), resultValue[0]);
        }

        [Fact]
        public async Task GetByProjectIdAsync_ReturnsNotFound()
        {
            // Arrange
            var projectId = 0;
            var error = new Error(ErrorCodes.ProjectNotExists, "[Mock] Message");

            _projectModulesService
               .Setup(mock => mock.GetByProjectIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<List<ProjectModule>>(error));

            // Act
            var result = await _classUnderTest.GetByProjectIdAsync(projectId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var resultValue = Assert.IsType<Error>(notFoundResult.Value);
            Assert.Equal(ErrorCodes.ProjectNotExists, resultValue.Code);
            _projectModulesService.Verify(mock => mock.GetByProjectIdAsync(projectId), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ReturnsOk()
        {
            // Arrange
            var projectModuleId = 1;
            var projectId = 1;
            var moduleId = 1;
            var projectModuleAlias = "[Mock] Alias";
            var projectModuleCost = 10;

            var projectModule = new ProjectModule()
            {
                Id = projectModuleId,
                ProjectId = projectId,
                ModuleId = moduleId,
                Alias = projectModuleAlias,
                Cost = projectModuleCost
            };

            _projectModulesService
               .Setup(mock => mock.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<ProjectModule>(projectModule));

            // Act
            var result = await _classUnderTest.GetAsync(projectModuleId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultValue = Assert.IsType<ProjectModuleDTO>(okResult.Value);

            CompareProjectModules(_mapper.Map<ProjectModuleDTO>(projectModule), resultValue);
            _projectModulesService.Verify(mock => mock.GetAsync(projectModuleId), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ReturnsNotFound()
        {
            // Arrange
            var projectModuleId = 1;

            _projectModulesService
                .Setup(mock => mock.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<ProjectModule>(new Error(ErrorCodes.ProjectModuleNotExists, "[Mock] Message")));

            // Act
            var result = await _classUnderTest.GetAsync(projectModuleId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var resultValue = Assert.IsType<Error>(notFoundResult.Value);
            Assert.Equal(ErrorCodes.ProjectModuleNotExists, resultValue.Code);
            _projectModulesService.Verify(mock => mock.GetAsync(projectModuleId), Times.Once);
        }

        [Fact]
        public async Task SaveAsync_ReturnsOk()
        {
            // Arrange
            var projectModuleId = 1;
            var projectId = 1;
            var moduleId = 1;
            var projectModuleAlias = "[Mock] Alias";
            var projectModuleCost = 10;

            var projectModuleToSave = new ProjectModuleUpsertDTO()
            {
                ProjectId = projectId,
                ModuleId = moduleId,
                Alias = projectModuleAlias,
                Cost = projectModuleCost
            };

            _projectModulesService
               .Setup(mock => mock.SaveAsync(It.IsAny<ProjectModule>()))
                .ReturnsAsync(new ServiceResponse<ProjectModule>(new ProjectModule()
                {
                    Id = projectModuleId,
                    ProjectId = projectId,
                    ModuleId = moduleId,
                    Alias = projectModuleAlias,
                    Cost = projectModuleCost
                }));

            // Act
            var result = await _classUnderTest.SaveAsync(projectModuleToSave);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultValue = Assert.IsType<ProjectModuleDTO>(okResult.Value);

            Assert.Equal(projectModuleId, resultValue.Id);
            Assert.Equal(projectModuleToSave.Alias, resultValue.Alias);
            Assert.Equal(projectModuleToSave.Cost, resultValue.Cost);
            Assert.Equal(projectModuleToSave.ProjectId, resultValue.ProjectId);
            Assert.Equal(projectModuleToSave.ModuleId, resultValue.ModuleId);
            _projectModulesService.Verify(mock => mock.SaveAsync(It.IsAny<ProjectModule>()), Times.Once);
        }

        [Fact]
        public async Task SaveAsync_ReturnsNotFound_ProjectNotExists()
        {
            // Arrange
            var projectId = 1;
            var moduleId = 1;
            var projectModuleAlias = "[Mock] Alias";
            var projectModuleCost = 10;

            var projectModuleToSave = new ProjectModuleUpsertDTO()
            {
                ProjectId = projectId,
                ModuleId = moduleId,
                Alias = projectModuleAlias,
                Cost = projectModuleCost
            };

            _projectModulesService
                .Setup(mock => mock.SaveAsync(It.IsAny<ProjectModule>()))
                .ReturnsAsync(new ServiceResponse<ProjectModule>(new Error(ErrorCodes.ProjectNotExists, "[Mock] Message")));

            // Act
            var result = await _classUnderTest.SaveAsync(projectModuleToSave);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var resultValue = Assert.IsType<Error>(notFoundResult.Value);
            Assert.Equal(ErrorCodes.ProjectNotExists, resultValue.Code);
            _projectModulesService.Verify(mock => mock.SaveAsync(It.IsAny<ProjectModule>()), Times.Once);
        }

        [Fact]
        public async Task SaveAsync_ReturnsBadRequest_InvalidCostForProjectModule()
        {
            // Arrange
            var projectId = 1;
            var moduleId = 1;
            var projectModuleAlias = "[Mock] Alias";
            var projectModuleCost = 10;

            var projectModuleToSave = new ProjectModuleUpsertDTO()
            {
                ProjectId = projectId,
                ModuleId = moduleId,
                Alias = projectModuleAlias,
                Cost = projectModuleCost
            };

            _projectModulesService
                .Setup(mock => mock.SaveAsync(It.IsAny<ProjectModule>()))
                .ReturnsAsync(new ServiceResponse<ProjectModule>(new Error(ErrorCodes.ModuleCostInvalid, "[Mock] Message")));

            // Act
            var result = await _classUnderTest.SaveAsync(projectModuleToSave);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var resultValue = Assert.IsType<Error>(badRequestResult.Value);
            Assert.Equal(ErrorCodes.ModuleCostInvalid, resultValue.Code);
            _projectModulesService.Verify(mock => mock.SaveAsync(It.IsAny<ProjectModule>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOk()
        {
            // Arrange
            var projectModuleId = 1;
            var projectId = 1;

            var moduleIdNew = 1;
            var projectModuleAliasNew = "[Mock] New Alias";
            var projectModuleCostNew = 10;

            var moduleIdOld = 2;
            var projectModuleAliasOld = "[Mock] Old Alias";
            var projectModuleCostOld = 20;

            var updatedProjectModule = new ProjectModuleUpsertDTO()
            {
                ProjectId = projectId,
                ModuleId = moduleIdNew,
                Alias = projectModuleAliasNew,
                Cost = projectModuleCostNew
            };

            var oldProjectModule = new ProjectModule()
            {
                Id = projectModuleId,
                ProjectId = projectId,
                ModuleId = moduleIdOld,
                Alias = projectModuleAliasOld,
                Cost = projectModuleCostOld
            };

            _projectModulesService
                .Setup(mock => mock.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<ProjectModule>(oldProjectModule));

            _projectModulesService
                .Setup(mock => mock.UpdateAsync(It.IsAny<ProjectModule>()))
                .ReturnsAsync(new ServiceResponse<ProjectModule>(new ProjectModule()
                {
                    Id = projectModuleId,
                    Alias = updatedProjectModule.Alias,
                    Cost = updatedProjectModule.Cost,
                    ProjectId = updatedProjectModule.ProjectId,
                    ModuleId = updatedProjectModule.ModuleId
                }));

            // Act
            var result = await _classUnderTest.UpdateAsync(oldProjectModule.Id, updatedProjectModule);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultValue = Assert.IsType<ProjectModuleDTO>(okResult.Value);

            Assert.Equal(updatedProjectModule.Alias, resultValue.Alias);
            Assert.Equal(updatedProjectModule.Cost, resultValue.Cost);
            Assert.Equal(updatedProjectModule.ProjectId, resultValue.ProjectId);
            Assert.Equal(updatedProjectModule.ModuleId, resultValue.ModuleId);

            _projectModulesService.Verify(mock => mock.UpdateAsync(It.IsAny<ProjectModule>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNotFound_ProjectNotExists()
        {
            // Arrange
            var projectModuleId = 1;
            var projectId = 1;
            var moduleId = 1;
            var alias = "[Mock] Alias";
            var cost = 10;

            var updatedProjectModule = new ProjectModuleUpsertDTO()
            {
                Cost = cost,
                Alias = alias,
                ProjectId = projectId,
                ModuleId = moduleId
            };

            _projectModulesService
                .Setup(mock => mock.UpdateAsync(It.IsAny<ProjectModule>()))
                .ReturnsAsync(new ServiceResponse<ProjectModule>(new Error(ErrorCodes.ProjectNotExists, "[Mock] Message")));

            // Act
            var result = await _classUnderTest.UpdateAsync(projectModuleId, updatedProjectModule);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var resultValue = Assert.IsType<Error>(notFoundResult.Value);
            Assert.Equal(ErrorCodes.ProjectNotExists, resultValue.Code);
            _projectModulesService.Verify(mock => mock.UpdateAsync(It.IsAny<ProjectModule>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadRequest_InvalidBeginDateForProjectModule()
        {
            // Arrange
            var projectModuleId = 1;
            var projectId = 1;
            var moduleId = 1;
            var alias = "[Mock] Alias";
            var cost = -1;

            var updatedProjectModule = new ProjectModuleUpsertDTO()
            {
                Cost = cost,
                Alias = alias,
                ProjectId = projectId,
                ModuleId = moduleId
            };

            _projectModulesService
                .Setup(mock => mock.UpdateAsync(It.IsAny<ProjectModule>()))
                .ReturnsAsync(new ServiceResponse<ProjectModule>(new Error(ErrorCodes.ModuleCostInvalid, "[Mock] Message")));

            // Act
            var result = await _classUnderTest.UpdateAsync(projectModuleId, updatedProjectModule);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var resultValue = Assert.IsType<Error>(badRequestResult.Value);
            Assert.Equal(ErrorCodes.ModuleCostInvalid, resultValue.Code);
            _projectModulesService.Verify(mock => mock.UpdateAsync(It.IsAny<ProjectModule>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNoContent()
        {
            // Arrange
            var idToDelete = 1;

            _projectModulesService
                .Setup(mock => mock.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<ProjectModule>(new ProjectModule()));

            // Act
            var result = await _classUnderTest.DeleteAsync(idToDelete);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _projectModulesService.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound()
        {
            // Arrange
            var idToDelete = 1;

            _projectModulesService
                .Setup(mock => mock.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<ProjectModule>(new Error(ErrorCodes.ProjectModuleNotExists, "Error message")));

            // Act
            var result = await _classUnderTest.DeleteAsync(idToDelete);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var resultValue = Assert.IsType<Error>(notFoundResult.Value);
            Assert.Equal(ErrorCodes.ProjectModuleNotExists, resultValue.Code);
            _projectModulesService.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Once);
        }

        private void CompareProjectModules(ProjectModuleDTO projectModuleExpected, ProjectModuleDTO projectModuleActual)
        {
            Assert.Equal(projectModuleExpected.Alias, projectModuleActual.Alias);
            Assert.Equal(projectModuleExpected.Cost, projectModuleActual.Cost);
            Assert.Equal(projectModuleExpected.ProjectId, projectModuleActual.ProjectId);
            Assert.Equal(projectModuleExpected.ModuleId, projectModuleActual.ModuleId);
        }
    }
}
