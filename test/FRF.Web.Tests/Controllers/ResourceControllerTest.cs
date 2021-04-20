﻿using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Response;
using FRF.Core.Services;
using FRF.Web.Controllers;
using FRF.Web.Dtos.Resources;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace FRF.Web.Tests.Controllers
{
    public class ResourceControllerTest
    {
        private readonly ResourcesController _classUnderTest;
        private readonly Mock<IResourcesService> _resourcesServices;
        private readonly IMapper _mapper = MapperBuilder.Build();

        public ResourceControllerTest()
        {
            _resourcesServices = new Mock<IResourcesService>();
            _classUnderTest = new ResourcesController(_resourcesServices.Object,
                _mapper);
        }

        [Fact]
        public async Task GetAsync_ReturnsOkAndResource()
        {
            // Arrange
            const int ResourceId = 1;
            var resource = CreateResource(ResourceId);
            _resourcesServices
                .Setup(mock => mock.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<Resource>(resource));

            // Act
            var response = await _classUnderTest.GetAsync(ResourceId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var returnValue = Assert.IsType<ResourceDTO>(okResult.Value);

            Assert.Equal(ResourceId, returnValue.Id);
            Assert.Equal(resource.RoleName, returnValue.RoleName);
            Assert.Equal(resource.Description, returnValue.Description);
            Assert.Equal(resource.SalaryPerMonth, returnValue.SalaryPerMonth);
            Assert.Equal(resource.WorkloadCapacity, returnValue.WorkloadCapacity);
            _resourcesServices.Verify(mock => mock.GetAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task GetAsync_WhenResourceIsNotFound_ReturnsNotFound()
        {
            // Arrange
            _resourcesServices
                .Setup(mock => mock.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<Resource>(new Error(ErrorCodes.ResourceNotExist,
                    "Mock Not Found message.")));

            // Act
            var response = await _classUnderTest.GetAsync(It.IsAny<int>());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(response);
            var returnValue = Assert.IsType<Error>(notFoundResult.Value);

            Assert.Equal(ErrorCodes.ResourceNotExist, returnValue.Code);
            _resourcesServices.Verify(mock => mock.GetAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task SaveAsync_ReturnsOkAndSavedResource()
        {
            // Arrange
            const int ResourceId = 1;
            var resource = CreateResource(ResourceId);
            _resourcesServices
                .Setup(mock => mock.SaveAsync(It.IsAny<Resource>()))
                .ReturnsAsync(new ServiceResponse<Resource>(resource));

            // Act
            var response = await _classUnderTest.SaveAsync(It.IsAny<ResourceInsertDTO>());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var returnValue = Assert.IsType<ResourceDTO>(okResult.Value);

            Assert.Equal(ResourceId, returnValue.Id);
            Assert.Equal(resource.RoleName, returnValue.RoleName);
            Assert.Equal(resource.Description, returnValue.Description);
            Assert.Equal(resource.SalaryPerMonth, returnValue.SalaryPerMonth);
            Assert.Equal(resource.WorkloadCapacity, returnValue.WorkloadCapacity);
            _resourcesServices.Verify(mock => mock.SaveAsync(It.IsAny<Resource>()), Times.Once);
        }

        [Fact]
        public async Task SaveAsync_WhenIsRoleNameRepeated_ReturnsBadRequest()
        {
            // Arrange
            _resourcesServices
                .Setup(mock => mock.SaveAsync(It.IsAny<Resource>()))
                .ReturnsAsync(new ServiceResponse<Resource>(new Error(ErrorCodes.ResourceNameRepeated,
                    "Mock Name repeated message.")));

            // Act
            var response = await _classUnderTest.SaveAsync(It.IsAny<ResourceInsertDTO>());

            // Assert
            var notFoundResult = Assert.IsType<BadRequestObjectResult>(response);
            var returnValue = Assert.IsType<Error>(notFoundResult.Value);

            Assert.Equal(ErrorCodes.ResourceNameRepeated, returnValue.Code);
            _resourcesServices.Verify(mock => mock.SaveAsync(It.IsAny<Resource>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOkAndUpdatedResource()
        {
            // Arrange
            const int ResourceId = 1;
            var resource = CreateResource(ResourceId);
            var updatedResource = CreateResourceUpdateDto(resource);
            _resourcesServices
                .Setup(mock => mock.UpdateAsync(It.IsAny<Resource>()))
                .ReturnsAsync(new ServiceResponse<Resource>(resource));

            // Act
            var response = await _classUnderTest.UpdateAsync(updatedResource);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var returnValue = Assert.IsType<ResourceDTO>(okResult.Value);

            Assert.Equal(ResourceId, returnValue.Id);
            Assert.Equal(resource.RoleName, returnValue.RoleName);
            Assert.Equal(resource.Description, returnValue.Description);
            Assert.Equal(resource.SalaryPerMonth, returnValue.SalaryPerMonth);
            Assert.Equal(resource.WorkloadCapacity, returnValue.WorkloadCapacity);
            _resourcesServices.Verify(mock => mock.UpdateAsync(It.IsAny<Resource>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenResourceIsNotFound_ReturnsNotFound()
        {
            // Arrange
            _resourcesServices
                .Setup(mock => mock.UpdateAsync(It.IsAny<Resource>()))
                .ReturnsAsync(new ServiceResponse<Resource>(new Error(ErrorCodes.ResourceNotExist,
                    "Mock Not Found message.")));

            // Act
            var response = await _classUnderTest.UpdateAsync(It.IsAny<ResourceUpdateDTO>());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(response);
            var returnValue = Assert.IsType<Error>(notFoundResult.Value);

            Assert.Equal(ErrorCodes.ResourceNotExist, returnValue.Code);
            _resourcesServices.Verify(mock => mock.UpdateAsync(It.IsAny<Resource>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenIsRoleNameRepeated_ReturnsBadRequest()
        {
            // Arrange
            _resourcesServices
                .Setup(mock => mock.UpdateAsync(It.IsAny<Resource>()))
                .ReturnsAsync(new ServiceResponse<Resource>(new Error(ErrorCodes.ResourceNameRepeated,
                    "Mock Name repeated message.")));

            // Act
            var response = await _classUnderTest.UpdateAsync(It.IsAny<ResourceUpdateDTO>());

            // Assert
            var notFoundResult = Assert.IsType<BadRequestObjectResult>(response);
            var returnValue = Assert.IsType<Error>(notFoundResult.Value);

            Assert.Equal(ErrorCodes.ResourceNameRepeated, returnValue.Code);
            _resourcesServices.Verify(mock => mock.UpdateAsync(It.IsAny<Resource>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenResourceIsNotFound_ReturnsNotFound()
        {
            // Arrange
            _resourcesServices
                .Setup(mock => mock.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<Resource>(new Error(ErrorCodes.ResourceNotExist,
                    "Mock Not Found message.")));

            // Act
            var response = await _classUnderTest.DeleteAsync(It.IsAny<int>());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(response);
            var returnValue = Assert.IsType<Error>(notFoundResult.Value);

            Assert.Equal(ErrorCodes.ResourceNotExist, returnValue.Code);
            _resourcesServices.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNoContent()
        {
            // Arrange
            const int ResourceId = 1;
            var resource = CreateResource(ResourceId);
            _resourcesServices
                .Setup(mock => mock.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<Resource>(resource));

            // Act
            var response = await _classUnderTest.DeleteAsync(It.IsAny<int>());

            // Assert
            var notFoundResult = Assert.IsType<NoContentResult>(response);

            _resourcesServices.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Once);
        }

        private Resource CreateResource(int resourceId)
        {
            var resource = new Resource
            {
                Id = resourceId,
                RoleName = "Mock Role",
                Description = "Mock Description",
                SalaryPerMonth = 999,
                WorkloadCapacity = 8
            };
            return resource;
        }

        private ResourceUpdateDTO CreateResourceUpdateDto(Resource resource)
        {
            var updateResource = new ResourceUpdateDTO
            {
                Description = resource.Description,
                Id = resource.Id,
                RoleName = "Updated Role Name",
                SalaryPerMonth = resource.SalaryPerMonth,
                WorkloadCapacity = resource.WorkloadCapacity
            };
            return updateResource;
        }
    }
}