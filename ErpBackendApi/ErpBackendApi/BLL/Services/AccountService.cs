using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.Services
{
    public class AccountService : IAccount
    {
        private readonly AppDataContext _context;
        public AccountService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            return await _context.accounts
                .Where(a => a.is_deleted == false)
                .ToListAsync();
        }

        public async Task<Account> GetAccountByIdAsync(int id)
        {
            return await _context.accounts.FirstOrDefaultAsync(a => a.id == id && a.is_deleted == false);
        }

        public async Task<Account> AddAccountAsync(Account account)
        {
            account.is_deleted = false;
            account.deleted_at = null;
            _context.accounts.Add(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<Account> UpdateAccountAsync(Account account)
        {
            var existingAccount = await _context.accounts.FirstOrDefaultAsync(a => a.id == account.id && a.is_deleted == false);
            if (existingAccount == null)
            {
                Logger("Unable to update account details. Account not found.");
                return null;
            }
            existingAccount.name = account.name;
            existingAccount.type = account.type;
            await _context.SaveChangesAsync();
            return existingAccount;
        }

        public async Task<Account> SoftDeleteAccountAsync(Account account)
        {
            var existingAccount = await _context.accounts.FirstOrDefaultAsync(a => a.id == account.id && a.is_deleted == false);
            if (existingAccount == null)
            {
                Logger("Unable to delete account details. Account not found.");
                return null;
            }
            existingAccount.is_deleted = true;
            existingAccount.deleted_at = DateTime.UtcNow;
            _context.accounts.Update(existingAccount);
            await _context.SaveChangesAsync();
            return existingAccount;
        }

        public async Task<Account> UndoSoftDeleteAccountAsync(Account account)
        {
            var existingAccount = await _context.accounts.FirstOrDefaultAsync(a => a.id == account.id && a.is_deleted == true);
            if (existingAccount == null)
            {
                Logger("Unable to restore account details. Account not found.");
                return null;
            }
            existingAccount.is_deleted = false;
            existingAccount.deleted_at = null;
            _context.accounts.Update(existingAccount);
            await _context.SaveChangesAsync();
            return existingAccount;
        }
    }
}
