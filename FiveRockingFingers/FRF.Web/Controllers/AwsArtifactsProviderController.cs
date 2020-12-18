using System.Net;
using FRF.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FRF.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AwsArtifactsProviderController : ControllerBase
    {
        private readonly IArtifactsProviderService _artifactsProviderService;

        public AwsArtifactsProviderController(IArtifactsProviderService artifactsProviderService)
        {
            _artifactsProviderService = artifactsProviderService;
        }

        /// <summary>
        /// Retrieve all the artifact names from AWS provider.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetNamesAsync()
        {
            var productsNames = await _artifactsProviderService.GetNamesAsync();
            if (productsNames==null)
            {
                return StatusCode(503);
            }
            return Ok(productsNames);
        }

        [HttpGet]
        public async Task<IActionResult> GetAttributesAsync(string serviceCode)
        {
            var attributes = await _artifactsProviderService.GetAttributes(serviceCode);

            return Ok(attributes);
        }

        [HttpPost]
        public async Task<IActionResult> GetProducts(List<KeyValuePair<string, string>> settings, string serviceCode)
        {
            var products = await _artifactsProviderService.GetProducts(settings, serviceCode);

            return Ok(products);
        }
    }
}