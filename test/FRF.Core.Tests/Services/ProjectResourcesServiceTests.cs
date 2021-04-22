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
    public class ProjectResourcesServiceTests
    {
        private readonly Mock<IConfiguration> _configuration;
        private readonly IMapper _mapper = MapperBuilder.Build();
        private readonly DataAccessContextForTest _dataAccess;
        private readonly ProjectResourcesService _classUnderTest;

        public ProjectResourcesServiceTests()
        {
            _configuration = new Mock<IConfiguration>();

            _dataAccess = new DataAccessContextForTest(Guid.NewGuid(), _configuration.Object);

            _dataAccess.Database.EnsureDeleted();
            _dataAccess.Database.EnsureCreated();

            _classUnderTest = new ProjectResourcesService(_dataAccess, _mapper);
        }

        private DataAccess.EntityModels.ProjectResource CreateProjectResource(int projectId, int resourceId)
        {
            var projectResource = new DataAccess.EntityModels.ProjectResource()
            {
                BeginDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(5),
                DedicatedHours = 8,
                ProjectId = projectId,
                ResourceId = resourceId

            };
            _dataAccess.ProjectResources.Add(projectResource);
            _dataAccess.SaveChanges();

            return projectResource;
        }

        private DataAccess.EntityModels.Resource CreateResource()
        {
            var resource = new DataAccess.EntityModels.Resource()
            {
                Description = "Description",
                RoleName = "Rol Name",
                SalaryPerMonth = 10m,
                WorkloadCapacity = 10
            };
            _dataAccess.Resources.Add(resource);
            _dataAccess.SaveChanges();

            return resource;
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
            var resource = CreateResource();
            var project = CreateProject();
            var projectResource = CreateProjectResource(project.Id, resource.Id);

            // Act
            var result = await _classUnderTest.GetByProjectIdAsync(project.Id);

            // Assert
            Assert.IsType<ServiceResponse<List<ProjectResource>>>(result);
            Assert.True(result.Success);
            var resultValue = Assert.Single(result.Value);

            Assert.Equal(projectResource.Id, resultValue.Id);
            Assert.Equal(projectResource.BeginDate, resultValue.BeginDate);
            Assert.Equal(projectResource.EndDate, resultValue.EndDate);
            Assert.Equal(projectResource.DedicatedHours, resultValue.DedicatedHours);
            Assert.Equal(projectResource.ProjectId, project.Id);
            Assert.Equal(projectResource.ResourceId, resource.Id);
        }

        [Fact]
        public async Task GetByProjectIdAsync_ReturnsEmptyList()
        {
            // Arrange
            var project = CreateProject();

            // Act
            var result = await _classUnderTest.GetByProjectIdAsync(project.Id);

            // Assert
            Assert.IsType<ServiceResponse<List<ProjectResource>>>(result);
            Assert.True(result.Success);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetAsync_ReturnsProjectResource()
        {
            // Arrange
            var resource = CreateResource();
            var project = CreateProject();
            var projectResource = CreateProjectResource(project.Id, resource.Id);

            // Act
            var result = await _classUnderTest.GetAsync(projectResource.Id);

            // Assert
            Assert.IsType<ServiceResponse<ProjectResource>>(result);
            Assert.True(result.Success);
            var resultValue = result.Value;

            Assert.Equal(projectResource.Id, resultValue.Id);
            Assert.Equal(projectResource.BeginDate, resultValue.BeginDate);
            Assert.Equal(projectResource.EndDate, resultValue.EndDate);
            Assert.Equal(projectResource.DedicatedHours, resultValue.DedicatedHours);
            Assert.Equal(projectResource.ProjectId, project.Id);
            Assert.Equal(projectResource.ResourceId, resource.Id);
        }

        [Fact]
        public async Task GetAsync_ReturnsNull()
        {
            // Arrange
            var projectResourceId = 0;

            // Act
            var result = await _classUnderTest.GetAsync(projectResourceId);

            // Assert
            Assert.IsType<ServiceResponse<ProjectResource>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorCodes.ProjectResourceNotExists, result.Error.Code);
        }

        [Fact]
        public async Task SaveAsync_ReturnsProjectResource()
        {
            // Arrange
            var resource = CreateResource();
            var project = CreateProject();
            var projectResourceToSave = new ProjectResource()
            {
                BeginDate = project.StartDate,
                EndDate = project.StartDate?.AddDays(5),
                DedicatedHours = 8,
                ProjectId = project.Id,
                ResourceId = resource.Id
            };

            // Act
            var result = await _classUnderTest.SaveAsync(projectResourceToSave);

            // Assert
            Assert.IsType<ServiceResponse<ProjectResource>>(result);
            Assert.True(result.Success);
            var resultValue = result.Value;

            Assert.Equal(projectResourceToSave.BeginDate, resultValue.BeginDate);
            Assert.Equal(projectResourceToSave.EndDate, resultValue.EndDate);
            Assert.Equal(projectResourceToSave.DedicatedHours, resultValue.DedicatedHours);
            Assert.Equal(projectResourceToSave.ProjectId, project.Id);
            Assert.Equal(projectResourceToSave.ResourceId, resource.Id);
        }

        [Fact]
        public async Task SaveAsync_ReturnsExceptionNoProject()
        {
            // Arrange
            var resource = CreateResource();
            var projectId = 0;
            var projectResourceToSave = new ProjectResource()
            {
                BeginDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(5),
                DedicatedHours = 8,
                ProjectId = projectId,
                ResourceId = resource.Id
            };

            // Act
            var result = await _classUnderTest.SaveAsync(projectResourceToSave);

            // Assert
            Assert.IsType<ServiceResponse<ProjectResource>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorCodes.ProjectNotExists, result.Error.Code);
        }

        [Fact]
        public async Task SaveAsync_ReturnsExceptionNoResource()
        {
            // Arrange
            var resourceId = 0;
            var project = CreateProject();
            var projectResourceToSave = new ProjectResource()
            {
                BeginDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(5),
                DedicatedHours = 8,
                ProjectId = project.Id,
                ResourceId = resourceId
            };

            // Act
            var result = await _classUnderTest.SaveAsync(projectResourceToSave);

            // Assert
            Assert.IsType<ServiceResponse<ProjectResource>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorCodes.ResourceNotExists, result.Error.Code);
        }

        [Fact]
        public async Task SaveAsync_ReturnsExceptionInvalidBeginDate()
        {
            // Arrange
            var resource = CreateResource();
            var project = CreateProject();
            var projectResourceToSave = new ProjectResource()
            {
                BeginDate = project.StartDate?.AddDays(-1),
                DedicatedHours = 8,
                ProjectId = project.Id,
                ResourceId = resource.Id
            };

            // Act
            var result = await _classUnderTest.SaveAsync(projectResourceToSave);

            // Assert
            Assert.IsType<ServiceResponse<ProjectResource>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorCodes.InvalidBeginDateForProjectResource, result.Error.Code);
        }

        [Fact]
        public async Task SaveAsync_ReturnsExceptionInvalidEndDate()
        {
            // Arrange
            var resource = CreateResource();
            var project = CreateProject();
            var projectResourceToSave = new ProjectResource()
            {
                BeginDate = project.StartDate,
                EndDate = project.StartDate?.AddDays(-1),
                DedicatedHours = 8,
                ProjectId = project.Id,
                ResourceId = resource.Id
            };

            // Act
            var result = await _classUnderTest.SaveAsync(projectResourceToSave);

            // Assert
            Assert.IsType<ServiceResponse<ProjectResource>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorCodes.InvalidEndDateForProjectResource, result.Error.Code);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsProjectResource()
        {
            // Arrange
            var resource = CreateResource();
            var project = CreateProject();
            var projectResource = CreateProjectResource(project.Id, resource.Id);

            var updatedProjectResource = new ProjectResource()
            {
                Id = projectResource.Id,
                BeginDate = DateTime.Now.AddDays(5),
                EndDate = DateTime.Now.AddDays(10),
                DedicatedHours = 6,
                ProjectId = project.Id,
                ResourceId = resource.Id
            };

            // Act
            var result = await _classUnderTest.UpdateAsync(updatedProjectResource);

            // Assert
            Assert.IsType<ServiceResponse<ProjectResource>>(result);
            Assert.True(result.Success);
            var resultValue = result.Value;

            Assert.Equal(updatedProjectResource.Id, resultValue.Id);
            Assert.Equal(updatedProjectResource.BeginDate, resultValue.BeginDate);
            Assert.Equal(updatedProjectResource.EndDate, resultValue.EndDate);
            Assert.Equal(updatedProjectResource.DedicatedHours, resultValue.DedicatedHours);
            Assert.Equal(updatedProjectResource.ProjectId, project.Id);
            Assert.Equal(updatedProjectResource.ResourceId, resource.Id);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsExceptionNoProject()
        {
            // Arrange
            var resource = CreateResource();
            var projectId = 0;
            var projectResource = CreateProjectResource(projectId, resource.Id);

            var updatedProjectResource = new ProjectResource()
            {
                Id = projectResource.Id,
                BeginDate = DateTime.Now.AddDays(5),
                EndDate = DateTime.Now.AddDays(10),
                DedicatedHours = 6,
                ProjectId = projectId,
                ResourceId = resource.Id
            };

            // Act
            var result = await _classUnderTest.UpdateAsync(updatedProjectResource);

            // Assert
            Assert.IsType<ServiceResponse<ProjectResource>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorCodes.ProjectNotExists, result.Error.Code);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsExceptionNoResource()
        {
            // Arrange
            var resourceId = 0;
            var project = CreateProject();
            var projectResource = CreateProjectResource(project.Id, resourceId);

            var updatedProjectResource = new ProjectResource()
            {
                Id = projectResource.Id,
                BeginDate = DateTime.Now.AddDays(5),
                EndDate = DateTime.Now.AddDays(10),
                DedicatedHours = 6,
                ProjectId = project.Id,
                ResourceId = resourceId
            };

            // Act
            var result = await _classUnderTest.UpdateAsync(updatedProjectResource);

            // Assert
            Assert.IsType<ServiceResponse<ProjectResource>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorCodes.ResourceNotExists, result.Error.Code);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsExceptionInvalidBeginDate()
        {
            // Arrange
            var resource = CreateResource();
            var project = CreateProject();
            var projectResource = CreateProjectResource(project.Id, resource.Id);

            var updatedProjectResource = new ProjectResource()
            {
                Id = projectResource.Id,
                BeginDate = project.StartDate?.AddDays(-1),
                DedicatedHours = 6,
                ProjectId = project.Id,
                ResourceId = resource.Id
            };

            // Act
            var result = await _classUnderTest.UpdateAsync(updatedProjectResource);

            // Assert
            Assert.IsType<ServiceResponse<ProjectResource>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorCodes.InvalidBeginDateForProjectResource, result.Error.Code);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsExceptionEndBeginDate()
        {
            // Arrange
            var resource = CreateResource();
            var project = CreateProject();
            var projectResource = CreateProjectResource(project.Id, resource.Id);

            var updatedProjectResource = new ProjectResource()
            {
                Id = projectResource.Id,
                BeginDate = project.StartDate,
                EndDate = project.StartDate?.AddDays(-1),
                DedicatedHours = 6,
                ProjectId = project.Id,
                ResourceId = resource.Id
            };

            // Act
            var result = await _classUnderTest.UpdateAsync(updatedProjectResource);

            // Assert
            Assert.IsType<ServiceResponse<ProjectResource>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorCodes.InvalidEndDateForProjectResource, result.Error.Code);
        }

        [Fact]
        public async Task DeleteAsync_Returns()
        {
            // Arrange
            var resource = CreateResource();
            var project = CreateProject();
            var projectResource = CreateProjectResource(project.Id, resource.Id);

            // Act
            var result = await _classUnderTest.DeleteAsync(projectResource.Id);

            // Assert
            Assert.IsType<ServiceResponse<ProjectResource>>(result);
            Assert.True(result.Success);
            var resultValue = result.Value;

            Assert.Equal(projectResource.Id, resultValue.Id);
            Assert.Equal(projectResource.BeginDate, resultValue.BeginDate);
            Assert.Equal(projectResource.EndDate, resultValue.EndDate);
            Assert.Equal(projectResource.DedicatedHours, resultValue.DedicatedHours);
            Assert.Equal(projectResource.ProjectId, project.Id);
            Assert.Equal(projectResource.ResourceId, resource.Id);
        }
    }
}
