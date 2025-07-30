using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.Services
{
    public class TransactionService : ITransaction
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
                    account_id = a.id,
                    account_name = a.name,
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
                    account_id = a.id,
                    account_name = a.name,
                    transaction_date = t.transaction_date,
                    description = t.description,
                    amount = t.amount,
                    type = t.type,
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<Transaction> AddTransactionAsync(Transaction transaction)
        {
            transaction.is_deleted = false;
            transaction.deleted_at = null;
            _context.transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction> UpdateTransactionAsync(Transaction transaction)
        {
            //var existingTransaction = await _context.transactions.FirstOrDefaultAsync(t => t.id == transaction.id && t.is_deleted == false);
            //if (existingTransaction == null)
            //{
            //    Logger("Unable to update transaction information. Transaction not found.");
            //    return null;
            //}
            throw new NotImplementedException();
        }

        public Task<Transaction> SoftDeleteTransactionAsync(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public Task<Transaction> UndoSoftDeleteTransactionAsync(Transaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}
