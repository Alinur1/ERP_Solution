using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface IRoles
    {
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role> GetRoleByIdAsync(int id);
        Task<Role> AddRoleAsync(Role role);
        Task<Role> UpdateRoleAsync(Role role);
        Task<Role> SoftDeleteRoleAsync(Role role);
        Task<Role> UndoSoftDeleteRoleAsync(Role role);
    }
}
