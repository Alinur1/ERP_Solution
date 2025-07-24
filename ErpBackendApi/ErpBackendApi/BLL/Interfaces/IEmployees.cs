using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface IEmployees
    {
        Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync();
        Task<EmployeeDTO> GetEmployeeByIdAsync(int id);
        Task<Employee> AddEmployeeAsync(Employee emp);
        Task<Employee> UpdateEmployeeAsync(Employee emp);
        Task<Employee> SoftDeleteEmployeeAsync(Employee emp);
        Task<Employee> UndoSoftDeleteEmployeeAsync(Employee emp);
    }
}
