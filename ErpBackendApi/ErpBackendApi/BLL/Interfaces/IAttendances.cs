using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface IAttendances
    {
        Task<IEnumerable<AttendanceDTO>> GetAllAttendancesAsync();
        Task<AttendanceDTO> GetAttendanceByIdAsync(int id);
        Task<Attendance> AddAttendanceAsync(Attendance attendance);
        Task<Attendance> UpdateAttendanceAsync(Attendance attendance);
        Task<bool> DeleteAttendanceAsync(int id);
    }
}
