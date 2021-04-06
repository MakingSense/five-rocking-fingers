using System;
using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Services;
using FRF.Web.Dtos.Artifacts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using FRF.Core.Response;
using FRF.Web.Authorization;

namespace FRF.Web.Controllers
{
    [ApiController]
    [Route("api/artifacts")]
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

        [HttpGet("~/api/projects/{projectId}/artifacts")]
        public async Task<IActionResult> GetAllByProjectIdAsync(int projectId)
        {
            var artifacts = await _artifactsService.GetAllByProjectId(projectId);

            if (!artifacts.Success)
            {
                return BadRequest($"Error {artifacts.Error.Code}: {artifacts.Error.Message}");
            }

            var artifactsDto = _mapper.Map<IEnumerable<ArtifactDTO>>(artifacts.Value);

            return Ok(artifactsDto);
        }

        [HttpGet("{artifactId}")]
        [Authorize(ArtifactAuthorization.Ownership)]
        public async Task<IActionResult> GetAsync(int artifactId)
        {
            var artifact = await _artifactsService.Get(artifactId);

            if (!artifact.Success)
            {
                return NotFound($"Error {artifact.Error.Code}: {artifact.Error.Message}");
            }

            var artifactDto = _mapper.Map<ArtifactDTO>(artifact.Value);

            return Ok(artifactDto);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAsync(ArtifactUpsertDTO artifactDto)
        {
            var artifact = _mapper.Map<Artifact>(artifactDto);

            var response = await _artifactsService.Save(artifact);
            if (!response.Success)
            {
                if (response.Error.Code == ErrorCodes.InvalidArtifactSettings)
                {
                    return BadRequest($"Error {response.Error.Code}: {response.Error.Message}");
                }

                return NotFound($"Error {response.Error.Code}: {response.Error.Message}");
            }
            var artifactCreated = _mapper.Map<ArtifactDTO>(response.Value);

            return Ok(artifactCreated);
        }

        [HttpPut("{artifactId}")]
        [Authorize(ArtifactAuthorization.Ownership)]
        public async Task<IActionResult> UpdateAsync(int artifactId, ArtifactUpsertDTO artifactDto)
        {
            var artifact = await _artifactsService.Get(artifactId);

            if (!artifact.Success)
            {
                if (artifact.Error.Code == ErrorCodes.InvalidArtifactSettings)
                {
                    return BadRequest($"Error {artifact.Error.Code}: {artifact.Error.Message}");
                }

                return NotFound($"Error {artifact.Error.Code}: {artifact.Error.Message}");
            }

            _mapper.Map(artifactDto, artifact.Value);

            var response = await _artifactsService.Update(artifact.Value);
            var updatedArtifact = _mapper.Map<ArtifactDTO>(response.Value);

            return Ok(updatedArtifact);
        }

        [HttpDelete("{artifactId}")]
        [Authorize(ArtifactAuthorization.Ownership)]
        public async Task<IActionResult> DeleteAsync(int artifactId)
        {
            var artifact = await _artifactsService.Get(artifactId);

            if (!artifact.Success)
            {
                return NotFound($"Error {artifact.Error.Code}: {artifact.Error.Message}");
            }

            await _artifactsService.Delete(artifactId);

            return NoContent();
        }

        [HttpPost("{artifactId}/relations")]
        [Authorize(ArtifactAuthorization.Ownership)]
        [Authorize(ArtifactAuthorization.RelationsListOwnership)]
        public async Task<IActionResult> SetRelationAsync(int artifactId, IList<ArtifactsRelationInsertDTO> artifactRelationList)
        {
            var artifactsRelations = _mapper.Map<IList<ArtifactsRelation>>(artifactRelationList);
            var result = await _artifactsService.SetRelationAsync(artifactId, artifactsRelations);
            if (!result.Success)
            {
                if (result.Error.Code == ErrorCodes.ArtifactNotExists ||
                    result.Error.Code == ErrorCodes.ProjectNotExists)
                {
                    return NotFound($"Error {result.Error.Code}: {result.Error.Message}");
                }
                return BadRequest($"Error {result.Error.Code}: {result.Error.Message}");
            }

            var artifactsResult = _mapper.Map<IList<ArtifactsRelationDTO>>(result.Value);
            return Ok(artifactsResult);
        }

        [HttpGet("{artifactId}/relations")]
        [Authorize(ArtifactAuthorization.Ownership)]
        public async Task<IActionResult> GetRelationsAsync(int artifactId)
        {
            var result = await _artifactsService.GetAllRelationsOfAnArtifactAsync(artifactId);
            if (!result.Success)
            {
                return NotFound($"Error {result.Error.Code}: {result.Error.Message}");
            }
            var artifactsRelationsDTO = _mapper.Map<IList<ArtifactsRelationDTO>>(result.Value);
            return Ok(artifactsRelationsDTO);
        }

        [HttpGet("~/api/projects/{projectId}/relations")]
        public async Task<IActionResult> GetAllRelationsByProjectIdAsync(int projectId)
        {
            var result = await _artifactsService.GetAllRelationsByProjectIdAsync(projectId);

             if (!result.Success) return BadRequest($"Error {result.Error.Code}: {result.Error.Message}");

             return Ok(_mapper.Map<IList<ArtifactsRelationDTO>>(result.Value));

        }

        [HttpDelete("~/api/relations/{relationId}")]
        public async Task<IActionResult> DeleteRelationAsync(Guid relationId)
        {
            var result = await _artifactsService.DeleteRelationAsync(relationId);
            if (!result.Success) return NotFound($"Error {result.Error.Code}: {result.Error.Message}");

            return NoContent();
        }

        [HttpDelete("~/api/relations")]
        [Authorize(ArtifactAuthorization.RelationsListOwnership)]
        public async Task<IActionResult> DeleteRelationsAsync(IList<Guid> artifactRelationIds)
        {
            var result = await _artifactsService.DeleteRelationsAsync(artifactRelationIds);
            if (!result.Success)
                switch (result.Error.Code)
                {
                    case ErrorCodes.ArtifactNotExists:
                        return NotFound($"Error {result.Error.Code}: {result.Error.Message}");
                    case ErrorCodes.InvalidArtifactSettings:
                        return BadRequest($"Error {result.Error.Code}: {result.Error.Message}");
                }

            return NoContent();
        }

        [HttpPut("{artifactId}/relations")]
        [Authorize(ArtifactAuthorization.Ownership)]
        [Authorize(ArtifactAuthorization.RelationsListOwnership)]
        public async Task<IActionResult> UpdateRelationsAsync(int artifactId,
            IList<ArtifactsRelationUpdateDTO> artifactRelationUpdatedList)
        {
            var artifactsRelationsList = _mapper.Map<IList<ArtifactsRelation>>(artifactRelationUpdatedList);
            var result = await _artifactsService.UpdateRelationAsync(artifactId, artifactsRelationsList);
            if (!result.Success)
            {
                if (result.Error.Code == ErrorCodes.ArtifactNotExists ||
                    result.Error.Code == ErrorCodes.ProjectNotExists)
                {
                    return NotFound($"Error {result.Error.Code}: {result.Error.Message}");
                }
                return BadRequest($"Error {result.Error.Code}: {result.Error.Message}");
            }

            var artifactsResult = _mapper.Map<IList<ArtifactsRelationDTO>>(result.Value);
            return Ok(artifactsResult);
        }
    }
}