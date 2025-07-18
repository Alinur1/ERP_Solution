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
                return NotFound("Inventory not found.");
            }
            return Ok(operation_GetInventoryById);
        }

        [HttpPost]
        public async Task<IActionResult> AddInventory(Inventory inventory)
        {
            var operation_AddInventory = await _iInventories.AddInventoryAsync(inventory);
            if (operation_AddInventory == null)
            {
                return NotFound("Tried to add same product in the inventory or the inventory already exists.");
            }
            return Ok("Inventory added successfully.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateInventory(Inventory inventory)
        {
            var operation_UpdateInventory = await _iInventories.UpdateInventoryAsync(inventory);
            if (operation_UpdateInventory == null)
            {
                return NotFound("Inventory not found to update.");
            }
            return Ok("Inventory updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            var operation_DeleteInventory = await _iInventories.DeleteInventoryAsync(id);
            return Ok("Inventory deleted successfully.");
        }
    }
}
