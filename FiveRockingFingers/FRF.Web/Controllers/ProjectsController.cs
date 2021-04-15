using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Services;
using FRF.Web.Dtos;
using FRF.Web.Dtos.Projects;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FiveRockingFingers.Controllers
{
    [ApiController]
    [Route("api/projects")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProjectsService _projectService;
        private readonly IUserService _userService;

        public ProjectsController(IProjectsService projectsService, IMapper mapper, IUserService userService)
        {
            _projectService = projectsService;
            _mapper = mapper;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var currentUserId = await _userService.GetCurrentUserIdAsync();
            var response = await _projectService.GetAllAsync(currentUserId.Value);

            var projectsDto = _mapper.Map<IEnumerable<ProjectDTO>>(response.Value);
            return Ok(projectsDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var response = await _projectService.GetAsync(id);
            if (!response.Success) return NotFound();

            var projectDto = _mapper.Map<ProjectDTO>(response.Value);
            return Ok(projectDto);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAsync(ProjectUpsertDTO projectDto)
        {
            var currentUserId = await _userService.GetCurrentUserIdAsync();
            if (!currentUserId.Success)
                return BadRequest(currentUserId.Error);

            projectDto.Users.Add(new UserProfileUpsertDTO() {UserId = currentUserId.Value});
            var project = _mapper.Map<Project>(projectDto);

            var projectSaved = await _projectService.SaveAsync(project);
            if (!projectSaved.Success)
                return BadRequest(projectSaved.Error);

            var projectCreated = _mapper.Map<ProjectDTO>(projectSaved.Value);
            return Ok(projectCreated);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, ProjectUpsertDTO projectDto)
        {
            var response = await _projectService.GetAsync(id);
            if (!response.Success) return NotFound(response.Error);

            _mapper.Map(projectDto, response.Value);
            var updated = await _projectService.UpdateAsync(response.Value);
            if (!updated.Success) return BadRequest(updated.Error);

            var updatedProject = _mapper.Map<ProjectDTO>(updated.Value);
            return Ok(updatedProject);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var project = await _projectService.GetAsync(id);
            if (!project.Success) return NotFound(project.Error);

            var isDeleted = await _projectService.DeleteAsync(id);
            if (!isDeleted.Success) return NotFound(isDeleted.Error);

            return NoContent();
        }
    }
}