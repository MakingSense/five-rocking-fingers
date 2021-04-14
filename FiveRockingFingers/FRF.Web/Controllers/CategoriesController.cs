using AutoMapper;
using FRF.Core.Services;
using FRF.Web.Dtos.Categories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FRF.Web.Controllers
{
    [ApiController]
    [Route("api/categories")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICategoriesService _categoriesService;

        public CategoriesController(ICategoriesService categoryService, IMapper mapper)
        {
            _categoriesService = categoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var response = await _categoriesService.GetAllAsync();

            var categoriesDto = _mapper.Map<IEnumerable<CategoryDTO>>(response.Value);

            return Ok(categoriesDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var response = await _categoriesService.GetAsync(id);

            if (!response.Success)
            {
                return NotFound(response.Error);
            }

            var categoryDto = _mapper.Map<CategoryDTO>(response.Value);

            return Ok(categoryDto);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAsync(CategoryUpsertDTO categoryDto)
        {
            var category = _mapper.Map<FRF.Core.Models.Category>(categoryDto);

            var response = await _categoriesService.SaveAsync(category);
            var categoryCreated = _mapper.Map<CategoryDTO>(response.Value);

            return Ok(categoryCreated);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, CategoryUpsertDTO categoryDto)
        {
            var response = await _categoriesService.GetAsync(id);

            if (!response.Success)
            {
                return NotFound(response.Error);
            }

            _mapper.Map(categoryDto, response.Value);

            var category = await _categoriesService.UpdateAsync(response.Value);
            var updatedCategory = _mapper.Map<CategoryDTO>(category.Value);

            return Ok(updatedCategory);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var response = await _categoriesService.GetAsync(id);

            if (!response.Success)
            {
                return NotFound(response.Error);
            }

            await _categoriesService.DeleteAsync(id);

            return NoContent();
        }
    }
}
