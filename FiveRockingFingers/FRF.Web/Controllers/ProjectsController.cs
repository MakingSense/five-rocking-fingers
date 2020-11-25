using System;
using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Services;
using FRF.Web.Dtos;
using FRF.Web.Dtos.Projects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FiveRockingFingers.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    // [Authorize] 
    public class ProjectsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProjectsService _projectService;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public ProjectsController(IProjectsService projectsService, IMapper mapper, IUserService userService,
            IConfiguration configuration)
        {
            _projectService = projectsService;
            _mapper = mapper;
            _userService = userService;
            _configuration = configuration;
        }

        //TODO: AWS Credentials, Loggin bypassed.Delete this method and Uncomment GetAll() after do:
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAllAsync(Guid userId)
        {   //TODO: AWS Credentials, Loggin bypassed. Use the method argument Guid userId instead of GetValue.
            var currentUserId = Guid.Parse(_configuration.GetValue<string>("MockUsers:UserId"));
            
            if (currentUserId != userId) return Unauthorized();

            var projects = await _projectService.GetAllAsync(userId);

            var projectsDto = _mapper.Map<IEnumerable<ProjectDTO>>(projects);
            return Ok(projectsDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            // TODO: AWS Credentials, Loggin bypassed.Uncomment after do:
            //var currentUserId = await _userService.GetCurrentUserId();
            var currentUserId = new Guid("9e9df404-3060-4904-bcb8-020f4344c5f0");
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
            if (project == null) return StatusCode(204);

            var isDeleted = await _projectService.DeleteAsync(id);

            return !isDeleted ? StatusCode(204) : Ok();
        }
    }
}