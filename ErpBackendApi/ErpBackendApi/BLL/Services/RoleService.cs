using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.Services
{
    public class RoleService : IRoles
    {
        private readonly AppDataContext _context;
        public RoleService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _context.roles
                .Where(r => r.is_deleted == false)
                .Select(r => new Role
                {
                    id = r.id,
                    name = r.name,
                    description = r.description
                })
                .ToListAsync();
        }

        public async Task<Role> GetRoleByIdAsync(int id)
        {
            return await _context.roles
                .Where(r => r.id == id && r.is_deleted == false)
                .Select(r => new Role
                {
                    id = r.id,
                    name = r.name,
                    description = r.description
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Role> AddRoleAsync(Role role)
        {
            var existingRole = _context.roles.FirstOrDefault(r => r.name == role.name && r.is_deleted == false);
            if (existingRole != null)
            {
                Logger("Tried to create same role.");
                return null;
            }
            role.is_deleted = false;
            _context.roles.Add(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<Role> UpdateRoleAsync(Role role)
        {
            var existingRole = await _context.roles.FirstOrDefaultAsync(r => r.id == role.id && r.is_deleted == false);
            if (existingRole != null)
            {
                existingRole.name = role.name;
                existingRole.description = role.description;
                _context.roles.Update(existingRole);
                await _context.SaveChangesAsync();
            }
            return existingRole;
        }

        public async Task<Role> SoftDeleteRoleAsync(Role role)
        {
            var existingRole = await _context.roles.FirstOrDefaultAsync(r => r.id == role.id && r.is_deleted == false);
            if (existingRole != null)
            {
                existingRole.is_deleted = true;
                existingRole.deleted_at = DateTime.UtcNow;
                _context.roles.Update(existingRole);
                await _context.SaveChangesAsync();
            }
            return existingRole;
        }

        public async Task<Role> UndoSoftDeleteRoleAsync(Role role)
        {
            var existingRole = await _context.roles.FirstOrDefaultAsync(r => r.id == role.id && r.is_deleted == true);
            if (existingRole != null)
            {
                existingRole.is_deleted = false;
                existingRole.deleted_at = null;
                _context.roles.Update(existingRole);
                await _context.SaveChangesAsync();
            }
            return existingRole;
        }
    }
}
