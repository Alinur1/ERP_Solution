using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErpBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoices _iInvoices;
        public InvoiceController(IInvoices iInvoices)
        {
            _iInvoices = iInvoices;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInvoice()
        {
            var operation_GetAllInvoice = await _iInvoices.GetAllInvoiceAsync();
            return Ok(operation_GetAllInvoice);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoiceById(int id)
        {
            var operation_GetInvoiceById = await _iInvoices.GetInvoiceByIdAsync(id);
            if (operation_GetInvoiceById == null)
            {
                return NotFound("Invoice not found.");
            }
            return Ok(operation_GetInvoiceById);
        }

        [HttpGet("by-order/{id}")]
        public async Task<IActionResult> GetInvoiceByOrderId(int orderId)
        {
            var operation_GetInvoiceByOrderId = await _iInvoices.GetInvoiceByOrderIdAsync(orderId);
            if (operation_GetInvoiceByOrderId == null)
            {
                return NotFound("Invoice not found or sales order is deleted.");
            }
            return Ok(operation_GetInvoiceByOrderId);
        }

        [HttpPost]
        public async Task<IActionResult> AddInvoice(Invoice invoice)
        {
            var operation_AddInvoice = await _iInvoices.AddInvoiceAsync(invoice);
            if (operation_AddInvoice == null)
            {
                return NotFound("Unable to add invoice or same sales order cannot be added to an invoice.");
            }
            return Ok("Invoice added successfully.");
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateInvoice(Invoice invoice)
        {
            var operation_UpdateInvoice = await _iInvoices.UpdateInvoiceAsync(invoice);
            if (operation_UpdateInvoice == null)
            {
                return NotFound("Invoice not found or deleted. Unable to update invoice.");
            }
            return Ok("Invoice information updated successfully.");
        }

        [HttpPost("delete")]
        public async Task<IActionResult> SoftDeleteInvoice(Invoice invoice)
        {
            var operation_SoftDeleteInvoice = await _iInvoices.SoftDeleteInvoiceAsync(invoice);
            if (operation_SoftDeleteInvoice == null)
            {
                return NotFound("Invoice not found or deleted. Unable to delete invoice.");
            }
            return Ok("Invoice deleted successfully.");
        }

        [HttpPost("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteInvoice(Invoice invoice)
        {
            var operation_UndoSoftDeleteInvoice = await _iInvoices.UndoSoftDeleteInvoiceAsync(invoice);
            if (operation_UndoSoftDeleteInvoice == null)
            {
                return NotFound("Invoice not found. Unable to restore deleted invoice.");
            }
            return Ok("Deleted invoice restored successfully.");
        }
    }
}
