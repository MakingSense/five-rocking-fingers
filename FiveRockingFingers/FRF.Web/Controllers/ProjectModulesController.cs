using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Response;
using FRF.Core.Services;
using FRF.Web.Dtos.ProjectModules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRF.Web.Controllers
{
    [ApiController]
    [Route("api/project-modules")]
    [Authorize]
    public class ProjectModulesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProjectModulesService _projectModulesService;

        public ProjectModulesController(IProjectModulesService projectModulesService, IMapper mapper)
        {
            _projectModulesService = projectModulesService;
            _mapper = mapper;
        }

        [HttpGet("~/api/projects/{projectId}/project-modules")]
        public async Task<IActionResult> GetByProjectIdAsync(int projectId)
        {
            var projectModules = await _projectModulesService.GetByProjectIdAsync(projectId);

            if (!projectModules.Success) return NotFound(projectModules.Error);

            var projectModulesDto = _mapper.Map<IEnumerable<ProjectModuleDTO>>(projectModules.Value);

            return Ok(projectModulesDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var response = await _projectModulesService.GetAsync(id);

            if (!response.Success) return NotFound(response.Error);

            var projectModuleDto = _mapper.Map<ProjectModuleDTO>(response.Value);

            return Ok(projectModuleDto);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAsync(ProjectModuleUpsertDTO projectModuleDto)
        {
            var projectModule = _mapper.Map<FRF.Core.Models.ProjectModule>(projectModuleDto);

            var response = await _projectModulesService.SaveAsync(projectModule);

            if (!response.Success && (response.Error.Code == ErrorCodes.ProjectNotExists || response.Error.Code == ErrorCodes.ModuleNotExists))
                return NotFound(response.Error);

            if (!response.Success && (response.Error.Code == ErrorCodes.ModuleCostInvalid))
                return BadRequest(response.Error);

            var projectModuleCreated = _mapper.Map<ProjectModuleDTO>(response.Value);

            return Ok(projectModuleCreated);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, ProjectModuleUpsertDTO projectModuleDto)
        {

            var projectModule = _mapper.Map<ProjectModule>(projectModuleDto);
            projectModule.Id = id;

            var response = await _projectModulesService.UpdateAsync(projectModule);

            if (!response.Success && (response.Error.Code == ErrorCodes.ProjectNotExists || response.Error.Code == ErrorCodes.ModuleNotExists))
                return NotFound(response.Error);

            if (!response.Success && (response.Error.Code == ErrorCodes.ModuleCostInvalid))
                return BadRequest(response.Error);

            var updatedProjectModule = _mapper.Map<ProjectModuleDTO>(response.Value);

            return Ok(updatedProjectModule);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var response = await _projectModulesService.DeleteAsync(id);

            if (!response.Success) return NotFound(response.Error);

            return NoContent();
        }
    }
}
