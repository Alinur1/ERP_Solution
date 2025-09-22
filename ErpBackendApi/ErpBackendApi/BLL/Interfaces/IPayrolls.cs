using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface IPayrolls
    {
        Task<IEnumerable<PayrollDTO>> GetAllPayrollAsync();
        Task<PayrollDTO> GetPayrollByIdAsync(int id);
        Task<Payroll> AddPayrollAsync(Payroll payroll);
    }
}
