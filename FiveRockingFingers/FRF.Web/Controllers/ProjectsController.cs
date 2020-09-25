using AutoMapper;
using FRF.Core.Services;
using FRF.Web.Controllers;
using FRF.Web.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace FiveRockingFingers.Controllers
{
    public class ProjectsController : BaseApiController<ProjectDto>
    {
        private readonly IMapper mapper;

        public IProjectsService ProjectService { get; set; }
        public ProjectsController(IProjectsService projectsService, IMapper mapper)
        {
            this.ProjectService = projectsService;
            this.mapper = mapper;
        }


        [HttpGet]
        override public IActionResult GetAll()
        {
            try
            {
                var projects = ProjectService.GetAll();

                Console.WriteLine("LLegue hasta acá");

                var projectsDto = mapper.Map<IEnumerable<ProjectDto>>(projects);

                return Ok(projectsDto);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
        
        [HttpGet("{id}")]
        override public IActionResult Get(int id)
        {
            try
            {
                var project = ProjectService.Get(id);

                if (project == null)
                {
                    return NotFound();
                }

                var projectDto = mapper.Map<ProjectDto>(project);

                return Ok(projectDto);
            }
            catch
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

            var project = mapper.Map<FRF.Core.Models.Project>(projectDto);

            try
            {
                ProjectService.Save(project);

                return CreatedAtRoute("DefaultApi", new { id = project.Id }, project);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
            
        }
        
        [HttpPut("{id}")]
        override public IActionResult Update(int id, ProjectDto projectDto)
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
                var project = ProjectService.Get(id);

                if(project == null)
                {
                    return NotFound();
                }

                mapper.Map(projectDto, project);

                ProjectService.Update(project);

                return NoContent();
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("{id}")]
        override public IActionResult Delete(int id)
        {
            try
            {
                var project = ProjectService.Get(id);

                if(project == null)
                {
                    return NotFound();
                }

                ProjectService.Delete(id);

                return NoContent();
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
