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
                return BadRequest("Purchase order not found.");
            }
            return Ok(operation_GetPurchaseOrderById);
        }

        [HttpGet("by-supplierId/{supplierId}")]
        public async Task<IActionResult> GetPurchaseOrderBySupplierId(int supplierId)
        {
            var operation_GetPurchaseOrderBySupplierId = await _iPurchaseOrders.GetPurchaseOrderBySupplierIdAsync(supplierId);
            if (operation_GetPurchaseOrderBySupplierId == null)
            {
                return BadRequest("Purchase order not found. Filtered by the suppier.");
            }
            return Ok(operation_GetPurchaseOrderBySupplierId);
        }

        [HttpPost]
        public async Task<IActionResult> AddPurchaseOrder(PurchaseOrder purchaseOrder)
        {
            try
            {
                var operation_AddPurchaseOrder = await _iPurchaseOrders.AddPurchaseOrderAsync(purchaseOrder);
                return Ok("Purchase order information added successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdatePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            try
            {
                var operation_UpdatePurchaseOrder = await _iPurchaseOrders.UpdatePurchaseOrderAsync(purchaseOrder);
                return Ok("Purchase order information updated successfullly.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeletePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            try
            {
                var operation_SoftDeletePurchaseOrder = await _iPurchaseOrders.SoftDeletePurchaseOrderAsync(purchaseOrder);
                return Ok("Purchase order information deleted successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeletePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            try
            {
                var operation_UndoSoftDeletePurchaseOrder = await _iPurchaseOrders.UndoSoftDeletePurchaseOrderAsync(purchaseOrder);
                return Ok("Deleted purchase order information restored successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("deleted-purchase-order")]
        public async Task<IActionResult> GetAllDeletedPurchaseOrders()
        {
            var operation_GetAllDeletedPurchaseOrders = await _iPurchaseOrders.GetAllDeletedPurchaseOrdersAsync();
            return Ok(operation_GetAllDeletedPurchaseOrders);
        }
    }
}
