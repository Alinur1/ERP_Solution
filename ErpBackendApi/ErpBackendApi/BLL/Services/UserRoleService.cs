using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Services
{
    public class UserRoleService : IUserRoles
    {
        private readonly AppDataContext _context;
        public UserRoleService(AppDataContext context)
        {
            _context = context;
        }

        public Task<IEnumerable<UserRole>> GetAllUserRolesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<UserRole> GetUserRoleByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<UserRole> AssignUserRoleAsync(UserRole userRole)
        {
            throw new NotImplementedException();
        }

        public Task<UserRole> UpdateUserRoleAsync(UserRole userRole)
        {
            throw new NotImplementedException();
        }
    }
}
