using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FRF.Core.Models;
using FRF.Core.Services;
using Microsoft.AspNetCore.Mvc;
using FRF.Web.Dtos.Artifacts;

namespace FRF.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
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

            var artifactsDto = _mapper.Map<IEnumerable<ArtifactDTO>>(artifacts);

            return Ok(artifactsDto);
        }

        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetAllByProjectIdAsync(int projectId)
        {
            var artifacts = await _artifactsService.GetAllByProjectId(projectId);

            var artifactsDto = _mapper.Map<IEnumerable<ArtifactDTO>>(artifacts);

            return Ok(artifactsDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var artifact = await _artifactsService.Get(id);

            if (artifact == null)
            {
                return NotFound();
            }

            var artifactDto = _mapper.Map<ArtifactDTO>(artifact);

            return Ok(artifactDto);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAsync(ArtifactUpsertDTO artifactDto)
        {
            var artifact = _mapper.Map<FRF.Core.Models.Artifact>(artifactDto);

            var artifactCreated = _mapper.Map<ArtifactDTO>(await _artifactsService.Save(artifact));

            return Ok(artifactCreated);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, ArtifactUpsertDTO artifactDto)
        {
            var artifact = await _artifactsService.Get(id);

            if (artifact == null)
            {
                return NotFound();
            }

            _mapper.Map(artifactDto, artifact);

            var updatedArtifact = _mapper.Map<ArtifactDTO>(await _artifactsService.Update(artifact));

            return Ok(updatedArtifact);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var artifact = await _artifactsService.Get(id);

            if (artifact == null)
            {
                return NotFound();
            }

            await _artifactsService.Delete(id);

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> SetRelation(IList<ArtifactsRelationDTO> artifactRelationList)
        {
            var artifactsRelations = _mapper.Map<IList<ArtifactsRelation>>(artifactRelationList);
            var result = await _artifactsService.SetRelation(artifactsRelations);
            var artifactsResult = _mapper.Map<IList<ArtifactsRelationDTO>>(result);
            
            if (artifactsResult == null) return BadRequest();

            return Ok(artifactsResult);
        }
    }
}
