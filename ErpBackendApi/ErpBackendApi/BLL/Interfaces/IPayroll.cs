using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface IPayroll
    {
        Task<IEnumerable<PayrollDTO>> GetAllPayrollAsync();
        Task<PayrollDTO> GetPayrollByIdAsync(int id);
        Task<Payroll> AddPayrollAsync(Payroll payroll);
        Task<Payroll> UpdatePayrollAsync(Payroll payroll);
        Task<Payroll> SoftDeletePayrollAsync(Payroll payroll);
        Task<Payroll> UndoSoftDeletePayrollAsync(Payroll payroll);
    }
}
