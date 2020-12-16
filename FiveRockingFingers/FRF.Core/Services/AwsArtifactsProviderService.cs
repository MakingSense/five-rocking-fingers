using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FRF.Core.Base;
using Microsoft.Extensions.Options;

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