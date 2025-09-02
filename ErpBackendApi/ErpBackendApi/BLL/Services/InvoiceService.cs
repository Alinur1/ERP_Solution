using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.Services
{
    public class InvoiceService : IInvoices
    {
        private readonly AppDataContext _context;
        public InvoiceService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InvoiceDTO>> GetAllInvoiceAsync()
        {
            return await
            (
                from i in _context.invoices
                join so in _context.sales_orders on i.sales_order_id equals so.id into salesOrdersGroup
                from so in salesOrdersGroup.DefaultIfEmpty()
                where i.is_deleted == false
                select new InvoiceDTO
                {
                    id = i.id,
                    sales_order_id = so != null ? so.id : null,
                    invoice_date = i.invoice_date,
                    total_amount = i.total_amount,
                    is_paid = i.is_paid,
                    due_date = i.due_date
                }
            ).ToListAsync();
        }

        public async Task<InvoiceDTO> GetInvoiceByIdAsync(int id)
        {
            return await
            (
                from i in _context.invoices
                join so in _context.sales_orders on i.sales_order_id equals so.id into salesOrdersGroup
                from so in salesOrdersGroup.DefaultIfEmpty()
                where i.id == id && i.is_deleted == false
                select new InvoiceDTO
                {
                    id = i.id,
                    sales_order_id = so != null ? so.id : null,
                    invoice_date = i.invoice_date,
                    total_amount = i.total_amount,
                    is_paid = i.is_paid,
                    due_date = i.due_date
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<InvoiceDTO> GetInvoiceByOrderIdAsync(int orderId)
        {
            return await
            (
                from i in _context.invoices
                join so in _context.sales_orders on i.sales_order_id equals so.id into salesOrdersGroup
                from so in salesOrdersGroup.DefaultIfEmpty()
                where so.id == orderId && i.is_deleted == false
                select new InvoiceDTO
                {
                    id = i.id,
                    sales_order_id = so != null ? so.id : null,
                    invoice_date = i.invoice_date,
                    total_amount = i.total_amount,
                    is_paid = i.is_paid,
                    due_date = i.due_date
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<Invoice> AddInvoiceAsync(Invoice invoice)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingInvoice = await _context.invoices.FirstOrDefaultAsync(i => i.sales_order_id == invoice.sales_order_id && i.is_deleted == false);

                if (existingInvoice != null)
                {
                    Logger("Same sales order cannot be added to an invoice.");
                    throw new InvalidOperationException("Same sales order cannot be added to an invoice.");
                }

                var orderItems = await _context.sales_order_items
                    .Where(soi => soi.sales_order_id == invoice.sales_order_id && soi.is_deleted == false)
                    .ToListAsync();

                if (orderItems == null || orderItems.Count == 0)
                {
                    Logger("Cannot create invoice without sales order items.");
                    throw new InvalidOperationException("Cannot create invoice without sales order items.");
                }

                // Validate and deduct inventory
                foreach (var item in orderItems)
                {
                    var inventory = await _context.inventory.FirstOrDefaultAsync(inv => inv.product_id == item.product_id && inv.is_deleted == false);

                    if (inventory == null)
                    {
                        Logger($"Inventory not found for product {item.product_id}.");
                        throw new InvalidOperationException($"Inventory not found for product {item.product_id}.");
                    }

                    if (inventory.quantity < item.quantity)
                    {
                        Logger($"Not enough stock for product {item.product_id}.");
                        throw new InvalidOperationException($"Not enough stock for product {item.product_id}.");
                    }

                    inventory.quantity -= item.quantity;
                    inventory.last_updated = DateTime.UtcNow;
                }

                // Calculate invoice total (flat or percent discount)
                invoice.total_amount = orderItems.Sum(item =>
                {
                    var discount = item.discount ?? 0;
                    var lineTotal = item.amount ?? 0;
                    return discount <= 1
                        ? lineTotal - (lineTotal * discount)   // treat <=1 as percentage
                        : lineTotal - discount;                // treat >1 as flat amount
                });

                invoice.is_deleted = false;
                invoice.deleted_at = null;
                _context.invoices.Add(invoice);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return invoice;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                Logger($"Concurrency conflict in AddInvoiceAsync: {ex.Message}");
                throw new InvalidOperationException("Stock was modified by another transaction. Please try again.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Logger($"Error in AddInvoiceAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<Invoice> UpdateInvoiceAsync(Invoice invoice)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingInvoice = await _context.invoices.FirstOrDefaultAsync(i => i.id == invoice.id && i.is_deleted == false);

                if (existingInvoice == null)
                {
                    Logger("Invoice not found or deleted. Unable to update.");
                    throw new InvalidOperationException("Invoice not found or deleted. Unable to update.");
                }

                existingInvoice.invoice_date = invoice.invoice_date;
                existingInvoice.is_paid = invoice.is_paid;
                existingInvoice.due_date = invoice.due_date;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return existingInvoice;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Logger($"Error in UpdateInvoiceAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<Invoice> SoftDeleteInvoiceAsync(Invoice invoice)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingInvoice = await _context.invoices.FirstOrDefaultAsync(i => i.id == invoice.id && i.is_deleted == false);

                if (existingInvoice == null)
                {
                    Logger("Invoice not found or deleted. Unable to delete.");
                    throw new InvalidOperationException("Invoice not found or deleted. Unable to delete.");
                }

                var orderItems = await _context.sales_order_items
                    .Where(soi => soi.sales_order_id == existingInvoice.sales_order_id && soi.is_deleted == false)
                    .ToListAsync();

                foreach (var item in orderItems)
                {
                    var inventory = await _context.inventory.FirstOrDefaultAsync(inv => inv.product_id == item.product_id && inv.is_deleted == false);

                    if (inventory != null)
                    {
                        inventory.quantity += item.quantity;
                        inventory.last_updated = DateTime.UtcNow;
                    }
                }

                existingInvoice.is_deleted = true;
                existingInvoice.deleted_at = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return existingInvoice;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Logger($"Error in SoftDeleteInvoiceAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<Invoice> UndoSoftDeleteInvoiceAsync(Invoice invoice)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingInvoice = await _context.invoices.FirstOrDefaultAsync(i => i.id == invoice.id && i.is_deleted == true);

                if (existingInvoice == null)
                {
                    Logger("Invoice not found. Unable to restore.");
                    throw new InvalidOperationException("Invoice not found. Unable to restore.");
                }

                var orderItems = await _context.sales_order_items
                    .Where(soi => soi.sales_order_id == existingInvoice.sales_order_id && soi.is_deleted == false)
                    .ToListAsync();

                foreach (var item in orderItems)
                {
                    var inventory = await _context.inventory.FirstOrDefaultAsync(inv => inv.product_id == item.product_id && inv.is_deleted == false);

                    if (inventory == null || inventory.quantity < item.quantity)
                    {
                        Logger($"Not enough stock to restore invoice for product {item.product_id}.");
                        throw new InvalidOperationException($"Not enough stock to restore invoice for product {item.product_id}.");
                    }

                    inventory.quantity -= item.quantity;
                    inventory.last_updated = DateTime.UtcNow;
                }

                existingInvoice.is_deleted = false;
                existingInvoice.deleted_at = null;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return existingInvoice;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Logger($"Error in UndoSoftDeleteInvoiceAsync: {ex.Message}");
                throw;
            }
        }
    }
}
