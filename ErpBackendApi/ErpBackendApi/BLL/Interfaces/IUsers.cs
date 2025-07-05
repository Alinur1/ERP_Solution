using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface IUsers
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<User> AddUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task<User> SoftDeleteUserAsync(User user);
        Task<User> UndoSoftDeleteUserAsync(User user);
        Task<User> ChangePasswordAsync(User user);
        Task<User> ValidateUserAsync(string email, string password);
    }
}