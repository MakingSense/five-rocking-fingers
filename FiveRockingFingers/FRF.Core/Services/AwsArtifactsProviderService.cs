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

namespace FRF.Core.Services
{
    public class AwsArtifactsProviderService : IArtifactsProviderService
    {
        private const string OffersCodeIndex = "offers.*.offerCode";
        private readonly AwsPricing _awsPricingOptions;
        private readonly IHttpClientFactory _httpClientFactory;

        public AwsArtifactsProviderService(IOptions<AwsPricing> awsApiString,
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _awsPricingOptions = awsApiString.Value;
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

        public async Task<List<ProviderArtifactSetting>> GetAttributes(string serviceCode)
        {
            var attributes = new List<ProviderArtifactSetting>();

            var client = new AmazonPricingClient("AKIAIXYTIP4OAHZ5C6LQ", "YvTfqjK5HQJIXAceBH6b657y9GiLTSJiKyA44gkM", RegionEndpoint.USEast1);
            var response = await client.DescribeServicesAsync(new DescribeServicesRequest
            {
                FormatVersion = "aws_v1",
                ServiceCode = serviceCode
            });

            foreach (var service in response.Services)
            {
                foreach (var a in service.AttributeNames)
                {
                    var attribute = new ProviderArtifactSetting();
                    attribute.Name = a;
                    attribute.Values = new List<string>();

                    var response2 = await client.GetAttributeValuesAsync(new GetAttributeValuesRequest
                    {
                        AttributeName = a,
                        ServiceCode = serviceCode
                    });

                    foreach (var attribute2 in response2.AttributeValues)
                    {
                        attribute.Values.Add(attribute2.Value);
                    }
                    attributes.Add(attribute);
                }
            }
            return attributes;
        }

        public async Task<List<PricingDetail>> GetProducts(List<KeyValuePair<string, string>> settings, string serviceCode)
        {
            var client = new AmazonPricingClient("AKIAIXYTIP4OAHZ5C6LQ", "YvTfqjK5HQJIXAceBH6b657y9GiLTSJiKyA44gkM", RegionEndpoint.USEast1);
            var filters = new List<Filter>();

            var pricingDetailsList = new List<PricingDetail>();
            
            foreach(var setting in settings)
            {
                var filter = new Filter
                {
                    Field = setting.Key,
                    Type = "TERM_MATCH",
                    Value = setting.Value
                };

                filters.Add(filter);
            }

            var response = await client.GetProductsAsync(new GetProductsRequest
            {
                Filters = filters,
                FormatVersion = "aws_v1",
                ServiceCode = serviceCode
            });

            foreach (var price in response.PriceList)
            {
                var priceJson = JObject.Parse(price);

                var sku = priceJson.SelectTokens("product.sku");

                var pricingDetails = new PricingDetail()
                {
                    Sku = (string)sku.ToList()[0]
                };

                pricingDetailsList.Add(pricingDetails);
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