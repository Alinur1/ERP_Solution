using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.Services
{
    public class SalesOrderItemService : ISalesOrderItems
    {
        private readonly AppDataContext _context;
        public SalesOrderItemService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SalesOrderItemDTO>> GetAllSalesOrderItemAsync()
        {
            return await
            (
                from soi in _context.sales_order_items
                join so in _context.sales_orders on soi.sales_order_id equals so.id into salesOrderGroup
                from so in salesOrderGroup.DefaultIfEmpty()
                join p in _context.products on soi.product_id equals p.id into productGroup
                from p in productGroup.DefaultIfEmpty()
                where soi.is_deleted == false
                select new SalesOrderItemDTO
                {
                    id = soi.id,
                    sales_order_id = so != null ? so.id : null,
                    sales_order_number = so != null && so.is_deleted == false ? so.order_number : "-",
                    product_id = p != null ? p.id : null,
                    product_name = p != null && p.is_deleted == false ? p.name : "-",
                    quantity = soi.quantity,
                    unit_price = soi.unit_price,
                    discount = soi.discount
                }
            ).ToListAsync();
        }

        public async Task<SalesOrderItemDTO> GetSalesOrderItemByIdAsync(int id)
        {
            return await
            (
                from soi in _context.sales_order_items
                join so in _context.sales_orders on soi.sales_order_id equals so.id into salesOrderGroup
                from so in salesOrderGroup.DefaultIfEmpty()
                join p in _context.products on soi.product_id equals p.id into productGroup
                from p in productGroup.DefaultIfEmpty()
                where soi.id == id && soi.is_deleted == false
                select new SalesOrderItemDTO
                {
                    id = soi.id,
                    sales_order_id = so != null ? so.id : null,
                    sales_order_number = so != null && so.is_deleted == false ? so.order_number : "-",
                    product_id = p != null ? p.id : null,
                    product_name = p != null && p.is_deleted == false ? p.name : "-",
                    quantity = soi.quantity,
                    unit_price = soi.unit_price,
                    discount = soi.discount
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<SalesOrderItemDTO> GetSalesOrderItemBySalesOrderIdAsync(int orderId)
        {
            return await
            (
                from soi in _context.sales_order_items
                join so in _context.sales_orders on soi.sales_order_id equals so.id into salesOrderGroup
                from so in salesOrderGroup.DefaultIfEmpty()
                join p in _context.products on soi.product_id equals p.id into productGroup
                from p in productGroup.DefaultIfEmpty()
                where so.id == orderId && soi.is_deleted == false
                select new SalesOrderItemDTO
                {
                    id = soi.id,
                    sales_order_id = so != null ? so.id : null,
                    sales_order_number = so != null && so.is_deleted == false ? so.order_number : "-",
                    product_id = p != null ? p.id : null,
                    product_name = p != null && p.is_deleted == false ? p.name : "-",
                    quantity = soi.quantity,
                    unit_price = soi.unit_price,
                    discount = soi.discount
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<SalesOrderItem> AddSalesOrderItemAsync(SalesOrderItem item)
        {
            var existingSalesOrderItem = await _context.sales_order_items.FirstOrDefaultAsync(soi => soi.sales_order_id == item.sales_order_id && soi.is_deleted == false);
            if (existingSalesOrderItem != null)
            {
                Logger("Same order number for an item is not allowed.");
                return null;
            }
            item.is_deleted = false;
            item.deleted_at = null;
            _context.sales_order_items.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<SalesOrderItem> UpdateSalesOrderItemAsync(SalesOrderItem item)
        {
            var existingSalesOrderItem = await _context.sales_order_items.FirstOrDefaultAsync(soi => soi.id == item.id && soi.is_deleted == false);
            if (existingSalesOrderItem == null)
            {
                Logger("Unable to update sales order item.");
                return null;
            }
            existingSalesOrderItem.sales_order_id = item.sales_order_id;
            existingSalesOrderItem.product_id = item.product_id;
            existingSalesOrderItem.quantity = item.quantity;
            existingSalesOrderItem.unit_price = item.unit_price;
            existingSalesOrderItem.discount = item.discount;
            _context.sales_order_items.Update(existingSalesOrderItem);
            await _context.SaveChangesAsync();
            return existingSalesOrderItem;
        }

        public async Task<SalesOrderItem> SoftDeleteSalesOrderItemAsync(SalesOrderItem item)
        {
            var existingSalesOrderItem = await _context.sales_order_items.FirstOrDefaultAsync(soi => soi.id == item.id && soi.is_deleted == false);
            if (existingSalesOrderItem == null)
            {
                Logger("Unable to delete sales order item.");
                return null;
            }
            existingSalesOrderItem.is_deleted = true;
            existingSalesOrderItem.deleted_at = DateTime.UtcNow;
            _context.sales_order_items.Update(existingSalesOrderItem);
            await _context.SaveChangesAsync();
            return existingSalesOrderItem;
        }

        public async Task<SalesOrderItem> UndoSoftDeleteSalesOrderItemAsync(SalesOrderItem item)
        {
            var existingSalesOrderItem = await _context.sales_order_items.FirstOrDefaultAsync(soi => soi.id == item.id && soi.is_deleted == true);
            if (existingSalesOrderItem == null)
            {
                Logger("Unable to delete sales order item.");
                return null;
            }
            existingSalesOrderItem.is_deleted = false;
            existingSalesOrderItem.deleted_at = null;
            _context.sales_order_items.Update(existingSalesOrderItem);
            await _context.SaveChangesAsync();
            return existingSalesOrderItem;
        }
    }
}
