﻿using AutoMapper;
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

                var projectsDto = mapper.Map<IEnumerable<ProjectDto>>(projects);

                return Ok(projectsDto);
            }
            catch (Exception e)
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

            var project = mapper.Map<FRF.Core.Models.Project>(projectDto);

            try
            {
                var projectCreated = mapper.Map<ProjectDto>(ProjectService.Save(project));

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
                var project = ProjectService.Get(projectDto.Id);

                if(project == null)
                {
                    return NotFound();
                }

                mapper.Map(projectDto, project);

                var updatedProject = mapper.Map<ProjectDto>(ProjectService.Update(project));

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
                var project = ProjectService.Get(id);

                if(project == null)
                {
                    return NotFound();
                }

                ProjectService.Delete(id);

                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
