using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErpBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderItemController : ControllerBase
    {
        private readonly IPurchaseOrderItems _items;
        public PurchaseOrderItemController(IPurchaseOrderItems items)
        {
            _items = items;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPurchaseOrderItem()
        {
            var operation_GetAllPurchaseOrderItem = await _items.GetAllPurchaseOrderItemsAsync();
            return Ok(operation_GetAllPurchaseOrderItem);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseOrderItemById(int id)
        {
            var operation_GetPurchaseOrderItemById = await _items.GetPurchaseOrderItemByIdAsync(id);
            if (operation_GetPurchaseOrderItemById == null)
            {
                return BadRequest("Purchase order item not found.");
            }
            return Ok(operation_GetPurchaseOrderItemById);
        }

        [HttpGet("purchase-order/{orderId}")]
        public async Task<IActionResult> GetPurchaseOrderItemByPurchaseOrderId(int orderId)
        {
            var operation_GetPurchaseOrderItemByPurchaseOrderId = await _items.GetPurchaseOrderItemByPurchaseOrderIdAsync(orderId);
            if (operation_GetPurchaseOrderItemByPurchaseOrderId == null)
            {
                return BadRequest("Purchase order item not found.");
            }
            return Ok(operation_GetPurchaseOrderItemByPurchaseOrderId);
        }

        [HttpPost]
        public async Task<IActionResult> AddPurchaseOrderItem(IEnumerable<PurchaseOrderItem> items)
        {
            try
            {
                var operation_AddPurchaseOrderItem = await _items.AddPurchaseOrderItemAsync(items);
                return Ok("Purchase order item added successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdatePurchaseOrderItem(PurchaseOrderItem item)
        {
            try
            {
                var operation_UpdatePurchaseOrderItem = await _items.UpdatePurchaseOrderItemAsync(item);
                return Ok("Purchase order item updated successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeletePurchaseOrderItem(PurchaseOrderItem item)
        {
            try
            {
                var operation_SoftDeletePurchaseOrderItem = await _items.SoftDeletePurchaseOrderItemAsync(item);
                return Ok("Purchase order item deleted successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeletePurchaseOrderItem(PurchaseOrderItem item)
        {
            try
            {
                var operation_UndoSoftDeletePurchaseOrderItem = await _items.UndoSoftDeletePurchaseOrderItemAsync(item);
                return Ok("Purchase order item restored successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("deleted-purchase-order-items")]
        public async Task<IActionResult> GetAllDeletedPurchaseOrderItems()
        {
            var operation_GetAllDeletedPurchaseOrderItems = await _items.GetAllDeletedPurchaseOrderItemsAsync();
            return Ok(operation_GetAllDeletedPurchaseOrderItems);
        }
    }
}
