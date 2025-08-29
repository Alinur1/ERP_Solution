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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalesOrderById(int id)
        {
            var operation_GetSalesOrderById = await _iSalesOrders.GetSalesOrderByIdAsync(id);
            if (operation_GetSalesOrderById == null)
            {
                return BadRequest("Sales order not found.");
            }
            return Ok(operation_GetSalesOrderById);
        }

        [HttpGet("by-customer/{customerId}")]
        public async Task<IActionResult> GetSalesOrderByCustomerId(int customerId)
        {
            var operation_GetSalesOrderByCustomerId = await _iSalesOrders.GetSalesOrderByCustomerIdAsync(customerId);
            if (operation_GetSalesOrderByCustomerId == null)
            {
                return BadRequest("Sales order not found for this customer.");
            }
            return Ok(operation_GetSalesOrderByCustomerId);
        }

        [HttpPost]
        public async Task<IActionResult> AddSalesOrder(SalesOrder salesOrder)
        {
            try
            {
                var operation_AddSalesOrder = await _iSalesOrders.AddSalesOrderAsync(salesOrder);
                return Ok("Sales order added successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateSalesOrder(SalesOrder salesOrder)
        {
            try
            {
                var operation_UpdateSalesOrder = await _iSalesOrders.UpdateSalesOrderAsync(salesOrder);
                return Ok("Sales order information updated successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteSalesOrder(SalesOrder salesOrder)
        {
            try
            {
                var operation_SoftDeleteSalesOrder = await _iSalesOrders.SoftDeleteSalesOrderAsync(salesOrder);
                return Ok("Sales order deleted successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteSalesOrder(SalesOrder salesOrder)
        {
            try
            {
                var operation_UndoSoftDeleteSalesOrder = await _iSalesOrders.UndoSoftDeleteSalesOrderAsync(salesOrder);
                return Ok("Deleted sales order restored successfully.");
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("deleted-salesorder")]
        public async Task<IActionResult> GetAllDeletedSalesOrders()
        {
            var operation_GetAllDeletedSalesOrder = await _iSalesOrders.GetAllDeletedSalesOrdersAsync();
            return Ok(operation_GetAllDeletedSalesOrder);
        }
    }
}
