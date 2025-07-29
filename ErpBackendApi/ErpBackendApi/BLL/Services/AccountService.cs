using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Services
{
    public class AccountService : IAccount
    {
        private readonly AppDataContext _context;
        public AccountService(AppDataContext context)
        {
            _context = context;
        }

        public Task<Account> GetAccountByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Account> AddAccountAsync(Account account)
        {
            throw new NotImplementedException();
        }

        public Task<Account> UpdateAccountAsync(Account account)
        {
            throw new NotImplementedException();
        }

        public Task<Account> SoftDeleteAccountAsync(Account account)
        {
            throw new NotImplementedException();
        }

        public Task<Account> UndoSoftDeleteAccountAsync(Account account)
        {
            throw new NotImplementedException();
        }
    }
}
