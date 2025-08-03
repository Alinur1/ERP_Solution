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
                select new LedgerDTO
                {
                    id = l.id,
                    entry_date = l.entry_date,
                    description = l.description,
                    debit_account_id = da != null ? da.id : null,
                    debit_account_name = da != null && da.is_deleted == false ? da.name : null,
                    credit_account_id = ca != null ? ca.id : null,
                    credit_account_name = ca != null && ca.is_deleted == false ? ca.name : null,
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
                    debit_account_name = da != null && da.is_deleted == false ? da.name : null,
                    credit_account_id = ca != null ? ca.id : null,
                    credit_account_name = ca != null && ca.is_deleted == false ? ca.name : null,
                    amount = l.amount,
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<Ledger> AddLedgerAsync(Ledger ledger)
        {
            var checkDebitAccount = await _context.accounts.FirstOrDefaultAsync(a => a.id == ledger.debit_account_id && a.is_deleted == false);
            var checkCreditAccount = await _context.accounts.FirstOrDefaultAsync(a => a.id == ledger.credit_account_id && a.is_deleted == false);
            if (checkDebitAccount == null)
            {
                Logger("Invalid account id for adding debit information.");
                return null;
            }
            if (checkCreditAccount == null)
            {
                Logger("Invalid account id for adding credit information.");
                return null;
            }
            if(checkDebitAccount == checkCreditAccount)
            {
                Logger("Debit and credit accounts cannot be the same.");
                return null;
            }
            ledger.is_deleted = false;
            ledger.deleted_at = null;
            _context.ledgers.Add(ledger);
            await _context.SaveChangesAsync();
            return ledger;
        }

        public async Task<Ledger> UpdateLedgerAsync(Ledger ledger)
        {
            var existingLedger = await _context.ledgers.FirstOrDefaultAsync(l => l.id == ledger.id && l.is_deleted == false);
            if (existingLedger == null)
            {
                Logger("Unable to update ledger information. Invalid ledger ID.");
                return null;
            }
            existingLedger.entry_date = ledger.entry_date;
            existingLedger.description = ledger.description;
            existingLedger.debit_account_id = ledger.debit_account_id;
            existingLedger.credit_account_id = ledger.credit_account_id;
            existingLedger.amount = ledger.amount;
            await _context.SaveChangesAsync();
            return existingLedger;
        }

        public async Task<Ledger> SoftDeleteLedgerAsync(Ledger ledger)
        {
            var existingLedger = await _context.ledgers.FirstOrDefaultAsync(l => l.id == ledger.id && l.is_deleted == false);
            if (existingLedger == null)
            {
                Logger("Unable to delete ledger information. Invalid ledger ID.");
                return null;
            }
            existingLedger.is_deleted = true;
            existingLedger.deleted_at = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existingLedger;
        }

        public async Task<Ledger> UndoSoftDeleteLedgerAsync(Ledger ledger)
        {
            var existingLedger = await _context.ledgers.FirstOrDefaultAsync(l => l.id == ledger.id && l.is_deleted == true);
            if (existingLedger == null)
            {
                Logger("Unable to restore deleted ledger information. Invalid ledger ID.");
                return null;
            }
            existingLedger.is_deleted = false;
            existingLedger.deleted_at = null;
            await _context.SaveChangesAsync();
            return existingLedger;
        }
    }
}
