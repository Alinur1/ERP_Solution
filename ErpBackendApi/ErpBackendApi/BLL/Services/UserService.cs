using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using static ErpBackendApi.Utilities.Helper.LoggerClass;
using static ErpBackendApi.Utilities.Helper.PasswordHasher;

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
            var existingPhone = await _context.users.FirstOrDefaultAsync(u => u.phone == user.phone && u.is_deleted == false);
            if (existingPhone != null)
            {
                Logger("This phone number is already being used.");
                throw new InvalidOperationException("This phone number is already being used.");
            }
            if (!string.IsNullOrEmpty(user.email) || !string.IsNullOrWhiteSpace(user.email))
            {
                var existingEmail = await _context.users.FirstOrDefaultAsync(u => u.email == user.email && u.is_deleted == false);
                if (existingEmail != null)
                {
                    Logger("This email is already being used.");
                    throw new InvalidOperationException("This email is already being used.");
                }
            }
            if (string.IsNullOrEmpty(user.password))
            {
                Logger("Password is required.");
                throw new InvalidOperationException("Password is required.");
            }
            user.password = HashPass(user.password);
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
            if ( existingUser == null )
            {
                Logger($"User ID: {user.id} doesn't exist.");
                throw new InvalidOperationException($"User ID: {user.id} doesn't exist.");
            }

            if (existingUser.phone != user.phone)
            {
                var phoneExists = await _context.users.FirstOrDefaultAsync(u => u.phone == user.phone && u.id != user.id && u.is_deleted == false);
                if (phoneExists != null)
                {
                    Logger("Can't use this phone number. It is already in use.");
                    throw new InvalidOperationException("Can't use this phone number. It is already in use.");
                }
            }

            if (!string.IsNullOrEmpty(user.email) || !string.IsNullOrWhiteSpace(user.email) && existingUser.email != user.email)
            {
                var existingEmail = await _context.users.FirstOrDefaultAsync(u => u.email == user.email && u.id != user.id && u.is_deleted == false);
                if (existingEmail != null)
                {
                    Logger("Can't use this email. It is already in use.");
                    throw new InvalidOperationException("Can't use this email. It is already in use.");
                }
            }

            existingUser.name = user.name;
            existingUser.email = user.email;
            existingUser.phone = user.phone;
            await _context.SaveChangesAsync();
            return existingUser;
        }

        public async Task<User> ChangePasswordAsync(User user)
        {
            var existingUser = await _context.users.FirstOrDefaultAsync(u => u.id == user.id && u.is_deleted == false);
            if (existingUser != null)
            {
                if (string.IsNullOrEmpty(user.password) || string.IsNullOrWhiteSpace(user.password))
                {
                    Logger("Password is required.");
                    throw new InvalidOperationException("Password is required.");
                }
                existingUser.password = HashPass(user.password);
                _context.users.Update(existingUser);
                await _context.SaveChangesAsync();
            }
            return existingUser;
        }

        public async Task<User> ValidateUserByEmailAsync(string email, string password)
        {
            return await _context.users.FirstOrDefaultAsync(u => u.email == email && u.password == HashPass(password) && u.is_deleted == false);
        }

        public async Task<User> ValidateUserByPhoneAsync(string phone, string password)
        {
            return await _context.users.FirstOrDefaultAsync(u => u.phone == phone && u.password == HashPass(password) && u.is_deleted == false);
        }

        public async Task<IEnumerable<User>> GetAllDeletedUsersAsync()
        {
            return await _context.users
                .Where(u => u.is_deleted == true)
                .Select(u => new User
                {
                    id = u.id,
                    name = u.name,
                    email = u.email,
                    phone = u.phone,
                    deleted_at = u.deleted_at,
                }).ToListAsync();
        }
    }
}
