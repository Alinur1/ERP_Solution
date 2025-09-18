using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.TransactionInterface
{
    public interface ITransactionGenerator
    {
        Task GenerateInvoiceTransactionsAsync(Invoice invoice, string description);
        Task ReverseInvoiceTransactionsAsync(int invoiceId, string reason);
    }
}
