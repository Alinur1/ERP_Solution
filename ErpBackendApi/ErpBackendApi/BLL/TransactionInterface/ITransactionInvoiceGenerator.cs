using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.TransactionInterface
{
    public interface ITransactionInvoiceGenerator
    {
        Task GenerateInvoiceTransactionsAsync(Invoice invoice, string description);
    }
}
