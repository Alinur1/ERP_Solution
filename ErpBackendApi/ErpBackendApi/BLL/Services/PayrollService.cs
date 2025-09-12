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
                await transaction.CommitAsync();
                return payroll;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Unable to save payroll.");
            }
        }

        public async Task<Payroll> UpdatePayrollAsync(Payroll payroll)
        {
            var existingPayroll = await _context.payroll.FirstOrDefaultAsync(p => p.id == payroll.id && p.is_deleted == false);
            if (existingPayroll == null)
            {
                Logger("Payroll not found or deleted.");
                throw new InvalidOperationException("Payroll not found or deleted.");
            }

            int? targetEmployeeId = payroll.employee_id ?? existingPayroll.employee_id;
            var existingEmployee = await _context.employees.FirstOrDefaultAsync(e => e.id == targetEmployeeId && e.is_deleted == false);

            if (existingEmployee == null)
            {
                Logger("Employee not found or inactive.");
                throw new InvalidOperationException("Employee not found or inactive.");
            }

            // Validate period dates
            if (payroll.period_start >= payroll.period_end)
            {
                Logger("Period end date must be after start date.");
                throw new InvalidOperationException("Period end date must be after start date.");
            }

            // Check for overlapping payroll periods (excluding current record)
            var overlappingPayroll = await _context.payroll
                .FirstOrDefaultAsync(p => p.id != payroll.id &&
                                        p.employee_id == targetEmployeeId &&
                                        p.is_deleted == false &&
                                        ((p.period_start <= payroll.period_end && p.period_end >= payroll.period_start) ||
                                         (payroll.period_start <= p.period_end && payroll.period_end >= p.period_start)));

            if (overlappingPayroll != null)
            {
                Logger("Payroll period overlaps with existing payroll for this employee.");
                throw new InvalidOperationException("Payroll period overlaps with existing payroll for this employee.");
            }

            // Validate financial values
            if (payroll.deductions < 0 || payroll.bonuses < 0)
            {
                Logger("Deductions and bonuses cannot be negative.");
                throw new InvalidOperationException("Deductions and bonuses cannot be negative.");
            }

            // Calculate net pay using employee's salary
            var employeeSalary = existingEmployee.salary ?? 0;
            var netPay = payroll.net_pay ?? employeeSalary +
                                                (payroll.bonuses ?? existingPayroll.bonuses) -
                                                (payroll.deductions ?? existingPayroll.deductions);

            if (netPay < 0)
            {
                Logger("Net pay cannot be negative.");
                throw new InvalidOperationException("Net pay cannot be negative.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                existingPayroll.employee_id = payroll.employee_id ?? existingPayroll.employee_id;
                existingPayroll.period_start = payroll.period_start ?? existingPayroll.period_start;
                existingPayroll.period_end = payroll.period_end ?? existingPayroll.period_end;
                existingPayroll.deductions = payroll.deductions ?? existingPayroll.deductions;
                existingPayroll.bonuses = payroll.bonuses ?? existingPayroll.bonuses;
                existingPayroll.net_pay = netPay;
                existingPayroll.paid_on = payroll.paid_on ?? existingPayroll.paid_on;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return existingPayroll;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Unable to update payroll.");
            }
        }

        public async Task<Payroll> SoftDeletePayrollAsync(Payroll payroll)
        {
            var existingPayroll = await _context.payroll.FirstOrDefaultAsync(p => p.id == payroll.id && p.is_deleted == false);
            if (existingPayroll == null)
            {
                Logger("Payroll not found or already deleted.");
                throw new InvalidOperationException("Payroll not found or already deleted.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                existingPayroll.is_deleted = true;
                existingPayroll.deleted_at = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return existingPayroll;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Unable to delete payroll.");
            }
        }

        public async Task<Payroll> UndoSoftDeletePayrollAsync(Payroll payroll)
        {
            var existingPayroll = await _context.payroll.FirstOrDefaultAsync(p => p.id == payroll.id && p.is_deleted == true);
            if (existingPayroll == null)
            {
                Logger("Deleted payroll not found.");
                throw new InvalidOperationException("Deleted payroll not found.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                existingPayroll.is_deleted = false;
                existingPayroll.deleted_at = null;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return existingPayroll;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Unable to restore payroll.");
            }
        }

        public async Task<IEnumerable<PayrollDTO>> GetAllDeletedPayrollAsync()
        {
            return await
            (
                from p in _context.payroll
                join e in _context.employees on p.employee_id equals e.id into employeeGroup
                from e in employeeGroup.DefaultIfEmpty()
                join u in _context.users on e.user_id equals u.id into userGroup
                from u in userGroup.DefaultIfEmpty()
                where p.is_deleted == true
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
                    paid_on = p.paid_on,
                    is_deleted = p.is_deleted,
                    deleted_at = p.deleted_at
                }
            ).ToListAsync();
        }
    }
}
