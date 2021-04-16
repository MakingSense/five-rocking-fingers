using System;
using System.Threading.Tasks;
using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Response;
using FRF.Core.Services;
using FRF.Core.XmlValidation;
using FRF.DataAccess.EntityModels;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using Resource = FRF.DataAccess.EntityModels.Resource;

namespace FRF.Core.Tests.Services
{
    public class ResourcesServiceTest
    {
        private readonly Mock<IConfiguration> _configuration;
        private readonly IMapper _mapper = MapperBuilder.Build();
        private readonly DataAccessContextForTest _dataAccess;
        private readonly ResourcesService _classUnderTest;
        private readonly Mock<ISettingsValidator> _settingsValidator;

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
            var resource = CreateResource();

            // Act
            var result = await _classUnderTest.GetAsync(resource.Id);

            // Assert
            Assert.IsType<ServiceResponse<Core.Models.Resource>>(result);
            Assert.True(result.Success);
            var resultValue = result.Value;

            Assert.Equal(resource.RoleName, resultValue.RoleName);
            Assert.Equal(resource.Description, resultValue.Description);
            Assert.Equal(resource.SalaryPerMonth, resultValue.SalaryPerMonth);
            Assert.Equal(resource.WorkloadCapacity, resultValue.WorkloadCapacity);
            Assert.Equal(resource.Id, resultValue.Id);
            
        }

        private Resource CreateResource()
        {
            var resource = new Resource()
            {
                Id = 1,
                RoleName = "Mock Role",
                Description = "Mock Description",
                SalaryPerMonth = 99999,
                WorkloadCapacity = 8
            };
            _dataAccess.Resources.Add(resource);
            _dataAccess.SaveChanges();

            return resource;
        }
    }
}