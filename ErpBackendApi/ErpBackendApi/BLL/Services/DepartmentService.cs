using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.Services
{
    public class DepartmentService : IDepartments
    {
        private readonly AppDataContext _context;
        public DepartmentService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            return await _context.departments
                .Where(d => d.is_deleted == false)
                .Select(d => new Department
                {
                    id = d.id,
                    name = d.name,
                    description = d.description,
                }).ToListAsync();
        }

        public async Task<Department> GetDepartmentByIdAsync(int id)
        {
            return await _context.departments
                .Where(d => d.id == id && d.is_deleted == false)
                .Select(d => new Department
                {
                    id = d.id,
                    name = d.name,
                    description = d.description
                }).FirstOrDefaultAsync();
        }

        public async Task<Department> AddDepartmentAsync(Department department)
        {
            var existingDept = await _context.departments.FirstOrDefaultAsync(d => d.name == department.name && d.is_deleted == false);
            if (existingDept != null)
            {
                Logger("Same department name already exists.");
                return null;
            }
            department.is_deleted = false;
            department.deleted_at = null;
            _context.departments.Add(department);
            await _context.SaveChangesAsync();
            return department;
        }

        public async Task<Department> UpdateDepartmentAsync(Department department)
        {
            var existingDept = await _context.departments.FirstOrDefaultAsync(d => d.id == department.id && d.is_deleted == false);
            if (existingDept == null)
            {
                Logger("Unable to update department information. Department not found.");
                return null;
            }
            existingDept.name = department.name;
            existingDept.description = department.description;
            _context.departments.Update(existingDept);
            await _context.SaveChangesAsync();
            return existingDept;
        }

        public async Task<Department> SoftDeleteDepartmentAsync(Department department)
        {
            var existingDept = await _context.departments.FirstOrDefaultAsync(d => d.id == department.id && d.is_deleted == false);
            if (existingDept == null)
            {
                Logger("Unable to delete department information. Department not found.");
                return null;
            }
            existingDept.is_deleted = true;
            existingDept.deleted_at = DateTime.UtcNow;
            _context.departments.Update(existingDept);
            await _context.SaveChangesAsync();
            return existingDept;
        }

        public async Task<Department> UndoSoftDeleteDepartmentAsync(Department department)
        {
            var existingDept = await _context.departments.FirstOrDefaultAsync(d => d.id == department.id && d.is_deleted == true);
            if (existingDept == null)
            {
                Logger("Unable to restore deleted department information. Department not found.");
                return null;
            }
            existingDept.is_deleted = false;
            existingDept.deleted_at = null;
            _context.departments.Update(existingDept);
            await _context.SaveChangesAsync();
            return existingDept;
        }
    }
}
