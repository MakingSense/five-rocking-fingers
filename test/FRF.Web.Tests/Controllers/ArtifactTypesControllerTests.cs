using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Response;
using FRF.Core.Services;
using FRF.Web.Controllers;
using FRF.Web.Dtos.Artifacts;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FRF.Web.Tests.Controllers
{
    public class ArtifactTypesControllerTests
    {
        private readonly IMapper _mapper = MapperBuilder.Build();
        private readonly Mock<IArtifactTypesService> _artifactTypesService;
        private readonly ArtifactTypesController _classUnderTest;

        public ArtifactTypesControllerTests()
        {
            _artifactTypesService = new Mock<IArtifactTypesService>();
            _classUnderTest = new ArtifactTypesController(_artifactTypesService.Object, _mapper);
        }

        private Provider CreateProvider(int id, string name)
        {
            return new Provider
            {
                Id = id,
                Name = name
            };
        }

        private ArtifactType CreateArtifactType(int id, string name, string description, Provider provider)
        {
            return new ArtifactType
            {
                Id = id,
                Name = name,
                Description = description,
                Provider = provider,
                ProviderId = provider.Id
            };
        }

        private void AssertCompareArtifactTypes(ArtifactType expected, ArtifactTypeDTO actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Description, actual.Description);
            
            Assert.Equal(expected.Provider.Id, actual.Provider.Id);
            Assert.Equal(expected.Provider.Name, actual.Provider.Name);
        }

        private void AssertCompareList(List<ArtifactType> expected, List<ArtifactTypeDTO> actual)
        {
            Assert.Equal(expected.Count, actual.Count);

            for (int i = 0; i < expected.Count; i++)
            {
                AssertCompareArtifactTypes(expected[i], actual[i]);
            }
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOk()
        {
            // Arrange
            var provider = CreateProvider(1, "[Mock] Provider Name 1");
            var artifactType1 = CreateArtifactType(1, "[Mock] Name AT1", "[Mock] Description AT1", provider);
            var artifactType2 = CreateArtifactType(1, "[Mock] Name AT2", "[Mock] Description AT2", provider);

            var artifactTypes = new List<ArtifactType> { artifactType1, artifactType2 };

            _artifactTypesService
                .Setup(mock => mock.GetAll())
                .ReturnsAsync(new ServiceResponse<List<ArtifactType>>(artifactTypes));

            // Act
            var result = await _classUnderTest.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultValue = Assert.IsAssignableFrom<IEnumerable<ArtifactTypeDTO>>(okResult.Value);
            var resultValueList = resultValue.ToList();

            AssertCompareList(artifactTypes, resultValueList);

            _artifactTypesService.Verify(mock => mock.GetAll(), Times.Once);
        }

        [Fact]
        public async Task GetAllByProviderAsync_ReturnsOk()
        {
            // Arrange
            var provider = CreateProvider(1, "[Mock] Provider Name 1");
            var artifactType1 = CreateArtifactType(1, "[Mock] Name AT1", "[Mock] Description AT1", provider);
            var artifactType2 = CreateArtifactType(1, "[Mock] Name AT2", "[Mock] Description AT2", provider);

            var artifactTypes = new List<ArtifactType> { artifactType1, artifactType2 };

            _artifactTypesService
                .Setup(mock => mock.GetAllByProvider(It.IsAny<string>()))
                .ReturnsAsync(new ServiceResponse<List<ArtifactType>>(artifactTypes));

            // Act
            var result = await _classUnderTest.GetAllByProviderAsync(provider.Name);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultValue = Assert.IsAssignableFrom<IEnumerable<ArtifactTypeDTO>>(okResult.Value);
            var resultValueList = resultValue.ToList();

            AssertCompareList(artifactTypes, resultValueList);

            _artifactTypesService.Verify(mock => mock.GetAllByProvider(It.IsAny<string>()), Times.Once);
        }
    }
}
