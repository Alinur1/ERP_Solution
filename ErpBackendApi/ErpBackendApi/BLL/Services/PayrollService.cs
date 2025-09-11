using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.Services
{
    public class PayrollService : IPayrolls
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

        public async Task<Payroll> AddPayrollAsync(Payroll payroll)
        {
            var existingPayroll = await _context.payroll.FirstOrDefaultAsync(p => p.employee_id == payroll.employee_id && p.is_deleted == false);
            if (existingPayroll != null)
            {
                Logger("Same employee cannot have more than one payroll.");
                throw new InvalidOperationException("Same employee cannot have more than one payroll.");
            }

            payroll.is_deleted = false;
            payroll.deleted_at = null;
            _context.payroll.Add(payroll);
            await _context.SaveChangesAsync();
            return payroll;
        }

        public async Task<Payroll> UpdatePayrollAsync(Payroll payroll)
        {
            var existingPayroll = await _context.payroll.FirstOrDefaultAsync(p => p.id == payroll.id && p.is_deleted == false);
            if (existingPayroll == null)
            {
                Logger("Unable to update payroll. Payroll not found.");
                throw new InvalidOperationException("Unable to update payroll. Payroll not found.");
            }

            existingPayroll.period_start = payroll.period_start;
            existingPayroll.period_end = payroll.period_end;
            existingPayroll.base_salary = payroll.base_salary;
            existingPayroll.deductions = payroll.deductions;
            existingPayroll.bonuses = payroll.bonuses;
            existingPayroll.net_pay = payroll.net_pay;
            existingPayroll.paid_on = payroll.paid_on;
            await _context.SaveChangesAsync();
            return existingPayroll;
        }

        public async Task<Payroll> SoftDeletePayrollAsync(Payroll payroll)
        {
            var existingPayroll = await _context.payroll.FirstOrDefaultAsync(p => p.id == payroll.id && p.is_deleted == false);
            if (existingPayroll == null)
            {
                Logger("Unable to delete payroll. Payroll not found.");
                throw new InvalidOperationException("Unable to delete payroll. Payroll not found.");
            }

            existingPayroll.is_deleted = true;
            existingPayroll.deleted_at = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existingPayroll;
        }

        public async Task<Payroll> UndoSoftDeletePayrollAsync(Payroll payroll)
        {
            var existingPayroll = await _context.payroll.FirstOrDefaultAsync(p => p.id == payroll.id && p.is_deleted == true);
            if (existingPayroll == null)
            {
                Logger("Unable to restore payroll. Payroll not found.");
                throw new InvalidOperationException("Unable to restore payroll. Payroll not found.");
            }

            existingPayroll.is_deleted = false;
            existingPayroll.deleted_at = null;
            await _context.SaveChangesAsync();
            return existingPayroll;
        }
    }
}
