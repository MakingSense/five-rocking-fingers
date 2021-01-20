using Amazon;
using Amazon.Pricing;
using Amazon.Pricing.Model;
using FRF.Core.Base;
using FRF.Core.Models;
using FRF.Core.Response;
using FRF.Core.Services;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Collections.Generic;
using System.Globalization;
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
        private readonly Mock<AmazonPricingClient> _client;

        public AwsArtifactsProviderServiceTest()
        {
            var awsPricing = new AwsPricing()
                {ApiUrl = "https://localhost/offers/v1.0/aws/index.json"};
            _awsApi = Options.Create(awsPricing);
            _httpClientFactory = new Mock<IHttpClientFactory>();
            _client = new Mock<AmazonPricingClient>("[Mock] Key 1", "[Mock] Key 2", RegionEndpoint.USEast1);
            var httpClientFactory = _httpClientFactory.Object;
            _classUnderTest = new AwsArtifactsProviderService(_awsApi, httpClientFactory, _client.Object);
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
            Assert.IsType<ServiceResponse<List<KeyValuePair<string, string>>>>(result);
            Assert.True(result.Success);
            var response = result.Value;
            //var response = Assert.IsType<List<KeyValuePair<string, string>>>(result);

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

        [Fact]
        public async Task GetAttributesAsync_ReturnList()
        {
            // Arange
            var serviceCode = "[Mock] Service Code";
            var attributeNames = new List<string>
            {
                "[Mock] Service Name"
            };
            var service = new Service
            {
                AttributeNames = attributeNames,
                ServiceCode = serviceCode
            };
            var services = new List<Service>
            {
                service
            };
            var response = new DescribeServicesResponse
            {
                FormatVersion = "[Mock] Format Version",
                NextToken = "[Mock] Next token",
                Services = services
            };

            var attributeValue = new AttributeValue
            {
                Value = "[Mock] Attribute Value"
            };

            var attributeValues = new List<AttributeValue>
            {
                attributeValue
            };

            var response2 = new GetAttributeValuesResponse
            {
                NextToken = "[Mock] Next token",
                AttributeValues = attributeValues
            };

            _client
                .Setup(mock => mock.DescribeServicesAsync(It.IsAny<DescribeServicesRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            _client
                .Setup(mock => mock.GetAttributeValuesAsync(It.IsAny<GetAttributeValuesRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response2);

            // Act
            var result = await _classUnderTest.GetAttributesAsync(serviceCode);

            // Assert
            Assert.IsType<ServiceResponse<List<ProviderArtifactSetting>>>(result);
            Assert.True(result.Success);

            var settingsList = result.Value;
            Assert.NotEmpty(settingsList);

            Assert.Equal(settingsList[0].Name.Key, service.AttributeNames[0]);
            Assert.Equal(settingsList[0].Values[0], attributeValue.Value);
            _client.Verify(mock => mock.DescribeServicesAsync(It.IsAny<DescribeServicesRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            _client.Verify(mock => mock.GetAttributeValuesAsync(It.IsAny<GetAttributeValuesRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAttributesAsync_ReturnsEmptyList()
        {
            // Arange
            var serviceCode = "[Mock] Service Code";

            // Act
            var result = await _classUnderTest.GetAttributesAsync(serviceCode);

            // Assert
            Assert.IsType<ServiceResponse<List<ProviderArtifactSetting>>>(result);
            Assert.True(result.Success);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetProductsAsync_ReturnList()
        {
            // Arange
            var serviceCode = "[Mock] Service Code";
            var sku = "[Mock] Sku";
            var term = "[Mock] Term";
            var unit = "[Mock] Unit";
            var endRange = "inf";
            var decription = "[Mock] Description";
            var rateCode = "[Mock] Rate Code";
            var beginRange = "10";
            var currency = "[Mock] Currency";
            var pricePerUnit = "99.99";
            var offerTermCode = "[Mock] Offer term code";
            var leaseContractLength = "[Mock] Lease contract length";
            var offeringClass = "[Mock] Offering class";
            var purchaseOption = "[Mock] Purchase option";
            var attributeName = "[Mock] Attribute name";
            var attributeValue = "[Mock] Attribute value";
            var settings = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("[Mock] Setting name", "[Mock] Setting value")
            };

            var response = new GetProductsResponse
            {
                FormatVersion = "[Mock] Format Version",
                NextToken = "[Mock] Next token",
                PriceList = new List<string>
                {
                    "{\"product\":" +
                    "{ \"productFamily\":\"[Mock] product family\"," +
                    "\"attributes\":" +
                    "{ \"" + attributeName + "\":\"" + attributeValue + "\"}," +
                    "\"sku\":\"" + sku + "\"}," +
                    "\"serviceCode\":\"" + serviceCode + "\"," +
                    "\"terms\":" +
                    "{ \"" + term + "\":" +
                    "{ \"2227U2PX8M4F95R6.JRTCKXETXF\":" +
                    "{ \"priceDimensions\":" +
                    "{ \"" + rateCode + "\":" +
                    "{ \"unit\":\"" + unit +"\"," +
                    "\"endRange\":\"" + endRange + "\"," +
                    "\"description\":\"" + decription + "\"," +
                    "\"appliesTo\":[]," +
                    "\"rateCode\":\"" + rateCode +"\"," +
                    "\"beginRange\":\"" + beginRange + "\"," +
                    "\"pricePerUnit\":{ \"" + currency + "\":\"" + pricePerUnit + "\"} " +
                    "} " +
                    "}," +
                    "\"sku\":\"" + sku + "\"," +
                    "\"effectiveDate\":\"2020-12-01T00:00:00Z\"," +
                    "\"offerTermCode\":\"" + offerTermCode + "\"," +
                    "\"termAttributes\":" +
                    "{ " +
                    "\"LeaseContractLength\": \"" + leaseContractLength + "\"," +
                    "\"OfferingClass\": \"" + offeringClass + "\"," +
                    "\"PurchaseOption\": \"" + purchaseOption + "\"" +
                    "}" +
                    " }" +
                    " }" +
                    " }," +
                    "\"version\":\"20201222221737\"," +
                    "\"publicationDate\":\"2020-12-22T22:17:37Z\"" +
                    "}"
                }
            };

            _client
                .Setup(mock => mock.GetProductsAsync(It.IsAny<GetProductsRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _classUnderTest.GetProductsAsync(settings, serviceCode);

            // Assert
            float.TryParse(beginRange, out float beginRangeFloat);
            float.TryParse(endRange, out float endRangeFloat);

            Assert.IsType<ServiceResponse<List<PricingTerm>>>(result);
            var resultValue = result.Value;
            Assert.NotEmpty(resultValue);

            Assert.Equal(resultValue[0].Sku, sku);
            Assert.Equal(resultValue[0].Term, term);
            Assert.Equal(resultValue[0].PurchaseOption, purchaseOption);
            Assert.Equal(resultValue[0].OfferingClass, offeringClass);
            Assert.Equal(resultValue[0].LeaseContractLength, leaseContractLength);
            Assert.Equal(resultValue[0].PricingDimension.BeginRange, beginRangeFloat);
            Assert.Equal(resultValue[0].PricingDimension.Currency, currency);
            Assert.Equal(resultValue[0].PricingDimension.Description, decription);
            Assert.Equal(resultValue[0].PricingDimension.EndRange, endRangeFloat);
            Assert.Equal(resultValue[0].PricingDimension.PricePerUnit, float.Parse(pricePerUnit, CultureInfo.InvariantCulture));
            Assert.Equal(resultValue[0].PricingDimension.RateCode, rateCode);
            Assert.Equal(resultValue[0].PricingDimension.Unit, unit);
        }

        [Fact]
        public async Task GetProductsAsync_ReturnsEmptyList()
        {
            // Arange
            var serviceCode = "[Mock] Service Code";
            var settings = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("[Mock] Setting name", "[Mock] Setting value")
            };

            // Act
            var result = await _classUnderTest.GetProductsAsync(settings, serviceCode);

            // Assert
            Assert.IsType<ServiceResponse<List<PricingTerm>>>(result);
            Assert.True(result.Success);
            Assert.Empty(result.Value);
        }
    }
}