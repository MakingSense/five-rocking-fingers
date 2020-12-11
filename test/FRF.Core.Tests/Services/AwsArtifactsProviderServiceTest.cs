using FRF.Core.Base;
using FRF.Core.Services;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace FRF.Core.Tests.Services
{
    public class AwsArtifactsProviderServiceTest
    {
        private readonly IOptions<AwsApiStringBase> _awsApiString;
        private readonly AwsArtifactsProviderService _classUnderTest;

        public AwsArtifactsProviderServiceTest()
        {
            var awsApiStringBase = new AwsApiStringBase(){UrlApiString = "https://pricing.us-east-1.amazonaws.com/offers/v1.0/aws/index.json" };
            _awsApiString = Options.Create(awsApiStringBase);
            _classUnderTest = new AwsArtifactsProviderService(_awsApiString);
        }

        [Fact]
        public async Task GetNamesAsync_ReturnList()
        {
            // Arange
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            
            // Act
            var result = await _classUnderTest.GetNamesAsync();

            // Assert
            var okResult = Assert.IsType<List<KeyValuePair<string, string>>>(result);

            Assert.NotEmpty(okResult);
        }
    }
}