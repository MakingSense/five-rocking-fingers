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
    public class ArtifactsController : BaseApiController<ArtifactDTO>
    {
        private readonly IMapper mapper;
        public IArtifactsService ArtifactsService { get; set; }

        public ArtifactsController(IArtifactsService artifactsService, IMapper mapper)
        {
            this.ArtifactsService = artifactsService;
            this.mapper = mapper;
        }

        [HttpGet]
        public override IActionResult GetAll()
        {
            var artifacts = ArtifactsService.GetAll();

            var artifactsDto = mapper.Map<IEnumerable<ArtifactDTO>>(artifacts);

            return Ok(artifactsDto);
        }

        [HttpGet("{id}")]
        public override IActionResult Get(int id)
        {
            var artifact = ArtifactsService.Get(id);

            if (artifact == null)
            {
                return NotFound();
            }

            var artifactDto = mapper.Map<ArtifactDTO>(artifact);

            return Ok(artifactDto);
        }

        [HttpPost]
        public override IActionResult Save(ArtifactDTO artifactDto)
        {
            if (artifactDto == null)
            {
                return BadRequest("Artifact object is null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values);
            }

            var artifact = mapper.Map<FRF.Core.Models.Artifact>(artifactDto);

            var artifactCreated = mapper.Map<ArtifactDTO>(ArtifactsService.Save(artifact));

            return Ok(artifactCreated);
        }

        [HttpPut]
        public override IActionResult Update(ArtifactDTO artifactDto)
        {
            if (artifactDto == null)
            {
                return BadRequest("Artifact object is null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values);
            }

            var artifact = ArtifactsService.Get(artifactDto.Id);

            if (artifact == null)
            {
                return NotFound();
            }

            mapper.Map(artifactDto, artifact);

            var updatedArtifact = mapper.Map<ArtifactDTO>(ArtifactsService.Update(artifact));

            return Ok(updatedArtifact);
        }

        [HttpDelete("{id}")]
        public override IActionResult Delete(int id)
        {
            var artifact = ArtifactsService.Get(id);

            if (artifact == null)
            {
                return NotFound();
            }

            ArtifactsService.Delete(id);

            return NoContent();
        }
    }
}
