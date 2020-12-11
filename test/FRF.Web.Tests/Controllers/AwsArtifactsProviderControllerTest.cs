using FRF.Core.Services;
using FRF.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace FRF.Web.Tests.Controllers
{
    public class AwsArtifactsProviderControllerTest
    {
        private readonly AwsArtifactsProviderController _classUnderTest;
        private readonly Mock<IArtifactsProviderService> _artifactProviderService;

        public AwsArtifactsProviderControllerTest()
        {
            _artifactProviderService = new Mock<IArtifactsProviderService>();
            _classUnderTest= new AwsArtifactsProviderController(_artifactProviderService.Object);
        }

        private List<KeyValuePair<string, string>> CreateArtifactsNamesList()
        {
            var artifact1 = new KeyValuePair<string, string>("awsArtifact", "Aws Artifact");
            var artifact2 = new KeyValuePair<string, string>("awsArtifact2", "Aws Artifact2");
            var namesList = new List<KeyValuePair<string, string>>
            {
                artifact1,
                artifact2
            };

            return namesList;
        }

        [Fact]
        public async Task GetNamesAsync_ReturnsOkAndListOfArtifactsNames()
        {
            // Arrange
            var artifactList = CreateArtifactsNamesList();
            _artifactProviderService
                .Setup(mock => mock.GetNamesAsync())
                .ReturnsAsync(artifactList);

            // Act
            var result = await _classUnderTest.GetNamesAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<KeyValuePair<string,string>>>(okResult.Value);

            Assert.Contains(artifactList[0].Value, returnValue[0].Value);
            Assert.Contains(artifactList[0].Key, returnValue[0].Key);

            Assert.Contains(artifactList[1].Key, returnValue[1].Key);
            Assert.Contains(artifactList[1].Key, returnValue[1].Key);
        }
    }
}
