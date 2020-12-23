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
using Amazon;
using FRF.Core.Models;
using Microsoft.Extensions.Configuration;

namespace FRF.Core.Services
{
    public class AwsArtifactsProviderService : IArtifactsProviderService
    {
        private const string OffersCodeIndex = "offers.*.offerCode";
        private readonly AwsPricing _awsPricingOptions;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AmazonPricingClient _client;

        public AwsArtifactsProviderService(IOptions<AwsPricing> awsApiString,
            IHttpClientFactory httpClientFactory, AmazonPricingClient client)
        {
            _httpClientFactory = httpClientFactory;
            _awsPricingOptions = awsApiString.Value;
            _client = client;
        }

        /// <summary>
        /// Get all the artifacts names with the service code from aws pricing api.
        /// </summary>
        /// <returns>List of KeyValuePair</returns>
        public async Task<List<KeyValuePair<string, string>>> GetNamesAsync()
        {
            var artifactsNames = new Dictionary<string, string>();
            var httpClient = _httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync(_awsPricingOptions.ApiUrl);
            if (!response.IsSuccessStatusCode)
            {
                return null;
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

            return artifactsNames.ToList();
        }

        public async Task<List<ProviderArtifactSetting>> GetAttributesAsync(string serviceCode)
        {
            var attributes = new List<ProviderArtifactSetting>();
            var response = await _client.DescribeServicesAsync(new DescribeServicesRequest
            {
                FormatVersion = "aws_v1",
                ServiceCode = serviceCode
            });

            if(response != null)
            {
                foreach (var service in response.Services)
                {
                    foreach (var attributeName in service.AttributeNames)
                    {
                        var attributeKeyValue = new KeyValuePair<string, string>(attributeName, ExtractName(attributeName));
                        var attribute = new ProviderArtifactSetting();
                        attribute.Name = attributeKeyValue;
                        attribute.Values = new List<string>();

                        var response2 = await _client.GetAttributeValuesAsync(new GetAttributeValuesRequest
                        {
                            AttributeName = attributeName,
                            ServiceCode = serviceCode
                        });

                        if(response2 != null)
                        {
                            foreach (var attributeValues in response2.AttributeValues)
                            {
                                attribute.Values.Add(attributeValues.Value);
                            }
                        }
                        attributes.Add(attribute);
                    }
                }
            }
            return attributes;
        }

        public async Task<List<PricingTerm>> GetProductsAsync(List<KeyValuePair<string, string>> settings, string serviceCode)
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

            var response = await _client.GetProductsAsync(new GetProductsRequest
            {
                Filters = filters,
                FormatVersion = "aws_v1",
                ServiceCode = serviceCode
            });

            if(response != null)
            {
                foreach (var price in response.PriceList)
                {
                    var priceJson = JObject.Parse(price);

                    var sku = (string)priceJson.SelectTokens("product.sku").ToList()[0];
                    var terms = priceJson.SelectToken("terms").ToObject<JObject>();

                    if(terms != null)
                    {
                        foreach (var term in terms.Properties())
                        {
                            var termName = term.Name;

                            var termProperties = term.Value.ToObject<JObject>();

                            if(termProperties != null)
                            {
                                foreach (var termOption in termProperties)
                                {
                                    var termOptionProperties = termOption.Value;
                                    var leaseContractLength = (string)termOptionProperties.SelectToken("termAttributes.LeaseContractLength");
                                    var offeringClass = (string)termOptionProperties.SelectToken("termAttributes.OfferingClass");
                                    var purchaseOption = (string)termOptionProperties.SelectToken("termAttributes.PurchaseOption");
                                    var termPriceDimensions = termOptionProperties.SelectToken("priceDimensions").ToObject<JObject>().Properties().ToList();
                                    var termPriceDimension = termPriceDimensions[0];
                                    var pricingDimension = new PricingDimension()
                                    {
                                        Unit = (string)termPriceDimension.Value.SelectToken("unit"),
                                        EndRange = (string)termPriceDimension.Value.SelectToken("endRange"),
                                        Description = (string)termPriceDimension.Value.SelectToken("description"),
                                        RateCode = (string)termPriceDimension.Value.SelectToken("rateCode"),
                                        BeginRange = (string)termPriceDimension.Value.SelectToken("beginRange"),
                                        Currency = termPriceDimension.Value.SelectToken("pricePerUnit").ToObject<JObject>().Properties().ElementAt(0).Name,
                                        PricePerUnit = (float)termPriceDimension.Value.SelectToken("pricePerUnit").ToObject<JObject>().Properties().ElementAt(0).Value,
                                    };
                                    var pricingDetails = new PricingDimension();
                                    if (termPriceDimensions.Count > 1)
                                    {
                                        var termPriceDetail = termPriceDimensions[1];
                                        pricingDetails = new PricingDimension()
                                        {
                                            Unit = (string)termPriceDetail.Value.SelectToken("unit"),
                                            EndRange = (string)termPriceDetail.Value.SelectToken("endRange"),
                                            Description = (string)termPriceDetail.Value.SelectToken("description"),
                                            RateCode = (string)termPriceDetail.Value.SelectToken("rateCode"),
                                            BeginRange = (string)termPriceDetail.Value.SelectToken("beginRange"),
                                            Currency = termPriceDetail.Value.SelectToken("pricePerUnit").ToObject<JObject>().Properties().ElementAt(0).Name,
                                            PricePerUnit = (float)termPriceDetail.Value.SelectToken("pricePerUnit").ToObject<JObject>().Properties().ElementAt(0).Value
                                        };
                                    }
                                    else
                                    {
                                        pricingDetails = null;
                                    }

                                    var pricingTerm = new PricingTerm()
                                    {
                                        Sku = sku,
                                        Term = termName,
                                        PricingDimension = pricingDimension,
                                        PricingDetail = pricingDetails,
                                        LeaseContractLength = leaseContractLength,
                                        OfferingClass = offeringClass,
                                        PurchaseOption = purchaseOption
                                    };

                                    pricingDetailsList.Add(pricingTerm);
                                }
                            } 
                        }
                    }
                }
            }
            return pricingDetailsList;
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