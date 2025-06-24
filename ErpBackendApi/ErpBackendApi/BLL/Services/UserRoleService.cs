using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;

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
                join r in _context.roles on ur.role_id equals r.id
                where u.is_deleted == false && r.is_deleted == false
                select new UserRoleDto
                {
                    user_id = u.id,
                    name = u.name,
                    email = u.email,
                    phone = u.phone,
                    created_at = u.created_at,
                    role_name = r.name,
                    role_description = r.description
                }
            ).ToListAsync();
        }

        public async Task<UserRoleDto> GetUserRoleByIdAsync(int id)
        {
            return await
            (
                from ur in _context.user_roles
                join u in _context.users on ur.user_id equals u.id
                join r in _context.roles on ur.role_id equals r.id
                where u.id == id && u.is_deleted == false && r.is_deleted == false
                select new UserRoleDto
                {
                    user_id = u.id,
                    name = u.name,
                    email = u.email,
                    phone = u.phone,
                    created_at = u.created_at,
                    role_name = r.name,
                    role_description = r.description
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<UserRole> AssignUserRoleAsync(UserRole userRole)
        {
            _context.user_roles.Add(userRole);
            await _context.SaveChangesAsync();
            return userRole;
        }

        public async Task<UserRole> UpdateUserRoleAsync(UserRole userRole)
        {
            _context.user_roles.Update(userRole);
            await _context.SaveChangesAsync();
            return userRole;
        }
    }
}
