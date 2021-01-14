﻿using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Services;
using FRF.Web.Dtos.Artifacts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;

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
        public async Task<IActionResult> SetRelationAsync(IList<ArtifactsRelationUpsertDTO> artifactRelationList)
        {
            var artifactsRelations = _mapper.Map<IList<ArtifactsRelation>>(artifactRelationList);
            var result = await _artifactsService.SetRelationAsync(artifactsRelations);
            if (result == null) return BadRequest();

            var artifactsResult = _mapper.Map<IList<ArtifactsRelationDTO>>(result);
            return Ok(artifactsResult);
        }

        [HttpGet("{arifactId}")]
        public async Task<IActionResult> GetRelationAsync(int artifactId)
        {
            var result = await _artifactsService.GetRelationsAsync(artifactId);

            return Ok(_mapper.Map<ArtifactsRelationDTO>(result));
        }

        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetAllRelationsByProjectIdAsync(int projectId)
        {
            var result = await _artifactsService.GetAllRelationsByProjectIdAsync(projectId);

             if (result == null) return BadRequest();

             return Ok(_mapper.Map<IList<ArtifactsRelationDTO>>(result));

        }

        [HttpGet("{arifactRelation}")]
        public async Task<IActionResult> DeleteRelationAsync(int artifactId1, int artifactId2)
        {
            var result = await _artifactsService.DeleteRelationAsync(artifactId1, artifactId2);

            return Ok(_mapper.Map<ArtifactsRelationDTO>(result));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateArtifactsRelationsAsync(
            IList<ArtifactsRelationDTO> artifactRelationUpdatedList)
        {
            var artifactsRelationsList = _mapper.Map<IList<ArtifactsRelation>>(artifactRelationUpdatedList);
            var result = await _artifactsService.UpdateRelationAsync(artifactsRelationsList);
            if (result == null) return BadRequest();

            var artifactsResult = _mapper.Map<IList<ArtifactsRelationDTO>>(result);
            return Ok(artifactsResult);
        }
    }
}