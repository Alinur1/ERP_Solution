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
            var POitem = await _items.GetAllPurchaseOrderItemsAsync();
            return Ok(POitem);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseOrderItemById(int id)
        {
            var POitem = await _items.GetPurchaseOrderItemByIdAsync(id);
            if (POitem == null)
            {
                return NotFound("Purchase order item not found.");
            }
            return Ok(POitem);
        }

        [HttpPost]
        public async Task<IActionResult> AddPurchaseOrderItem(PurchaseOrderItem item)
        {
            var POitem = await _items.AddPurchaseOrderItemAsync(item);
            if (POitem == null)
            {
                return NotFound("Unable to add purchase order item or this order already exists.");
            }
            return Ok("Purchase order item added successfully.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePurchaseOrderItem(PurchaseOrderItem item)
        {
            var POitem = await _items.UpdatePurchaseOrderItemAsync(item);
            if (POitem == null)
            {
                return NotFound("Unable to update purchase order item.");
            }
            return Ok("Purchase order item updated successfully.");
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeletePurchaseOrderItem(PurchaseOrderItem item)
        {
            var POitem = await _items.SoftDeletePurchaseOrderItemAsync(item);
            if (POitem == null)
            {
                return NotFound("Unable to delete purchase order item.");
            }
            return Ok("Purchase order item deleted successfully.");
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeletePurchaseOrderItem(PurchaseOrderItem item)
        {
            var POitem = await _items.UndoSoftDeletePurchaseOrderItemAsync(item);
            if (POitem == null)
            {
                return NotFound("Unable to restore deleted purchase order item.");
            }
            return Ok("Purchase order item restored successfully.");
        }
    }
}
