using AutoMapper;
using FRF.Core.Services;
using FRF.Web.Dtos.Artifacts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FRF.Web.Controllers
{
    [Route("api/aws-artifacts-provider")]
    [ApiController]
    [Authorize]
    public class AwsArtifactsProviderController : ControllerBase
    {
        private readonly IArtifactsProviderService _artifactsProviderService;
        private readonly IMapper _mapper;

        public AwsArtifactsProviderController(IArtifactsProviderService artifactsProviderService, IMapper mapper)
        {
            _artifactsProviderService = artifactsProviderService;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieve all the artifact names from AWS provider.
        /// </summary>
        /// <returns></returns>
        [HttpGet("names")]
        public async Task<IActionResult> GetNamesAsync()
        {
            var response = await _artifactsProviderService.GetNamesAsync();
            if (!response.Success)
            {
                return StatusCode(503);
            }
            return Ok(response.Value);
        }

        [HttpPost("products")]
        public async Task<IActionResult> GetProductsAsync(List<KeyValuePair<string, string>> settings, string serviceCode)
        {
            var response = await _artifactsProviderService.GetProductsAsync(settings, serviceCode);

            var products = _mapper.Map<List<PricingTermDTO>>(response.Value);
            return Ok(products);
        }

        [HttpGet("required-fields")]
        public async Task<IActionResult> GetRequiredFieldsAsync(string serviceCode)
        {
            var response = await _artifactsProviderService.GetRequiredFieldsAsync(serviceCode);
            return Ok(response.Value);
        }
    }
}
