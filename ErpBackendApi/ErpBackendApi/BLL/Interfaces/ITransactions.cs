using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface ITransactions
    {
        Task<IEnumerable<TransactionDTO>> GetAllTransactionsAsync();
        Task<TransactionDTO> GetTransactionByIdAsync(int id);
        Task<Transaction> AddTransactionAsync(Transaction transaction);
        Task<Transaction> UpdateTransactionAsync(Transaction transaction);
        Task<Transaction> SoftDeleteTransactionAsync(Transaction transaction);
        Task<Transaction> UndoSoftDeleteTransactionAsync(Transaction transaction);
    }
}
