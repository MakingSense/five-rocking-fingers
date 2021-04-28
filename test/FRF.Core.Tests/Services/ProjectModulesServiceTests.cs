using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Response;
using FRF.Core.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FRF.Core.Tests.Services
{
    public class ProjectModulesServiceTests
    {
        private readonly Mock<IConfiguration> _configuration;
        private readonly IMapper _mapper = MapperBuilder.Build();
        private readonly DataAccessContextForTest _dataAccess;
        private readonly ProjectModulesService _classUnderTest;

        public ProjectModulesServiceTests()
        {
            _configuration = new Mock<IConfiguration>();

            _dataAccess = new DataAccessContextForTest(Guid.NewGuid(), _configuration.Object);

            _dataAccess.Database.EnsureDeleted();
            _dataAccess.Database.EnsureCreated();

            _classUnderTest = new ProjectModulesService(_dataAccess, _mapper);
        }

        private DataAccess.EntityModels.ProjectModule CreateProjectModule(int projectId, int moduleId)
        {
            var projectModule = new DataAccess.EntityModels.ProjectModule()
            {
                Alias = "[Mock] Alias",
                Cost = 10,
                ProjectId = projectId,
                ModuleId = moduleId
            };
            _dataAccess.ProjectModules.Add(projectModule);
            _dataAccess.SaveChanges();

            return projectModule;
        }

        private DataAccess.EntityModels.Module CreateModule()
        {
            var module = new DataAccess.EntityModels.Module()
            {
                Name = "[Mock] Name",
                Description = "[Mock] Description",
                SuggestedCost = 10
            };
            _dataAccess.Modules.Add(module);
            _dataAccess.SaveChanges();

            return module;
        }

        private DataAccess.EntityModels.Project CreateProject()
        {
            var project = new DataAccess.EntityModels.Project()
            {
                Name = "[Mock] Project name",
                Budget = 100,
                ProjectCategories = new List<DataAccess.EntityModels.ProjectCategory>(),
                StartDate = DateTime.Now
            };

            _dataAccess.Projects.Add(project);
            _dataAccess.SaveChanges();

            return project;
        }

        [Fact]
        public async Task GetByProjectIdAsync_ReturnsList()
        {
            // Arrange
            var module = CreateModule();
            var project = CreateProject();
            var projectModule = CreateProjectModule(project.Id, module.Id);

            // Act
            var result = await _classUnderTest.GetByProjectIdAsync(project.Id);

            // Assert
            Assert.IsType<ServiceResponse<List<ProjectModule>>>(result);
            Assert.True(result.Success);
            var resultValue = Assert.Single(result.Value);

            CompareProjectModules(_mapper.Map<ProjectModule>(projectModule), resultValue);
        }

        [Fact]
        public async Task GetByProjectIdAsync_ReturnsEmptyList()
        {
            // Arrange
            var project = CreateProject();

            // Act
            var result = await _classUnderTest.GetByProjectIdAsync(project.Id);

            // Assert
            Assert.IsType<ServiceResponse<List<ProjectModule>>>(result);
            Assert.True(result.Success);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetAsync_ReturnsProjectModule()
        {
            // Arrange
            var module = CreateModule();
            var project = CreateProject();
            var projectModule = CreateProjectModule(project.Id, module.Id);

            // Act
            var result = await _classUnderTest.GetAsync(projectModule.Id);

            // Assert
            Assert.IsType<ServiceResponse<ProjectModule>>(result);
            Assert.True(result.Success);
            var resultValue = result.Value;

            CompareProjectModules(_mapper.Map<ProjectModule>(projectModule), resultValue);
        }

        [Fact]
        public async Task GetAsync_ReturnsNull()
        {
            // Arrange
            var projectResourceId = 0;

            // Act
            var result = await _classUnderTest.GetAsync(projectResourceId);

            // Assert
            Assert.IsType<ServiceResponse<ProjectModule>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorCodes.ProjectModuleNotExists, result.Error.Code);
        }

        [Fact]
        public async Task SaveAsync_ReturnsProjectModule()
        {
            // Arrange
            var module = CreateModule();
            var project = CreateProject();
            var projectModuleAlias = "[Mock] Alias";
            var projectModuleCost = 10;

            var projectModuleToSave = new ProjectModule()
            {
                Alias = projectModuleAlias,
                Cost = projectModuleCost,
                ProjectId = project.Id,
                ModuleId = module.Id
            };

            // Act
            var result = await _classUnderTest.SaveAsync(projectModuleToSave);

            // Assert
            Assert.IsType<ServiceResponse<ProjectModule>>(result);
            Assert.True(result.Success);
            var resultValue = result.Value;

            CompareProjectModules(projectModuleToSave, resultValue);
        }

        [Fact]
        public async Task SaveAsync_ReturnsExceptionNoProject()
        {
            // Arrange
            var module = CreateModule();
            var projectId = 0;
            var projectModuleAlias = "[Mock] Alias";
            var projectModuleCost = 10;

            var projectModuleToSave = new ProjectModule()
            {
                Alias = projectModuleAlias,
                Cost = projectModuleCost,
                ProjectId = projectId,
                ModuleId = module.Id
            };

            // Act
            var result = await _classUnderTest.SaveAsync(projectModuleToSave);

            // Assert
            Assert.IsType<ServiceResponse<ProjectModule>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorCodes.ProjectNotExists, result.Error.Code);
        }

        [Fact]
        public async Task SaveAsync_ReturnsExceptionNoModule()
        {
            // Arrange
            var moduleId = 0;
            var project = CreateProject();
            var projectModuleAlias = "[Mock] Alias";
            var projectModuleCost = 10;

            var projectResourceToSave = new ProjectModule()
            {
                Alias = projectModuleAlias,
                Cost = projectModuleCost,
                ProjectId = project.Id,
                ModuleId = moduleId
            };

            // Act
            var result = await _classUnderTest.SaveAsync(projectResourceToSave);

            // Assert
            Assert.IsType<ServiceResponse<ProjectModule>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorCodes.ModuleNotExists, result.Error.Code);
        }

        [Fact]
        public async Task SaveAsync_ReturnsExceptionInvalidCost()
        {
            // Arrange
            var module = CreateModule();
            var project = CreateProject();
            var projectModuleAlias = "[Mock] Alias";
            var projectModuleCost = -1;

            var projectModuleToSave = new ProjectModule()
            {
                Alias = projectModuleAlias,
                Cost = projectModuleCost,
                ProjectId = project.Id,
                ModuleId = module.Id
            };

            // Act
            var result = await _classUnderTest.SaveAsync(projectModuleToSave);

            // Assert
            Assert.IsType<ServiceResponse<ProjectModule>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorCodes.ModuleCostInvalid, result.Error.Code);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsProjectResource()
        {
            // Arrange
            var module = CreateModule();
            var project = CreateProject();
            var projectModule = CreateProjectModule(project.Id, module.Id);
            var projectModuleAlias = "[Mock] Updated Alias";
            var projectModuleCost = 1000;

            var updatedProjectModule = new ProjectModule()
            {
                Id = projectModule.Id,
                Alias = projectModuleAlias,
                Cost = projectModuleCost,
                ProjectId = project.Id,
                ModuleId = module.Id
            };

            // Act
            var result = await _classUnderTest.UpdateAsync(updatedProjectModule);

            // Assert
            Assert.IsType<ServiceResponse<ProjectModule>>(result);
            Assert.True(result.Success);
            var resultValue = result.Value;

            CompareProjectModules(updatedProjectModule, resultValue);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsExceptionNoProject()
        {
            // Arrange
            var module = CreateModule();
            var projectId = 0;
            var projectModule = CreateProjectModule(projectId, module.Id);
            var projectModuleAlias = "[Mock] Updated Alias";
            var projectModuleCost = 1000;

            var updatedProjectModule = new ProjectModule()
            {
                Id = projectModule.Id,
                Alias = projectModuleAlias,
                Cost = projectModuleCost,
                ProjectId = projectId,
                ModuleId = module.Id
            };

            // Act
            var result = await _classUnderTest.UpdateAsync(updatedProjectModule);

            // Assert
            Assert.IsType<ServiceResponse<ProjectModule>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorCodes.ProjectNotExists, result.Error.Code);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsExceptionNoModule()
        {
            // Arrange
            var moduleId = 0;
            var project = CreateProject();
            var projectModule = CreateProjectModule(project.Id, moduleId);
            var projectModuleAlias = "[Mock] Updated Alias";
            var projectModuleCost = 1000;

            var updatedProjectModule = new ProjectModule()
            {
                Id = projectModule.Id,
                Alias = projectModuleAlias,
                Cost = projectModuleCost,
                ProjectId = project.Id,
                ModuleId = moduleId
            };

            // Act
            var result = await _classUnderTest.UpdateAsync(updatedProjectModule);

            // Assert
            Assert.IsType<ServiceResponse<ProjectModule>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorCodes.ModuleNotExists, result.Error.Code);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsExceptionInvalidCost()
        {
            // Arrange
            var module = CreateModule();
            var project = CreateProject();
            var projectModule = CreateProjectModule(project.Id, module.Id);
            var projectModuleAlias = "[Mock] Updated Alias";
            var projectModuleCost = -1;

            var updatedProjectModule = new ProjectModule()
            {
                Id = projectModule.Id,
                Alias = projectModuleAlias,
                Cost = projectModuleCost,
                ProjectId = project.Id,
                ModuleId = module.Id
            };

            // Act
            var result = await _classUnderTest.UpdateAsync(updatedProjectModule);

            // Assert
            Assert.IsType<ServiceResponse<ProjectModule>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorCodes.ModuleCostInvalid, result.Error.Code);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsProjectModule()
        {
            // Arrange
            var module = CreateModule();
            var project = CreateProject();
            var projectModule = CreateProjectModule(project.Id, module.Id);

            // Act
            var result = await _classUnderTest.DeleteAsync(projectModule.Id);

            // Assert
            Assert.IsType<ServiceResponse<ProjectModule>>(result);
            Assert.True(result.Success);
            var resultValue = result.Value;

            CompareProjectModules(_mapper.Map<ProjectModule>(projectModule), resultValue);
        }

        private void CompareProjectModules(ProjectModule projectModuleExpected, ProjectModule projectModuleActual)
        {
            Assert.Equal(projectModuleExpected.Alias, projectModuleActual.Alias);
            Assert.Equal(projectModuleExpected.Cost, projectModuleActual.Cost);
            Assert.Equal(projectModuleExpected.ProjectId, projectModuleActual.ProjectId);
            Assert.Equal(projectModuleExpected.ModuleId, projectModuleActual.ModuleId);
        }
    }
}
