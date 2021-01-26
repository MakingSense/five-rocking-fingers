using AutoMapper;
using FRF.Core.Services;
using FRF.Web.Dtos.Artifacts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FRF.Web.Controllers
{
    [Route("api/[controller]/[action]")]
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
        [HttpGet]
        public async Task<IActionResult> GetNamesAsync()
        {
            var response = await _artifactsProviderService.GetNamesAsync();
            if (!response.Success)
            {
                return StatusCode(503);
            }
            return Ok(response.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetAttributesAsync(string serviceCode)
        {
            var response = await _artifactsProviderService.GetAttributesAsync(serviceCode);

            var attributes = _mapper.Map<List<ProviderArtifactSettingDTO>>(response.Value);
            return Ok(attributes);
        }

        [HttpPost]
        public async Task<IActionResult> GetProductsAsync(List<KeyValuePair<string, string>> settings, string serviceCode)
        {
            var response = await _artifactsProviderService.GetProductsAsync(settings, serviceCode);

            var products = _mapper.Map<List<PricingTermDTO>>(response.Value);
            return Ok(products);
        }
    }
}
