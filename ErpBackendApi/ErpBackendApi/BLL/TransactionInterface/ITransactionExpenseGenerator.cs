using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.TransactionInterface
{
    public interface ITransactionExpenseGenerator
    {
        Task GenerateExpenseTransactionsAsync(Expense expense, string description);
    }
}
