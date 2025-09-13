using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.DTOs;
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
                Logger("Unable to update transaction information. Transaction not found.");
                throw new InvalidOperationException("Unable to update transaction information. Transaction not found.");
            }

            using var DataTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                existingTransaction.transaction_date = transaction.transaction_date;
                existingTransaction.description = transaction.description;
                existingTransaction.amount = transaction.amount;
                existingTransaction.type = transaction.type;
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
