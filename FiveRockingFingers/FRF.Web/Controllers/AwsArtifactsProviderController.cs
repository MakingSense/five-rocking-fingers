using System.Net;
using FRF.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FRF.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
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
    }
}