using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErpBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategories _iCategories;
        public CategoryController(ICategories iCategories)
        {
            _iCategories = iCategories;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var operation_GetAllCategories = await _iCategories.GetAllCategoriesAsync();
            return Ok(operation_GetAllCategories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var operation_GetCategoryById = await _iCategories.GetCategoryByIdAsync(id);
            if(operation_GetCategoryById == null)
            {
                return NotFound("Category not found.");
            }
            return Ok(operation_GetCategoryById);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(Category category)
        {
            try
            {
                var operation_AddCategory = await _iCategories.AddCategoryAsync(category);
                return Ok("Category added successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCategory(Category category)
        {
            try
            {
                var operation_UpdateCategory = await _iCategories.UpdateCategoryAsync(category);
                return Ok("Category updated successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteCategory(Category category)
        {
            try
            {
                var operation_SoftDeleteCategory = await _iCategories.SoftDeleteCategoryAsync(category);
                return Ok("Category deleted successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> RestoreDeletedCategory(Category category)
        {
            try
            {
                var operation_RestoreDeletedCategory = await _iCategories.UndoSoftDeleteCategoryAsync(category);
                return Ok("Deleted category restored successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("deleted-categories")]
        public async Task<IActionResult> GetAllDeletedCategory()
        {
            var operation_GetAllDeletedCategory = await _iCategories.GetAllDeletedCategoriesAsync();
            return Ok(operation_GetAllDeletedCategory);
        }
    }
}
