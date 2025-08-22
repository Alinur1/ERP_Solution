using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErpBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ISuppliers _iSuppliers;
        public SupplierController(ISuppliers iSuppliers)
        {
            _iSuppliers = iSuppliers;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSuppliers()
        {
            var operation_GetAllSuppliers = await _iSuppliers.GetAllSuppliersAsync();
            return Ok(operation_GetAllSuppliers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupplierById(int id)
        {
            var operation_GetSupplierById = await _iSuppliers.GetSupplierByIdAsync(id);
            if (operation_GetSupplierById == null)
            {
                return BadRequest("Supplier not found.");
            }
            return Ok(operation_GetSupplierById);
        }

        [HttpPost]
        public async Task<IActionResult> AddSupplier(Supplier supplier)
        {
            try
            {
                var operation_AddSupplier = await _iSuppliers.AddSupplierAsync(supplier);
                return Ok("Supplier added successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateSupplier(Supplier supplier)
        {
            try
            {
                var operation_UpdateSupplier = await _iSuppliers.UpdateSupplierAsync(supplier);
                return Ok("Supplier information updated successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("delete/{id}")]
        public async Task<IActionResult> SoftDeleteSupplier(int id)
        {
            try
            {
                var operation_SoftDeleteSupplier = await _iSuppliers.SoftDeleteSupplierAsync(id);
                return Ok("Supplier information deleted successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("undo-delete/{id}")]
        public async Task<IActionResult> UndoSoftDeleteSupplier(int id)
        {
            try
            {
                var operation_UndoSoftDeleteSupplier = await _iSuppliers.UndoSoftDeleteSupplierAsync(id);
                return Ok("Supplier information restored successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("deleted-suppliers")]
        public async Task<IActionResult> GetAllDeletedSuppliers()
        {
            var operation_GetAllDeletedSuppliers = await _iSuppliers.GetAllDeletedSuppliersAsync();
            return Ok(operation_GetAllDeletedSuppliers);
        }
    }
}
