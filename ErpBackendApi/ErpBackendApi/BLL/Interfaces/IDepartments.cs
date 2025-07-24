using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface IDepartments
    {
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();
        Task<Department> GetDepartmentByIdAsync(int id);
        Task<Department> AddDepartmentAsync(Department department);
        Task<Department> UpdateDepartmentAsync(Department department);
        Task<Department> SoftDeleteDepartmentAsync(Department department);
        Task<Department> UndoSoftDeleteDepartmentAsync(Department department);
    }
}
