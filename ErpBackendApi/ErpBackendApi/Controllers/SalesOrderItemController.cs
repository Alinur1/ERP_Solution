using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErpBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesOrderItemController : ControllerBase
    {
        private readonly ISalesOrderItems _iSalesOrderItem;
        public SalesOrderItemController(ISalesOrderItems salesOrderItem)
        {
            _iSalesOrderItem = salesOrderItem;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSalesOrderItems()
        {
            var operation_SalesOrderItems = await _iSalesOrderItem.GetAllSalesOrderItemAsync();
            return Ok(operation_SalesOrderItems);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalesOrderItemById(int id)
        {
            var operation_GetSalesOrderItemById = await _iSalesOrderItem.GetSalesOrderItemByIdAsync(id);
            if (operation_GetSalesOrderItemById == null)
            {
                return BadRequest("Sales order item not found.");
            }
            return Ok(operation_GetSalesOrderItemById);
        }

        [HttpGet("sales-order/{orderId}")]
        public async Task<IActionResult> GetSalesOrderItemBySalesOrderId(int orderId)
        {
            var operation_GetSalesOrderItemBySalesOrderId = await _iSalesOrderItem.GetSalesOrderItemBySalesOrderIdAsync(orderId);
            if (operation_GetSalesOrderItemBySalesOrderId == null)
            {
                return BadRequest("Sales order not found.");
            }
            return Ok(operation_GetSalesOrderItemBySalesOrderId);
        }

        [HttpPost]
        public async Task<IActionResult> AddSalesOrderItem(SalesOrderItem item)
        {
            try
            {
                var operation_AddSalesOrderItem = await _iSalesOrderItem.AddSalesOrderItemAsync(item);
                return Ok("Sales order item added successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateSalesOrderItem(SalesOrderItem item)
        {
            try
            {
                var operation_UpdateSalesOrderItem = await _iSalesOrderItem.UpdateSalesOrderItemAsync(item);
                return Ok("Sales order item information updated successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteSalesOrderItem(SalesOrderItem item)
        {
            try
            {
                var operation_SoftDeleteSalesOrderItem = await _iSalesOrderItem.SoftDeleteSalesOrderItemAsync(item);
                return Ok("Sales order item information deleted successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteSalesOrderItem(SalesOrderItem item)
        {
            try
            {
                var operation_UndoSoftDeleteSalesOrderItem = await _iSalesOrderItem.UndoSoftDeleteSalesOrderItemAsync(item);
                return Ok("Deleted sales order item information restored successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("deleted-salesorderitem")]
        public async Task<IActionResult> GetAllDeletedSalesOrderItems()
        {
            var operation_GetAllDeletedSalesOrderItems = await _iSalesOrderItem.GetAllDeletedSalesOrderItemAsync();
            return Ok(operation_GetAllDeletedSalesOrderItems);
        }
    }
}
