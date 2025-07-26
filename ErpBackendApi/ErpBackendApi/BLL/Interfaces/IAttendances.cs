using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface IAttendances
    {
        Task<IEnumerable<AttendanceDTO>> GetAllAttendancesAsync();
        Task<AttendanceDTO> GetAttendanceByIdAsync(int id);
        Task<Attendance> AddAttendanceAsync(Attendance att);
        Task<Attendance> UpdateAttendanceAsync(Attendance att);
        Task<Attendance> SoftDeleteAttendanceAsync(Attendance att);
        Task<Attendance> UndoSoftDeleteAttendanceAsync(Attendance att);
    }
}
