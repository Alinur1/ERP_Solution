using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface IRolePermissions
    {
        Task<IEnumerable<RolePermission>> GetAllRolePermissionAsync();
        Task<RolePermission> GetRolePermissionAsyncById(int id);
        Task<RolePermission> AddRolePermissionAsync(RolePermission rolePermission);
        Task<RolePermission> UpdateRolePermissionAsync(RolePermission rolePermission);
        Task<bool> DeleteRolePermissionAsync(int id);
    }
}
