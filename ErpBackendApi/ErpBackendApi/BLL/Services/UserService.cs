using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using static ErpBackendApi.Helper.LoggerClass;

namespace ErpBackendApi.BLL.Services
{
    public class UserService : IUsers
    {
        private readonly AppDataContext _context;
        public UserService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<User> AddUserAsync(User user)
        {
            var existingUser = await _context.users.FirstOrDefaultAsync(u => u.email == user.email);
            if (existingUser != null)
            {
                Logger("Tried to create an account with same email.");
                return null;
            }
            user.created_at = DateTime.UtcNow;
            user.is_deleted = false;
            _context.users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.users
                .Where(u => u.is_deleted == false)
                .Select(u => new User
                {
                    id = u.id,
                    name = u.name,
                    email = u.email,
                    phone = u.phone,
                    created_at = u.created_at
                }).ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.users
                .Where(u => u.id == id && u.is_deleted == false)
                .Select(u => new User
                {
                    id = u.id,
                    name = u.name,
                    email = u.email,
                    phone = u.phone,
                    created_at = u.created_at
                })
                .FirstOrDefaultAsync();
        }

        public async Task<User> SoftDeleteUserAsync(User user)
        {
            var existingUser = await _context.users.FirstOrDefaultAsync(u => u.id == user.id && u.is_deleted == false);
            if (existingUser != null)
            {
                existingUser.is_deleted = true;
                existingUser.deleted_at = DateTime.UtcNow;
                _context.users.Update(existingUser);
                await _context.SaveChangesAsync();
            }
            return existingUser;
        }

        public async Task<User> UndoSoftDeleteUserAsync(User user)
        {
            var existingUser = await _context.users.FirstOrDefaultAsync(u => u.id == user.id && u.is_deleted == true);
            if (existingUser != null)
            {
                existingUser.is_deleted = false;
                existingUser.deleted_at = null;
                _context.users.Update(existingUser);
                await _context.SaveChangesAsync();
            }
            return existingUser;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            var existingUser = await _context.users.FirstOrDefaultAsync(u => u.id == user.id && u.is_deleted == false);
            if (existingUser != null)
            {
                existingUser.name = user.name;
                existingUser.email = user.email;
                existingUser.phone = user.phone;
                _context.users.Update(existingUser);
                await _context.SaveChangesAsync();
            }
            return existingUser;
        }
    }
}