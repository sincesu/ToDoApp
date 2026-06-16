using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDo.Application.Abstractions;
using ToDo.Application.DTOs.Category;

namespace ToDo.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _categoryService.GetAllCategoriesAsync());
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _categoryService.GetCategoryByIdAsync(id));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add(CategorySaveDto dto)
        {
            await _categoryService.AddCategoryAsync(dto);
            return StatusCode(201, $"{dto.name} has created in category list");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update/{id:Guid}")]
        public async Task<IActionResult> Update(Guid id, CategorySaveDto dto)
        {
            await _categoryService.UpdateCategoryAsync(id, dto);
            return StatusCode(204, $"{id} has been updated in category list");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return NoContent();
        }
    }
}
