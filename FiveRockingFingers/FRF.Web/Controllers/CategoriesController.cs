using AutoMapper;
using FRF.Core.Services;
using FRF.Web.Dtos.Projects;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRF.Web.Controllers
{
    public class CategoriesController : BaseApiControllerAsync<CategoryDTO>
    {
        private readonly IMapper _mapper;
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public override async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAll();

            var categoriesDto = _mapper.Map<IEnumerable<CategoryDTO>>(categories);

            return Ok(categoriesDto);
        }

        [HttpGet("{id}")]
        public override async Task<IActionResult> Get(int id)
        {
            var category = await _categoryService.Get(id);

            if (category == null)
            {
                return NotFound();
            }

            var categoryDto = _mapper.Map<CategoryDTO>(category);

            return Ok(categoryDto);
        }

        [HttpPost]
        public override async Task<IActionResult> Save(CategoryDTO categoryDto)
        {
            if (categoryDto == null)
            {
                return BadRequest("Project object is null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model object");
            }

            var category = _mapper.Map<FRF.Core.Models.Category>(categoryDto);

            var categoryCreated = _mapper.Map<CategoryDTO>(await _categoryService.Save(category));

            return Ok(categoryCreated);
        }

        [HttpPut]
        public override async Task<IActionResult> Update(CategoryDTO categoryDto)
        {
            if (categoryDto == null)
            {
                return BadRequest("Project object is null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model object");
            }

            var category = await _categoryService.Get(categoryDto.Id);

            if (category == null)
            {
                return NotFound();
            }

            _mapper.Map(categoryDto, category);

            var updatedCategory = _mapper.Map<CategoryDTO>(await _categoryService.Update(category));

            return Ok(updatedCategory);
        }

        [HttpDelete("{id}")]
        public override async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.Get(id);

            if (category == null)
            {
                return NotFound();
            }

            await _categoryService.Delete(id);

            return NoContent();
        }
    }
}
