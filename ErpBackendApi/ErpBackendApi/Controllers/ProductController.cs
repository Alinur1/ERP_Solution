using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErpBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProducts _iProducts;
        public ProductController(IProducts iProducts)
        {
            _iProducts = iProducts;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var operation_GetAllProducts = await _iProducts.GetAllProductsAsync();
            return Ok(operation_GetAllProducts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var operation_GetProductById = await _iProducts.GetProductByIdAsync(id);
            if (operation_GetProductById == null)
            {
                return NotFound("Product not found.");
            }
            return Ok(operation_GetProductById);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product)
        {
            var operation_AddProduct = await _iProducts.AddProductAsync(product);
            if (operation_AddProduct == null)
            {
                return NotFound("Same Barcode/QR Code cannot be applied on different types of products.");
            }
            return Ok("Product added successfully.");
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            var operation_UpdateProduct = await _iProducts.UpdateProductAsync(product);
            if (operation_UpdateProduct == null)
            {
                return NotFound("Unable to update product information.");
            }
            return Ok("Product information updated successfully.");
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteProduct(Product product)
        {
            var operation_SoftDeleteProduct = await _iProducts.SoftDeleteProductAsync(product);
            if (operation_SoftDeleteProduct == null)
            {
                return NotFound("Unable to delete the product information.");
            }
            return Ok("Product information delete successfully.");
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteProduct(Product product)
        {
            var operation_UndoSoftDeleteProduct = await _iProducts.UndoSoftDeleteProductAsync(product);
            if (operation_UndoSoftDeleteProduct == null)
            {
                return NotFound("Unable to restore the deleted product information.");
            }
            return Ok("Deleted product information restored successfully.");
        }
    }
}
