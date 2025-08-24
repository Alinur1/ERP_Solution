using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

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
                    category_name = c != null && c.is_deleted == false ? c.name : null,
                    supplier_id = s != null ? s.id : null,
                    supplier_company_name = s != null && s.is_deleted == false ? s.company_name : null,
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
                    category_name = c != null && c.is_deleted == false ? c.name : null,
                    supplier_id = s != null ? s.id : null,
                    supplier_company_name = s != null && s.is_deleted == false ? s.company_name : null,
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
            var existingProduct = await _context.products.FirstOrDefaultAsync(p => p.sku == product.sku && p.is_deleted == false);

            if (existingCategory == null)
            {
                Logger("Unable to assign a category for this product. Category not found or deleted.");
                throw new InvalidOperationException("Unable to assign a category for this product. Category not found or deleted.");
            }

            if (existingSupplier == null)
            {
                Logger("Unable to assign a supplier for this product. Supplier not found or deleted.");
                throw new InvalidOperationException("Unable to assign a supplier for this product. Supplier not found or deleted.");
            }

            if (!string.IsNullOrEmpty(product.sku))
            {
                if (existingProduct != null)
                {
                    Logger("Same Barcode/QR Code cannot be applied on different types of products.");
                    throw new InvalidOperationException("Same Barcode/QR Code cannot be applied on different types of products.");
                }
            }

            if (product.price < 0)
            {
                Logger("Product's price cannot be negative.");
                throw new InvalidOperationException("Product's price cannot be negative.");
            }

            if (string.IsNullOrWhiteSpace(product.name) || string.IsNullOrWhiteSpace(product.sku) || string.IsNullOrWhiteSpace(product.unit) || product.price == null)
            {
                Logger("Product name, code, unit and price cannot be empty.");
                throw new InvalidOperationException("Product name, code, unit and price cannot be empty.");
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
                Logger("Unable to update category for this product. Category not found or deleted.");
                throw new InvalidOperationException("Unable to update category for this product. Category not found or deleted.");
            }

            if (existingSupplier == null)
            {
                Logger("Unable to update supplier for this product. Supplier not found or deleted.");
                throw new InvalidOperationException("Unable to update supplier for this product. Supplier not found or deleted.");
            }

            if (existingProduct == null)
            {
                Logger("Product not found or deleted.");
                throw new InvalidOperationException("Product not found or deleted.");
            }

            if (!string.IsNullOrWhiteSpace(product.sku) && existingProduct.sku != product.sku)
            {
                if (duplicateSku != null)
                {
                    Logger("Duplicate SKU found on another product.");
                    throw new InvalidOperationException("Duplicate SKU found on another product.");
                }
            }

            if (product.price < 0)
            {
                Logger("Product's price cannot be negative.");
                throw new InvalidOperationException("Product's price cannot be negative.");
            }

            if (string.IsNullOrWhiteSpace(product.name) || string.IsNullOrWhiteSpace(product.sku) || string.IsNullOrWhiteSpace(product.unit) || product.price == null)
            {
                Logger("Product name, code, unit and price cannot be empty.");
                throw new InvalidOperationException("Product name, code, unit and price cannot be empty.");
            }

            existingProduct.name = product.name;
            existingProduct.category_id = product.category_id;
            existingProduct.supplier_id = product.supplier_id;
            existingProduct.sku = product.sku;
            existingProduct.description = product.description;
            existingProduct.unit = product.unit;
            existingProduct.price = product.price;
            await _context.SaveChangesAsync();            
            return existingProduct;
        }

        public async Task<Product> SoftDeleteProductAsync(Product product)
        {
            var existingProduct = await _context.products.FirstOrDefaultAsync(p => p.id == product.id && p.is_deleted == false);

            if (existingProduct == null)
            {
                Logger("Unable to delete product. Product not found.");
                throw new InvalidOperationException("Unable to delete product. Product not found.");
            }

            existingProduct.is_deleted = true;
            existingProduct.deleted_at = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existingProduct;
        }

        public async Task<Product> UndoSoftDeleteProductAsync(Product product)
        {
            var existingProduct = await _context.products.FirstOrDefaultAsync(p => p.id == product.id && p.is_deleted == true);

            if (existingProduct == null)
            {
                Logger("Unable to restore deleted product. Deleted product not found.");
                throw new InvalidOperationException("Unable to restore deleted product. Deleted Product not found.");
            }

            existingProduct.is_deleted = false;
            existingProduct.deleted_at = null;
            await _context.SaveChangesAsync();
            return existingProduct;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllDeletedProductsAsync()
        {
            return await
            (
                from p in _context.products
                join c in _context.categories on p.category_id equals c.id into catGroup
                from c in catGroup.DefaultIfEmpty()
                join s in _context.suppliers on p.supplier_id equals s.id into supGroup
                from s in supGroup.DefaultIfEmpty()
                where p.is_deleted == true
                select new ProductDTO
                {
                    id = p.id,
                    name = p.name,
                    category_id = c != null ? c.id : null,
                    category_name = c != null && c.is_deleted == false ? c.name : null,
                    supplier_id = s != null ? s.id : null,
                    supplier_company_name = s != null && s.is_deleted == false ? s.company_name : null,
                    sku = p.sku,
                    description = p.description,
                    unit = p.unit,
                    price = p.price,
                    created_at = p.created_at,
                }
            ).ToListAsync();
        }
    }
}
