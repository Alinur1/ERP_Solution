using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace ErpBackendApi.BLL.Services
{
    public class PayrollService : IPayroll
    {
        private readonly AppDataContext _context;
        public PayrollService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PayrollDTO>> GetAllPayrollAsync()
        {
            return await
            (
                from p in _context.payroll
                join e in _context.employees on p.employee_id equals e.id into employeeGroup
                from e in employeeGroup.DefaultIfEmpty()
                join u in _context.users on e.user_id equals u.id into userGroup
                from u in userGroup.DefaultIfEmpty()
                where p.is_deleted == false
                select new PayrollDTO
                {
                    id = p.id,
                    employee_id = e.id,
                    user_id = u.id,
                    employee_name = u.name,
                    period_start = p.period_start,
                    period_end = p.period_end,
                    base_salary = p.base_salary,
                    deductions = p.deductions,
                    bonuses = p.bonuses,
                    net_pay = p.net_pay,
                    paid_on = p.paid_on
                }
            ).ToListAsync();
        }

        public async Task<PayrollDTO> GetPayrollByIdAsync(int id)
        {
            return await
            (
                from p in _context.payroll
                join e in _context.employees on p.employee_id equals e.id into employeeGroup
                from e in employeeGroup.DefaultIfEmpty()
                join u in _context.users on e.user_id equals u.id into userGroup
                from u in userGroup.DefaultIfEmpty()
                where p.id == id && p.is_deleted == false
                select new PayrollDTO
                {
                    id = p.id,
                    employee_id = e.id,
                    user_id = u.id,
                    employee_name = u.name,
                    period_start = p.period_start,
                    period_end = p.period_end,
                    base_salary = p.base_salary,
                    deductions = p.deductions,
                    bonuses = p.bonuses,
                    net_pay = p.net_pay,
                    paid_on = p.paid_on
                }
            ).FirstOrDefaultAsync();
        }

        public Task<Payroll> AddPayrollAsync(Payroll payroll)
        {
            throw new NotImplementedException();
        }

        public Task<Payroll> UpdatePayrollAsync(Payroll payroll)
        {
            throw new NotImplementedException();
        }

        public Task<Payroll> SoftDeletePayrollAsync(Payroll payroll)
        {
            throw new NotImplementedException();
        }

        public Task<Payroll> UndoSoftDeletePayrollAsync(Payroll payroll)
        {
            throw new NotImplementedException();
        }
    }
}
