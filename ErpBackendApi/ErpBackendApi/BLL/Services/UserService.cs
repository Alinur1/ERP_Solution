using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
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
            try
            {
                var existingUser = await _context.users.FirstOrDefaultAsync(u => u.email == user.email);
                if (existingUser != null)
                {
                    throw new Exception("An user with this email already exists.");
                }
                _context.users.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                Logger("An error occurred at UserService in AddUserAsync#1: " + ex.Message + "/n" + ex.StackTrace);
                throw new Exception("An error occurred at UserService in AddUserAsync#1. Please see the ERP_API_Logger.txt for more information.");
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                return await _context.users.ToListAsync();
            }
            catch (Exception ex)
            {
                Logger("An error occurred at UserService in GetAllUsersAsync#1: " + ex.Message + "/n" + ex.StackTrace);
                throw new Exception("An error occurred at UserService in GetAllUsersAsync#1. Please see the ERP_API_Logger.txt for more information.");
            }
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            try
            {
                return await _context.users.FirstOrDefaultAsync(u => u.id == id);
            }
            catch (Exception ex)
            {
                Logger("An error occurred at UserService in GetUserByIdAsync#1: " + ex.Message + "/n" + ex.StackTrace);
                throw new Exception("An error occurred at UserService in GetUserByIdAsync#1. Please see the ERP_API_Logger.txt for more information.");
            }
        }

        public async Task<User> SoftDeleteUserAsync(User user)
        {
            try
            {
                var existingUser = await _context.users.FindAsync(user.id);
                if (existingUser != null)
                {
                    existingUser.is_deleted = user.is_deleted;
                    existingUser.deleted_at = user.deleted_at;
                    _context.users.Update(existingUser);
                    await _context.SaveChangesAsync();
                }
                return existingUser ?? throw new KeyNotFoundException("User not found. Unable to delete user.");
            }
            catch (Exception ex)
            {
                Logger("An error occurred at UserService in SoftDeleteUserAsync#1: " + ex.Message + "/n" + ex.StackTrace);
                throw new Exception("An error occurred at UserService in SoftDeleteUserAsync#1. Please see the ERP_API_Logger.txt for more information.");
            }
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            try
            {
                var existingUser = await _context.users.FindAsync(user.id);
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
            catch (Exception ex)
            {
                Logger("An error occurred at UserService in UpdateUserAsync#1: " + ex.Message + "/n" + ex.StackTrace);
                throw new Exception("An error occurred at UserService in UpdateUserAsync#1. Please see the ERP_API_Logger.txt for more information.");
            }
        }
    }
}