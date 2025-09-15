using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.Enums;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.Services
{
    public class TransactionService : ITransactions
    {
        private readonly AppDataContext _context;
        public TransactionService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TransactionDTO>> GetAllTransactionsAsync()
        {
            return await
            (
                from t in _context.transactions
                join a in _context.accounts on t.account_id equals a.id into accGroup
                from a in accGroup.DefaultIfEmpty()
                where t.is_deleted == false
                select new TransactionDTO
                {
                    id = t.id,
                    account_id = a != null && a.is_deleted == false ? a.id : null,
                    account_name = a != null && a.is_deleted == false ? a.name : a.name + " (Deleted account)",
                    transaction_date = t.transaction_date,
                    description = t.description,
                    amount = t.amount,
                    type = t.type,
                }
            ).ToListAsync();
        }

        public async Task<TransactionDTO> GetTransactionByIdAsync(int id)
        {
            return await
            (
                from t in _context.transactions
                join a in _context.accounts on t.account_id equals a.id into accGroup
                from a in accGroup.DefaultIfEmpty()
                where t.id == id && t.is_deleted == false
                select new TransactionDTO
                {
                    id = t.id,
                    account_id = a != null && a.is_deleted == false ? a.id : null,
                    account_name = a != null && a.is_deleted == false ? a.name : a.name + " (Deleted account)",
                    transaction_date = t.transaction_date,
                    description = t.description,
                    amount = t.amount,
                    type = t.type,
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<Transaction> AddTransactionAsync(Transaction transaction)
        {
            var existingAccount = await _context.accounts.FirstOrDefaultAsync(a => a.id == transaction.account_id && a.is_deleted == false);
            if (existingAccount == null)
            {
                Logger("Account not found or inactive.");
                throw new InvalidOperationException("Account not found or inactive.");
            }

            if (transaction.type == null)
            {
                Logger("Transaction type is required.");
                throw new InvalidOperationException("Transaction type is required.");
            }

            if (!Enum.IsDefined(typeof(TransactionType), transaction.type.Value))
            {
                Logger($"Invalid transaction type. Valid values: {string.Join(", ", Enum.GetValues(typeof(TransactionType)).Cast<int>())}");
                throw new InvalidOperationException($"Invalid transaction type. Valid values: {string.Join(", ", Enum.GetValues(typeof(TransactionType)).Cast<int>())}");
            }

            if (transaction.amount <= 0)
            {
                Logger("Transaction amount must be greater than zero.");
                throw new InvalidOperationException("Transaction amount must be greater than zero.");
            }

            if (transaction.transaction_date > DateTime.UtcNow)
            {
                Logger("Transaction date cannot be in the future.");
                throw new InvalidOperationException("Transaction date cannot be in the future.");
            }

            using var DataTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                transaction.is_deleted = false;
                transaction.deleted_at = null;
                _context.transactions.Add(transaction);
                await _context.SaveChangesAsync();
                await DataTransaction.CommitAsync();
                return transaction;
            }
            catch
            {
                await DataTransaction.RollbackAsync();
                throw new InvalidOperationException("Unable to add transaction.");
            }
        }

        public async Task<Transaction> UpdateTransactionAsync(Transaction transaction)
        {
            var existingTransaction = await _context.transactions.FirstOrDefaultAsync(t => t.id == transaction.id && t.is_deleted == false);
            if (existingTransaction == null)
            {
                Logger("Transaction not found or deleted.");
                throw new InvalidOperationException("Transaction not found or deleted.");
            }

            if (transaction.account_id.HasValue && transaction.account_id != existingTransaction.account_id)
            {
                var existingAccount = await _context.accounts.FirstOrDefaultAsync(a => a.id == transaction.account_id && a.is_deleted == false);
                if (existingAccount == null)
                {
                    Logger("Account not found or inactive.");
                    throw new InvalidOperationException("Account not found or inactive.");
                }
            }

            if (transaction.type == null)
            {
                Logger("Transaction type is required.");
                throw new InvalidOperationException("Transaction type is required.");
            }

            if (!Enum.IsDefined(typeof(TransactionType), transaction.type.Value))
            {
                Logger($"Invalid transaction type. Valid values: {string.Join(", ", Enum.GetValues(typeof(TransactionType)).Cast<int>())}");
                throw new InvalidOperationException($"Invalid transaction type. Valid values: {string.Join(", ", Enum.GetValues(typeof(TransactionType)).Cast<int>())}");
            }

            if (transaction.amount <= 0)
            {
                Logger("Transaction amount must be greater than zero.");
                throw new InvalidOperationException("Transaction amount must be greater than zero.");
            }

            if (transaction.transaction_date > DateTime.UtcNow)
            {
                Logger("Transaction date cannot be in the future.");
                throw new InvalidOperationException("Transaction date cannot be in the future.");
            }

            using var DataTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                existingTransaction.account_id = transaction.account_id ?? existingTransaction.account_id;
                existingTransaction.transaction_date = transaction.transaction_date ?? existingTransaction.transaction_date;
                existingTransaction.description = transaction.description ?? existingTransaction.description;
                existingTransaction.amount = transaction.amount ?? existingTransaction.amount;
                existingTransaction.type = transaction.type ?? existingTransaction.type;

                await _context.SaveChangesAsync();
                await DataTransaction.CommitAsync();
                return existingTransaction;
            }
            catch
            {
                await DataTransaction.RollbackAsync();
                throw new InvalidOperationException("Unable to update transaction.");
            }
        }

        public async Task<Transaction> SoftDeleteTransactionAsync(Transaction transaction)
        {
            var existingTransaction = await _context.transactions.FirstOrDefaultAsync(t => t.id == transaction.id && t.is_deleted == false);
            if (existingTransaction == null)
            {
                Logger("Unable to delete transaction information. Transaction not found.");
                throw new InvalidOperationException("Unable to delete transaction information. Transaction not found.");
            }

            using var DataTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                existingTransaction.is_deleted = true;
                existingTransaction.deleted_at = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                await DataTransaction.CommitAsync();
                return existingTransaction;
            }
            catch
            {
                await DataTransaction.RollbackAsync();
                throw new InvalidOperationException("Unable to delete transaction.");
            }
        }

        public async Task<Transaction> UndoSoftDeleteTransactionAsync(Transaction transaction)
        {
            var existingTransaction = await _context.transactions.FirstOrDefaultAsync(t => t.id == transaction.id && t.is_deleted == true);
            if (existingTransaction == null)
            {
                Logger("Unable to restore deleted transaction information. Transaction not found.");
                throw new InvalidOperationException("Unable to restore deleted transaction information. Transaction not found.");
            }

            using var DataTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                existingTransaction.is_deleted = false;
                existingTransaction.deleted_at = null;
                await _context.SaveChangesAsync();
                await DataTransaction.CommitAsync();
                return existingTransaction;
            }
            catch
            {
                await DataTransaction.RollbackAsync();
                throw new InvalidOperationException("Unable to restore deleted transaction.");
            }
        }
    }
}
