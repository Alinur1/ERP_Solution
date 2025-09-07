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
                join s in _context.suppliers on po.supplier_id equals s.id into supplierGroup
                from s in supplierGroup.DefaultIfEmpty()
                where poi.is_deleted == false
                select new PurchaseOrderItemDTO
                {
                    id = poi.id,
                    purchase_order_id = po != null && po.is_deleted == false ? po.id : null,
                    supplier_id = s != null && s.is_deleted == false ? s.id : null,
                    supplier_name = s != null && s.is_deleted == false ? s.contact_person_name : null,
                    product_id = p != null && p.is_deleted == false ? p.id : null,
                    product_name = p != null && p.is_deleted == false ? p.name : null,
                    sku = p != null && p.is_deleted == false ? p.sku : null,
                    price = p != null && p.is_deleted == false ? p.price : null,
                    quantity = poi.quantity,
                    amount = poi.amount,
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
                join s in _context.suppliers on po.supplier_id equals s.id into supplierGroup
                from s in supplierGroup.DefaultIfEmpty()
                where poi.id == id && poi.is_deleted == false
                select new PurchaseOrderItemDTO
                {
                    id = poi.id,
                    purchase_order_id = po != null && po.is_deleted == false ? po.id : null,
                    supplier_id = s != null && s.is_deleted == false ? s.id : null,
                    supplier_name = s != null && s.is_deleted == false ? s.contact_person_name : null,
                    product_id = p != null && p.is_deleted == false ? p.id : null,
                    product_name = p != null && p.is_deleted == false ? p.name : null,
                    sku = p != null && p.is_deleted == false ? p.sku : null,
                    price = p != null && p.is_deleted == false ? p.price : null,
                    quantity = poi.quantity,
                    amount = poi.amount,
                    discount = poi.discount,
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<PurchaseOrderItemDTO> GetPurchaseOrderItemByPurchaseOrderIdAsync(int orderId)
        {
            return await
            (
                from poi in _context.purchase_order_items
                join po in _context.purchase_orders on poi.purchase_order_id equals po.id into purchaseGroup
                from po in purchaseGroup.DefaultIfEmpty()
                join p in _context.products on poi.product_id equals p.id into productGroup
                from p in productGroup.DefaultIfEmpty()
                join s in _context.suppliers on po.supplier_id equals s.id into supplierGroup
                from s in supplierGroup.DefaultIfEmpty()
                where po.id == orderId && poi.is_deleted == false
                select new PurchaseOrderItemDTO
                {
                    id = poi.id,
                    purchase_order_id = po != null && po.is_deleted == false ? po.id : null,
                    supplier_id = s != null && s.is_deleted == false ? s.id : null,
                    supplier_name = s != null && s.is_deleted == false ? s.contact_person_name : null,
                    product_id = p != null && p.is_deleted == false ? p.id : null,
                    product_name = p != null && p.is_deleted == false ? p.name : null,
                    sku = p != null && p.is_deleted == false ? p.sku : null,
                    price = p != null && p.is_deleted == false ? p.price : null,
                    quantity = poi.quantity,
                    amount = poi.amount,
                    discount = poi.discount,
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PurchaseOrderItem>> AddPurchaseOrderItemAsync(IEnumerable<PurchaseOrderItem> items)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var item in items)
                {
                    var existingPurchaseOrder = await _context.purchase_orders.FirstOrDefaultAsync(po => po.id == item.purchase_order_id && po.is_deleted == false);
                    if (existingPurchaseOrder == null)
                    {
                        Logger($"Purchase order id {item.purchase_order_id} not found.");
                        throw new InvalidOperationException($"Purchase order id {item.purchase_order_id} not found.");
                    }

                    var existingProduct = await _context.products.FirstOrDefaultAsync(p => p.id == item.product_id && p.is_deleted == false);
                    if (existingProduct == null)
                    {
                        Logger("Product not found.");
                        throw new InvalidOperationException("Product not found.");
                    }

                    var existingPurchaseOrderItem = await _context.purchase_order_items.FirstOrDefaultAsync(pot => pot.purchase_order_id == item.purchase_order_id && pot.product_id == item.product_id && pot.is_deleted == false);
                    if (existingPurchaseOrderItem != null)
                    {
                        Logger($"Product ID {item.product_id} is already added to the purchase order {item.purchase_order_id}.");
                        throw new InvalidOperationException($"Product ID {item.product_id} is already added to the purchase order {item.purchase_order_id}.");
                    }

                    if (item.quantity < 0 || item.amount < 0 || item.discount < 0)
                    {
                        Logger("Quantity, amount or discount cannot be negative.");
                        throw new InvalidOperationException("Quantity, amount or discount cannot be negative.");
                    }

                    if (item.quantity == null)
                    {
                        Logger("Quantity is required.");
                        throw new InvalidOperationException("Quantity is required.");
                    }

                    item.amount = item.quantity * existingProduct.price;
                    item.is_deleted = false;
                    item.deleted_at = null;
                    _context.purchase_order_items.Add(item);
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return items;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Changes rollback occurred unexpectedly.");
            }
        }

        public async Task<PurchaseOrderItem> UpdatePurchaseOrderItemAsync(PurchaseOrderItem item)
        {
            var existingPurchaseOrderItem = await _context.purchase_order_items.FirstOrDefaultAsync(poi => poi.id == item.id && poi.is_deleted == false);
            if (existingPurchaseOrderItem == null)
            {
                Logger("Purchase order item not found or deleted.");
                throw new InvalidOperationException("Purchase order item not found or deleted.");
            }

            var existingExpense = await _context.expenses.FirstOrDefaultAsync(e => e.purchase_order_id == existingPurchaseOrderItem.purchase_order_id && e.is_deleted == false);
            if (existingExpense != null)
            {
                Logger("Cannot modify purchase order item once expense is created.");
                throw new InvalidOperationException("Cannot modify purchase order item once expense is created.");
            }

            var existingProduct = await _context.products.FirstOrDefaultAsync(p => p.id == item.product_id && p.is_deleted == false);
            if (existingProduct == null)
            {
                Logger("Product not found.");
                throw new InvalidOperationException("Product not found.");
            }

            if (item.quantity < 0 || item.amount < 0 || item.discount < 0)
            {
                Logger("Quantity, amount or discount cannot be negative.");
                throw new InvalidOperationException("Quantity, amount or discount cannot be negative.");
            }

            if (item.quantity == null)
            {
                Logger("Quantity is required.");
                throw new InvalidOperationException("Quantity is required.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                existingPurchaseOrderItem.product_id = item.product_id;
                existingPurchaseOrderItem.quantity = item.quantity;
                existingPurchaseOrderItem.amount = item.quantity * existingProduct.price;
                existingPurchaseOrderItem.discount = item.discount;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return existingPurchaseOrderItem;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Changes rollback occurred unexpectedly.");
            }
        }

        public async Task<PurchaseOrderItem> SoftDeletePurchaseOrderItemAsync(PurchaseOrderItem item)
        {
            var existingPurchaseOrderItem = await _context.purchase_order_items.FirstOrDefaultAsync(poi => poi.id == item.id && poi.is_deleted == false);
            if (existingPurchaseOrderItem == null)
            {
                Logger("Unable to delete. Purchase order item not found.");
                throw new InvalidOperationException("Unable to delete. Purchase order item not found.");
            }

            var existingExpense = await _context.expenses.FirstOrDefaultAsync(e => e.purchase_order_id == existingPurchaseOrderItem.purchase_order_id && e.is_deleted == false);
            if (existingExpense != null)
            {
                Logger("Cannot delete purchase order item once expense is created.");
                throw new InvalidOperationException("Cannot delete purchase order item once expense is created.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                existingPurchaseOrderItem.is_deleted = true;
                existingPurchaseOrderItem.deleted_at = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return existingPurchaseOrderItem;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Changes rollback occurred unexpectedly.");
            }
        }

        public async Task<PurchaseOrderItem> UndoSoftDeletePurchaseOrderItemAsync(PurchaseOrderItem item)
        {
            var existingPurchaseOrderItem = await _context.purchase_order_items.FirstOrDefaultAsync(poi => poi.id == item.id && poi.is_deleted == true);
            if (existingPurchaseOrderItem == null)
            {
                Logger("Unable to restore deleted purchase order item.");
                throw new InvalidOperationException("Unable to restore deleted purchase order item.");
            }

            var existingExpense = await _context.expenses.FirstOrDefaultAsync(e => e.purchase_order_id == existingPurchaseOrderItem.purchase_order_id && e.is_deleted == false);
            if (existingExpense != null)
            {
                Logger("Cannot restore deleted purchase order item once expense is created.");
                throw new InvalidOperationException("Cannot restore deleted purchase order item once expense is created.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                existingPurchaseOrderItem.is_deleted = false;
                existingPurchaseOrderItem.deleted_at = null;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return existingPurchaseOrderItem;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Changes rollback occurred unexpectedly.");
            }
        }

        public async Task<IEnumerable<PurchaseOrderItemDTO>> GetAllDeletedPurchaseOrderItemsAsync()
        {
            return await
            (
                from poi in _context.purchase_order_items
                join po in _context.purchase_orders on poi.purchase_order_id equals po.id into purchaseGroup
                from po in purchaseGroup.DefaultIfEmpty()
                join p in _context.products on poi.product_id equals p.id into productGroup
                from p in productGroup.DefaultIfEmpty()
                join s in _context.suppliers on po.supplier_id equals s.id into supplierGroup
                from s in supplierGroup.DefaultIfEmpty()
                where poi.is_deleted == true
                select new PurchaseOrderItemDTO
                {
                    id = poi.id,
                    purchase_order_id = po != null && po.is_deleted == false ? po.id : null,
                    supplier_id = s != null && s.is_deleted == false ? s.id : null,
                    supplier_name = s != null && s.is_deleted == false ? s.contact_person_name : null,
                    product_id = p != null && p.is_deleted == false ? p.id : null,
                    product_name = p != null && p.is_deleted == false ? p.name : null,
                    sku = p != null && p.is_deleted == false ? p.sku : null,
                    price = p != null && p.is_deleted == false ? p.price : null,
                    quantity = poi.quantity,
                    amount = poi.amount,
                    discount = poi.discount,
                }
            ).ToListAsync();
        }
    }
}
