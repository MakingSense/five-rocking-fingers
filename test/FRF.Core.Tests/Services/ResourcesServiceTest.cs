using AutoMapper;
using FRF.Core.Response;
using FRF.Core.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using Resource = FRF.Core.Models.Resource;

namespace FRF.Core.Tests.Services
{
    public class ResourcesServiceTest
    {
        private readonly Mock<IConfiguration> _configuration;
        private readonly IMapper _mapper = MapperBuilder.Build();
        private readonly DataAccessContextForTest _dataAccess;
        private readonly ResourcesService _classUnderTest;

        public ResourcesServiceTest()
        {
            _configuration = new Mock<IConfiguration>();
            _dataAccess = new DataAccessContextForTest(Guid.NewGuid(), _configuration.Object);
            _dataAccess.Database.EnsureDeleted();
            _dataAccess.Database.EnsureCreated();
            _classUnderTest = new ResourcesService(_dataAccess, _mapper);
        }

        [Fact]
        public async Task GetAsync_ReturnsResource()
        {
            // Arrange
            var resource = CreateAndSaveResource();

            // Act
            var result = await _classUnderTest.GetAsync(resource.Id);

            // Assert
            Assert.IsType<ServiceResponse<Resource>>(result);
            Assert.True(result.Success);
            var resultValue = result.Value;

            Assert.Equal(resource.RoleName, resultValue.RoleName);
            Assert.Equal(resource.Description, resultValue.Description);
            Assert.Equal(resource.SalaryPerMonth, resultValue.SalaryPerMonth);
            Assert.Equal(resource.WorkloadCapacity, resultValue.WorkloadCapacity);
            Assert.Equal(resource.Id, resultValue.Id);
        }

        [Fact]
        public async Task GetAsync_WhenResourceNotExist_ReturnsNotExistError()
        {
            // Act
            var result = await _classUnderTest.GetAsync(It.IsAny<int>());

            // Assert
            Assert.IsType<ServiceResponse<Resource>>(result);
            Assert.False(result.Success);
            Assert.Equal(ErrorCodes.ResourceNotExist,result.Error.Code);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsUpdatedResource()
        {
            // Arrange
            var baseResource = CreateAndSaveResource();
            var updatedResource = CreateUpdateResource(baseResource);

            // Act
            var result = await _classUnderTest.UpdateAsync(updatedResource);

            // Assert
            Assert.IsType<ServiceResponse<Resource>>(result);
            Assert.True(result.Success);
            var resultValue = result.Value;

            Assert.Equal(baseResource.RoleName, resultValue.RoleName);
            Assert.Equal(baseResource.Description, resultValue.Description);
            Assert.Equal(updatedResource.SalaryPerMonth, resultValue.SalaryPerMonth);
            Assert.Equal(baseResource.WorkloadCapacity, resultValue.WorkloadCapacity);
            Assert.Equal(baseResource.Id, resultValue.Id);
        }

        [Fact]
        public async Task UpdateAsync_WhenResourceNotExist_ReturnsNotExistError()
        {
            // Arrange
            var resource = new Resource
            {
                Description = "Mock description",
                Id = 1,
                RoleName = "Mock role name",
                SalaryPerMonth = 999,
                WorkloadCapacity = 8
            };
            // Act
            var result = await _classUnderTest.UpdateAsync(resource);

            // Assert
            Assert.IsType<ServiceResponse<Resource>>(result);
            Assert.False(result.Success);
            Assert.Equal(ErrorCodes.ResourceNotExist,result.Error.Code);
        }

        [Fact]
        public async Task UpdateAsync_WhenResourceRoleNameIsRepeated_ReturnsRepeatedResourceError()
        {
            // Arrange
            var baseResource = CreateAndSaveResource();
            var anotherResource = CreateAndSaveResource();
            var updatedResource = new Resource
            {
                Description = "Mock description",
                Id = baseResource.Id,
                RoleName = anotherResource.RoleName,
                SalaryPerMonth = 100,
                WorkloadCapacity = 7
            };

            // Act
            var result = await _classUnderTest.UpdateAsync(updatedResource);

            // Assert
            Assert.IsType<ServiceResponse<Resource>>(result);
            Assert.False(result.Success);
            Assert.Equal(ErrorCodes.ResourceNameRepeated,result.Error.Code);
            Assert.Equal(anotherResource.RoleName, updatedResource.RoleName);
        }

        [Fact]
        public async Task DeleteAsync_WhenResourceNotExist_ReturnsNotExistError()
        {
            // Act
            var result = await _classUnderTest.DeleteAsync(It.IsAny<int>());

            // Assert
            Assert.IsType<ServiceResponse<Resource>>(result);
            Assert.False(result.Success);
            Assert.Equal(ErrorCodes.ResourceNotExist,result.Error.Code);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsDeletedResource()
        {
            // Arrange
            var baseResource = CreateAndSaveResource();

            // Act
            var result = await _classUnderTest.DeleteAsync(baseResource.Id);

            // Assert
            Assert.IsType<ServiceResponse<Resource>>(result);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task SaveAsync_ReturnsSavedResource()
        {
            // Arrange
            var resource = new Resource
            {
                Description = "Mock Description",
                RoleName = "Mock role name",
                SalaryPerMonth = 1000000,
                WorkloadCapacity = 8
            };

            // Act
            var result = await _classUnderTest.SaveAsync(resource);

            // Assert
            Assert.IsType<ServiceResponse<Resource>>(result);
            Assert.True(result.Success);
            var resultValue = result.Value;

            Assert.Equal(resource.RoleName, resultValue.RoleName);
            Assert.Equal(resource.Description, resultValue.Description);
            Assert.Equal(resource.SalaryPerMonth, resultValue.SalaryPerMonth);
            Assert.Equal(resource.WorkloadCapacity, resultValue.WorkloadCapacity);
        }

        [Fact]
        public async Task SaveAsync_WhenResourceRoleNameIsRepeated_ReturnsRepeatedResourceError()
        {
            // Arrange
            var resourceInDb = CreateAndSaveResource();
            var toSaveResource = new Resource
            {
                Description = "Mock Description",
                RoleName = resourceInDb.RoleName,
                SalaryPerMonth = 1000000,
                WorkloadCapacity = 8
            };

            // Act
            var result = await _classUnderTest.SaveAsync(toSaveResource);

            // Assert
            Assert.IsType<ServiceResponse< Resource>>(result);
            Assert.False(result.Success);
            Assert.Equal(ErrorCodes.ResourceNameRepeated,result.Error.Code);
            Assert.Equal(resourceInDb.RoleName, toSaveResource.RoleName);
        }

        private Resource CreateAndSaveResource()
        {
            var rnd = new Random();
            var id = rnd.Next();
            var resource = new DataAccess.EntityModels.Resource()
            {
                Id = id,
                RoleName = $"Mock Role name {id}",
                Description = "Mock Description",
                SalaryPerMonth = 99999,
                WorkloadCapacity = 8
            };
            _dataAccess.Resources.Add(resource);
            _dataAccess.SaveChanges();
            
            return _mapper.Map<Resource>(resource);;
        }

        private Resource CreateUpdateResource(Resource resource)
        {
            return new Resource()
            {
                Id = resource.Id,
                RoleName = resource.RoleName,
                Description = resource.Description,
                SalaryPerMonth = 1000000,
                WorkloadCapacity = resource.WorkloadCapacity
            };
        }
    }
}