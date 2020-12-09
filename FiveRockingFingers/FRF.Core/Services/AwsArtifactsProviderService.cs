using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public class AwsArtifactsProviderService : IArtifactsProviderService
    {
        private readonly IConfiguration _configuration;

        public AwsArtifactsProviderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Get all the artifacts names with the service code from aws pricing api.
        /// </summary>
        /// <returns>List of KeyValuePair</returns>
        public async Task<List<KeyValuePair<string, string>>> GetAllNamesAsync()
        {
            var artifactsNames = new Dictionary<string, string>();
            var httpClient = new HttpClient();

            var awsPricingList =
                await httpClient.GetStringAsync(_configuration.GetValue<string>("AWSPricingApi:Url"));

            var awsArtifactsNames = JObject
                .Parse(awsPricingList)
                .SelectTokens("offers.*.offerCode");

            foreach (var artifactName in awsArtifactsNames)
            {
                var serviceCode = artifactName.ToString();
                var name = SplitCamelCase(artifactName.ToString());
                artifactsNames.Add(serviceCode, name);
            }

            return artifactsNames.ToList();
        }

        private static string SplitCamelCase(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;

            var splittedString = Regex.Replace(Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"),
                @"(\p{Ll})(\P{Ll})", "$1 $2");
            var firstUpperAndSplittedString = splittedString.ToCharArray();
            firstUpperAndSplittedString[0] = char.ToUpper(firstUpperAndSplittedString[0]);

            return new string(firstUpperAndSplittedString);
        }
    }
}