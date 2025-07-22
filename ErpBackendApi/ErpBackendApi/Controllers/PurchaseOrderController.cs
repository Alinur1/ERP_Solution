using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErpBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IPurchaseOrders _iPurchaseOrders;
        public PurchaseOrderController(IPurchaseOrders iPurchaseOrders)
        {
            _iPurchaseOrders = iPurchaseOrders;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPurchaseOrders()
        {
            var operation_GetAllPurchaseOrders = await _iPurchaseOrders.GetAllPurchaseOrdersAsync();
            return Ok(operation_GetAllPurchaseOrders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseOrderById(int id)
        {
            var operation_GetPurchaseOrderById = await _iPurchaseOrders.GetPurchaseOrderByIdAsync(id);
            if (operation_GetPurchaseOrderById == null)
            {
                return NotFound("Purchase order not found.");
            }
            return Ok(operation_GetPurchaseOrderById);
        }

        [HttpGet("by-supplierId/{id}")]
        public async Task<IActionResult> GetPurchaseOrderBySupplierId(int supplierId)
        {
            var operation_GetPurchaseOrderBySupplierId = await _iPurchaseOrders.GetPurchaseOrderBySupplierIdAsync(supplierId);
            if (operation_GetPurchaseOrderBySupplierId == null)
            {
                return NotFound("Purchase order not found. Filtered by the suppier.");
            }
            return Ok(operation_GetPurchaseOrderBySupplierId);
        }

        [HttpPost]
        public async Task<IActionResult> AddPurchaseOrder(PurchaseOrder purchaseOrder)
        {
            var operation_AddPurchaseOrder = await _iPurchaseOrders.AddPurchaseOrderAsync(purchaseOrder);
            if (operation_AddPurchaseOrder == null)
            {
                return NotFound("Unable to add purchase order information.");
            }
            return Ok("Purchase order information added successfully.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            var operation_UpdatePurchaseOrder = await _iPurchaseOrders.UpdatePurchaseOrderAsync(purchaseOrder);
            if (operation_UpdatePurchaseOrder == null)
            {
                return NotFound("Unable to update purchase order information.");
            }
            return Ok("Purchase order information updated successfullly.");
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeletePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            var operation_SoftDeletePurchaseOrder = await _iPurchaseOrders.SoftDeletePurchaseOrderAsync(purchaseOrder);
            if (operation_SoftDeletePurchaseOrder == null)
            {
                return NotFound("Unable to delete purchase order information.");
            }
            return Ok("Purchase order information deleted successfully.");
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeletePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            var operation_UndoSoftDeletePurchaseOrder = await _iPurchaseOrders.UndoSoftDeletePurchaseOrderAsync(purchaseOrder);
            if (operation_UndoSoftDeletePurchaseOrder == null)
            {
                return NotFound("Unable to restore deleted purchase order information.");
            }
            return Ok("Deleted purchase order information restored successfully.");
        }
    }
}
