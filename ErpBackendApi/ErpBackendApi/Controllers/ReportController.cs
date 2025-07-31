using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErpBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReports _iReports;
        public ReportController(IReports iReports)
        {
            _iReports = iReports;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReports()
        {
            var operation_GetAllReports = await _iReports.GetAllReportsAsync();
            return Ok(operation_GetAllReports);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReportById(int id)
        {
            var operation_GetReportById = await _iReports.GetReportByIdAsync(id);
            if (operation_GetReportById == null)
            {
                return NotFound("Report not found.");
            }
            return Ok(operation_GetReportById);
        }

        [HttpPost]
        public async Task<IActionResult> AddReport(Report report)
        {
            var operation_AddReport = await _iReports.AddReportAsync(report);
            if (operation_AddReport == null)
            {
                return NotFound("Something went wrong. Unable to add report.");
            }
            return Ok("Report added successfully.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateReport(Report report)
        {
            var operation_UpdateReport = await _iReports.UpdateReportAsync(report);
            if (operation_UpdateReport == null)
            {
                return NotFound("Report not found. Unable to update report.");
            }
            return Ok("Report updated successfully.");
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteReport(Report report)
        {
            var operation_SoftDeleteReport = await _iReports.SoftDeleteReportAsync(report);
            if (operation_SoftDeleteReport == null)
            {
                return NotFound("Report not found. Unable to delete report.");
            }
            return Ok("Report deleted successfully.");
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteReport(Report report)
        {
            var operation_UndoSoftDeleteReport = await _iReports.UndoSoftDeleteReportAsync(report);
            if (operation_UndoSoftDeleteReport == null)
            {
                return NotFound("Report not found. Unable to restore deleted report.");
            }
            return Ok("Report restored successfully.");
        }
    }
}
