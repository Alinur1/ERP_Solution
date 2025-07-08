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
                return NotFound("Supplier not found.");
            }
            return Ok(operation_GetSupplierById);
        }

        [HttpPost]
        public async Task<IActionResult> AddSupplier(Supplier supplier)
        {
            var operation_AddSupplier = await _iSuppliers.AddSupplierAsync(supplier);
            if (operation_AddSupplier == null)
            {
                return NotFound("A supplier with this email already exists.");
            }
            return Ok("Supplier added successfully.");
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateSupplier(Supplier supplier)
        {
            var operation_UpdateSupplier = await _iSuppliers.UpdateSupplierAsync(supplier);
            if (operation_UpdateSupplier == null)
            {
                return NotFound("Supplier not found or is deleted.");
            }
            return Ok("Supplier information updated successfully.");
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteSupplier(Supplier supplier)
        {
            var operation_SoftDeleteSupplier = await _iSuppliers.SoftDeleteSupplierAsync(supplier);
            if (operation_SoftDeleteSupplier == null)
            {
                return NotFound("Supplier not found or already deleted.");
            }
            return Ok("Supplier information deleted successfully.");
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteSupplier(Supplier supplier)
        {
            var operation_UndoSoftDeleteSupplier = await _iSuppliers.UndoSoftDeleteSupplierAsync(supplier);
            if (operation_UndoSoftDeleteSupplier == null)
            {
                return NotFound("Unable to restore delete supplier information.");
            }
            return Ok("Supplier information restored successfully.");
        }
    }
}
