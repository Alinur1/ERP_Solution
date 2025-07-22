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
            var existingInvoice = await _context.invoices.FirstOrDefaultAsync(i => i.sales_order_id == invoice.sales_order_id && i.is_deleted == false);
            if (existingInvoice != null)
            {
                Logger("Same sales order cannot be added to an invoice.");
                return null;
            }
            invoice.is_deleted = false;
            invoice.deleted_at = null;
            _context.invoices.Add(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }

        public async Task<Invoice> UpdateInvoiceAsync(Invoice invoice)
        {
            var existingInvoice = await _context.invoices.FirstOrDefaultAsync(i => i.id == invoice.id && i.is_deleted == false);
            if (existingInvoice == null)
            {
                Logger("Invoice not found or deleted. Unable to update invoice.");
                return null;
            }
            existingInvoice.sales_order_id = invoice.sales_order_id;
            existingInvoice.invoice_date = invoice.invoice_date;
            existingInvoice.total_amount = invoice.total_amount;
            existingInvoice.is_paid = invoice.is_paid;
            existingInvoice.due_date = invoice.due_date;
            _context.invoices.Update(existingInvoice);
            await _context.SaveChangesAsync();
            return existingInvoice;
        }

        public async Task<Invoice> SoftDeleteInvoiceAsync(Invoice invoice)
        {
            var existingInvoice = await _context.invoices.FirstOrDefaultAsync(i => i.id == invoice.id && i.is_deleted == false);
            if (existingInvoice == null)
            {
                Logger("Invoice not found or deleted. Unable to delete invoice.");
                return null;
            }
            existingInvoice.is_deleted = true;
            existingInvoice.deleted_at = DateTime.UtcNow;
            _context.invoices.Update(existingInvoice);
            await _context.SaveChangesAsync();
            return existingInvoice;
        }

        public async Task<Invoice> UndoSoftDeleteInvoiceAsync(Invoice invoice)
        {
            var existingInvoice = await _context.invoices.FirstOrDefaultAsync(i => i.id == invoice.id && i.is_deleted == true);
            if (existingInvoice == null)
            {
                Logger("Invoice not found. Unable to restore deleted invoice.");
                return null;
            }
            existingInvoice.is_deleted = false;
            existingInvoice.deleted_at = null;
            _context.invoices.Update(existingInvoice);
            await _context.SaveChangesAsync();
            return existingInvoice;
        }
    }
}
