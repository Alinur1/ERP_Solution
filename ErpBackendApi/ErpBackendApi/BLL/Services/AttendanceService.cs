using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Services
{
    public class AttendanceService : IAttendances
    {
        private readonly AppDataContext _context;
        public AttendanceService(AppDataContext context)
        {
            _context = context;
        }

        public Task<IEnumerable<AttendanceDTO>> GetAllAttendancesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AttendanceDTO> GetAttendanceByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Attendance> AddAttendanceAsync(Attendance attendance)
        {
            throw new NotImplementedException();
        }

        public Task<Attendance> UpdateAttendanceAsync(Attendance attendance)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAttendanceAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
