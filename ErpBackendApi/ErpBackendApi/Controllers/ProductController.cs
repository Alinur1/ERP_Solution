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
                return BadRequest("Product not found.");
            }
            return Ok(operation_GetProductById);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product)
        {
            try
            {
                var operation_AddProduct = await _iProducts.AddProductAsync(product);
                return Ok("Product added successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            try
            {
                var operation_UpdateProduct = await _iProducts.UpdateProductAsync(product);
                return Ok("Product information updated successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteProduct(Product product)
        {
            try
            {
                var operation_SoftDeleteProduct = await _iProducts.SoftDeleteProductAsync(product);
                return Ok("Product information delete successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteProduct(Product product)
        {
            try
            {
                var operation_UndoSoftDeleteProduct = await _iProducts.UndoSoftDeleteProductAsync(product);
                return Ok("Deleted product information restored successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("deleted-products")]
        public async Task<IActionResult> GetAllDeletedProducts()
        {
            var operation_GetAllDeletedProducts = await _iProducts.GetAllDeletedProductsAsync();
            return Ok(operation_GetAllDeletedProducts);
        }
    }
}
