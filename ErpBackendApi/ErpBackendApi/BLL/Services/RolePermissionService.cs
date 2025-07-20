using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.Services
{
    public class RolePermissionService : IRolePermissions
    {
        private readonly AppDataContext _context;
        public RolePermissionService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RolePermission>> GetAllRolePermissionAsync()
        {
            return await _context.role_permissions.ToListAsync();
        }

        public async Task<RolePermission> GetRolePermissionAsyncById(int id)
        {
            return await _context.role_permissions.FindAsync(id);
        }

        public async Task<RolePermission> AddRolePermissionAsync(RolePermission rolePermission)
        {
            var existingRoleForFeature = await _context.role_permissions.FirstOrDefaultAsync(rp => rp.role_id == rolePermission.role_id && rp.feature_id == rolePermission.feature_id);
            if (existingRoleForFeature != null)
            {
                Logger("Tried to add same feature to the same role.");
                return null;
            }
            _context.role_permissions.Add(rolePermission);
            await _context.SaveChangesAsync();
            return rolePermission;
        }

        public async Task<RolePermission> UpdateRolePermissionAsync(RolePermission rolePermission)
        {
            var existingRoleForFeature = await _context.role_permissions.FindAsync(rolePermission.id);
            if (existingRoleForFeature != null)
            {
                existingRoleForFeature.feature_id = rolePermission.feature_id;
                existingRoleForFeature.can_read = rolePermission.can_read;
                existingRoleForFeature.can_create = rolePermission.can_create;
                existingRoleForFeature.can_update = rolePermission.can_update;
                existingRoleForFeature.can_delete = rolePermission.can_delete;
                _context.role_permissions.Update(existingRoleForFeature);
                await _context.SaveChangesAsync();
            }
            return existingRoleForFeature;
        }

        public async Task<bool> DeleteRolePermissionAsync(int id)
        {
            var existingRoleForFeature = await _context.role_permissions.FindAsync(id);
            if (existingRoleForFeature != null)
            {
                _context.role_permissions.Remove(existingRoleForFeature);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
