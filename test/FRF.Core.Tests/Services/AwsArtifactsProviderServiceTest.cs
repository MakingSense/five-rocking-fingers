using FRF.Core.Base;
using FRF.Core.Services;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FRF.Core.Tests.Services
{
    public class AwsArtifactsProviderServiceTest
    {
        private readonly IOptions<AwsPricing> _awsApi;
        private readonly AwsArtifactsProviderService _classUnderTest;
        private readonly Mock<IHttpClientFactory> _httpClientFactory;

        public AwsArtifactsProviderServiceTest()
        {
            var awsPricing = new AwsPricing()
                {ApiUrl = "https://localhost/offers/v1.0/aws/index.json"};
            _awsApi = Options.Create(awsPricing);
            _httpClientFactory = new Mock<IHttpClientFactory>();
            var httpClientFactory = _httpClientFactory.Object;
            _classUnderTest = new AwsArtifactsProviderService(_awsApi, httpClientFactory);
        }

        [Fact]
        public async Task GetNamesAsync_ReturnList()
        {
            // Arange
            var httpMessage = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    "{\n\"offers\" : {" +
                    "\n\"AwsArtifact1\" : {\n\"offerCode\" : \"AwsArtifact1\"}," +
                    "\n\"AwsArtifact2\" : {\n\"offerCode\" : \"AwsArtifact2\"}," +
                    "\n\"AwsArtifact3\" : {\n\"offerCode\" : \"AwsArtifact3\"}," +
                    "\n\"AwsArtifact4\" : {\n\"offerCode\" : \"AwsArtifact4\"\n}\n}\n}")
            };

            mockResponse.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            httpMessage
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(mockResponse);
            var mockHttpClient = new HttpClient(httpMessage.Object);

            _httpClientFactory.Setup(mock => mock.CreateClient(string.Empty)).Returns(mockHttpClient);

            // Act
            var result = await _classUnderTest.GetNamesAsync();

            // Assert
            var response = Assert.IsType<List<KeyValuePair<string, string>>>(result);

            Assert.NotEmpty(response);
            _httpClientFactory.Verify(mock => mock.CreateClient(string.Empty), Times.Once);

            Assert.Equal("Aws Artifact 1", response[0].Value);
            Assert.Equal("Aws Artifact 2", response[1].Value);
            Assert.Equal("Aws Artifact 3", response[2].Value);
            Assert.Equal("Aws Artifact 4", response[3].Value);

            Assert.Equal("AwsArtifact1", response[0].Key);
            Assert.Equal("AwsArtifact2", response[1].Key);
            Assert.Equal("AwsArtifact3", response[2].Key);
            Assert.Equal("AwsArtifact4", response[3].Key);
        }
    }
}