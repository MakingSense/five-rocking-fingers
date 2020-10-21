using AutoMapper;
using FRF.Core.Services;
using FRF.Web.Controllers;
using FRF.Web.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FiveRockingFingers.Controllers
{
    public class ProjectsController : BaseApiController<ProjectDto>
    {
        private readonly IMapper _mapper;
        private readonly IProjectsService _projectService;
        private readonly IUserService _userService;

        public ProjectsController(IProjectsService projectsService, IMapper mapper, IUserService userService)
        {
            this._projectService = projectsService;
            this._mapper = mapper;
            this._userService = userService;

        }


        [HttpGet]
        override public IActionResult GetAll()
        {
            try
            {
                var projects = _projectService.GetAll();

                var projectsDto = _mapper.Map<IEnumerable<ProjectDto>>(projects);

                return Ok(projectsDto);
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAllByUserId(string userId)
        {
            try
            {
                var currentUserId = await _userService.GetCurrentUserId();
                if (currentUserId != userId) return Unauthorized();

                var projects =await _projectService.GetAllByUserId(userId);
                if (projects == null) return NotFound();

                var projectsDto = _mapper.Map<IEnumerable<ProjectDto>>(projects);
                return Ok(projectsDto);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("{id}")]
        override public IActionResult Get(int id)
        {
            try
            {
                var project = _projectService.Get(id);

                if (project == null)
                {
                    return NotFound();
                }

                var projectDto = _mapper.Map<ProjectDto>(project);

                return Ok(projectDto);
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        override public IActionResult Save(ProjectDto projectDto )
        {
            if(projectDto == null)
            {
                return BadRequest("Project object is null");
            }

            if(!ModelState.IsValid)
            {
                return BadRequest("Invalid model object");
            }

            var project = _mapper.Map<FRF.Core.Models.Project>(projectDto);

            try
            {
                var projectCreated = _mapper.Map<ProjectDto>(_projectService.Save(project));

                return Ok(projectCreated);
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal Server Error");
            }
            
        }
        
        [HttpPut]
        override public IActionResult Update(ProjectDto projectDto)
        {
            if(projectDto == null)
            {
                return BadRequest("Project object is null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model object");
            }

            try
            {
                var project = _projectService.Get(projectDto.Id);

                if(project == null)
                {
                    return NotFound();
                }

                _mapper.Map(projectDto, project);

                var updatedProject = _mapper.Map<ProjectDto>(_projectService.Update(project));

                return Ok(updatedProject);
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("{id}")]
        override public IActionResult Delete(int id)
        {
            try
            {
                var project = _projectService.Get(id);

                if(project == null)
                {
                    return NotFound();
                }

                _projectService.Delete(id);

                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
