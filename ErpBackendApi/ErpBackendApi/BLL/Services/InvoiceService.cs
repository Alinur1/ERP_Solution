using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.BLL.TransactionInterface;
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
        private readonly ITransactionInvoiceGenerator _transactionGenerator;
        public InvoiceService(AppDataContext context, ITransactionInvoiceGenerator transactionGenerator)
        {
            _context = context;
            _transactionGenerator = transactionGenerator;
        }

        public async Task<IEnumerable<InvoiceDTO>> GetAllInvoiceAsync()
        {
            var invoices = await _context.invoices
                .Where(i => i.is_deleted == false)
                .Select(i => new InvoiceDTO
                {
                    id = i.id,
                    invoice_date = i.invoice_date,
                    total_amount = i.total_amount,
                    is_paid = i.is_paid,
                    due_date = i.due_date,
                    is_deleted = i.is_deleted,
                    deleted_at = i.deleted_at,

                    sales_order = _context.sales_orders
                        .Where(so => so.id == i.sales_order_id && so.is_deleted == false)
                        .Select(so => new SalesOrderDTO
                        {
                            id = so.id,
                            order_date = so.order_date,
                            delivery_date = so.delivery_date,
                            delivery_status = so.delivery_status,
                            status = so.status,
                            notes = so.notes,
                            last_updated = so.last_updated,

                            customer = _context.customers
                                .Where(c => c.id == so.customer_id && c.is_deleted == false)
                                .Select(c => new CustomerDTO
                                {
                                    id = c.id,
                                    name = c.name,
                                    email = c.email,
                                    phone = c.phone
                                })
                                .FirstOrDefault(),

                            items = _context.sales_order_items
                                .Where(soi => soi.sales_order_id == so.id && soi.is_deleted == false)
                                .Select(soi => new SalesOrderItemDTO
                                {
                                    id = soi.id,
                                    quantity = soi.quantity,
                                    amount = soi.amount,
                                    discount = soi.discount,
                                    product = _context.products
                                        .Where(p => p.id == soi.product_id && p.is_deleted == false)
                                        .Select(p => new ProductDTO
                                        {
                                            id = p.id,
                                            name = p.name,
                                            sku = p.sku,
                                            unit = p.unit,
                                            price = p.price
                                        })
                                        .FirstOrDefault()
                                })
                                .ToList()
                        })
                        .FirstOrDefault()
                })
                .ToListAsync();

            return invoices;
        }

        public async Task<InvoiceDTO> GetInvoiceByIdAsync(int id)
        {
            return await GetAllInvoiceAsync().ContinueWith(t => t.Result.FirstOrDefault(i => i.id == id));
        }

        public async Task<InvoiceDTO> GetInvoiceByOrderIdAsync(int orderId)
        {
            return await GetAllInvoiceAsync().ContinueWith(t => t.Result.FirstOrDefault(i => i.sales_order != null && i.sales_order.id == orderId));
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

                // Calculate invoice total
                invoice.total_amount = orderItems.Sum(item =>
                {
                    var discount = item.discount ?? 0;
                    var lineTotal = item.amount ?? 0;
                    return discount <= 1
                        ? lineTotal - (lineTotal * discount)
                        : lineTotal - discount;
                });

                invoice.is_deleted = false;
                invoice.deleted_at = null;

                // SAVE INVOICE FIRST TO GET ID
                _context.invoices.Add(invoice);
                await _context.SaveChangesAsync();

                // GENERATE AUTOMATIC TRANSACTIONS!
                string description = $"Sales Order #{invoice.sales_order_id}";
                await _transactionGenerator.GenerateInvoiceTransactionsAsync(invoice, description);

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
    }
}
