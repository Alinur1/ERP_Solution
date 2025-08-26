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
                join p in _context.products on i.product_id equals p.id into productGroup
                from p in productGroup.DefaultIfEmpty()
                where p.is_deleted == false
                select new InventoryDTO
                {
                    id = i.id,
                    product_id = p != null && p.is_deleted == false ? p.id : null,
                    product_name = p != null && p.is_deleted == false ? p.name : null,
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
                join p in _context.products on i.product_id equals p.id into productGroup
                from p in productGroup.DefaultIfEmpty()
                where i.id == id && p.is_deleted == false
                select new InventoryDTO
                {
                    id = i.id,
                    product_id = p != null && p.is_deleted == false ? p.id : null,
                    product_name = p != null && p.is_deleted == false ? p.name : null,
                    quantity = i.quantity,
                    reorder_level = i.reorder_level,
                    last_updated = i.last_updated,
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<Inventory> AddInventoryAsync(Inventory inventory)
        {
            var existingInventory = await _context.inventory.FirstOrDefaultAsync(i => i.product_id == inventory.product_id && i.is_deleted == false);
            var existingProduct = await _context.products.FirstOrDefaultAsync(p => p.id == inventory.product_id && p.is_deleted == false);

            if (existingProduct == null)
            {
                Logger("Product does not exist or deleted.");
                throw new InvalidOperationException("Product does not exist or deleted.");
            }

            if (existingInventory != null)
            {
                Logger("Tried to add same product in the inventory.");
                throw new InvalidOperationException("Tried to add same product in the inventory.");
            }

            if (inventory.quantity == null || inventory.reorder_level == null || inventory.quantity < 0 || inventory.reorder_level < 0)
            {
                Logger("Quantity and minimum quantity level must be a positive value.");
                throw new InvalidOperationException("Quantity and minimum quantity level must be a positive value.");
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
            var existingInventory = await _context.inventory.FirstOrDefaultAsync(i => i.id == inventory.id && i.is_deleted == false);

            if (existingInventory == null)
            {
                Logger("Inventory not found to update.");
                throw new InvalidOperationException("Inventory not found to update.");
            }

            if (inventory.quantity == null || inventory.reorder_level == null || inventory.quantity < 0 || inventory.reorder_level < 0)
            {
                Logger("Quantity and minimum quantity level must be a positive value.");
                throw new InvalidOperationException("Quantity and minimum quantity level must be a positive value.");
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

        public async Task<IEnumerable<InventoryDTO>> GetAllDeletedInventoriesAsync()
        {
            return await
            (
                from i in _context.inventory
                join p in _context.products on i.product_id equals p.id into productGroup
                from p in productGroup.DefaultIfEmpty()
                where p.is_deleted == true
                select new InventoryDTO
                {
                    id = i.id,
                    product_id = p != null && p.is_deleted == false ? p.id : null,
                    product_name = p != null && p.is_deleted == false ? p.name : null,
                    quantity = i.quantity,
                    reorder_level = i.reorder_level,
                    last_updated = i.last_updated,
                    deleted_at = i.deleted_at,
                }
            ).ToListAsync();
        }
    }
}
