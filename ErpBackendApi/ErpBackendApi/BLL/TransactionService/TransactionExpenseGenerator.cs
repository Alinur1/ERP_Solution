using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.BLL.TransactionInterface;
using ErpBackendApi.DAL.Enums;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.TransactionService
{
    public class TransactionExpenseGenerator : ITransactionExpenseGenerator
    {
        private readonly AppDataContext _context;

        public TransactionExpenseGenerator(AppDataContext context)
        {
            _context = context;
        }

        private const int InventoryAccountId = 16;
        private const int AccountsPayableAccountId = 22;

        public async Task GenerateExpenseTransactionsAsync(Expense expense, string description)
        {
            var inventoryAccount = await _context.accounts.FirstOrDefaultAsync(a => a.id == InventoryAccountId && a.is_deleted == false);
            var accountsPayableAccount = await _context.accounts.FirstOrDefaultAsync(a => a.id == AccountsPayableAccountId && a.is_deleted == false);

            if (inventoryAccount == null || accountsPayableAccount == null)
            {
                Logger("Critical system accounts (Inventory or Accounts Payable) not found for expense transaction.");
                throw new InvalidOperationException("Critical system accounts not found. Cannot generate transactions.");
            }

            var debitTransaction = new Transaction
            {
                account_id = inventoryAccount.id,
                transaction_date = expense.expense_date ?? DateTime.UtcNow,
                description = $"EXP-{expense.id}: {description}",
                amount = expense.total_amount,
                normal_balance = DebitCreditType.Debit,
                is_deleted = false,
                deleted_at = null
            };

            var creditTransaction = new Transaction
            {
                account_id = accountsPayableAccount.id,
                transaction_date = expense.expense_date ?? DateTime.UtcNow,
                description = $"EXP-{expense.id}: {description}",
                amount = expense.total_amount,
                normal_balance = DebitCreditType.Credit,
                is_deleted = false,
                deleted_at = null
            };

            await _context.transactions.AddRangeAsync(debitTransaction, creditTransaction);
            await _context.SaveChangesAsync();
        }
    }
}