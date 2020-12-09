using FRF.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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

        [HttpGet]
        public async Task<IActionResult> GetAllNamesAsync()
        {
            var productsNames = await _artifactsProviderService.GetAllNamesAsync();

            return Ok(productsNames);
        }
    }
}