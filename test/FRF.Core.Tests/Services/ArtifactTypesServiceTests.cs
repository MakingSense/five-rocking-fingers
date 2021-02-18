using AutoMapper;
using FRF.Core.Response;
using FRF.Core.Services;
using FRF.DataAccess.EntityModels;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using CoreModels = FRF.Core.Models;

namespace FRF.Core.Tests.Services
{
    public class ArtifactTypesServiceTests
    {
        private readonly Mock<IConfiguration> _configuration;
        private readonly IMapper _mapper = MapperBuilder.Build();
        private readonly DataAccessContextForTest _dataAccess;
        private readonly ArtifactTypesService _classUnderTest;

        public ArtifactTypesServiceTests()
        {
            _configuration = new Mock<IConfiguration>();
            _dataAccess = new DataAccessContextForTest(Guid.NewGuid(), _configuration.Object);

            _dataAccess.Database.EnsureDeleted();
            _dataAccess.Database.EnsureCreated();

            _classUnderTest = new ArtifactTypesService(_dataAccess, _mapper);
        }

        private Provider CreateProvider(string name)
        {
            var provider = new Provider();
            provider.Name = name;
            _dataAccess.Providers.Add(provider);
            _dataAccess.SaveChanges();

            return provider;
        }

        private ArtifactType CreateArtifactType(Provider provider, string name, string description)
        {
            var artifactType = new ArtifactType();
            artifactType.Name = name;
            artifactType.Description = description;
            artifactType.Provider = provider;
            artifactType.ProviderId = provider.Id;
            _dataAccess.ArtifactType.Add(artifactType);
            _dataAccess.SaveChanges();

            return artifactType;
        }

        private void AssertCompareArtifactTypes(ArtifactType expected, CoreModels.ArtifactType actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Description, actual.Description);
            Assert.Equal(expected.ProviderId, actual.ProviderId);

            Assert.Equal(expected.Provider.Id, actual.Provider.Id);
            Assert.Equal(expected.Provider.Name, actual.Provider.Name);
        }

        private void AssertCompareList(List<ArtifactType> expected, List<CoreModels.ArtifactType> actual)
        {
            Assert.Equal(expected.Count, actual.Count);

            for(int i = 0; i < expected.Count; i++)
            {
                AssertCompareArtifactTypes(expected[i], actual[i]);
            }
        }

        [Fact]
        public async Task GetAllAsync_ReturnsList()
        {
            // Arrange
            var provider1 = CreateProvider("[Mock] Provider Name 1");
            var artifactType1 = CreateArtifactType(provider1, "[Mock] Name 1", "[Mock] Description 1");
            var artifactType2 = CreateArtifactType(provider1, "[Mock] Name 2", "[Mock] Description 2");

            var provider2 = CreateProvider("[Mock] Provider Name 2");
            var artifactType3 = CreateArtifactType(provider2, "[Mock] Name 3", "[Mock] Description 3");

            var expectedList = new List<ArtifactType> { artifactType1, artifactType2, artifactType3 };

            // Act
            var result = await _classUnderTest.GetAllAsync();

            // Assert
            Assert.IsType<ServiceResponse<List<CoreModels.ArtifactType>>>(result);
            Assert.True(result.Success);

            AssertCompareList(expectedList, result.Value);            
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList()
        {
            // Act
            var result = await _classUnderTest.GetAllAsync();

            // Assert
            Assert.IsType<ServiceResponse<List<CoreModels.ArtifactType>>>(result);
            Assert.True(result.Success);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetAllByProviderAsync_ReturnsList()
        {
            // Arrange
            var provider1 = CreateProvider("[Mock] Provider Name 1");
            var artifactType1 = CreateArtifactType(provider1, "[Mock] Name 1", "[Mock] Description 1");
            var artifactType2 = CreateArtifactType(provider1, "[Mock] Name 2", "[Mock] Description 2");

            var provider2 = CreateProvider("[Mock] Provider Name 2");
            var artifactType3 = CreateArtifactType(provider2, "[Mock] Name 3", "[Mock] Description 3");

            var expectedList = new List<ArtifactType> { artifactType1, artifactType2 };

            // Act
            var result = await _classUnderTest.GetAllByProviderAsync(provider1.Name);

            // Assert
            Assert.IsType<ServiceResponse<List<CoreModels.ArtifactType>>>(result);
            Assert.True(result.Success);

            AssertCompareList(expectedList, result.Value);
        }

        [Fact]
        public async Task GetAllByProviderAsync_ReturnsEmptyList()
        {
            // Arrange
            var provider = CreateProvider("[Mock] Provider Name 1");

            // Act
            var result = await _classUnderTest.GetAllByProviderAsync(provider.Name);

            // Assert
            Assert.IsType<ServiceResponse<List<CoreModels.ArtifactType>>>(result);
            Assert.True(result.Success);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetAllByProviderAsync_ErrorProviderNotExists()
        {
            // Arrange
            var providerName = "[Mock] Provider name";

            // Act
            var result = await _classUnderTest.GetAllByProviderAsync(providerName);

            // Assert
            Assert.IsType<ServiceResponse<List<CoreModels.ArtifactType>>>(result);
            Assert.False(result.Success);
            Assert.Equal(ErrorCodes.ProviderNotExists, result.Error.Code);
        }
    }
}
