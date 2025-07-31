using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface IReports
    {
        Task<IEnumerable<ReportDTO>> GetAllReportsAsync();
        Task<ReportDTO> GetReportByIdAsync(int id);
        Task<Report> AddReportAsync(Report report);
        Task<Report> UpdateReportAsync(Report report);
        Task<Report> SoftDeleteReportAsync(Report report);
        Task<Report> UndoSoftDeleteReportAsync(Report report);
    }
}
