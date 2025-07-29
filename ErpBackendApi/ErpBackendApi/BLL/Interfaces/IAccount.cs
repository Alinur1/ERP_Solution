using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface IAccount
    {
        Task<IEnumerable<Account>> GetAllAccountsAsync();
        Task<Account> GetAccountByIdAsync(int id);
        Task<Account> AddAccountAsync(Account account);
        Task<Account> UpdateAccountAsync(Account account);
        Task<Account> SoftDeleteAccountAsync(Account account);
        Task<Account> UndoSoftDeleteAccountAsync(Account account);
    }
}
