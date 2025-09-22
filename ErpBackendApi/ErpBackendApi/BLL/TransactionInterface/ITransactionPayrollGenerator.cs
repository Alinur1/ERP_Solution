using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.TransactionInterface
{
    public interface ITransactionPayrollGenerator
    {
        Task GeneratePayrollTransactionsAsync(Payroll payroll, string description);
    }
}
