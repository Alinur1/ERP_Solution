using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.Services
{
    public class LedgerService : ILedgers
    {
        private readonly AppDataContext _context;
        public LedgerService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LedgerDTO>> GetAllLedgersAsync()
        {
            return await
            (
                from l in _context.ledgers
                join da in _context.accounts on l.debit_account_id equals da.id into daGroup
                from da in daGroup.DefaultIfEmpty()
                join ca in _context.accounts on l.credit_account_id equals ca.id into caGroup
                from ca in caGroup.DefaultIfEmpty()
                where l.is_deleted == false
                orderby l.entry_date descending, l.id descending // Show latest first
                select new LedgerDTO
                {
                    id = l.id,
                    entry_date = l.entry_date,
                    description = l.description,
                    debit_account_id = da != null ? da.id : null,
                    debit_account_name = da != null && da.is_deleted == false ? da.name : "Account Deleted",
                    credit_account_id = ca != null ? ca.id : null,
                    credit_account_name = ca != null && ca.is_deleted == false ? ca.name : "Account Deleted",
                    amount = l.amount,
                }
            ).ToListAsync();
        }

        public async Task<LedgerDTO> GetLedgerByIdAsync(int id)
        {
            return await
            (
                from l in _context.ledgers
                join da in _context.accounts on l.debit_account_id equals da.id into daGroup
                from da in daGroup.DefaultIfEmpty()
                join ca in _context.accounts on l.credit_account_id equals ca.id into caGroup
                from ca in caGroup.DefaultIfEmpty()
                where l.id == id && l.is_deleted == false
                select new LedgerDTO
                {
                    id = l.id,
                    entry_date = l.entry_date,
                    description = l.description,
                    debit_account_id = da != null ? da.id : null,
                    debit_account_name = da != null && da.is_deleted == false ? da.name : "Account Deleted",
                    credit_account_id = ca != null ? ca.id : null,
                    credit_account_name = ca != null && ca.is_deleted == false ? ca.name : "Account Deleted",
                    amount = l.amount,
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<Ledger> AddLedgerAsync(Ledger ledger)
        {
            // VALIDATION: Ensure accounts exist and are active
            var debitAccount = await _context.accounts.FirstOrDefaultAsync(a => a.id == ledger.debit_account_id && a.is_deleted == false);
            var creditAccount = await _context.accounts.FirstOrDefaultAsync(a => a.id == ledger.credit_account_id && a.is_deleted == false);

            if (debitAccount == null)
            {
                Logger($"Debit account ID {ledger.debit_account_id} not found or inactive.");
                throw new InvalidOperationException($"Debit account not found or inactive.");
            }

            if (creditAccount == null)
            {
                Logger($"Credit account ID {ledger.credit_account_id} not found or inactive.");
                throw new InvalidOperationException($"Credit account not found or inactive.");
            }

            // VALIDATION: Prevent same account for debit and credit
            if (ledger.debit_account_id == ledger.credit_account_id)
            {
                Logger("Debit and credit accounts cannot be the same.");
                throw new InvalidOperationException("Debit and credit accounts cannot be the same.");
            }

            // VALIDATION: Amount must be positive
            if (ledger.amount <= 0)
            {
                Logger("Ledger amount must be greater than zero.");
                throw new InvalidOperationException("Ledger amount must be greater than zero.");
            }

            // VALIDATION: Entry date cannot be in the future
            if (ledger.entry_date > DateTime.UtcNow)
            {
                Logger("Ledger entry date cannot be in the future.");
                throw new InvalidOperationException("Ledger entry date cannot be in the future.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                ledger.is_deleted = false;
                ledger.deleted_at = null;
                _context.ledgers.Add(ledger);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return ledger;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Logger($"Error adding ledger entry: {ex.Message}");
                throw new InvalidOperationException("Unable to add ledger entry.");
            }
        }
    }
}


//TODO: The entire ledger system is currently unusable. Proper implementation is needed. Commiting for now to avoid errors.