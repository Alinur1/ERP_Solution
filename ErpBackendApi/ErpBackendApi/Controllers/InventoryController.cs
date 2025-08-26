using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErpBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventories _iInventories;
        public InventoryController(IInventories iInventories)
        {
            _iInventories = iInventories;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInventories()
        {
            var operation_GetAllInventories = await _iInventories.GetAllInventoriesAsync();
            return Ok(operation_GetAllInventories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInventoryById(int id)
        {
            var operation_GetInventoryById = await _iInventories.GetInventoryByIdAsync(id);
            if (operation_GetInventoryById == null)
            {
                return BadRequest("Inventory not found.");
            }
            return Ok(operation_GetInventoryById);
        }

        [HttpPost]
        public async Task<IActionResult> AddInventory(Inventory inventory)
        {
            try
            {
                var operation_AddInventory = await _iInventories.AddInventoryAsync(inventory);
                return Ok("Inventory added successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateInventory(Inventory inventory)
        {
            try
            {
                var operation_UpdateInventory = await _iInventories.UpdateInventoryAsync(inventory);
                return Ok("Inventory updated successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteInventory(Inventory inventory)
        {
            try
            {
                var operation_SoftDeleteInventory = await _iInventories.SoftDeleteInventoryAsync(inventory);
                return Ok("Inventory successfully deleted.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteInventory(Inventory inventory)
        {
            try
            {
                var operation_UndoSoftDeleteInventory = await _iInventories.UndoSoftDeleteInventoryAsync(inventory);
                return Ok("Inventory successfully restored.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("deleted-inventory")]
        public async Task<IActionResult> GetAllDeletedInventory()
        {            
            var operation_GetAllDeletedInventory = await _iInventories.GetAllDeletedInventoriesAsync();
            return Ok(operation_GetAllDeletedInventory);
        }
    }
}
