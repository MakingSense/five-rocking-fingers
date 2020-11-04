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
    public class CategoriesController : BaseApiController<CategoryDTO>
    {
        private readonly IMapper _mapper;
        private readonly ICategoriesService _categoriesService;

        public CategoriesController(ICategoriesService categoryService, IMapper mapper)
        {
            _categoriesService = categoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public override async Task<IActionResult> GetAllAsync()
        {
            var categories = await _categoriesService.GetAllAsync();

            var categoriesDto = _mapper.Map<IEnumerable<CategoryDTO>>(categories);

            return Ok(categoriesDto);
        }

        [HttpGet("{id}")]
        public override async Task<IActionResult> GetAsync(int id)
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
        public override async Task<IActionResult> SaveAsync(CategoryDTO categoryDto)
        {
            var category = _mapper.Map<FRF.Core.Models.Category>(categoryDto);

            var categoryCreated = _mapper.Map<CategoryDTO>(await _categoriesService.SaveAsync(category));

            return Ok(categoryCreated);
        }

        [HttpPut("{id}")]
        public override async Task<IActionResult> UpdateAsync(int id, CategoryDTO categoryDto)
        {
            if(id != categoryDto.Id)
            {
                return BadRequest();
            }

            var category = await _categoriesService.GetAsync(categoryDto.Id);

            if (category == null)
            {
                return NotFound();
            }

            _mapper.Map(categoryDto, category);

            var updatedCategory = _mapper.Map<CategoryDTO>(await _categoriesService.UpdateAsync(category));

            return Ok(updatedCategory);
        }

        [HttpDelete("{id}")]
        public override async Task<IActionResult> DeleteAsync(int id)
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
