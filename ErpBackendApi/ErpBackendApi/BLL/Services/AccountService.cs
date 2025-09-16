using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.Enums;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.Services
{
    public class AccountService : IAccounts
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
                    .OrderBy(a => a.type)
                    .ThenBy(a => a.name)
                    .ToListAsync();
        }

        public async Task<Account> GetAccountByIdAsync(int id)
        {
            return await _context.accounts
                    .Where(a => a.id == id && a.is_deleted == false)
                    .FirstOrDefaultAsync();
        }

        public async Task<Account> AddAccountAsync(Account account)
        {
            var existingAccount = await _context.accounts.FirstOrDefaultAsync(a => a.name == account.name && a.is_deleted == false);
            if (existingAccount != null)
            {
                Logger("Account with this name already exists.");
                throw new InvalidOperationException("Account with this name already exists.");
            }

            if (string.IsNullOrWhiteSpace(account.name))
            {
                Logger("Account name cannot be empty.");
                throw new InvalidOperationException("Account name cannot be empty.");
            }

            if (account.type == null)
            {
                Logger("Account type is required.");
                throw new InvalidOperationException("Account type is required.");
            }

            if (!Enum.IsDefined(typeof(AccountType), account.type.Value))
            {
                Logger($"Invalid account type. Valid values are: {string.Join(", ", Enum.GetValues(typeof(AccountType)).Cast<int>())}");
                throw new InvalidOperationException($"Invalid account type. Valid values are: {string.Join(", ", Enum.GetValues(typeof(AccountType)).Cast<int>())}");
            }

            if (account.normal_balance == null)
            {
                account.normal_balance = account.type switch
                {
                    AccountType.Asset => DebitCreditType.Debit,      // 0
                    AccountType.Liability => DebitCreditType.Credit, // 1
                    AccountType.Equity => DebitCreditType.Credit,    // 1
                    AccountType.Income => DebitCreditType.Credit,    // 1
                    AccountType.Expense => DebitCreditType.Debit,    // 0
                    _ => throw new InvalidOperationException("Invalid account type")
                };
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                account.is_deleted = false;
                account.deleted_at = null;
                _context.accounts.Add(account);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return account;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Unable to add account.");
            }
        }

        public async Task<Account> UpdateAccountAsync(Account account)
        {
            var existingAccount = await _context.accounts.FirstOrDefaultAsync(a => a.id == account.id && a.is_deleted == false);
            if (existingAccount == null)
            {
                Logger("Unable to update account details. Account not found.");
                throw new InvalidOperationException("Unable to update account details. Account not found.");
            }

            if (existingAccount.is_system_account == true)
            {
                Logger("System accounts cannot be modified or deleted.");
                throw new InvalidOperationException("System accounts cannot be modified or deleted.");
            }

            var duplicateAccount = await _context.accounts.FirstOrDefaultAsync(a => a.id != account.id && a.name == account.name && a.is_deleted == false);
            if (duplicateAccount != null)
            {
                Logger("Another account with this name already exists.");
                throw new InvalidOperationException("Another account with this name already exists.");
            }

            if (string.IsNullOrWhiteSpace(account.name))
            {
                Logger("Account name cannot be empty.");
                throw new InvalidOperationException("Account name cannot be empty.");
            }

            if (account.type == null)
            {
                Logger("Account type is required.");
                throw new InvalidOperationException("Account type is required.");
            }

            if (!Enum.IsDefined(typeof(AccountType), account.type.Value))
            {
                Logger($"Invalid account type. Valid values are: {string.Join(", ", Enum.GetValues(typeof(AccountType)).Cast<int>())}");
                throw new InvalidOperationException($"Invalid account type. Valid values are: {string.Join(", ", Enum.GetValues(typeof(AccountType)).Cast<int>())}");
            }

            if (account.normal_balance.HasValue && !Enum.IsDefined(typeof(DebitCreditType), account.normal_balance.Value))
            {
                Logger($"Invalid normal balance. Valid values: {string.Join(", ", Enum.GetValues(typeof(DebitCreditType)).Cast<int>())}");
                throw new InvalidOperationException($"Invalid normal balance. Valid values: {string.Join(", ", Enum.GetValues(typeof(DebitCreditType)).Cast<int>())}");
            }

            bool typeChanged = account.type != existingAccount.type;
            bool normalBalanceNotProvided = !account.normal_balance.HasValue;

            if (normalBalanceNotProvided || typeChanged)
            {
                account.normal_balance = account.type switch
                {
                    AccountType.Asset => DebitCreditType.Debit,      // 0
                    AccountType.Liability => DebitCreditType.Credit, // 1
                    AccountType.Equity => DebitCreditType.Credit,    // 1
                    AccountType.Income => DebitCreditType.Credit,    // 1
                    AccountType.Expense => DebitCreditType.Debit,    // 0
                    _ => throw new InvalidOperationException("Invalid account type")
                };
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                existingAccount.name = account.name;
                existingAccount.type = account.type;
                existingAccount.normal_balance = account.normal_balance;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return existingAccount;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Unable to update account.");
            }
        }

        public async Task<Account> SoftDeleteAccountAsync(Account account)
        {
            var existingAccount = await _context.accounts.FirstOrDefaultAsync(a => a.id == account.id && a.is_deleted == false);
            if (existingAccount == null)
            {
                Logger("Unable to delete account details. Account not found.");
                throw new InvalidOperationException("Unable to delete account details. Account not found.");
            }

            if (existingAccount.is_system_account == true)
            {
                Logger("System accounts cannot be modified or deleted.");
                throw new InvalidOperationException("System accounts cannot be modified or deleted.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                existingAccount.is_deleted = true;
                existingAccount.deleted_at = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return existingAccount;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Unable to delete account.");
            }
        }

        public async Task<Account> UndoSoftDeleteAccountAsync(Account account)
        {
            var existingAccount = await _context.accounts.FirstOrDefaultAsync(a => a.id == account.id && a.is_deleted == true);
            if (existingAccount == null)
            {
                Logger("Unable to restore account details. Account not found.");
                throw new InvalidOperationException("Unable to restore account details. Account not found.");
            }

            if (existingAccount.is_system_account == true)
            {
                Logger("How did you delete a system accounts bruh -_- ???. Can't restore account if you force deleted it. Hack again xD.");
                throw new InvalidOperationException("How did you delete a system accounts bruh -_- ???. Can't restore account if you force deleted it. Hack again xD.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                existingAccount.is_deleted = false;
                existingAccount.deleted_at = null;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return existingAccount;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Unable to restore deleted account.");
            }
        }

        public async Task<IEnumerable<Account>> GetAllDeletedAccountsAsync()
        {
            return await _context.accounts
                    .Where(a => a.is_deleted == true)
                    .OrderBy(a => a.type)
                    .ThenBy(a => a.name)
                    .ToListAsync();
        }
    }
}
