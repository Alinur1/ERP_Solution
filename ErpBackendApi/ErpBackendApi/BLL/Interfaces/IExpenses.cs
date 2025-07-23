using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface IExpenses
    {
        Task<IEnumerable<ExpenseDTO>> GetAllExpenseAsync();
        Task<ExpenseDTO> GetExpenseByIdAsync(int id);
        Task<Expense> AddExpenseAsync(Expense expense);
        Task<Expense> UpdateExpenseAsync(Expense expense);
        Task<Expense> SoftDeleteExpenseAsync(Expense expense);
        Task<Expense> UndoSoftDeleteExpenseAsync(Expense expense);
    }
}
