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
            var operation_AddCategory = await _iCategories.AddCategoryAsync(category);
            if(operation_AddCategory == null)
            {
                return NotFound("This category already exists.");
            }
            return Ok("Category added successfully.");
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCategory(Category category)
        {
            var operation_UpdateCategory = await _iCategories.UpdateCategoryAsync(category);
            if(operation_UpdateCategory == null)
            {
                return NotFound("Category not found or category is deleted.");
            }
            return Ok("Category updated successfully.");
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteCategory(Category category)
        {
            var operation_SoftDeleteCategory = await _iCategories.SoftDeleteCategoryAsync(category);
            if (operation_SoftDeleteCategory == null)
            {
                return NotFound("Category not found or already deleted.");
            }
            return Ok("Category deleted successfully.");
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> RestoreDeletedCategory(Category category)
        {
            var operation_RestoreDeletedCategory = await _iCategories.UndoSoftDeleteCategoryAsync(category);
            if (operation_RestoreDeletedCategory == null)
            {
                return NotFound("Unable to restore deleted category.");
            }
            return Ok("Deleted category restored successfully.");
        }
    }
}
