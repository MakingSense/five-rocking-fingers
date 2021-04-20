using System.Threading.Tasks;
using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Response;
using FRF.Core.Services;
using FRF.Web.Dtos.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FRF.Web.Controllers
{
    [ApiController]
    [Route("api/resources")]
    [Authorize]
    public class ResourcesController : ControllerBase
    {
        private readonly IResourcesService _resourcesService;
        private readonly IMapper _mapper;

        public ResourcesController(IResourcesService resourcesService, IMapper mapper)
        {
            _resourcesService = resourcesService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var response = await _resourcesService.GetAsync(id);
            if (!response.Success) return NotFound(response.Error);

            var resource = _mapper.Map<ResourceDTO>(response.Value);
            return Ok(resource);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAsync(ResourceInsertDTO resource)
        {
            var resourceToSave = _mapper.Map<Resource>(resource);
            var response = await _resourcesService.SaveAsync(resourceToSave);
            if (!response.Success) return BadRequest(response.Error);

            var savedResource = _mapper.Map<ResourceDTO>(response.Value);
            return Ok(savedResource);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(ResourceUpdateDTO resource)
        {
            var resourceToUpdate = _mapper.Map<Resource>(resource);
            var response = await _resourcesService.UpdateAsync(resourceToUpdate);
            switch (response.Success)
            {
                case false when response.Error.Code == ErrorCodes.ResourceNotExist:
                    return NotFound(response.Error);
                case false when response.Error.Code == ErrorCodes.ResourceNameRepeated:
                    return BadRequest(response.Error);
                default:
                {
                    var updatedResource = _mapper.Map<ResourceDTO>(response.Value);
                    return Ok(updatedResource);
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var response = await _resourcesService.DeleteAsync(id);
            if (!response.Success) return NotFound(response.Error);

            return NoContent();
        }
    }
}