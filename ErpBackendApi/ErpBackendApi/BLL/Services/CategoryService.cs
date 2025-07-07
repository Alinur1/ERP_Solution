using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Helper.LoggerClass;

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
                return null;
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
            if (existingCategory != null)
            {
                existingCategory.name = category.name;
                existingCategory.description = category.description;
                _context.categories.Update(existingCategory);
                await _context.SaveChangesAsync();
            }
            return existingCategory;
        }

        public async Task<Category> SoftDeleteCategoryAsync(Category category)
        {
            var existingCategory = await _context.categories.FirstOrDefaultAsync(c => c.id == category.id && c.is_deleted == false);
            if (existingCategory != null)
            {
                existingCategory.is_deleted = true;
                existingCategory.deleted_at = DateTime.UtcNow;
                _context.categories.Update(existingCategory);
                await _context.SaveChangesAsync();
            }
            return existingCategory;
        }

        public async Task<Category> UndoSoftDeleteCategoryAsync(Category category)
        {
            var deletedCategory = await _context.categories.FirstOrDefaultAsync(c => c.id == category.id && c.is_deleted == true);
            if (deletedCategory != null)
            {
                deletedCategory.is_deleted = false;
                deletedCategory.deleted_at = null;
                _context.categories.Update(deletedCategory);
                await _context.SaveChangesAsync();
            }
            return deletedCategory;
        }
    }
}
