using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Response;
using FRF.Core.Services;
using FRF.Web.Dtos.ProjectResources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRF.Web.Controllers
{
    [ApiController]
    [Route("api/project-resources")]
    [Authorize]
    public class ProjectResourcesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProjectResourcesService _projectResourcesService;

        public ProjectResourcesController(IProjectResourcesService projectResourcesService, IMapper mapper)
        {
            _projectResourcesService = projectResourcesService;
            _mapper = mapper;
        }

        [HttpGet("~/api/projects/{projectId}/project-resources")]
        public async Task<IActionResult> GetByProjectIdAsync(int projectId)
        {
            var projectResources = await _projectResourcesService.GetByProjectIdAsync(projectId);

            if (!projectResources.Success) return NotFound(projectResources.Error);

            var projectResourcesDto = _mapper.Map<IEnumerable<ProjectResourceDTO>>(projectResources.Value);

            return Ok(projectResourcesDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var response = await _projectResourcesService.GetAsync(id);

            if (!response.Success) return NotFound(response.Error);

            var projectResourceDto = _mapper.Map<ProjectResourceDTO>(response.Value);

            return Ok(projectResourceDto);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAsync(ProjectResourceUpsertDTO projectResourceDto)
        {
            var projectResource = _mapper.Map<FRF.Core.Models.ProjectResource>(projectResourceDto);

            var response = await _projectResourcesService.SaveAsync(projectResource);

            if (!response.Success && (response.Error.Code == ErrorCodes.ProjectNotExists || response.Error.Code == ErrorCodes.ResourceNotExists))
                return NotFound(response.Error);

            if (!response.Success && (response.Error.Code == ErrorCodes.InvalidBeginDateForProjectResource || response.Error.Code == ErrorCodes.InvalidEndDateForProjectResource))
                return BadRequest(response.Error);

            var projectResourceCreated = _mapper.Map<ProjectResourceDTO>(response.Value);

            return Ok(projectResourceCreated);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, ProjectResourceUpsertDTO projectResourceDto)
        {

            var projectResource = _mapper.Map<ProjectResource>(projectResourceDto);
            projectResource.Id = id;

            var response = await _projectResourcesService.UpdateAsync(projectResource);

            if (!response.Success && (response.Error.Code == ErrorCodes.ProjectNotExists || response.Error.Code == ErrorCodes.ResourceNotExists))
                return NotFound(response.Error);

            if (!response.Success && (response.Error.Code == ErrorCodes.InvalidBeginDateForProjectResource || response.Error.Code == ErrorCodes.InvalidEndDateForProjectResource))
                return BadRequest(response.Error);

            var updatedProjectResource = _mapper.Map<ProjectResourceDTO>(response.Value);

            return Ok(updatedProjectResource);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var response = await _projectResourcesService.GetAsync(id);

            if (!response.Success) return NotFound(response.Error);

            await _projectResourcesService.DeleteAsync(id);

            return NoContent();
        }
    }
}
