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
                    customer_id = c != null && c.is_deleted == false ? c.id : null,
                    customer_name = c != null && c.is_deleted == false ? c.name : null,
                    order_date = so.order_date,
                    delivery_date = so.delivery_date,
                    delivery_status = so.delivery_status,
                    status = so.status,
                    notes = so.notes,
                    last_updated = so.last_updated,
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
                    customer_id = c != null && c.is_deleted == false ? c.id : null,
                    customer_name = c != null && c.is_deleted == false ? c.name : null,
                    order_date = so.order_date,
                    delivery_date = so.delivery_date,
                    delivery_status = so.delivery_status,
                    status = so.status,
                    notes = so.notes,
                    last_updated = so.last_updated,
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<SalesOrderDTO>> GetSalesOrderByCustomerIdAsync(int customerId)
        {
            return await
            (
                from so in _context.sales_orders
                join c in _context.customers on so.customer_id equals c.id into customerGroup
                from c in customerGroup.DefaultIfEmpty()
                where c.id == customerId && c.is_deleted == false && so.is_deleted == false
                select new SalesOrderDTO
                {
                    id = so.id,
                    customer_id = c != null && c.is_deleted == false ? c.id : null,
                    customer_name = c != null && c.is_deleted == false ? c.name : null,
                    order_date = so.order_date,
                    delivery_date = so.delivery_date,
                    delivery_status = so.delivery_status,
                    status = so.status,
                    notes = so.notes,
                    last_updated = so.last_updated,
                }
            ).ToListAsync();
        }

        public async Task<SalesOrder> AddSalesOrderAsync(SalesOrder salesOrder)
        {
            var existingCustomer = await _context.customers.FirstOrDefaultAsync(c => c.id == salesOrder.customer_id && c.is_deleted == false);

            if (salesOrder.customer_id != null)
            {
                if (existingCustomer == null)
                {
                    Logger("Customer not found.");
                    throw new InvalidOperationException("Customer not found.");
                }
            }

            if (salesOrder.order_date == null || salesOrder.delivery_status == null || salesOrder.status == null)
            {
                Logger("Order date, delivery status and order status cannot be empty.");
                throw new InvalidOperationException("Order date, delivery status and order status cannot be empty.");
            }

            salesOrder.last_updated = DateTime.UtcNow;
            salesOrder.is_deleted = false;
            salesOrder.deleted_at = null;
            _context.sales_orders.Add(salesOrder);
            await _context.SaveChangesAsync();
            return salesOrder;
        }

        public async Task<SalesOrder> UpdateSalesOrderAsync(SalesOrder salesOrder)
        {
            var existingSalesOrder = await _context.sales_orders.FirstOrDefaultAsync(so => so.id == salesOrder.id && so.is_deleted == false);
            var existingCustomer = await _context.customers.FirstOrDefaultAsync(c => c.id == salesOrder.customer_id && c.is_deleted == false);

            if (salesOrder.customer_id != null)
            {
                if (existingCustomer == null)
                {
                    Logger("Customer not found.");
                    throw new InvalidOperationException("Customer not found.");
                }
            }

            if (existingSalesOrder == null)
            {
                Logger("Unable to update sales order. Check if it is not deleted.");
                throw new InvalidOperationException("Unable to update sales order. Check if it is not deleted.");
            }

            if (salesOrder.order_date == null || salesOrder.delivery_status == null || salesOrder.status == null)
            {
                Logger("Order date, delivery status and order status cannot be empty.");
                throw new InvalidOperationException("Order date, delivery status and order status cannot be empty.");
            }

            existingSalesOrder.customer_id = salesOrder.customer_id;
            existingSalesOrder.order_date = salesOrder.order_date;
            existingSalesOrder.delivery_date = salesOrder.delivery_date;
            existingSalesOrder.delivery_status = salesOrder.delivery_status;
            existingSalesOrder.status = salesOrder.status;
            existingSalesOrder.notes = salesOrder.notes;
            existingSalesOrder.last_updated = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existingSalesOrder;
        }

        public async Task<SalesOrder> SoftDeleteSalesOrderAsync(SalesOrder salesOrder)
        {
            var existingSalesOrder = await _context.sales_orders.FirstOrDefaultAsync(so => so.id == salesOrder.id && so.is_deleted == false);

            if (existingSalesOrder == null)
            {
                Logger("Unable to delete sales order. Sales order not found or already deleted.");
                throw new InvalidOperationException("Unable to delete sales order. Sales order not found or already deleted.");
            }

            existingSalesOrder.is_deleted = true;
            existingSalesOrder.deleted_at = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existingSalesOrder;
        }

        public async Task<SalesOrder> UndoSoftDeleteSalesOrderAsync(SalesOrder salesOrder)
        {
            var existingSalesOrder = await _context.sales_orders.FirstOrDefaultAsync(so => so.id == salesOrder.id && so.is_deleted == true);

            if (existingSalesOrder == null)
            {
                Logger("Unable to restore deleted sales order.");
                throw new InvalidOperationException("Unable to restore deleted sales order.");
            }

            existingSalesOrder.is_deleted = false;
            existingSalesOrder.deleted_at = null;
            await _context.SaveChangesAsync();
            return existingSalesOrder;
        }

        public async Task<IEnumerable<SalesOrderDTO>> GetAllDeletedSalesOrdersAsync()
        {
            return await
            (
                from so in _context.sales_orders
                join c in _context.customers on so.customer_id equals c.id into customerGroup
                from c in customerGroup.DefaultIfEmpty()
                where so.is_deleted == true
                select new SalesOrderDTO
                {
                    id = so.id,
                    customer_id = c != null && c.is_deleted == false ? c.id : null,
                    customer_name = c != null && c.is_deleted == false ? c.name : null,
                    order_date = so.order_date,
                    delivery_date = so.delivery_date,
                    delivery_status = so.delivery_status,
                    status = so.status,
                    notes = so.notes,
                    last_updated = so.last_updated,
                }
            ).ToListAsync();
        }
    }
}
