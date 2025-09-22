using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.BLL.TransactionInterface;
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
        private readonly ITransactionPayrollGenerator _transactionGenerator;
        public PayrollService(AppDataContext context, ITransactionPayrollGenerator transactionGenerator)
        {
            _context = context;
            _transactionGenerator = transactionGenerator;
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
                    employee_id = e != null && e.is_deleted == false ? e.id : null,
                    user_id = u != null && u.is_deleted == false ? u.id : null,
                    employee_name = u != null && u.is_deleted == false ? u.name : u.name + " (Deleted User)",
                    employee_salary = e != null ? e.salary : null,
                    period_start = p.period_start,
                    period_end = p.period_end,
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
                    employee_id = e != null && e.is_deleted == false ? e.id : null,
                    user_id = u != null && u.is_deleted == false ? u.id : null,
                    employee_name = u != null && u.is_deleted == false ? u.name : u.name + " (Deleted User)",
                    employee_salary = e != null ? e.salary : null,
                    period_start = p.period_start,
                    period_end = p.period_end,
                    deductions = p.deductions,
                    bonuses = p.bonuses,
                    net_pay = p.net_pay,
                    paid_on = p.paid_on
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<Payroll> AddPayrollAsync(Payroll payroll)
        {
            var existingEmployee = await _context.employees.FirstOrDefaultAsync(e => e.id == payroll.employee_id && e.is_deleted == false);
            if (existingEmployee == null)
            {
                Logger("Employee not found or inactive.");
                throw new InvalidOperationException("Employee not found or inactive.");
            }

            if (payroll.period_start >= payroll.period_end)
            {
                Logger("Period end date must be after start date.");
                throw new InvalidOperationException("Period end date must be after start date.");
            }

            if (payroll.period_end > DateTime.UtcNow)
            {
                Logger("Period end date cannot be in the future.");
                throw new InvalidOperationException("Period end date cannot be in the future.");
            }

            // Check for overlapping payroll periods for same employee
            var existingPayroll = await _context.payroll
                .FirstOrDefaultAsync(p => p.employee_id == payroll.employee_id &&
                                        p.is_deleted == false &&
                                        ((p.period_start <= payroll.period_end && p.period_end >= payroll.period_start) ||
                                         (payroll.period_start <= p.period_end && payroll.period_end >= p.period_start)));
            if (existingPayroll != null)
            {
                Logger("Payroll period overlaps with existing payroll for this employee.");
                throw new InvalidOperationException("Payroll period overlaps with existing payroll for this employee.");
            }

            if (payroll.deductions < 0 || payroll.bonuses < 0)
            {
                Logger("Deductions and bonuses cannot be negative.");
                throw new InvalidOperationException("Deductions and bonuses cannot be negative.");
            }

            // Calculate net pay using employee's salary
            var employeeSalary = existingEmployee.salary ?? 0;
            if (payroll.net_pay == null)
            {
                payroll.net_pay = employeeSalary + (payroll.bonuses ?? 0) - (payroll.deductions ?? 0);
            }

            // Validate net pay calculation
            if (payroll.net_pay < 0)
            {
                Logger("Net pay cannot be negative.");
                throw new InvalidOperationException("Net pay cannot be negative.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                payroll.is_deleted = false;
                payroll.deleted_at = null;
                _context.payroll.Add(payroll);
                await _context.SaveChangesAsync();

                // GENERATE AUTOMATIC TRANSACTIONS!
                string desc = $"Payroll for Employee #{payroll.employee_id}, Period: {payroll.period_start:yyyy-MM-dd} to {payroll.period_end:yyyy-MM-dd}";
                await _transactionGenerator.GeneratePayrollTransactionsAsync(payroll, desc);

                await transaction.CommitAsync();
                return payroll;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Unable to save payroll.");
            }
        }        
    }
}
