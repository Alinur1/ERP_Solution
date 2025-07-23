using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Pqc.Crypto.Falcon;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.Services
{
    public class PurchaseOrderItemService : IPurchaseOrderItems
    {
        private readonly AppDataContext _context;
        public PurchaseOrderItemService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PurchaseOrderItemDTO>> GetAllPurchaseOrderItemsAsync()
        {
            return await
            (
                from poi in _context.purchase_order_items
                join po in _context.purchase_orders on poi.purchase_order_id equals po.id into purchaseGroup
                from po in purchaseGroup.DefaultIfEmpty()
                join p in _context.products on poi.product_id equals p.id into productGroup
                from p in productGroup.DefaultIfEmpty()
                where poi.is_deleted == false
                select new PurchaseOrderItemDTO
                {
                    id = poi.id,
                    purchase_order_id = po != null ? po.id : null,
                    product_id = p != null ? p.id : null,
                    product_name = p != null && p.is_deleted == false ? p.name : "-",
                    quantity = poi.quantity,
                    unit_price = poi.unit_price,
                    discount = poi.discount,
                }
            ).ToListAsync();
        }

        public async Task<PurchaseOrderItemDTO> GetPurchaseOrderItemByIdAsync(int id)
        {
            return await
            (
                from poi in _context.purchase_order_items
                join po in _context.purchase_orders on poi.purchase_order_id equals po.id into purchaseGroup
                from po in purchaseGroup.DefaultIfEmpty()
                join p in _context.products on poi.product_id equals p.id into productGroup
                from p in productGroup.DefaultIfEmpty()
                where poi.id == id && poi.is_deleted == false
                select new PurchaseOrderItemDTO
                {
                    id = poi.id,
                    purchase_order_id = po != null ? po.id : null,
                    product_id = p != null ? p.id : null,
                    product_name = p != null && p.is_deleted == false ? p.name : "-",
                    quantity = poi.quantity,
                    unit_price = poi.unit_price,
                    discount = poi.discount,
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<PurchaseOrderItem> AddPurchaseOrderItemAsync(PurchaseOrderItem item)
        {
            var existingPurchaseOrderItem = await _context.purchase_order_items.FirstOrDefaultAsync(poi => poi.purchase_order_id == item.purchase_order_id && poi.is_deleted == false);
            if (existingPurchaseOrderItem != null)
            {
                Logger("Same purchase order ID already exists.");
                return null;
            }
            item.is_deleted = false;
            item.deleted_at = null;
            _context.purchase_order_items.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<PurchaseOrderItem> UpdatePurchaseOrderItemAsync(PurchaseOrderItem item)
        {
            var existingPurchaseOrderItem = await _context.purchase_order_items.FirstOrDefaultAsync(poi => poi.id == item.id && poi.is_deleted == false);
            if (existingPurchaseOrderItem == null)
            {
                Logger("Unable to update. Purchase order item not found.");
                return null;
            }
            existingPurchaseOrderItem.product_id = item.product_id;
            existingPurchaseOrderItem.quantity = item.quantity;
            existingPurchaseOrderItem.unit_price = item.unit_price;
            existingPurchaseOrderItem.discount = item.discount;

            _context.purchase_order_items.Update(existingPurchaseOrderItem);
            await _context.SaveChangesAsync();
            return existingPurchaseOrderItem;
        }

        public async Task<PurchaseOrderItem> SoftDeletePurchaseOrderItemAsync(PurchaseOrderItem item)
        {
            var existingPurchaseOrderItem = await _context.purchase_order_items.FirstOrDefaultAsync(poi => poi.id == item.id && poi.is_deleted == false);
            if (existingPurchaseOrderItem == null)
            {
                Logger("Unable to delete. Purchase order item not found.");
                return null;
            }
            existingPurchaseOrderItem.is_deleted = true;
            existingPurchaseOrderItem.deleted_at = DateTime.UtcNow;

            _context.purchase_order_items.Update(existingPurchaseOrderItem);
            await _context.SaveChangesAsync();
            return existingPurchaseOrderItem;
        }

        public async Task<PurchaseOrderItem> UndoSoftDeletePurchaseOrderItemAsync(PurchaseOrderItem item)
        {
            var existingPurchaseOrderItem = await _context.purchase_order_items.FirstOrDefaultAsync(poi => poi.id == item.id && poi.is_deleted == true);
            if (existingPurchaseOrderItem == null)
            {
                Logger("Unable to restore deleted purchase order item.");
                return null;
            }
            existingPurchaseOrderItem.is_deleted = false;
            existingPurchaseOrderItem.deleted_at = null;

            _context.purchase_order_items.Update(existingPurchaseOrderItem);
            await _context.SaveChangesAsync();
            return existingPurchaseOrderItem;
        }
    }
}
