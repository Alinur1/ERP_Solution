using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErpBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendances _attendances;
        public AttendanceController(IAttendances attendances)
        {
            _attendances = attendances;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAttendance()
        {
            var operation_GetAllAttendance = await _attendances.GetAllAttendancesAsync();
            return Ok(operation_GetAllAttendance);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAttendanceById(int id)
        {
            var operation_GetAttendanceById = await _attendances.GetAttendanceByIdAsync(id);
            if (operation_GetAttendanceById == null)
            {
                return NotFound("Attendance not found.");
            }
            return Ok(operation_GetAttendanceById);
        }

        [HttpPost]
        public async Task<IActionResult> AddAttendance(Attendance att)
        {
            var operation_AddAttendance = await _attendances.AddAttendanceAsync(att);
            if (operation_AddAttendance == null)
            {
                return NotFound("Duplicate attendance for same employee on the same day.");
            }
            return Ok("Attendance added successfully.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAttendance(Attendance att)
        {
            var operation_UpdateAttendance = await _attendances.UpdateAttendanceAsync(att);
            if (operation_UpdateAttendance == null)
            {
                return NotFound("Unable to update attendance information. Not found.");
            }
            return Ok("Attendance information updated successfully.");
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteAttendance(Attendance att)
        {
            var operation_SoftDeleteAttendance = await _attendances.SoftDeleteAttendanceAsync(att);
            if (operation_SoftDeleteAttendance == null)
            {
                return NotFound("Attendance not found. Unable to delete attendance.");
            }
            return Ok("Attendance deleted successfully.");
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteAttendance(Attendance att)
        {
            var operation_UndoSoftDeleteAttendance = await _attendances.UndoSoftDeleteAttendanceAsync(att);
            if (operation_UndoSoftDeleteAttendance == null)
            {
                return NotFound("Attendance not found. Unable to restore deleted attendance.");
            }
            return Ok("Deleted attendance restored successfully.");
        }
    }
}
