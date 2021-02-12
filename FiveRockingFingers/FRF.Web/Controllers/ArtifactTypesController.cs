using AutoMapper;
using FRF.Core.Services;
using FRF.Web.Dtos.Artifacts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FRF.Web.Controllers
{
    [ApiController]
    [Route("api/artifact-types")]
    [Authorize]
    public class ArtifactTypesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IArtifactTypesService _artifactTypesService;

        public ArtifactTypesController(IArtifactTypesService artifactTypesService, IMapper mapper)
        {
            _artifactTypesService = artifactTypesService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var artifactTypes = await _artifactTypesService.GetAllAsync();
            var artifactTypesDto = _mapper.Map<IEnumerable<ArtifactTypeDTO>>(artifactTypes.Value);

            return Ok(artifactTypesDto);
        }

        [HttpGet("{providerName}")]
        public async Task<IActionResult> GetAllByProviderAsync(string providerName)
        {
            var artifactTypes = await _artifactTypesService.GetAllByProviderAsync(providerName);
            if (!artifactTypes.Success)
            {
                return NotFound();
            }

            var artifactTypesDto = _mapper.Map<IEnumerable<ArtifactTypeDTO>>(artifactTypes.Value);
            return Ok(artifactTypesDto);
        }
    }
}
