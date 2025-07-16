using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErpBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesOrderController : ControllerBase
    {
        private readonly ISalesOrders _iSalesOrders;
        public SalesOrderController(ISalesOrders salesOrders)
        {
            _iSalesOrders = salesOrders;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSalesOrder()
        {
            var operation_GetAllSalesOrder = await _iSalesOrders.GetAllSalesOrderAsync();
            return Ok(operation_GetAllSalesOrder);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetSalesOrderById(int id)
        {
            var operation_GetSalesOrderById = await _iSalesOrders.GetSalesOrderByIdAsync(id);
            if (operation_GetSalesOrderById == null)
            {
                return NotFound("Sales order not found.");
            }
            return Ok(operation_GetSalesOrderById);
        }

        [HttpGet("by-customer")]
        public async Task<IActionResult> GetSalesOrderByCustomerId(int customerId)
        {
            var operation_GetSalesOrderByCustomerId = await _iSalesOrders.GetSalesOrderByCustomerIdAsync(customerId);
            if (operation_GetSalesOrderByCustomerId == null)
            {
                return NotFound("Sales order not found for this customer.");
            }
            return Ok(operation_GetSalesOrderByCustomerId);
        }

        [HttpPost]
        public async Task<IActionResult> AddSalesOrder(SalesOrder salesOrder)
        {
            var operation_AddSalesOrder = await _iSalesOrders.AddSalesOrderAsync(salesOrder);
            if (operation_AddSalesOrder == null)
            {
                return NotFound("Unable to add sales order. Order number cannot be same.");
            }
            return Ok("Sales order added successfully.");
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateSalesOrder(SalesOrder salesOrder)
        {
            var operation_UpdateSalesOrder = await _iSalesOrders.UpdateSalesOrderAsync(salesOrder);
            if (operation_UpdateSalesOrder == null)
            {
                return NotFound("Unable to update sales order. Check if it is not deleted.");
            }
            return Ok("Sales order information updated successfully.");
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteSalesOrder(SalesOrder salesOrder)
        {
            var operation_SoftDeleteSalesOrder = await _iSalesOrders.SoftDeleteSalesOrderAsync(salesOrder);
            if (operation_SoftDeleteSalesOrder == null)
            {
                return NotFound("Unable to delete sales order. Sales order not found or already deleted.");
            }
            return Ok("Sales order deleted successfully.");
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteSalesOrder(SalesOrder salesOrder)
        {
            var operation_UndoSoftDeleteSalesOrder = await _iSalesOrders.UndoSoftDeleteSalesOrderAsync(salesOrder);
            if (operation_UndoSoftDeleteSalesOrder == null)
            {
                return NotFound("Unable to restore deleted sales order.");
            }
            return Ok("Deleted sales order restored successfully.");
        }
    }
}
