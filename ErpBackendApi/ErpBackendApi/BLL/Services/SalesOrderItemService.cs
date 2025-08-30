using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
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
                    sales_order_id = so != null && so.is_deleted == false ? so.id : null,
                    product_id = p != null && p.is_deleted == false ? p.id : null,
                    product_name = p != null && p.is_deleted == false ? p.name : null,
                    sku = p != null && p.is_deleted == false ? p.sku : null,
                    price = p != null && p.is_deleted == false ? p.price : null,
                    quantity = soi.quantity,
                    amount = soi.amount,
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
                    sales_order_id = so != null && so.is_deleted == false ? so.id : null,
                    product_id = p != null && p.is_deleted == false ? p.id : null,
                    product_name = p != null && p.is_deleted == false ? p.name : null,
                    sku = p != null && p.is_deleted == false ? p.sku : null,
                    price = p != null && p.is_deleted == false ? p.price : null,
                    quantity = soi.quantity,
                    amount = soi.amount,
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
                    sales_order_id = so != null && so.is_deleted == false ? so.id : null,
                    product_id = p != null && p.is_deleted == false ? p.id : null,
                    product_name = p != null && p.is_deleted == false ? p.name : null,
                    sku = p != null && p.is_deleted == false ? p.sku : null,
                    price = p != null && p.is_deleted == false ? p.price : null,
                    quantity = soi.quantity,
                    amount = soi.amount,
                    discount = soi.discount
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<SalesOrderItem> AddSalesOrderItemAsync(SalesOrderItem item)
        {
            var existingSalesOrderItem = await _context.sales_order_items.FirstOrDefaultAsync(sot => sot.sales_order_id == item.sales_order_id && sot.product_id == item.product_id && sot.is_deleted == false);
            var existingSalesOrder = await _context.sales_orders.FirstOrDefaultAsync(so => so.id == item.sales_order_id && so.is_deleted == false);
            var existingProduct = await _context.products.FirstOrDefaultAsync(p => p.id == item.product_id && p.is_deleted == false);

            if (existingSalesOrderItem != null)
            {
                Logger("This product is already added to the order.");
                throw new InvalidOperationException("This product is already added to the order.");
            }

            if (existingSalesOrder == null)
            {
                Logger("Order ID not found or is deleted.");
                throw new InvalidOperationException("Order ID not found or is deleted.");
            }

            if (existingProduct == null)
            {
                Logger("Product not found or is deleted.");
                throw new InvalidOperationException("Product not found or is deleted.");
            }

            if (item.quantity < 0 || item.amount < 0 || item.discount < 0)
            {
                Logger("Quantity, amount, or discount cannot be negative.");
                throw new InvalidOperationException("Quantity, amount, or discount cannot be negative.");
            }

            if (item.quantity == null || item.amount == null)
            {
                Logger("Quantity or amount cannot be empty.");
                throw new InvalidOperationException("Quantity or amount cannot be empty.");
            }

            item.amount = item.quantity * existingProduct.price;
            item.is_deleted = false;
            item.deleted_at = null;

            _context.sales_order_items.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }


        public async Task<SalesOrderItem> UpdateSalesOrderItemAsync(SalesOrderItem item)
        {
            var existingSalesOrderItem = await _context.sales_order_items.FirstOrDefaultAsync(soi => soi.id == item.id && soi.is_deleted == false);
            var existingProduct = await _context.products.FirstOrDefaultAsync(p => p.id == item.product_id && p.is_deleted == false);

            if (existingSalesOrderItem == null)
            {
                Logger("Sales order item not found or is deleted.");
                throw new InvalidOperationException("Sales order item not found or is deleted.");
            }

            if (existingProduct == null)
            {
                Logger("Product not found or is deleted.");
                throw new InvalidOperationException("Product not found or is deleted.");
            }

            if (item.quantity < 0 || item.amount < 0 || item.discount < 0)
            {
                Logger("Quantity, amount, or discount cannot be negative.");
                throw new InvalidOperationException("Quantity, amount, or discount cannot be negative.");
            }

            if (item.quantity == null || item.amount == null)
            {
                Logger("Quantity or amount cannot be empty.");
                throw new InvalidOperationException("Quantity or amount cannot be empty.");
            }

            existingSalesOrderItem.product_id = item.product_id;
            existingSalesOrderItem.quantity = item.quantity;
            existingSalesOrderItem.discount = item.discount;
            existingSalesOrderItem.amount = item.quantity * existingProduct.price;
            await _context.SaveChangesAsync();
            return existingSalesOrderItem;
        }

        public async Task<SalesOrderItem> SoftDeleteSalesOrderItemAsync(SalesOrderItem item)
        {
            var existingSalesOrderItem = await _context.sales_order_items.FirstOrDefaultAsync(soi => soi.id == item.id && soi.is_deleted == false);

            if (existingSalesOrderItem == null)
            {
                Logger("Unable to delete sales order item.");
                throw new InvalidOperationException("Unable to delete sales order item.");
            }

            existingSalesOrderItem.is_deleted = true;
            existingSalesOrderItem.deleted_at = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existingSalesOrderItem;
        }

        public async Task<SalesOrderItem> UndoSoftDeleteSalesOrderItemAsync(SalesOrderItem item)
        {
            var existingSalesOrderItem = await _context.sales_order_items.FirstOrDefaultAsync(soi => soi.id == item.id && soi.is_deleted == true);

            if (existingSalesOrderItem == null)
            {
                Logger("Unable to delete sales order item.");
                throw new InvalidOperationException("Unable to delete sales order item.");
            }

            existingSalesOrderItem.is_deleted = false;
            existingSalesOrderItem.deleted_at = null;
            await _context.SaveChangesAsync();
            return existingSalesOrderItem;
        }

        public async Task<IEnumerable<SalesOrderItemDTO>> GetAllDeletedSalesOrderItemAsync()
        {
            return await
            (
                from soi in _context.sales_order_items
                join so in _context.sales_orders on soi.sales_order_id equals so.id into salesOrderGroup
                from so in salesOrderGroup.DefaultIfEmpty()
                join p in _context.products on soi.product_id equals p.id into productGroup
                from p in productGroup.DefaultIfEmpty()
                where soi.is_deleted == true
                select new SalesOrderItemDTO
                {
                    id = soi.id,
                    sales_order_id = so != null && so.is_deleted == false ? so.id : null,
                    product_id = p != null && p.is_deleted == false ? p.id : null,
                    product_name = p != null && p.is_deleted == false ? p.name : null,
                    sku = p != null && p.is_deleted == false ? p.sku : null,
                    price = p != null && p.is_deleted == false ? p.price : null,
                    quantity = soi.quantity,
                    amount = soi.amount,
                    discount = soi.discount,
                    deleted_at = soi.deleted_at,
                }
            ).ToListAsync();
        }
    }
}
