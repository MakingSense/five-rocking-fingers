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
    public class ArtifactsController : BaseApiControllerAsync<ArtifactDTO>
    {
        private readonly IMapper _mapper;
        private readonly IArtifactsService _artifactsService;

        public ArtifactsController(IArtifactsService artifactsService, IMapper mapper)
        {
            _artifactsService = artifactsService;
            _mapper = mapper;
        }

        [HttpGet]
        public override async Task<IActionResult> GetAll()
        {
            var artifacts = await _artifactsService.GetAll();

            var artifactsDto = _mapper.Map<IEnumerable<ArtifactDTO>>(artifacts);

            return Ok(artifactsDto);
        }

        [HttpGet("{id}")]
        public override async Task<IActionResult> Get(int id)
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
        public override async Task<IActionResult> Save(ArtifactDTO artifactDto)
        {
            if (artifactDto == null)
            {
                return BadRequest("Artifact object is null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values);
            }

            var artifact = _mapper.Map<FRF.Core.Models.Artifact>(artifactDto);

            var artifactCreated = _mapper.Map<ArtifactDTO>(await _artifactsService.Save(artifact));

            return Ok(artifactCreated);
        }

        [HttpPut]
        public override async Task<IActionResult> Update(ArtifactDTO artifactDto)
        {
            if (artifactDto == null)
            {
                return BadRequest("Artifact object is null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values);
            }

            var artifact = await _artifactsService.Get(artifactDto.Id);

            if (artifact == null)
            {
                return NotFound();
            }

            _mapper.Map(artifactDto, artifact);

            var updatedArtifact = _mapper.Map<ArtifactDTO>(await _artifactsService.Update(artifact));

            return Ok(updatedArtifact);
        }

        [HttpDelete("{id}")]
        public override async Task<IActionResult> Delete(int id)
        {
            var artifact = await _artifactsService.Get(id);

            if (artifact == null)
            {
                return NotFound();
            }

            await _artifactsService.Delete(id);

            return NoContent();
        }
    }
}
