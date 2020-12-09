using FRF.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FRF.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AwsProviderController : ControllerBase
    {
        private readonly IProviderService _providerService;

        public AwsProviderController(IProviderService providerService)
        {
            _providerService = providerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNamesAsync()
        {
            var productsNames = await _providerService.GetAllNamesAsync();

            return Ok(productsNames);
        }
    }
}