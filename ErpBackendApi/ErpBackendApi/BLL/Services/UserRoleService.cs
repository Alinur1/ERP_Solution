using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.Services
{
    public class UserRoleService : IUserRoles
    {
        private readonly AppDataContext _context;
        public UserRoleService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserRoleDto>> GetAllUserRolesAsync()
        {
            return await
            (
                from ur in _context.user_roles
                join u in _context.users on ur.user_id equals u.id
                join r in _context.roles on ur.role_id equals r.id into roleGroup
                from r in roleGroup.DefaultIfEmpty()
                where u.is_deleted == false
                select new UserRoleDto
                {
                    id = ur.id,
                    user_id = u.id,
                    name = u.name,
                    email = u.email,
                    phone = u.phone,
                    created_at = u.created_at,
                    role_id = r != null ? r.id : null,
                    role_name = r != null && r.is_deleted == false ? r.name : "-",
                    role_description = r != null && r.is_deleted == false ? r.description : "-",
                }
            ).ToListAsync();
        }

        public async Task<UserRoleDto> GetUserRoleByIdAsync(int id)
        {
            return await
            (
                from ur in _context.user_roles
                join u in _context.users on ur.user_id equals u.id
                join r in _context.roles on ur.role_id equals r.id into roleGroup
                from r in roleGroup.DefaultIfEmpty()
                where ur.id == id && u.is_deleted == false
                select new UserRoleDto
                {
                    id = ur.id,
                    user_id = u.id,
                    name = u.name,
                    email = u.email,
                    phone = u.phone,
                    created_at = u.created_at,
                    role_id = r != null ? r.id : null,
                    role_name = r != null && r.is_deleted == false ? r.name : "-",
                    role_description = r != null && r.is_deleted == false ? r.description : "-"
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<UserRoleDto>> GetUserRoleByUserIdAsync(int userId)
        {
            return await
            (
                from ur in _context.user_roles
                join u in _context.users on ur.user_id equals u.id
                join r in _context.roles on ur.role_id equals r.id into roleGroup
                from r in roleGroup.DefaultIfEmpty()
                where ur.user_id == userId && u.is_deleted == false && r.is_deleted == false
                select new UserRoleDto
                {
                    id = ur.id,
                    user_id = u.id,
                    name = u.name,
                    email = u.email,
                    phone = u.phone,
                    created_at = u.created_at,
                    role_id = r != null ? r.id : null,
                    role_name = r != null && r.is_deleted == false ? r.name : "-",
                    role_description = r != null && r.is_deleted == false ? r.description : "-"
                }
            ).ToListAsync();
        }

        public async Task<UserRole> AssignUserRoleAsync(UserRole userRole)
        {
            var existingUser = await _context.users.FirstOrDefaultAsync(u => u.id == userRole.user_id && u.is_deleted == false);
            var existingRole = await _context.roles.FirstOrDefaultAsync(r => r.id == userRole.role_id && r.is_deleted == false);
            var assignedAlready = await _context.user_roles.FirstOrDefaultAsync(ur => ur.user_id == userRole.user_id && ur.role_id == userRole.role_id);
            if (existingUser == null)
            {
                Logger("User not found or deleted#1-AssignUserRoleAsync");
                return null;
            }
            if (existingRole == null)
            {
                Logger("Role not found or deleted#2-AssignUserRoleAsync");
                return null;
            }
            if (assignedAlready != null)
            {
                Logger("The role is already assigned to the user#3-AssignUserRoleAsync");
                return null;
            }
            _context.user_roles.Add(userRole);
            await _context.SaveChangesAsync();
            return userRole;
        }

        public async Task<UserRole> UpdateUserRoleAsync(UserRole userRole)
        {
            var existingUser = await _context.users.FirstOrDefaultAsync(u => u.id == userRole.user_id && u.is_deleted == false);
            var existingRole = await _context.roles.FirstOrDefaultAsync(r => r.id == userRole.role_id && r.is_deleted == false);
            var existingUserRole = await _context.user_roles.FirstOrDefaultAsync(ur => ur.id == userRole.id);
            if (existingUserRole != null && existingUser != null && existingRole != null)
            {
                existingUserRole.user_id = userRole.user_id;
                existingUserRole.role_id = userRole.role_id;
                _context.user_roles.Update(existingUserRole);
                await _context.SaveChangesAsync();
            }
            return existingUserRole;
        }

        public async Task<UserRole> RemoveUserRoleAsync(UserRole userRole)
        {
            var existingUserRole = await _context.user_roles.FirstOrDefaultAsync(ur => ur.id == userRole.id);
            if (existingUserRole != null)
            {
                _context.user_roles.Remove(existingUserRole);
                await _context.SaveChangesAsync();
            }
            return existingUserRole;
        }
    }
}
