using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using static ErpBackendApi.Helper.LoggerClass;

//TODO: Handle saving null values for the SoftDeleteUserAsync and UpdateUserAsync methods = completed
//TODO: Undo deletion of the deleted users
//TODO: Prevent update for the deleted users
//TODO: Replace "throw new exception" with "throw new BadHttpRequestException" in AddUserAsync method

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
            var existingUser = await _context.users.FirstOrDefaultAsync(u => u.email == user.email && u.is_deleted == false);
            if (existingUser != null)
            {
                throw new Exception("An user with this email already exists.");
            }
            user.created_at = DateTime.UtcNow;
            user.is_deleted = false;
            _context.users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                return await _context.users
                    .Where(u => u.is_deleted == false)
                    .Select(u => new User { 
                        id = u.id,
                        name = u.name,
                        email = u.email,
                        phone = u.phone,
                        created_at = u.created_at
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                Logger("An error occurred at UserService in GetAllUsersAsync#1: " + ex.Message + "\n" + ex.StackTrace);
                throw new Exception("An error occurred at UserService in GetAllUsersAsync#1. Please see the ERP_API_Logger.txt for more information.");
            }
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            try
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
            catch (Exception ex)
            {
                Logger("An error occurred at UserService in GetUserByIdAsync#1: " + ex.Message + "\n" + ex.StackTrace);
                throw new Exception("An error occurred at UserService in GetUserByIdAsync#1. Please see the ERP_API_Logger.txt for more information.");
            }
        }

        public async Task<User> SoftDeleteUserAsync(User user)
        {            
            var existingUser = await _context.users.FirstOrDefaultAsync(u => u.id == user.id && u.is_deleted == false);
            if (existingUser != null)
            {
                existingUser.is_deleted = user.is_deleted;
                existingUser.deleted_at = DateTime.UtcNow;
                _context.users.Update(existingUser);
                await _context.SaveChangesAsync();
            }
            return existingUser ?? throw new KeyNotFoundException("User not found. Unable to delete user.");            
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
            return existingUser ?? throw new KeyNotFoundException("User not found. Unable to update user information.");
        }
    }
}