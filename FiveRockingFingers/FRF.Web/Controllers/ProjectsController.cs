using System;
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
    [Route("api/[controller]/[action]")]
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
            var projects = await _projectService.GetAllAsync(currentUserId);

            var projectsDto = _mapper.Map<IEnumerable<ProjectDTO>>(projects);
            return Ok(projectsDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var project = await _projectService.GetAsync(id);
            if (project == null) return NotFound();

            var projectDto = _mapper.Map<ProjectDTO>(project);
            return Ok(projectDto);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAsync(ProjectUpsertDTO projectDto)
        {
            var currentUserId = await _userService.GetCurrentUserIdAsync();

            projectDto.Users.Add(new UserProfileUpsertDTO() {UserId = currentUserId});

            var project = _mapper.Map<Project>(projectDto);
            if (project == null) return BadRequest();

            var projectSaved = await _projectService.SaveAsync(project);
            if (projectSaved == null) return BadRequest();

            var projectCreated = _mapper.Map<ProjectDTO>(projectSaved);
            return Ok(projectCreated);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(int id, ProjectUpsertDTO projectDto)
        {
            var project = await _projectService.GetAsync(id);
            if (project == null) return NotFound();

            //To improve
            if (!projectDto.Users.Any()) return BadRequest();

            _mapper.Map(projectDto, project);
            var updated = await _projectService.UpdateAsync(project);
            if (updated == null) return BadRequest();

            var updatedProject = _mapper.Map<ProjectDTO>(updated);
            return Ok(updatedProject);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var project = await _projectService.GetAsync(id);
            if (project == null) return NotFound();

            var isDeleted = await _projectService.DeleteAsync(id);
            if (!isDeleted) return NotFound();

            return NoContent();
        }
    }
}