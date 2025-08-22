using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.Services
{
    public class CategoryService : ICategories
    {
        private readonly AppDataContext _context;
        public CategoryService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.categories
                .Where(c => c.is_deleted == false)
                .Select(c => new Category
                {
                    id = c.id,
                    name = c.name,
                    description = c.description
                }).ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _context.categories
                .Where(c => c.id == id && c.is_deleted == false)
                .Select(c => new Category
                {
                    id = c.id,
                    name = c.name,
                    description = c.description
                }).FirstOrDefaultAsync();
        }

        public async Task<Category> AddCategoryAsync(Category category)
        {
            var existingCategory = await _context.categories.FirstOrDefaultAsync(c => c.name == category.name && c.is_deleted == false);
            if (existingCategory != null)
            {
                Logger("Tried to create same category.");
                throw new InvalidOperationException("Tried to create same category.");
            }
            if (string.IsNullOrWhiteSpace(category.name))
            {
                Logger("Category name cannot be empty.");
                throw new InvalidOperationException("Category name cannot be empty.");
            }
            category.is_deleted = false;
            category.deleted_at = null;
            _context.categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            var existingCategory = await _context.categories.FirstOrDefaultAsync(c => c.id == category.id && c.is_deleted == false);
            if (existingCategory == null)
            {
                Logger("Category not found.");
                throw new InvalidOperationException("Category not found.");
            }
            if (!string.Equals(existingCategory.name, category.name, StringComparison.OrdinalIgnoreCase))
            {
                var existingCategoryName = await _context.categories.FirstOrDefaultAsync(c => c.name == category.name && c.id != category.id && c.is_deleted == false);
                if (existingCategoryName != null)
                {
                    Logger("This category name already exists.");
                    throw new InvalidOperationException("This category name already exists.");
                }               
            }
            if (string.IsNullOrWhiteSpace(category.name))
            {
                Logger("Category name cannot be empty.");
                throw new InvalidOperationException("Category name cannot be empty.");
            }
            existingCategory.name = category.name;
            existingCategory.description = category.description;
            await _context.SaveChangesAsync();
            return existingCategory;
        }

        public async Task<Category> SoftDeleteCategoryAsync(Category category)
        {
            var existingCategory = await _context.categories.FirstOrDefaultAsync(c => c.id == category.id && c.is_deleted == false);
            if (existingCategory == null)
            {
                Logger("Unable to delete the category.");
                throw new InvalidOperationException("Unable to delete the category.");
            }
            existingCategory.is_deleted = true;
            existingCategory.deleted_at = DateTime.UtcNow;
            _context.categories.Update(existingCategory);
            await _context.SaveChangesAsync();
            return existingCategory;
        }

        public async Task<Category> UndoSoftDeleteCategoryAsync(Category category)
        {
            var deletedCategory = await _context.categories.FirstOrDefaultAsync(c => c.id == category.id && c.is_deleted == true);
            if (deletedCategory == null)
            {
                Logger("Unable to restore deleted category.");
                throw new InvalidOperationException("Unable to restore deleted category.");
            }
            deletedCategory.is_deleted = false;
            deletedCategory.deleted_at = null;
            _context.categories.Update(deletedCategory);
            await _context.SaveChangesAsync();
            return deletedCategory;
        }

        public async Task<IEnumerable<Category>> GetAllDeletedCategoriesAsync()
        {
            return await _context.categories
                .Where(c => c.is_deleted == true)
                .Select(c => new Category
                {
                    id = c.id,
                    name = c.name,
                    description = c.description,
                    deleted_at = c.deleted_at,
                }).ToListAsync();
        }
    }
}
