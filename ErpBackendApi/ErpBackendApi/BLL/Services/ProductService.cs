using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Helper.LoggerClass;

namespace ErpBackendApi.BLL.Services
{
    public class ProductService : IProducts
    {
        private readonly AppDataContext _context;
        public ProductService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            return await
            (
                from p in _context.products
                join c in _context.categories on p.category_id equals c.id into catGroup
                from c in catGroup.DefaultIfEmpty()
                join s in _context.suppliers on p.supplier_id equals s.id into supGroup
                from s in supGroup.DefaultIfEmpty()
                where p.is_deleted == false
                select new ProductDTO
                {
                    id = p.id,
                    name = p.name,
                    category_id = c != null ? c.id : null,
                    category_name = c != null && c.is_deleted == false ? c.name : "-",
                    supplier_id = s != null ? s.id : null,
                    supplier_company_name = s != null && s.is_deleted == false ? s.company_name : "-",
                    sku = p.sku,
                    description = p.description,
                    unit = p.unit,
                    price = p.price,
                    created_at = p.created_at,
                }
            ).ToListAsync();
        }

        public async Task<ProductDTO> GetProductByIdAsync(int id)
        {
            return await
            (
                from p in _context.products
                join c in _context.categories on p.category_id equals c.id into catGroup
                from c in catGroup.DefaultIfEmpty()
                join s in _context.suppliers on p.supplier_id equals s.id into supGroup
                from s in supGroup.DefaultIfEmpty()
                where p.id == id && p.is_deleted == false
                select new ProductDTO
                {
                    id = p.id,
                    name = p.name,
                    category_id = c != null ? c.id : null,
                    category_name = c != null && c.is_deleted == false ? c.name : "-",
                    supplier_id = s != null ? s.id : null,
                    supplier_company_name = s != null && s.is_deleted == false ? s.company_name : "-",
                    sku = p.sku,
                    description = p.description,
                    unit = p.unit,
                    price = p.price,
                    created_at = p.created_at,
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            var existingCategory = await _context.categories.FirstOrDefaultAsync(c => c.id == product.category_id && c.is_deleted == false);
            var existingSupplier = await _context.suppliers.FirstOrDefaultAsync(s => s.id == product.supplier_id && s.is_deleted == false);
            if (existingCategory == null)
            {
                Logger("Category not found or deleted.#1-AddProductAsync");
                return null;
            }
            if (existingSupplier == null)
            {
                Logger("Supplier not found or deleted.#2-AddProductAsync");
                return null;
            }
            if (!string.IsNullOrEmpty(product.sku))
            {
                var existingProduct = await _context.products.FirstOrDefaultAsync(p => p.sku == product.sku && p.is_deleted == false);
                if (existingProduct != null)
                {
                    Logger("Same Barcode/QR Code cannot be applied on different types of products.");
                    return null;
                }
            }
            product.created_at = DateTime.UtcNow;
            product.is_deleted = false;
            product.deleted_at = null;
            _context.products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            var existingCategory = await _context.categories.FirstOrDefaultAsync(c => c.id == product.category_id && c.is_deleted == false);
            var existingSupplier = await _context.suppliers.FirstOrDefaultAsync(s => s.id == product.supplier_id && s.is_deleted == false);
            var existingProduct = await _context.products.FirstOrDefaultAsync(p => p.id == product.id && p.is_deleted == false);
            var duplicateSku = await _context.products.FirstOrDefaultAsync(p => p.sku == product.sku && p.id != product.id && p.is_deleted == false);

            if (existingCategory == null)
            {
                Logger("Category not found or deleted. #1-UpdateProductAsync");
                return null;
            }

            if (existingSupplier == null)
            {
                Logger("Supplier not found or deleted. #2-UpdateProductAsync");
                return null;
            }

            if (existingProduct == null)
            {
                Logger("Product to update not found or deleted. #3-UpdateProductAsync");
                return null;
            }

            if (duplicateSku != null)
            {
                Logger("Duplicate SKU found on another product. #4-UpdateProductAsync");
                return null;
            }

            existingProduct.name = product.name;
            existingProduct.category_id = product.category_id;
            existingProduct.supplier_id = product.supplier_id;
            existingProduct.sku = product.sku;
            existingProduct.description = product.description;
            existingProduct.unit = product.unit;
            existingProduct.price = product.price;
            _context.products.Update(existingProduct);
            await _context.SaveChangesAsync();            
            return existingProduct;
        }

        public async Task<Product> SoftDeleteProductAsync(Product product)
        {
            var existingProduct = await _context.products.FirstOrDefaultAsync(p => p.id == product.id && p.is_deleted == false);
            if (existingProduct != null)
            {
                existingProduct.is_deleted = true;
                existingProduct.deleted_at = DateTime.UtcNow;
                _context.products.Update(existingProduct);
                await _context.SaveChangesAsync();
            }
            return existingProduct;
        }

        public async Task<Product> UndoSoftDeleteProductAsync(Product product)
        {
            var existingProduct = await _context.products.FirstOrDefaultAsync(p => p.id == product.id && p.is_deleted == true);
            if (existingProduct != null)
            {
                existingProduct.is_deleted = false;
                existingProduct.deleted_at = null;
                _context.products.Update(existingProduct);
                await _context.SaveChangesAsync();
            }
            return existingProduct;
        }
    }
}
