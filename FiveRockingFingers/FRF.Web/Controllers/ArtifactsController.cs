using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Services;
using FRF.Web.Dtos.Artifacts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FRF.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class ArtifactsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IArtifactsService _artifactsService;

        public ArtifactsController(IArtifactsService artifactsService, IMapper mapper)
        {
            _artifactsService = artifactsService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var artifacts = await _artifactsService.GetAll();

            var artifactsDto = _mapper.Map<IEnumerable<ArtifactDTO>>(artifacts.Value);

            return Ok(artifactsDto);
        }

        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetAllByProjectIdAsync(int projectId)
        {
            var artifacts = await _artifactsService.GetAllByProjectId(projectId);

            var artifactsDto = _mapper.Map<IEnumerable<ArtifactDTO>>(artifacts.Value);

            return Ok(artifactsDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var artifact = await _artifactsService.Get(id);

            if (!artifact.Success)
            {
                return NotFound();
            }

            var artifactDto = _mapper.Map<ArtifactDTO>(artifact.Value);

            return Ok(artifactDto);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAsync(ArtifactUpsertDTO artifactDto)
        {
            var artifact = _mapper.Map<FRF.Core.Models.Artifact>(artifactDto);

            var response = await _artifactsService.Save(artifact);
            var artifactCreated = _mapper.Map<ArtifactDTO>(response.Value);

            return Ok(artifactCreated);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, ArtifactUpsertDTO artifactDto)
        {
            var artifact = await _artifactsService.Get(id);

            if (!artifact.Success)
            {
                return NotFound();
            }

            _mapper.Map(artifactDto, artifact.Value);

            var response = await _artifactsService.Update(artifact.Value);
            var updatedArtifact = _mapper.Map<ArtifactDTO>(response.Value);

            return Ok(updatedArtifact);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var artifact = await _artifactsService.Get(id);

            if (!artifact.Success)
            {
                return NotFound();
            }

            await _artifactsService.Delete(id);

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> SetRelationAsync(IList<ArtifactsRelationDTO> artifactRelationList)
        {
            var artifactsRelations = _mapper.Map<IList<ArtifactsRelation>>(artifactRelationList);
            var result = await _artifactsService.SetRelationAsync(artifactsRelations);
            if (!result.Success) return BadRequest();

            var artifactsResult = _mapper.Map<IList<ArtifactsRelationDTO>>(result.Value);
            return Ok(artifactsResult);
        }
    }
}
