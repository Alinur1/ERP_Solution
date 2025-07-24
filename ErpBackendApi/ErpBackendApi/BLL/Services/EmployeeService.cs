using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.Services
{
    public class EmployeeService : IEmployees
    {
        private readonly AppDataContext _context;
        public EmployeeService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync()
        {
            return await
            (
                from e in _context.employees
                join u in _context.users on e.user_id equals u.id into userGroup
                from u in userGroup.DefaultIfEmpty()
                join d in _context.departments on e.department_id equals d.id into deptGroup
                from d in deptGroup.DefaultIfEmpty()
                where e.is_deleted == false
                select new EmployeeDTO
                {
                    id = e.id,
                    user_id = u != null ? u.id : null,
                    employee_name = u != null && u.is_deleted == false ? u.name : "-",
                    employee_email = u != null && u.is_deleted == false ? u.email : "-",
                    employee_phone = u != null && u.is_deleted == false ? u.phone : "-",
                    employee_created_at = u != null && u.is_deleted == false ? u.created_at: null,
                    department_id = d != null ? d.id : null,
                    department_name = d != null && d.is_deleted == false ? d.name : "-",
                    date_hired = e.date_hired,
                    salary = e.salary,
                    status = e.status,
                }
            ).ToListAsync();
        }

        public async Task<EmployeeDTO> GetEmployeeByIdAsync(int id)
        {
            return await
            (
                from e in _context.employees
                join u in _context.users on e.user_id equals u.id into userGroup
                from u in userGroup.DefaultIfEmpty()
                join d in _context.departments on e.department_id equals d.id into deptGroup
                from d in deptGroup.DefaultIfEmpty()
                where e.id == id && e.is_deleted == false
                select new EmployeeDTO
                {
                    id = e.id,
                    user_id = u != null ? u.id : null,
                    employee_name = u != null && u.is_deleted == false ? u.name : "-",
                    employee_email = u != null && u.is_deleted == false ? u.email : "-",
                    employee_phone = u != null && u.is_deleted == false ? u.phone : "-",
                    employee_created_at = u != null && u.is_deleted == false ? u.created_at : null,
                    department_id = d != null ? d.id : null,
                    department_name = d != null && d.is_deleted == false ? d.name : "-",
                    date_hired = e.date_hired,
                    salary = e.salary,
                    status = e.status,
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<Employee> AddEmployeeAsync(Employee emp)
        {
            var existingEmployee = await _context.employees.FirstOrDefaultAsync(e => e.user_id == emp.user_id && e.department_id == emp.department_id && e.is_deleted == false);
            if (existingEmployee != null)
            {
                Logger("Same employee cannot be added in the same department.");
                return null;
            }
            emp.is_deleted = false;
            emp.deleted_at = null;
            _context.employees.Add(emp);
            await _context.SaveChangesAsync();
            return emp;
        }

        public async Task<Employee> UpdateEmployeeAsync(Employee emp)
        {
            var existingEmployee = await _context.employees.FirstOrDefaultAsync(e => e.id == emp.id && e.is_deleted == false);
            if (existingEmployee == null)
            {
                Logger("Employee not found. Unable to update employee information.");
                return null;
            }
            existingEmployee.department_id = emp.department_id;
            existingEmployee.date_hired = emp.date_hired;
            existingEmployee.salary = emp.salary;
            existingEmployee.status = emp.status;
            _context.employees.Update(existingEmployee);
            await _context.SaveChangesAsync();
            return existingEmployee;
        }

        public async Task<Employee> SoftDeleteEmployeeAsync(Employee emp)
        {
            var existingEmployee = await _context.employees.FirstOrDefaultAsync(e => e.id == emp.id && e.is_deleted == false);
            if (existingEmployee == null)
            {
                Logger("Employee not found. Unable to delete employee information.");
                return null;
            }
            existingEmployee.is_deleted = true;
            existingEmployee.deleted_at = DateTime.UtcNow;
            _context.employees.Update(existingEmployee);
            await _context.SaveChangesAsync();
            return existingEmployee;
        }

        public async Task<Employee> UndoSoftDeleteEmployeeAsync(Employee emp)
        {
            var existingEmployee = await _context.employees.FirstOrDefaultAsync(e => e.id == emp.id && e.is_deleted == true);
            if (existingEmployee == null)
            {
                Logger("Employee not found. Unable to restore deleted employee information.");
                return null;
            }
            existingEmployee.is_deleted = false;
            existingEmployee.deleted_at = null;
            _context.employees.Update(existingEmployee);
            await _context.SaveChangesAsync();
            return existingEmployee;
        }
    }
}
