using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface IUserRoles
    {
        Task<IEnumerable<UserRoleDto>> GetAllUserRolesAsync();
        Task<UserRoleDto> GetUserRoleByIdAsync(int id);
        Task<UserRole> AssignUserRoleAsync(UserRole userRole);
        Task<UserRole> UpdateUserRoleAsync(UserRole userRole);
        Task<UserRole> RemoveUserRoleAsync(UserRole userRole);
    }
}
