using AutoMapper;
using FRF.Core.Services;
using FRF.Web.Controllers;
using FRF.Web.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FiveRockingFingers.Controllers
{
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

            var projectsDto = _mapper.Map<IEnumerable<ProjectDto>>(projects);
            return Ok(projectsDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            /*
                        var currentUserId = await _userService.GetCurrentUserId();
                        var projects = await _projectService.GetAllAsync(currentUserId);
            
                        if (projects == null) return StatusCode(204);
            
                        var projectsDto = _mapper.Map<IEnumerable<ProjectDto>>(projects);
                        return Ok(projectsDto);*/
            return NotFound();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            //TODO: AWS Credentials, Loggin bypassed. Uncomment after do:
            //var userId = await _userService.GetCurrentUserId();
            var project = await _projectService.GetAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            var projectDto = _mapper.Map<ProjectDto>(project);

            return Ok(projectDto);
        }

        [HttpPost]
        public async Task<IActionResult> Save(ProjectDto projectDto)
        {
            if (projectDto == null)
            {
                return BadRequest("Project object is null");
            }

            var project = _mapper.Map<FRF.Core.Models.Project>(projectDto);
            var projectSaved = await _projectService.SaveAsync(project);

            if (projectSaved == null) return BadRequest();

            var projectCreated = _mapper.Map<ProjectDto>(projectSaved);

            return Ok(projectCreated);
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, ProjectDto projectDto)
        {
            if (projectDto == null || projectDto.Id != id)
            {
                return BadRequest();
            }

            var project = await _projectService.GetAsync(projectDto.Id);

            if (project == null)
            {
                return NotFound();
            }

            _mapper.Map(projectDto, project);

            var updatedProject = _mapper.Map<ProjectDto>(_projectService.UpdateAsync(project));

            return Ok(updatedProject);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _projectService.GetAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            var isDeleted = await _projectService.DeleteAsync(id);
            if (!isDeleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
