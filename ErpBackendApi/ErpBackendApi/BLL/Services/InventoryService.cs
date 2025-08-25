using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.Services
{
    public class InventoryService : IInventories
    {
        private readonly AppDataContext _context;
        public InventoryService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InventoryDTO>> GetAllInventoriesAsync()
        {
            return await
            (
                from i in _context.inventory
                join p in _context.products on i.product_id equals p.id
                where p.is_deleted == false
                select new InventoryDTO
                {
                    id = i.id,
                    product_id = p.id,
                    product_name = p.name,
                    quantity = i.quantity,
                    reorder_level = i.reorder_level,
                    last_updated = i.last_updated,
                }
            ).ToListAsync();
        }

        public async Task<InventoryDTO> GetInventoryByIdAsync(int id)
        {
            return await
            (
                from i in _context.inventory
                join p in _context.products on i.product_id equals p.id
                where i.id == id && p.is_deleted == false
                select new InventoryDTO
                {
                    id = i.id,
                    product_id = p.id,
                    product_name = p.name,
                    quantity = i.quantity,
                    reorder_level = i.reorder_level,
                    last_updated = i.last_updated,
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<Inventory> AddInventoryAsync(Inventory inventory)
        {
            var existingInventory = await _context.inventory.FirstOrDefaultAsync(i => i.product_id == inventory.product_id);
            var existingProduct = await _context.products.AnyAsync(p => p.id == inventory.product_id || p.is_deleted == true);
            if (!existingProduct)
            {
                Logger("Product does not exist or deleted.");
                throw new InvalidOperationException("Product does not exist or deleted.");
            }
            if (existingInventory != null)
            {
                Logger("Tried to add same product in the inventory.");
                throw new InvalidOperationException("Tried to add same product in the inventory.");
            }
            inventory.last_updated = DateTime.UtcNow;
            inventory.is_deleted = false;
            inventory.deleted_at = null;
            _context.inventory.Add(inventory);
            await _context.SaveChangesAsync();
            return inventory;
        }

        public async Task<Inventory> UpdateInventoryAsync(Inventory inventory)
        {
            var existingInventory = await _context.inventory.FirstOrDefaultAsync(i => i.id == inventory.id);
            if (existingInventory == null)
            {
                Logger("Inventory not found to update.");
                throw new InvalidOperationException("Inventory not found to update.");
            }
            existingInventory.quantity = inventory.quantity;
            existingInventory.reorder_level = inventory.reorder_level;
            existingInventory.last_updated = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existingInventory;
        }

        public async Task<Inventory> SoftDeleteInventoryAsync(Inventory inventory)
        {
            var existingInventory = await _context.inventory.FirstOrDefaultAsync(i => i.id == inventory.id && i.is_deleted == false);

            if (existingInventory == null)
            {
                Logger("Unable to delete inventory. Inventory not found.");
                throw new InvalidOperationException("Unable to delete inventory. Inventory not found.");
            }
            existingInventory.is_deleted = true;
            existingInventory.deleted_at = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existingInventory;
        }

        public async Task<Inventory> UndoSoftDeleteInventoryAsync(Inventory inventory)
        {
            var existingInventory = await _context.inventory.FirstOrDefaultAsync(i => i.id == inventory.id && i.is_deleted == true);

            if (existingInventory == null)
            {
                Logger("Unable to restore deleted inventory. Deleted inventory not found.");
                throw new InvalidOperationException("Unable to restore deleted inventory. Deleted inventory not found.");
            }
            existingInventory.is_deleted = false;
            existingInventory.deleted_at = null;
            await _context.SaveChangesAsync();
            return existingInventory;
        }
    }
}
