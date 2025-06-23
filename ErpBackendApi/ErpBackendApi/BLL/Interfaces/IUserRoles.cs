using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface IUserRoles
    {
        Task<IEnumerable<UserRole>> GetAllUserRolesAsync();
        Task<UserRole> GetUserRoleByIdAsync(int id);
        Task<UserRole> AssignUserRoleAsync(UserRole userRole);
        Task<UserRole> UpdateUserRoleAsync(UserRole userRole);
    }
}
