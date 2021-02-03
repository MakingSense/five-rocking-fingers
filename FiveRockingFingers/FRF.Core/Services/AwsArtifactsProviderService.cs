using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FRF.Core.Base;
using Microsoft.Extensions.Options;
using Amazon.Pricing;
using Amazon.Pricing.Model;
using FRF.Core.Models;
using FRF.Core.Response;
using System;
using Newtonsoft.Json;

namespace FRF.Core.Services
{
    public class AwsArtifactsProviderService : IArtifactsProviderService
    {
        private const string OffersCodeIndex = "offers.*.offerCode";
        private readonly AwsPricing _awsPricingOptions;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAmazonPricing _pricingClient;

        public AwsArtifactsProviderService(IOptions<AwsPricing> awsApiString,
            IHttpClientFactory httpClientFactory, IAmazonPricing pricingClient)
        {
            _httpClientFactory = httpClientFactory;
            _awsPricingOptions = awsApiString.Value;
            _pricingClient = pricingClient;
        }

        /// <summary>
        /// Get all the artifacts names with the service code from aws pricing api.
        /// </summary>
        /// <returns>List of KeyValuePair</returns>
        public async Task<ServiceResponse<List<KeyValuePair<string, string>>>> GetNamesAsync()
        {
            var artifactsNames = new Dictionary<string, string>();
            var httpClient = _httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync(_awsPricingOptions.ApiUrl);
            if (!response.IsSuccessStatusCode)
            {
                return new ServiceResponse<List<KeyValuePair<string, string>>>(
                    new Error(ErrorCodes.AmazonApiError, "There was an error connecting to the Amazon API"));
            }

            var pricingList = await response.Content.ReadAsStringAsync();
            var awsArtifactsNames = JObject
                .Parse(pricingList)
                .SelectTokens(OffersCodeIndex);

            foreach (var artifactName in awsArtifactsNames)
            {
                var serviceCode = artifactName.ToString();
                var name = ExtractName(artifactName.ToString());
                artifactsNames.Add(serviceCode, name);
            }

            return new ServiceResponse<List<KeyValuePair<string, string>>>(artifactsNames.ToList());
        }

        public async Task<ServiceResponse<List<ProviderArtifactSetting>>> GetAttributesAsync(string serviceCode)
        {
            var attributes = new List<ProviderArtifactSetting>();
            var response = await _pricingClient.DescribeServicesAsync(new DescribeServicesRequest
            {
                FormatVersion = "aws_v1",
                ServiceCode = serviceCode
            });

            if(response == null)
            {
                return new ServiceResponse<List<ProviderArtifactSetting>>(attributes);
            }

            foreach (var service in response.Services)
            {
                foreach (var attributeName in service.AttributeNames)
                {
                    var attributeKeyValue = new KeyValuePair<string, string>(attributeName, ExtractName(attributeName));
                    var attribute = new ProviderArtifactSetting();
                    attribute.Name = attributeKeyValue;
                    attribute.Values = await GetAttributeValue(attributeName, serviceCode);
                    attributes.Add(attribute);
                }
            }

            return new ServiceResponse<List<ProviderArtifactSetting>>(attributes);
        }

        private async Task<List<string>> GetAttributeValue(string attributeName, string serviceCode)
        {
            var attributeValues = new List<string>();

            var response = await _pricingClient.GetAttributeValuesAsync(new GetAttributeValuesRequest
            {
                AttributeName = attributeName,
                ServiceCode = serviceCode
            });

            if(response == null)
            {
                return attributeValues;
            }

            foreach (var attributeValue in response.AttributeValues)
            {
                attributeValues.Add(attributeValue.Value);
            }

            return attributeValues;
        }

        public async Task<ServiceResponse<List<PricingTerm>>> GetProductsAsync(List<KeyValuePair<string, string>> settings, string serviceCode)
        {
            var filters = new List<Filter>();

            var pricingDetailsList = new List<PricingTerm>();

            foreach (var setting in settings)
            {
                var filter = new Filter
                {
                    Field = setting.Key,
                    Type = "TERM_MATCH",
                    Value = setting.Value
                };

                filters.Add(filter);
            }

            var response = await _pricingClient.GetProductsAsync(new GetProductsRequest
            {
                Filters = filters,
                FormatVersion = "aws_v1",
                ServiceCode = serviceCode
            });

            if(response == null)
            {
                return new ServiceResponse<List<PricingTerm>>(pricingDetailsList);
            }
            AddProductToPricingDetails(response, pricingDetailsList);
            return new ServiceResponse<List<PricingTerm>>(pricingDetailsList);
        }

        public async Task<ServiceResponse<List<PricingTerm>>> GetS3ProductsAsync(
            List<KeyValuePair<string, string>> settings, bool isAutomaticMonitoring)
        {
            var pricingDetailsList = new List<PricingTerm>();
            var storageFilters = new List<Filter>();
            var writeRequestFilters = new List<Filter>();
            var retrieveRequestFilters = new List<Filter>();
            var locationFilter = new Filter {Field = "location", Type = "TERM_MATCH", Value = ""};
            var storageClassFilter = new Filter {Field = "storageClass", Type = "TERM_MATCH", Value = ""};
            var volumeTypeFilter = new Filter {Field = "volumeType", Type = "TERM_MATCH", Value = ""};
            var writeRequestGroupValue = AwsS3Constants.WriteFrequentGroup;
            var retrieveRequestGroupValue = AwsS3Constants.RetrieveFrequentGroup;

            foreach (var (key, value) in settings)
            {
                var storageFilter = new Filter {Field = key, Type = "TERM_MATCH", Value = value};

                storageFilters.Add(storageFilter);
                switch (key)
                {
                    case "location":
                        locationFilter.Value = value;
                        break;
                    case "storageClass":
                        storageClassFilter.Value = value;
                        break;
                    case "volumeType":
                        volumeTypeFilter.Value = value;
                        break;
                }
            }

            var storagePrice = await _pricingClient.GetProductsAsync(new GetProductsRequest
            {
                Filters = storageFilters,
                FormatVersion = "aws_v1",
                ServiceCode = AwsS3Constants.Service
            });

            if (storagePrice == null) return new ServiceResponse<List<PricingTerm>>(pricingDetailsList);

            AddProductToPricingDetails(storagePrice, pricingDetailsList);

            //Check if the product is Standard - Infrequent Access.
            if (volumeTypeFilter.Value.Equals(
                AwsS3Constants.StandardInfrequentAccessProduct, StringComparison.InvariantCultureIgnoreCase))
            {
                writeRequestGroupValue = AwsS3Constants.WriteInfrequentGroup;
                retrieveRequestGroupValue = AwsS3Constants.RetrieveInfrequentGroup;
            }
            //Check if the product is One Zone - Infrequent Access.
            else if (volumeTypeFilter.Value.Equals(AwsS3Constants.InfrequentAccessProduct,
                StringComparison.InvariantCultureIgnoreCase))
            {
                writeRequestGroupValue = AwsS3Constants.WriteOneZoneInfrequentGroup;
                retrieveRequestGroupValue = AwsS3Constants.RetrieveOneZoneInfrequentGroup;
            }

            writeRequestFilters.Add(locationFilter);
            writeRequestFilters.Add(new Filter {Field = "group", Type = "TERM_MATCH", Value = writeRequestGroupValue});

            var writeRequestPrice = await _pricingClient.GetProductsAsync(new GetProductsRequest
            {
                Filters = writeRequestFilters,
                FormatVersion = "aws_v1",
                ServiceCode = AwsS3Constants.Service
            });

            if (writeRequestPrice == null) return new ServiceResponse<List<PricingTerm>>(pricingDetailsList);

            AddProductToPricingDetails(writeRequestPrice, pricingDetailsList);

            retrieveRequestFilters.Add(locationFilter);
            retrieveRequestFilters.Add(new Filter
                {Field = "group", Type = "TERM_MATCH", Value = retrieveRequestGroupValue});

            var retrieveRequestPrice = await _pricingClient.GetProductsAsync(new GetProductsRequest
            {
                Filters = retrieveRequestFilters,
                FormatVersion = "aws_v1",
                ServiceCode = AwsS3Constants.Service
            });

            if (retrieveRequestPrice == null) return new ServiceResponse<List<PricingTerm>>(pricingDetailsList);

            AddProductToPricingDetails(retrieveRequestPrice, pricingDetailsList);

            //Check if the product is Intelligent-Tiering.
            if (storageClassFilter.Value.Equals(AwsS3Constants.IntelligentTieringProduct , StringComparison.InvariantCultureIgnoreCase))
                pricingDetailsList =
                    await GetS3IntelligentTieringDetailListAsync(locationFilter, pricingDetailsList, isAutomaticMonitoring);

            return new ServiceResponse<List<PricingTerm>>(pricingDetailsList);
        }

        private async Task<List<PricingTerm>> GetS3IntelligentTieringDetailListAsync(Filter locationFilter,
            List<PricingTerm> pricingDetailsList, bool isAutomaticMonitoring)
        {
            var infrequentAccessFilters = new List<Filter>();
            var frequentAccessFilters = new List<Filter>();

            infrequentAccessFilters.Add(locationFilter);
            infrequentAccessFilters.Add(new Filter
                {Field = "volumeType", Type = "TERM_MATCH", Value = AwsS3Constants.IntelligentInfrequentAccessProduct});
            var infrequentAccessPrice = await _pricingClient.GetProductsAsync(new GetProductsRequest
            {
                Filters = infrequentAccessFilters,
                FormatVersion = "aws_v1",
                ServiceCode = AwsS3Constants.Service
            });

            AddProductToPricingDetails(infrequentAccessPrice, pricingDetailsList);

            frequentAccessFilters.Add(locationFilter);
            frequentAccessFilters.Add(new Filter
                {Field = "volumeType", Type = "TERM_MATCH", Value = AwsS3Constants.IntelligentFrequentAccessProduct});
            var frequentAccessPrice = await _pricingClient.GetProductsAsync(new GetProductsRequest
            {
                Filters = frequentAccessFilters,
                FormatVersion = "aws_v1",
                ServiceCode = AwsS3Constants.Service
            });

            AddProductToPricingDetails(frequentAccessPrice, pricingDetailsList);

            if (!isAutomaticMonitoring) return pricingDetailsList;

            var monitoringFilters = new List<Filter>
            {
                locationFilter,
                new Filter
                    {Field = "feeCode", Type = "TERM_MATCH", Value = AwsS3Constants.AutomationObjectCountFee}
            };

            var automaticMonitoringPrice = await _pricingClient.GetProductsAsync(new GetProductsRequest
            {
                Filters = monitoringFilters,
                FormatVersion = "aws_v1",
                ServiceCode = AwsS3Constants.Service
            });

            AddProductToPricingDetails(automaticMonitoringPrice, pricingDetailsList);

            return pricingDetailsList;
        }

        private void AddProductToPricingDetails(GetProductsResponse response,
            List<PricingTerm> pricingDetailsList)
        {
            foreach (var price in response.PriceList)
            {
                if (!price.StartsWith("{") || !price.EndsWith("}")) continue;

                var priceJson = JObject.Parse(price);
                if (priceJson == null) continue;
                
                var sku = priceJson.SelectToken("product.sku").Value<string>();
                var terms = priceJson.SelectToken("terms").ToObject<JObject>();
                if (terms == null) continue;

                foreach (var term in terms.Properties())
                {
                    var termName = term.Name;
                    var termProperties = term.Value.ToObject<JObject>();
                    if (termProperties == null)
                    {
                        continue;
                    }

                    foreach (var termOption in termProperties)
                    {
                        var pricingTerm = CreatePricingTerm(sku, termName, termOption);
                        pricingDetailsList.Add(pricingTerm);
                    }
                }
            }
        }

        private PricingTerm CreatePricingTerm(string sku, string termName, KeyValuePair<string, JToken> termOption)
        {
            var termOptionProperties = termOption.Value;
            var leaseContractLength = (string)termOptionProperties.SelectToken("termAttributes.LeaseContractLength");
            var offeringClass = (string)termOptionProperties.SelectToken("termAttributes.OfferingClass");
            var purchaseOption = (string)termOptionProperties.SelectToken("termAttributes.PurchaseOption");
            var termPriceDimensions = termOptionProperties.SelectToken("priceDimensions").ToObject<JObject>().Properties().ToList();
            var pricingDimensions = new List<PricingDimension>();

            foreach(var termPriceDimension in termPriceDimensions)
            {
                var pricingDimension = ExtractPricingDimension(termPriceDimension);
                pricingDimensions.Add(pricingDimension);
            }

            var pricingTerm = new PricingTerm()
            {
                Sku = sku,
                Term = termName,
                PricingDimensions = pricingDimensions,
                LeaseContractLength = leaseContractLength,
                OfferingClass = offeringClass,
                PurchaseOption = purchaseOption
            };

            return pricingTerm;
        }

        private PricingDimension ExtractPricingDimension(JProperty termPriceDimension)
        {
            var termValues = termPriceDimension.Value;
            
            if (termValues.SelectToken("endRange").Value<string>().Equals("Inf")) termValues["endRange"] = -1;

            var pricePerUnit = (decimal)termValues.SelectToken("pricePerUnit").ToObject<JObject>().Properties().ElementAt(0).Value;
            termValues.SelectToken("pricePerUnit").Replace(pricePerUnit);

            

            return termValues.ToObject<PricingDimension>();
        }

        private static string ExtractName(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;

            var splittedString = Regex.Replace(
                Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"),
                @"(\p{Ll})(\P{Ll})", "$1 $2");
            var firstUpperAndSplittedString = splittedString.ToCharArray();
            firstUpperAndSplittedString[0] = char.ToUpper(firstUpperAndSplittedString[0]);

            return new string(firstUpperAndSplittedString);
        }
    }
}