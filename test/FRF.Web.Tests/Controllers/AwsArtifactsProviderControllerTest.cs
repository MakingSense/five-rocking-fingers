using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Services;
using FRF.Web.Controllers;
using FRF.Web.Dtos.Artifacts;
using Microsoft.AspNetCore.Mvc;
using Moq;
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
            var pricingDimension = new PricingDimension
            {
                Unit = "[Mock] Unit",
                EndRange = 100000f,
                Description = "[Mock] Description",
                RateCode = "[Mock] Rate code",
                BeginRange = 0f,
                Currency = "[Mock] Currency",
                PricePerUnit = 99.99f
            };
            var pricingDetail = new PricingDimension
            {
                Unit = "[Mock] Unit 2",
                EndRange = 100000f,
                Description = "[Mock] Description 2",
                RateCode = "[Mock] Rate code 2",
                BeginRange = 0f,
                Currency = "[Mock] Currency 2",
                PricePerUnit = 99.99f
            };
            var pricingTerm = new PricingTerm()
            {
                Sku = "[Mock] Sku",
                Term = "[Mock] Term",
                PricingDimension = pricingDimension,
                PricingDetail = pricingDetail,
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

        [Fact]
        public async Task GetNamesAsync_ReturnsServiceUnavailable()
        {
            // Arrange
            List<KeyValuePair<string, string>> artifactList = null;
            _artifactProviderService
                .Setup(mock => mock.GetNamesAsync())
                .ReturnsAsync(artifactList);
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
                .ReturnsAsync(artifactSettings);

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
                .ReturnsAsync(pricingTermList);

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
            Assert.Equal(pricingTermList[0].PricingDimension.BeginRange, returnValue[0].PricingDimension.BeginRange);
            Assert.Equal(pricingTermList[0].PricingDimension.Currency, returnValue[0].PricingDimension.Currency);
            Assert.Equal(pricingTermList[0].PricingDimension.Description, returnValue[0].PricingDimension.Description);
            Assert.Equal(pricingTermList[0].PricingDimension.EndRange, returnValue[0].PricingDimension.EndRange);
            Assert.Equal(pricingTermList[0].PricingDimension.PricePerUnit, returnValue[0].PricingDimension.PricePerUnit);
            Assert.Equal(pricingTermList[0].PricingDimension.RateCode, returnValue[0].PricingDimension.RateCode);
            Assert.Equal(pricingTermList[0].PricingDimension.Unit, returnValue[0].PricingDimension.Unit);
            Assert.Equal(pricingTermList[0].PricingDetail.BeginRange, returnValue[0].PricingDetail.BeginRange);
            Assert.Equal(pricingTermList[0].PricingDetail.Currency, returnValue[0].PricingDetail.Currency);
            Assert.Equal(pricingTermList[0].PricingDetail.Description, returnValue[0].PricingDetail.Description);
            Assert.Equal(pricingTermList[0].PricingDetail.EndRange, returnValue[0].PricingDetail.EndRange);
            Assert.Equal(pricingTermList[0].PricingDetail.PricePerUnit, returnValue[0].PricingDetail.PricePerUnit);
            Assert.Equal(pricingTermList[0].PricingDetail.RateCode, returnValue[0].PricingDetail.RateCode);
            Assert.Equal(pricingTermList[0].PricingDetail.Unit, returnValue[0].PricingDetail.Unit);
            _artifactProviderService.Verify(mock => mock.GetProductsAsync(artifactSettingsList, serviceCode), Times.Once);
        }
    }
}
