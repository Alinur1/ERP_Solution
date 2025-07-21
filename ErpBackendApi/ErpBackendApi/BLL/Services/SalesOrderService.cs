using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.Services
{
    public class SalesOrderService : ISalesOrders
    {
        private readonly AppDataContext _context;
        public SalesOrderService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SalesOrderDTO>> GetAllSalesOrderAsync()
        {
            return await
            (
                from so in _context.sales_orders
                join c in _context.customers on so.customer_id equals c.id into customerGroup
                from c in customerGroup.DefaultIfEmpty()
                where so.is_deleted == false
                select new SalesOrderDTO
                {
                    id = so.id,
                    customer_id = c != null ? c.id : null,
                    customer_name = c != null && c.is_deleted == false ? c.name : "-",
                    order_date = so.order_date,
                    delivery_date = so.delivery_date,
                    delivery_status = so.delivery_status,
                    status = so.status,
                    notes = so.notes,
                }
            ).ToListAsync();
        }

        public async Task<SalesOrderDTO> GetSalesOrderByIdAsync(int id)
        {
            return await
            (
                from so in _context.sales_orders
                join c in _context.customers on so.customer_id equals c.id into customersGroup
                from c in customersGroup.DefaultIfEmpty()
                where so.id == id && so.is_deleted == false
                select new SalesOrderDTO
                {
                    id = so.id,
                    customer_id = c != null ? c.id : null,
                    customer_name = c != null && c.is_deleted == false ? c.name : "-",
                    order_date = so.order_date,
                    delivery_date = so.delivery_date,
                    delivery_status = so.delivery_status,
                    status = so.status,
                    notes = so.notes,
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<SalesOrderDTO> GetSalesOrderByCustomerIdAsync(int customerId)
        {
            return await
            (
                from so in _context.sales_orders
                join c in _context.customers on so.customer_id equals c.id into customerGroup
                from c in customerGroup.DefaultIfEmpty()
                where c.id == customerId && so.is_deleted == false
                select new SalesOrderDTO
                {
                    id = so.id,
                    customer_id = c != null ? c.id : null,
                    customer_name = c != null && c.is_deleted == false ? c.name : "-",
                    order_date = so.order_date,
                    delivery_date = so.delivery_date,
                    delivery_status = so.delivery_status,
                    status = so.status,
                    notes = so.notes
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<SalesOrder> AddSalesOrderAsync(SalesOrder salesOrder)
        {
            salesOrder.is_deleted = false;
            salesOrder.deleted_at = null;
            _context.sales_orders.Add(salesOrder);
            await _context.SaveChangesAsync();
            return salesOrder;
        }

        public async Task<SalesOrder> UpdateSalesOrderAsync(SalesOrder salesOrder)
        {
            var existingSalesOrder = await _context.sales_orders.FirstOrDefaultAsync(so => so.id == salesOrder.id && so.is_deleted == false);
            if (existingSalesOrder == null)
            {
                Logger("Unable to update sales order. Check if it is not deleted.");
                return null;
            }
            existingSalesOrder.customer_id = salesOrder.customer_id;
            existingSalesOrder.order_date = salesOrder.order_date;
            existingSalesOrder.delivery_date = salesOrder.delivery_date;
            existingSalesOrder.delivery_status = salesOrder.delivery_status;
            existingSalesOrder.status = salesOrder.status;
            existingSalesOrder.notes = salesOrder.notes;
            _context.sales_orders.Update(existingSalesOrder);
            await _context.SaveChangesAsync();
            return existingSalesOrder;
        }

        public async Task<SalesOrder> SoftDeleteSalesOrderAsync(SalesOrder salesOrder)
        {
            var existingSalesOrder = await _context.sales_orders.FirstOrDefaultAsync(so => so.id == salesOrder.id && so.is_deleted == false);
            if (existingSalesOrder == null)
            {
                Logger("Unable to delete sales order. Sales order not found or already deleted.");
                return null;
            }
            existingSalesOrder.is_deleted = true;
            existingSalesOrder.deleted_at = DateTime.UtcNow;
            _context.sales_orders.Update(existingSalesOrder);
            await _context.SaveChangesAsync();
            return existingSalesOrder;
        }

        public async Task<SalesOrder> UndoSoftDeleteSalesOrderAsync(SalesOrder salesOrder)
        {
            var existingSalesOrder = await _context.sales_orders.FirstOrDefaultAsync(so => so.id == salesOrder.id && so.is_deleted == true);
            if (existingSalesOrder == null)
            {
                Logger("Unable to restore deleted sales order.");
                return null;
            }
            existingSalesOrder.is_deleted = false;
            existingSalesOrder.deleted_at = null;
            _context.sales_orders.Update(existingSalesOrder);
            await _context.SaveChangesAsync();
            return existingSalesOrder;
        }
    }
}
