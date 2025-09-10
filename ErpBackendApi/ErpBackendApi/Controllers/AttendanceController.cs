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
                return BadRequest("Attendance not found.");
            }
            return Ok(operation_GetAttendanceById);
        }

        [HttpPost]
        public async Task<IActionResult> AddAttendance(Attendance att)
        {
            try
            {
                var operation_AddAttendance = await _attendances.AddAttendanceAsync(att);
                return Ok("Attendance added successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateAttendance(Attendance att)
        {
            try
            {
                var operation_UpdateAttendance = await _attendances.UpdateAttendanceAsync(att);
                return Ok("Attendance information updated successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteAttendance(Attendance att)
        {
            try
            {
                var operation_SoftDeleteAttendance = await _attendances.SoftDeleteAttendanceAsync(att);
                return Ok("Attendance deleted successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteAttendance(Attendance att)
        {
            try
            {
                var operation_UndoSoftDeleteAttendance = await _attendances.UndoSoftDeleteAttendanceAsync(att);
                return Ok("Deleted attendance restored successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("deleted-attendance")]
        public async Task<IActionResult> GetAllDeletedAttendance()
        {
            var operation_GetAllDeletedAttendance = await _attendances.GetAllDeletedAttendancesAsync();
            return Ok(operation_GetAllDeletedAttendance);
        }
    }
}
