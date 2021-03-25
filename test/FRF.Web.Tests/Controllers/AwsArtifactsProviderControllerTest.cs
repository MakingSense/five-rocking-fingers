using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Response;
using FRF.Core.Services;
using FRF.Web.Controllers;
using FRF.Web.Dtos.Artifacts;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace FRF.Web.Tests.Controllers
{
    public class AwsArtifactsProviderControllerTest
    {
        private readonly IMapper _mapper = MapperBuilder.Build();
        private readonly AwsArtifactsProviderController _classUnderTest;
        private readonly Mock<IArtifactsProviderService> _artifactProviderService;

        public AwsArtifactsProviderControllerTest()
        {
            _artifactProviderService = new Mock<IArtifactsProviderService>();
            _classUnderTest= new AwsArtifactsProviderController(_artifactProviderService.Object, _mapper);
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

        private List<ProviderArtifactSetting> CreateArtifactSetting()
        {
            var artifactName = new KeyValuePair<string, string>("awsArtifact", "Aws Artifact");
            var artifactValues = new List<string>
            {
                "Valor 1"
            };
            var providerArtifactSetting = new ProviderArtifactSetting
            {
                Name = artifactName,
                Values = artifactValues
            };
            var providerArtifactSettings = new List<ProviderArtifactSetting>
            {
                providerArtifactSetting
            };
            return providerArtifactSettings;
        }

        private List<PricingTerm> CreatePricingTermList()
        {
            var pricingDimension1 = new PricingDimension
            {
                Unit = "[Mock] Unit",
                EndRange = 100000f,
                Description = "[Mock] Description",
                RateCode = "[Mock] Rate code",
                BeginRange = 0f,
                Currency = "[Mock] Currency",
                PricePerUnit = 99.99m
            };
            var pricingDimension2 = new PricingDimension
            {
                Unit = "[Mock] Unit 2",
                EndRange = 100000f,
                Description = "[Mock] Description 2",
                RateCode = "[Mock] Rate code 2",
                BeginRange = 0f,
                Currency = "[Mock] Currency 2",
                PricePerUnit = 99.99m
            };
            var pricingDimensions = new List<PricingDimension>
            {
                pricingDimension1,
                pricingDimension2
            };

            var pricingTerm = new PricingTerm()
            {
                Sku = "[Mock] Sku",
                Term = "[Mock] Term",
                PricingDimensions = pricingDimensions,
                LeaseContractLength = "[Mock] Contract length",
                OfferingClass = "[Mock] Offering class",
                PurchaseOption = "[Mock] Purchase option"
            };

            var pricingTermList = new List<PricingTerm>
            {
                pricingTerm
            };
            return pricingTermList;
        }

        [Fact]
        public async Task GetNamesAsync_ReturnsOkAndListOfArtifactsNames()
        {
            // Arrange
            var artifactList = CreateArtifactsNamesList();
            _artifactProviderService
                .Setup(mock => mock.GetNamesAsync())
                .ReturnsAsync(new ServiceResponse<List<KeyValuePair<string, string>>>(artifactList));

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

        [Fact]
        public async Task GetNamesAsync_ReturnsServiceUnavailable()
        {
            // Arrange
            _artifactProviderService
                .Setup(mock => mock.GetNamesAsync())
                .ReturnsAsync(new ServiceResponse<List<KeyValuePair<string, string>>>(
                    new Error(ErrorCodes.AmazonApiError, "Error message")));
            // Act
            var result = await _classUnderTest.GetNamesAsync();

            // Assert
            var response = Assert.IsType<StatusCodeResult>(result);

            Assert.Equal(503,response.StatusCode);
        }

        [Fact]
        public async Task GetAttributesAsync_ReturnsOk()
        {
            // Arrange
            var serviceCode = "[Mock] Service code";
            var artifactSettings = CreateArtifactSetting();

            _artifactProviderService
                .Setup(mock => mock.GetAttributesAsync(It.IsAny<string>()))
                .ReturnsAsync(new ServiceResponse<List<ProviderArtifactSetting>>(artifactSettings));

            // Act
            var result = await _classUnderTest.GetAttributesAsync(serviceCode);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<ProviderArtifactSettingDTO>>(okResult.Value);

            Assert.Equal(artifactSettings[0].Name.Key, returnValue[0].Name.Key);
            Assert.Equal(artifactSettings[0].Name.Value, returnValue[0].Name.Value);
            Assert.Equal(artifactSettings[0].Values[0], returnValue[0].Values[0]);
            _artifactProviderService.Verify(mock => mock.GetAttributesAsync(serviceCode), Times.Once);
        }

        [Fact]
        public async Task GetProductsAsync_ReturnsOk()
        {
            // Arrange
            var serviceCode = "[Mock] Service code";
            var artifactSettings = new KeyValuePair<string, string>("[Mock] Setting Name", "[Mock] Setting Value");
            var artifactSettingsList = new List<KeyValuePair<string, string>>
            {
                artifactSettings
            };
            var pricingTermList = CreatePricingTermList();

            _artifactProviderService
                .Setup(mock => mock.GetProductsAsync(It.IsAny<List<KeyValuePair<string, string>>>(), It.IsAny<string>()))
                .ReturnsAsync(new ServiceResponse<List<PricingTerm>>(pricingTermList));

            // Act
            var result = await _classUnderTest.GetProductsAsync(artifactSettingsList, serviceCode);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<PricingTermDTO>>(okResult.Value);

            Assert.Equal(pricingTermList[0].Sku, returnValue[0].Sku);
            Assert.Equal(pricingTermList[0].Term, returnValue[0].Term);
            Assert.Equal(pricingTermList[0].LeaseContractLength, returnValue[0].LeaseContractLength);
            Assert.Equal(pricingTermList[0].OfferingClass, returnValue[0].OfferingClass);
            Assert.Equal(pricingTermList[0].PurchaseOption, returnValue[0].PurchaseOption);
            Assert.Equal(pricingTermList[0].PricingDimensions[0].BeginRange, returnValue[0].PricingDimensions[0].BeginRange);
            Assert.Equal(pricingTermList[0].PricingDimensions[0].Currency, returnValue[0].PricingDimensions[0].Currency);
            Assert.Equal(pricingTermList[0].PricingDimensions[0].Description, returnValue[0].PricingDimensions[0].Description);
            Assert.Equal(pricingTermList[0].PricingDimensions[0].EndRange, returnValue[0].PricingDimensions[0].EndRange);
            Assert.Equal(pricingTermList[0].PricingDimensions[0].PricePerUnit, returnValue[0].PricingDimensions[0].PricePerUnit);
            Assert.Equal(pricingTermList[0].PricingDimensions[0].RateCode, returnValue[0].PricingDimensions[0].RateCode);
            Assert.Equal(pricingTermList[0].PricingDimensions[0].Unit, returnValue[0].PricingDimensions[0].Unit);
            Assert.Equal(pricingTermList[0].PricingDimensions[1].BeginRange, returnValue[0].PricingDimensions[1].BeginRange);
            Assert.Equal(pricingTermList[0].PricingDimensions[1].Currency, returnValue[0].PricingDimensions[1].Currency);
            Assert.Equal(pricingTermList[0].PricingDimensions[1].Description, returnValue[0].PricingDimensions[1].Description);
            Assert.Equal(pricingTermList[0].PricingDimensions[1].EndRange, returnValue[0].PricingDimensions[1].EndRange);
            Assert.Equal(pricingTermList[0].PricingDimensions[1].PricePerUnit, returnValue[0].PricingDimensions[1].PricePerUnit);
            Assert.Equal(pricingTermList[0].PricingDimensions[1].RateCode, returnValue[0].PricingDimensions[1].RateCode);
            Assert.Equal(pricingTermList[0].PricingDimensions[1].Unit, returnValue[0].PricingDimensions[1].Unit);
            _artifactProviderService.Verify(mock => mock.GetProductsAsync(artifactSettingsList, serviceCode), Times.Once);
        }

        [Fact]
        public async Task GetRequiredFieldsAsync_ReturnsOk()
        {
            // Arrange
            var serviceCode = "[Mock] Service code";
            var requiredFields = JObject.Parse("{\"jsonPropertie\":\"jsonValue\"}");

            _artifactProviderService
                .Setup(mock => mock.GetRequiredFieldsAsync(It.IsAny<string>()))
                .ReturnsAsync(new ServiceResponse<JObject>(requiredFields));

            // Act
            var result = await _classUnderTest.GetRequiredFieldsAsync(serviceCode);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<JObject>(okResult.Value);
        }
    }
}
