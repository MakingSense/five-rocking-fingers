using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FiveRockingFingers.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FiveRockingFingers.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        public IProjectsService ProjectService { get; set; }
        public ProjectsController(IProjectsService projectsService)
        {
            this.ProjectService = projectsService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(ProjectService.GetAll());
        }
    }
}
