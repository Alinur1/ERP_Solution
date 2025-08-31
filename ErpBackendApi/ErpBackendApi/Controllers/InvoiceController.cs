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
                return BadRequest("Invoice not found.");
            }
            return Ok(operation_GetInvoiceById);
        }

        [HttpGet("by-order/{id}")]
        public async Task<IActionResult> GetInvoiceByOrderId(int orderId)
        {
            try
            {
                var operation_GetInvoiceByOrderId = await _iInvoices.GetInvoiceByOrderIdAsync(orderId);
                return Ok(operation_GetInvoiceByOrderId);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddInvoice(Invoice invoice)
        {
            try
            {
                var operation_AddInvoice = await _iInvoices.AddInvoiceAsync(invoice);
                return Ok("Invoice added successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateInvoice(Invoice invoice)
        {
            try
            {
                var operation_UpdateInvoice = await _iInvoices.UpdateInvoiceAsync(invoice);
                return Ok("Invoice information updated successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteInvoice(Invoice invoice)
        {
            try
            {
                var operation_SoftDeleteInvoice = await _iInvoices.SoftDeleteInvoiceAsync(invoice);
                return Ok("Invoice deleted successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteInvoice(Invoice invoice)
        {
            try
            {
                var operation_UndoSoftDeleteInvoice = await _iInvoices.UndoSoftDeleteInvoiceAsync(invoice);
                return Ok("Deleted invoice restored successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
