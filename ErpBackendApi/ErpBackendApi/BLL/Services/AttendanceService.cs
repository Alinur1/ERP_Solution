using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.Services
{
    public class AttendanceService : IAttendances
    {
        private readonly AppDataContext _context;
        public AttendanceService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AttendanceDTO>> GetAllAttendancesAsync()
        {
            return await
            (
                from a in _context.attendance
                join e in _context.employees on a.employee_id equals e.id into employeeGroup
                from e in employeeGroup.DefaultIfEmpty()
                join u in _context.users on e.user_id equals u.id into userGroup
                from u in userGroup.DefaultIfEmpty()
                where a.is_deleted == false
                select new AttendanceDTO
                {
                    id = a.id,
                    employee_id = e != null && e.is_deleted == false ? e.id : null,
                    employee_name = u != null && u.is_deleted == false ? u.name : null,
                    date_of_attendance = a.date_of_attendance,
                    check_in = a.check_in,
                    check_out = a.check_out,
                    status = a.status,
                }
            ).ToListAsync();
        }

        public async Task<AttendanceDTO> GetAttendanceByIdAsync(int id)
        {
            return await
            (
                from a in _context.attendance
                join e in _context.employees on a.employee_id equals e.id into employeeGroup
                from e in employeeGroup.DefaultIfEmpty()
                join u in _context.users on e.user_id equals u.id into userGroup
                from u in userGroup.DefaultIfEmpty()
                where a.id == id && a.is_deleted == false
                select new AttendanceDTO
                {
                    id = a.id,
                    employee_id = e != null && e.is_deleted == false ? e.id : null,
                    employee_name = u != null && u.is_deleted == false ? u.name : null,
                    date_of_attendance = a.date_of_attendance,
                    check_in = a.check_in,
                    check_out = a.check_out,
                    status = a.status,
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<Attendance> AddAttendanceAsync(Attendance att)
        {
            var existingAttendace = await _context.attendance.FirstOrDefaultAsync(a => a.employee_id == att.employee_id && a.date_of_attendance == att.date_of_attendance && a.is_deleted == false);
            if (existingAttendace != null)
            {
                Logger("Duplicate attendance for same employee on the same day.");
                throw new InvalidOperationException("Duplicate attendance for same employee on the same day.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                att.is_deleted = false;
                att.deleted_at = null;
                _context.attendance.Add(att);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return att;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Unable to save attendance.");
            }
        }

        public async Task<Attendance> UpdateAttendanceAsync(Attendance att)
        {
            var existingAttendance = await _context.attendance.FirstOrDefaultAsync(a => a.id == att.id);
            if (existingAttendance == null)
            {
                Logger("Unable to update attendance information. Not found.");
                throw new InvalidOperationException("Unable to update attendance information. Not found.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                existingAttendance.date_of_attendance = att.date_of_attendance;
                existingAttendance.check_in = att.check_in;
                existingAttendance.check_out = att.check_out;
                existingAttendance.status = att.status;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return existingAttendance;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Unable to update attendance.");
            }
        }

        public async Task<Attendance> SoftDeleteAttendanceAsync(Attendance att)
        {
            var existingAttendance = await _context.attendance.FirstOrDefaultAsync(a => a.id == att.id && a.is_deleted == false);
            if (existingAttendance == null)
            {
                Logger("Attendance not found. Unable to delete attendance.");
                throw new InvalidOperationException("Attendance not found. Unable to delete attendance.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                existingAttendance.is_deleted = true;
                existingAttendance.deleted_at = DateTime.UtcNow;
                _context.attendance.Update(existingAttendance);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return existingAttendance;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Unable to delete attendance.");
            }
        }

        public async Task<Attendance> UndoSoftDeleteAttendanceAsync(Attendance att)
        {
            var existingAttendance = await _context.attendance.FirstOrDefaultAsync(a => a.id == att.id && a.is_deleted == true);
            if (existingAttendance == null)
            {
                Logger("Attendance not found. Unable to restore deleted attendance.");
                throw new InvalidOperationException("Attendance not found. Unable to restore deleted attendance.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                existingAttendance.is_deleted = false;
                existingAttendance.deleted_at = null;
                _context.attendance.Update(existingAttendance);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return existingAttendance;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Unable to restore deleted attendance.");
            }
        }
    }
}
