using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface IInvoices
    {
        Task<IEnumerable<InvoiceDTO>> GetAllInvoiceAsync();
        Task<InvoiceDTO> GetInvoiceByIdAsync(int id);
        Task<InvoiceDTO> GetInvoiceByOrderIdAsync(int orderId);
        Task<Invoice> AddInvoiceAsync(Invoice invoice);
        Task<Invoice> UpdateInvoiceAsync(Invoice invoice);
        Task<Invoice> SoftDeleteInvoiceAsync(Invoice invoice);
        Task<Invoice> UndoSoftDeleteInvoiceAsync(Invoice invoice);
    }
}
