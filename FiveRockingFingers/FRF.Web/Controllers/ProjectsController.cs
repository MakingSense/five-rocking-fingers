using System;
using AutoMapper;
using FRF.Core.Services;
using FRF.Web.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FRF.Web.Dtos.Projects;

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
        public async Task<IActionResult> GetAll(string userId)
        {
            var currentUserId = _configuration.GetValue<string>("MockUsers:UserId");

            if (currentUserId != userId) return Unauthorized();

            var projects = await _projectService.GetAllAsync(userId);

            if (projects == null) return StatusCode(204);

            var projectsDto = _mapper.Map<IEnumerable<ProjectDTO>>(projects);
            return Ok(projectsDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            /*var currentUserId = await _userService.GetCurrentUserId();
             var projects = await _projectService.GetAllAsync(currentUserId);
 
             if (projects == null) return StatusCode(204);
 
             var projectsDto = _mapper.Map<IEnumerable<ProjectDTO>>(projects);
             return Ok(projectsDto);*/
            return NotFound();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var project = await _projectService.GetAsync(id);
            if (project == null) return NotFound();

            //TODO: AWS Credentials, Loggin bypassed. Uncomment after do:
            //var currentUserId = await _userService.GetCurrentUserId();
            var currentUserId = _configuration.GetValue<string>("MockUsers:UserId");
            if (!_projectService.IsAuthorized(project, currentUserId)) return Unauthorized();

            var projectDto = _mapper.Map<ProjectDTO>(project);

            return Ok(projectDto);
        }

        [HttpPost]
        public async Task<IActionResult> Save(ProjectUpsertDTO projectDto)
        {
            var project = _mapper.Map<FRF.Core.Models.Project>(projectDto);

            if (project == null) return BadRequest();
            var projectSaved = await _projectService.SaveAsync(project);

            if (projectSaved == null) return BadRequest();

            var projectCreated = _mapper.Map<ProjectDTO>(projectSaved);

            return Ok(projectCreated);
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, ProjectUpsertDTO projectDto)
        {
            var project = await _projectService.GetAsync(id);
            if (project == null) return NotFound();

            /* TODO:Pending AWS Credentials. Login is bypassed![FIVE-6] */
            /*Uncomment this after do.*/
            /* var currentUserId = _userService.GetCurrentUserId();*/
            var currentUserId = _configuration.GetValue<string>("MockUsers:UserId");
            if (!_projectService.IsAuthorized(project, currentUserId)) return Unauthorized();

            _mapper.Map(projectDto, project);
            var updated = await _projectService.UpdateAsync(project);
            var updatedProject = _mapper.Map<ProjectDTO>(updated);

            return Ok(updatedProject);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _projectService.GetAsync(id);
            if (project == null) return NotFound();

            /* TODO:Pending AWS Credentials. Login is bypassed![FIVE-6] */
            /*Uncomment this after do.*/
            /* var currentUserId = _userService.GetCurrentUserId();*/
            var currentUserId = _configuration.GetValue<string>("MockUsers:UserId");
            if (!_projectService.IsAuthorized(project, currentUserId)) return Unauthorized();

            var isDeleted = await _projectService.DeleteAsync(id);
            if (!isDeleted) return NotFound();

            return NoContent();
        }
    }
}