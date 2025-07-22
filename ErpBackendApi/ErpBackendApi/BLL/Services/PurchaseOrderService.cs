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
            purchaseOrder.is_deleted = false;
            purchaseOrder.deleted_at = null;
            _context.purchase_orders.Add(purchaseOrder);
            await _context.SaveChangesAsync();
            return purchaseOrder;
        }

        public async Task<PurchaseOrder> UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder)
        {
            var existingPurchaseOrder = await _context.purchase_orders.FirstOrDefaultAsync(po => po.id == purchaseOrder.id && po.is_deleted == false);
            if (existingPurchaseOrder == null)
            {
                Logger("Purchase order information not found. Unable to update purchase order.");
                return null;
            }
            existingPurchaseOrder.supplier_id = purchaseOrder.supplier_id;
            existingPurchaseOrder.order_date = purchaseOrder.order_date;
            existingPurchaseOrder.expected_delivery_date = purchaseOrder.expected_delivery_date;
            existingPurchaseOrder.delivery_status = purchaseOrder.delivery_status;
            existingPurchaseOrder.notes = purchaseOrder.notes;

            _context.purchase_orders.Update(existingPurchaseOrder);
            await _context.SaveChangesAsync();
            return existingPurchaseOrder;
        }

        public async Task<PurchaseOrder> SoftDeletePurchaseOrderAsync(PurchaseOrder purchaseOrder)
        {
            var existingPurchaseOrder = await _context.purchase_orders.FirstOrDefaultAsync(po => po.id == purchaseOrder.id && po.is_deleted == false);
            if (existingPurchaseOrder == null)
            {
                Logger("Purchase order information not found. Unable to delete purchase order.");
                return null;
            }
            existingPurchaseOrder.is_deleted = true;
            existingPurchaseOrder.deleted_at = DateTime.UtcNow;

            _context.purchase_orders.Update(existingPurchaseOrder);
            await _context.SaveChangesAsync();
            return existingPurchaseOrder;
        }

        public async Task<PurchaseOrder> UndoSoftDeletePurchaseOrderAsync(PurchaseOrder purchaseOrder)
        {
            var existingPurchaseOrder = await _context.purchase_orders.FirstOrDefaultAsync(po => po.id == purchaseOrder.id && po.is_deleted == true);
            if (existingPurchaseOrder == null)
            {
                Logger("Unable to restore deleted purchase order.");
                return null;
            }
            existingPurchaseOrder.is_deleted = false;
            existingPurchaseOrder.deleted_at = null;

            _context.purchase_orders.Update(existingPurchaseOrder);
            await _context.SaveChangesAsync();
            return existingPurchaseOrder;
        }
    }
}
