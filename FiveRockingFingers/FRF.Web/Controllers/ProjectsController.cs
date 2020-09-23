using FRF.Core.Services;
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
