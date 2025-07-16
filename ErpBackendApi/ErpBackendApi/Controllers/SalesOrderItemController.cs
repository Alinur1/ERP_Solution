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
                return NotFound("Sales order item not found.");
            }
            return Ok(operation_GetSalesOrderItemById);
        }

        [HttpGet("sales-order/{id}")]
        public async Task<IActionResult> GetSalesOrderItemBySalesOrderId(int orderId)
        {
            var operation_GetSalesOrderItemBySalesOrderId = await _iSalesOrderItem.GetSalesOrderItemBySalesOrderIdAsync(orderId);
            if (operation_GetSalesOrderItemBySalesOrderId == null)
            {
                return NotFound("Sales order not found.");
            }
            return Ok(operation_GetSalesOrderItemBySalesOrderId);
        }

        [HttpPost]
        public async Task<IActionResult> AddSalesOrderItem(SalesOrderItem item)
        {
            var operation_AddSalesOrderItem = await _iSalesOrderItem.AddSalesOrderItemAsync(item);
            if (operation_AddSalesOrderItem == null)
            {
                return NotFound("Unable to add sales order item / Same order number for an item is not allowed.");
            }
            return Ok("Sales order item added successfully.");
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateSalesOrderItem(SalesOrderItem item)
        {
            var operation_UpdateSalesOrderItem = await _iSalesOrderItem.UpdateSalesOrderItemAsync(item);
            if (operation_UpdateSalesOrderItem == null)
            {
                return NotFound("Unable to update sales order item.");
            }
            return Ok("Sales order item information updated successfully.");
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteSalesOrderItem(SalesOrderItem item)
        {
            var operation_SoftDeleteSalesOrderItem = await _iSalesOrderItem.SoftDeleteSalesOrderItemAsync(item);
            if (operation_SoftDeleteSalesOrderItem == null)
            {
                return NotFound("Unable to delete sales order item.");
            }
            return Ok("Sales order item information deleted successfully.");
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteSalesOrderItem(SalesOrderItem item)
        {
            var operation_UndoSoftDeleteSalesOrderItem = await _iSalesOrderItem.UndoSoftDeleteSalesOrderItemAsync(item);
            if (operation_UndoSoftDeleteSalesOrderItem == null)
            {
                return NotFound("Unable to restore sales order item.");
            }
            return Ok("Deleted sales order item information restored successfully.");
        }
    }
}
