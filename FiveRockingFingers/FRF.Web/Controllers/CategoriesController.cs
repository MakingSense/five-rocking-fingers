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
    public class CategoriesController : BaseApiController<CategoryDTO>
    {
        private readonly IMapper _mapper;
        public ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public override IActionResult GetAll()
        {
            var categories = _categoryService.GetAll();

            var categoriesDto = _mapper.Map<IEnumerable<CategoryDTO>>(categories);

            return Ok(categoriesDto);
        }

        [HttpGet("{id}")]
        public override IActionResult Get(int id)
        {
            var category = _categoryService.Get(id);

            if (category == null)
            {
                return NotFound();
            }

            var categoryDto = _mapper.Map<CategoryDTO>(category);

            return Ok(categoryDto);
        }

        [HttpPost]
        public override IActionResult Save(CategoryDTO categoryDto)
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

            var categoryCreated = _mapper.Map<CategoryDTO>(_categoryService.Save(category));

            return Ok(categoryCreated);
        }

        [HttpPut]
        public override IActionResult Update(CategoryDTO categoryDto)
        {
            if (categoryDto == null)
            {
                return BadRequest("Project object is null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model object");
            }

            var category = _categoryService.Get(categoryDto.Id);

            if (category == null)
            {
                return NotFound();
            }

            _mapper.Map(categoryDto, category);

            var updatedCategory = _mapper.Map<CategoryDTO>(_categoryService.Update(category));

            return Ok(updatedCategory);
        }

        [HttpDelete("{id}")]
        public override IActionResult Delete(int id)
        {
            var category = _categoryService.Get(id);

            if (category == null)
            {
                return NotFound();
            }

            _categoryService.Delete(id);

            return NoContent();
        }
    }
}
