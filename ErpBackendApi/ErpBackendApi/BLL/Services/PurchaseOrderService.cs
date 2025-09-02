using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.Services
{
    public class PurchaseOrderService : IPurchaseOrders
    {
        private readonly AppDataContext _context;
        public PurchaseOrderService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PurchaseOrderDTO>> GetAllPurchaseOrdersAsync()
        {
            return await
            (
                from po in _context.purchase_orders
                join s in _context.suppliers on po.supplier_id equals s.id into supplierGroup
                from s in supplierGroup.DefaultIfEmpty()
                where po.is_deleted == false
                select new PurchaseOrderDTO
                {
                    id = po.id,
                    supplier_id = s != null ? s.id : null,
                    company_name = s != null && s.is_deleted == false ? s.company_name : "-",
                    order_date = po.order_date,
                    expected_delivery_date = po.expected_delivery_date,
                    delivery_status = po.delivery_status,
                    notes = po.notes
                }
            ).ToListAsync();
        }

        public async Task<PurchaseOrderDTO> GetPurchaseOrderByIdAsync(int id)
        {
            return await
            (
                from po in _context.purchase_orders
                join s in _context.suppliers on po.supplier_id equals s.id into supplierGroup
                from s in supplierGroup.DefaultIfEmpty()
                where po.id == id && po.is_deleted == false
                select new PurchaseOrderDTO
                {
                    id = po.id,
                    supplier_id = s != null ? s.id : null,
                    company_name = s != null && s.is_deleted == false ? s.company_name : "-",
                    order_date = po.order_date,
                    expected_delivery_date = po.expected_delivery_date,
                    delivery_status = po.delivery_status,
                    notes = po.notes
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<PurchaseOrderDTO> GetPurchaseOrderBySupplierIdAsync(int supplierId)
        {
            return await
            (
                from po in _context.purchase_orders
                join s in _context.suppliers on po.supplier_id equals s.id into supplierGroup
                from s in supplierGroup.DefaultIfEmpty()
                where s.id == supplierId && po.is_deleted == false
                select new PurchaseOrderDTO
                {
                    id = po.id,
                    supplier_id = s != null ? s.id : null,
                    company_name = s != null && s.is_deleted == false ? s.company_name : "-",
                    order_date = po.order_date,
                    expected_delivery_date = po.expected_delivery_date,
                    delivery_status = po.delivery_status,
                    notes = po.notes
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<PurchaseOrder> AddPurchaseOrderAsync(PurchaseOrder purchaseOrder)
        {
            var existingSupplier = await _context.suppliers.FirstOrDefaultAsync(s => s.id == purchaseOrder.supplier_id && s.is_deleted == false);

            if (purchaseOrder.supplier_id == null || purchaseOrder.order_date == null || purchaseOrder.delivery_status == null)
            {
                Logger("Supplier id/information, order date, delivery status cannot be empty.");
                throw new InvalidOperationException("Supplier id/information, order date, delivery status cannot be empty.");
            }

            if (existingSupplier == null)
            {
                Logger("Supplier not found to add in purchase order.");
                throw new InvalidOperationException("Supplier not found to add in purchase order.");
            }

            if (purchaseOrder.expected_delivery_date.HasValue &&
                purchaseOrder.order_date.HasValue &&
                purchaseOrder.expected_delivery_date < purchaseOrder.order_date)
            {
                Logger("Expected delivery date cannot be earlier than order date.");
                throw new InvalidOperationException("Expected delivery date cannot be earlier than order date.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                purchaseOrder.is_deleted = false;
                purchaseOrder.deleted_at = null;
                _context.purchase_orders.Add(purchaseOrder);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return purchaseOrder;
            }
            catch
            {
                await transaction.RollbackAsync();
                Logger("Data transaction rolled back.");
                throw new InvalidOperationException("Data transaction rolled back.");
            }
        }

        public async Task<PurchaseOrder> UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder)
        {
            var existingPurchaseOrder = await _context.purchase_orders.FirstOrDefaultAsync(po => po.id == purchaseOrder.id && po.is_deleted == false);
            var existingSupplier = await _context.suppliers.FirstOrDefaultAsync(s => s.id == purchaseOrder.supplier_id && s.is_deleted == false);

            if (existingPurchaseOrder == null)
            {
                Logger("Purchase order information not found. Unable to update purchase order.");
                throw new InvalidOperationException("Purchase order information not found. Unable to update purchase order.");
            }

            if (purchaseOrder.supplier_id == null || purchaseOrder.order_date == null || purchaseOrder.delivery_status == null)
            {
                Logger("Supplier id/information, order date, delivery status cannot be empty.");
                throw new InvalidOperationException("Supplier id/information, order date, delivery status cannot be empty.");
            }

            if (existingSupplier == null)
            {
                Logger("Supplier not found to add in purchase order.");
                throw new InvalidOperationException("Supplier not found to add in purchase order.");
            }

            if (purchaseOrder.expected_delivery_date.HasValue &&
                purchaseOrder.order_date.HasValue &&
                purchaseOrder.expected_delivery_date < purchaseOrder.order_date)
            {
                Logger("Expected delivery date cannot be earlier than order date.");
                throw new InvalidOperationException("Expected delivery date cannot be earlier than order date.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                existingPurchaseOrder.supplier_id = purchaseOrder.supplier_id;
                existingPurchaseOrder.order_date = purchaseOrder.order_date;
                existingPurchaseOrder.expected_delivery_date = purchaseOrder.expected_delivery_date;
                existingPurchaseOrder.delivery_status = purchaseOrder.delivery_status;
                existingPurchaseOrder.notes = purchaseOrder.notes;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return existingPurchaseOrder;
            }
            catch
            {
                await transaction.RollbackAsync();
                Logger("Data transaction rolled back.");
                throw new InvalidOperationException("Data transaction rolled back.");
            }
        }

        public async Task<PurchaseOrder> SoftDeletePurchaseOrderAsync(PurchaseOrder purchaseOrder)
        {
            var existingPurchaseOrder = await _context.purchase_orders.FirstOrDefaultAsync(po => po.id == purchaseOrder.id && po.is_deleted == false);
            
            if (existingPurchaseOrder == null)
            {
                Logger("Purchase order information not found. Unable to delete purchase order.");
                throw new InvalidOperationException("Purchase order information not found. Unable to delete purchase order.");
            }

            existingPurchaseOrder.is_deleted = true;
            existingPurchaseOrder.deleted_at = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existingPurchaseOrder;
        }

        public async Task<PurchaseOrder> UndoSoftDeletePurchaseOrderAsync(PurchaseOrder purchaseOrder)
        {
            var existingPurchaseOrder = await _context.purchase_orders.FirstOrDefaultAsync(po => po.id == purchaseOrder.id && po.is_deleted == true);
            
            if (existingPurchaseOrder == null)
            {
                Logger("Unable to restore deleted purchase order.");
                throw new InvalidOperationException("Unable to restore deleted purchase order.");
            }

            existingPurchaseOrder.is_deleted = false;
            existingPurchaseOrder.deleted_at = null;
            await _context.SaveChangesAsync();
            return existingPurchaseOrder;
        }
    }
}
