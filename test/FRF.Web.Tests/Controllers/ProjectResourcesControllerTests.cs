using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Response;
using FRF.Core.Services;
using FRF.Web.Controllers;
using FRF.Web.Dtos.ProjectResources;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FRF.Web.Tests.Controllers
{
    public class ProjectResourcesControllerTests
    {
        private readonly IMapper _mapper = MapperBuilder.Build();
        private readonly Mock<IProjectResourcesService> _projectResourcesService;
        private readonly ProjectResourcesController _classUnderTest;

        public ProjectResourcesControllerTests()
        {
            _projectResourcesService = new Mock<IProjectResourcesService>();

            _classUnderTest = new ProjectResourcesController(_projectResourcesService.Object, _mapper);
        }

        [Fact]
        public async Task GetByProjectIdAsync_ReturnsOk()
        {
            // Arrange
            var projectResourceId = 1;
            var beginDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(5);
            var dedicatedHours = 8;
            var projectId = 1;
            var resourceId = 1;

            var projectResources = new List<ProjectResource>()
            {
                new ProjectResource()
                {
                    Id = projectResourceId,
                    BeginDate = beginDate,
                    EndDate = endDate,
                    DedicatedHours = dedicatedHours,
                    ProjectId = projectId,
                    ResourceId = resourceId
                }
            };

            _projectResourcesService
                .Setup(mock => mock.GetByProjectIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<List<ProjectResource>>(projectResources));

            // Act
            var result = await _classUnderTest.GetByProjectIdAsync(projectId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultValue = Assert.IsType<List<ProjectResourceDTO>>(okResult.Value);

            Assert.Equal(projectResources.Count, resultValue.Count);
            for (int i = 0; i < projectResources.Count; i++)
            {
                Assert.Equal(projectResources[i].Id, resultValue[i].Id);
                Assert.Equal(projectResources[i].BeginDate, resultValue[i].BeginDate);
                Assert.Equal(projectResources[i].EndDate, resultValue[i].EndDate);
                Assert.Equal(projectResources[i].DedicatedHours, resultValue[i].DedicatedHours);
                Assert.Equal(projectResources[i].ProjectId, resultValue[i].ProjectId);
                Assert.Equal(projectResources[i].ResourceId, resultValue[i].ResourceId);
            }
        }

        [Fact]
        public async Task GetByProjectIdAsync_ReturnsNotFound()
        {
            // Arrange
            var projectId = 0;
            var error = new Error(ErrorCodes.ProjectNotExists, "[Mock] Message");

            _projectResourcesService
               .Setup(mock => mock.GetByProjectIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<List<ProjectResource>>(error));

            // Act
            var result = await _classUnderTest.GetByProjectIdAsync(projectId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var resultValue = Assert.IsType<Error>(notFoundResult.Value);
            Assert.Equal(ErrorCodes.ProjectNotExists, resultValue.Code);
            _projectResourcesService.Verify(mock => mock.GetByProjectIdAsync(projectId), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ReturnsOk()
        {
            // Arrange
            var projectResourceId = 1;
            var beginDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(5);
            var dedicatedHours = 8;
            var projectId = 1;
            var resourceId = 1;

            var projectResource = new ProjectResource
            {
                Id = projectResourceId,
                BeginDate = beginDate,
                EndDate = endDate,
                DedicatedHours = dedicatedHours,
                ProjectId = projectId,
                ResourceId = resourceId
            };

            _projectResourcesService
               .Setup(mock => mock.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<ProjectResource>(projectResource));

            // Act
            var result = await _classUnderTest.GetAsync(projectResourceId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultValue = Assert.IsType<ProjectResourceDTO>(okResult.Value);

            Assert.Equal(projectResource.Id, resultValue.Id);
            Assert.Equal(projectResource.BeginDate, resultValue.BeginDate);
            Assert.Equal(projectResource.EndDate, resultValue.EndDate);
            Assert.Equal(projectResource.DedicatedHours, resultValue.DedicatedHours);
            Assert.Equal(projectResource.ProjectId, resultValue.ProjectId);
            Assert.Equal(projectResource.ResourceId, resultValue.ResourceId);

            _projectResourcesService.Verify(mock => mock.GetAsync(projectResourceId), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ReturnsNotFound()
        {
            // Arrange
            var projectResourceId = 1;

            _projectResourcesService
                .Setup(mock => mock.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<ProjectResource>(new Error(ErrorCodes.ProjectResourceNotExists, "[Mock] Message")));

            // Act
            var result = await _classUnderTest.GetAsync(projectResourceId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var resultValue = Assert.IsType<Error>(notFoundResult.Value);
            Assert.Equal(ErrorCodes.ProjectResourceNotExists, resultValue.Code);
            _projectResourcesService.Verify(mock => mock.GetAsync(projectResourceId), Times.Once);
        }

        [Fact]
        public async Task SaveAsync_ReturnsOk()
        {
            // Arrange
            var projectResourceId = 1;
            var beginDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(5);
            var dedicatedHours = 8;
            var projectId = 1;
            var resourceId = 1;

            var projectResourceToSave = new ProjectResourceUpsertDTO()
            {
                BeginDate = beginDate,
                EndDate = endDate,
                DedicatedHours = dedicatedHours,
                ProjectId = projectId,
                ResourceId = resourceId
            };

            _projectResourcesService
               .Setup(mock => mock.SaveAsync(It.IsAny<ProjectResource>()))
                .ReturnsAsync(new ServiceResponse<ProjectResource>(new ProjectResource()
                {
                    Id = projectResourceId,
                    BeginDate = beginDate,
                    EndDate = endDate,
                    DedicatedHours = dedicatedHours,
                    ProjectId = projectId,
                    ResourceId = resourceId
                }));

            // Act
            var result = await _classUnderTest.SaveAsync(projectResourceToSave);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultValue = Assert.IsType<ProjectResourceDTO>(okResult.Value);

            Assert.Equal(projectResourceId, resultValue.Id);
            Assert.Equal(projectResourceToSave.BeginDate, resultValue.BeginDate);
            Assert.Equal(projectResourceToSave.EndDate, resultValue.EndDate);
            Assert.Equal(projectResourceToSave.DedicatedHours, resultValue.DedicatedHours);
            Assert.Equal(projectResourceToSave.ProjectId, resultValue.ProjectId);
            Assert.Equal(projectResourceToSave.ResourceId, resultValue.ResourceId);

            _projectResourcesService.Verify(mock => mock.SaveAsync(It.IsAny<ProjectResource>()), Times.Once);
        }

        [Fact]
        public async Task SaveAsync_ReturnsNotFound_ProjectNotExists()
        {
            // Arrange
            var beginDate = DateTime.Now.AddDays(10);
            var endDate = DateTime.Now.AddDays(5);
            var dedicatedHours = 8;
            var projectId = 1;
            var resourceId = 1;

            var projectResourceToSave = new ProjectResourceUpsertDTO()
            {
                BeginDate = beginDate,
                EndDate = endDate,
                DedicatedHours = dedicatedHours,
                ProjectId = projectId,
                ResourceId = resourceId
            };

            _projectResourcesService
                .Setup(mock => mock.SaveAsync(It.IsAny<ProjectResource>()))
                .ReturnsAsync(new ServiceResponse<ProjectResource>(new Error(ErrorCodes.ProjectNotExists, "[Mock] Message")));

            // Act
            var result = await _classUnderTest.SaveAsync(projectResourceToSave);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var resultValue = Assert.IsType<Error>(notFoundResult.Value);
            Assert.Equal(ErrorCodes.ProjectNotExists, resultValue.Code);
            _projectResourcesService.Verify(mock => mock.SaveAsync(It.IsAny<ProjectResource>()), Times.Once);
        }

        [Fact]
        public async Task SaveAsync_ReturnsBadRequest_InvalidBeginDateForProjectResource()
        {
            // Arrange
            var beginDate = DateTime.Now.AddDays(10);
            var endDate = DateTime.Now.AddDays(5);
            var dedicatedHours = 8;
            var projectId = 1;
            var resourceId = 1;

            var projectResourceToSave = new ProjectResourceUpsertDTO()
            {
                BeginDate = beginDate,
                EndDate = endDate,
                DedicatedHours = dedicatedHours,
                ProjectId = projectId,
                ResourceId = resourceId
            };

            _projectResourcesService
                .Setup(mock => mock.SaveAsync(It.IsAny<ProjectResource>()))
                .ReturnsAsync(new ServiceResponse<ProjectResource>(new Error(ErrorCodes.InvalidBeginDateForProjectResource, "[Mock] Message")));

            // Act
            var result = await _classUnderTest.SaveAsync(projectResourceToSave);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var resultValue = Assert.IsType<Error>(badRequestResult.Value);
            Assert.Equal(ErrorCodes.InvalidBeginDateForProjectResource, resultValue.Code);
            _projectResourcesService.Verify(mock => mock.SaveAsync(It.IsAny<ProjectResource>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOk()
        {
            // Arrange
            var projectResourceId = 1;
            var projectId = 1;

            var beginDateNew = DateTime.Now.AddDays(5);
            var endDateNew = DateTime.Now.AddDays(10);
            var dedicatedHoursNew = 8;            
            var resourceIdNew = 1;

            var beginDateOld = DateTime.Now;
            var endDateOld = DateTime.Now.AddDays(5);
            var dedicatedHoursOld = 6;
            var resourceIdOld = 2;

            var updatedProjectResource = new ProjectResourceUpsertDTO()
            {
                BeginDate = beginDateNew,
                EndDate = endDateNew,
                DedicatedHours = dedicatedHoursNew,
                ProjectId = projectId,
                ResourceId = resourceIdNew
            };

            var oldProjectResource = new ProjectResource()
            {
                Id = projectResourceId,
                BeginDate = beginDateOld,
                EndDate = endDateOld,
                DedicatedHours = dedicatedHoursOld,
                ProjectId = projectId,
                ResourceId = resourceIdOld
            };

            _projectResourcesService
                .Setup(mock => mock.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<ProjectResource>(oldProjectResource));

            _projectResourcesService
                .Setup(mock => mock.UpdateAsync(It.IsAny<ProjectResource>()))
                .ReturnsAsync(new ServiceResponse<ProjectResource>(new ProjectResource()
                {
                    Id = projectResourceId,
                    BeginDate = updatedProjectResource.BeginDate,
                    EndDate = updatedProjectResource.EndDate,
                    DedicatedHours = updatedProjectResource.DedicatedHours,
                    ProjectId = updatedProjectResource.ProjectId,
                    ResourceId = updatedProjectResource.ResourceId
                }));

            // Act
            var result = await _classUnderTest.UpdateAsync(oldProjectResource.Id, updatedProjectResource);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultValue = Assert.IsType<ProjectResourceDTO>(okResult.Value);

            Assert.Equal(updatedProjectResource.BeginDate, resultValue.BeginDate);
            Assert.Equal(updatedProjectResource.EndDate, resultValue.EndDate);
            Assert.Equal(updatedProjectResource.DedicatedHours, resultValue.DedicatedHours);
            Assert.Equal(updatedProjectResource.ProjectId, resultValue.ProjectId);
            Assert.Equal(updatedProjectResource.ResourceId, resultValue.ResourceId);

            _projectResourcesService.Verify(mock => mock.UpdateAsync(It.IsAny<ProjectResource>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNotFound_ProjectNotExists()
        {
            // Arrange
            var projectResourceId = 0;
            var beginDate = DateTime.Now.AddDays(5);
            var endDate = DateTime.Now.AddDays(10);
            var dedicatedHours = 8;
            var projectId = 1;
            var resourceId = 1;

            var updatedProjectResource = new ProjectResourceUpsertDTO()
            {
                BeginDate = beginDate,
                EndDate = endDate,
                DedicatedHours = dedicatedHours,
                ProjectId = projectId,
                ResourceId = resourceId
            };

            _projectResourcesService
                .Setup(mock => mock.UpdateAsync(It.IsAny<ProjectResource>()))
                .ReturnsAsync(new ServiceResponse<ProjectResource>(new Error(ErrorCodes.ProjectNotExists, "[Mock] Message")));

            // Act
            var result = await _classUnderTest.UpdateAsync(projectResourceId, updatedProjectResource);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var resultValue = Assert.IsType<Error>(notFoundResult.Value);
            Assert.Equal(ErrorCodes.ProjectNotExists, resultValue.Code);
            _projectResourcesService.Verify(mock => mock.UpdateAsync(It.IsAny<ProjectResource>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadRequest_InvalidBeginDateForProjectResource()
        {
            // Arrange
            var projectResourceId = 1;
            var beginDate = DateTime.Now.AddDays(10);
            var endDate = DateTime.Now.AddDays(5);
            var dedicatedHours = 8;
            var projectId = 1;
            var resourceId = 1;

            var updatedProjectResource = new ProjectResourceUpsertDTO()
            {
                BeginDate = beginDate,
                EndDate = endDate,
                DedicatedHours = dedicatedHours,
                ProjectId = projectId,
                ResourceId = resourceId
            };

            _projectResourcesService
                .Setup(mock => mock.UpdateAsync(It.IsAny<ProjectResource>()))
                .ReturnsAsync(new ServiceResponse<ProjectResource>(new Error(ErrorCodes.InvalidBeginDateForProjectResource, "[Mock] Message")));

            // Act
            var result = await _classUnderTest.UpdateAsync(projectResourceId, updatedProjectResource);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var resultValue = Assert.IsType<Error>(badRequestResult.Value);
            Assert.Equal(ErrorCodes.InvalidBeginDateForProjectResource, resultValue.Code);
            _projectResourcesService.Verify(mock => mock.UpdateAsync(It.IsAny<ProjectResource>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNoContent()
        {
            // Arrange
            var idToDelete = 1;

            _projectResourcesService
                .Setup(mock => mock.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<ProjectResource>(new ProjectResource()));

            // Act
            var result = await _classUnderTest.DeleteAsync(idToDelete);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _projectResourcesService.Verify(mock => mock.GetAsync(It.IsAny<int>()), Times.Once);
            _projectResourcesService.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound()
        {
            // Arrange
            _projectResourcesService
                .Setup(mock => mock.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<ProjectResource>(new Error(ErrorCodes.CategoryNotExists, "Error message")));

            // Act
            var result = await _classUnderTest.DeleteAsync(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            _projectResourcesService.Verify(mock => mock.GetAsync(It.IsAny<int>()), Times.Once);
            _projectResourcesService.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}
