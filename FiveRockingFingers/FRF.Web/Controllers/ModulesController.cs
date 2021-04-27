using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Response;
using FRF.Core.Services;
using FRF.Web.Dtos.Modules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FRF.Web.Controllers
{
    [ApiController]
    [Route("api/modules")]
    [Authorize]
    public class ModulesController : ControllerBase
    {
        private readonly IModulesService _modulesService;
        private readonly IMapper _mapper;

        public ModulesController(IModulesService modulesService, IMapper mapper)
        {
            _modulesService = modulesService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var response = await _modulesService.GetAsync(id);
            if (!response.Success) return NotFound(response.Error);

            var module = _mapper.Map<ModuleDTO>(response.Value);
            return Ok(module);
        }

        [HttpGet("~/api/categories/{categoryId}/modules")]
        public async Task<IActionResult> GetAllByCategoryIdAsync(int categoryId)
        {
            var response = await _modulesService.GetAllByCategoryIdAsync(categoryId);
            if (!response.Success) return NotFound(response.Error);

            var modules = _mapper.Map<IList<ModuleDTO>>(response.Value);
            return Ok(modules);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAsync(ModuleUpsertDTO module)
        {
            var moduleToSave = _mapper.Map<Module>(module);
            var response = await _modulesService.SaveAsync(moduleToSave);
            if (!response.Success) return BadRequest(response.Error);

            var savedModule = _mapper.Map<ModuleDTO>(response.Value);
            return Ok(savedModule);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, ModuleUpsertDTO module)
        {
            var moduleToUpdate = _mapper.Map<Module>(module);
            moduleToUpdate.Id = id;
            var response = await _modulesService.UpdateAsync(moduleToUpdate);
            switch (response.Success)
            {
                case false when response.Error.Code == ErrorCodes.ModuleNotExist:
                    return NotFound(response.Error);
                default:
                {
                    var updatedModule = _mapper.Map<ModuleDTO>(response.Value);
                    return Ok(updatedModule);
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var response = await _modulesService.DeleteAsync(id);
            if (!response.Success) return NotFound(response.Error);

            return NoContent();
        }
    }
}
