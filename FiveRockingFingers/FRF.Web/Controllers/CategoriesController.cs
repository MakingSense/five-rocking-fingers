using AutoMapper;
using FRF.Core.Services;
using FRF.Web.Dtos.Categories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRF.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
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
            var categories = await _categoriesService.GetAllAsync();

            var categoriesDto = _mapper.Map<IEnumerable<CategoryDTO>>(categories);

            return Ok(categoriesDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var category = await _categoriesService.GetAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            var categoryDto = _mapper.Map<CategoryDTO>(category);

            return Ok(categoryDto);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAsync(CategoryUpsertDTO categoryDto)
        {
            var category = _mapper.Map<FRF.Core.Models.Category>(categoryDto);

            var categoryCreated = _mapper.Map<CategoryDTO>(await _categoriesService.SaveAsync(category));

            return Ok(categoryCreated);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, CategoryUpsertDTO categoryDto)
        {
            var category = await _categoriesService.GetAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            _mapper.Map(categoryDto, category);

            var updatedCategory = _mapper.Map<CategoryDTO>(await _categoriesService.UpdateAsync(category));

            return Ok(updatedCategory);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var category = await _categoriesService.GetAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            await _categoriesService.DeleteAsync(id);

            return NoContent();
        }
    }
}
