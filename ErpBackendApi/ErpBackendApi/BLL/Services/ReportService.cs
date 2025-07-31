using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.Services
{
    public class ReportService : IReports
    {
        private readonly AppDataContext _context;
        public ReportService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReportDTO>> GetAllReportsAsync()
        {
            return await
            (
                from r in _context.reports
                join u in _context.users on r.created_by equals u.id into usersGroup
                from u in usersGroup.DefaultIfEmpty()
                where r.is_deleted == false
                select new ReportDTO
                {
                    id = r.id,
                    name = r.name,
                    module = r.module,
                    created_by = u != null ? u.id : null,
                    created_by_name = u !=null && u.is_deleted == false ? u.name : null,
                    created_at = r.created_at,
                    filters_json = r.filters_json,
                }
            ).ToListAsync();
        }

        public async Task<ReportDTO> GetReportByIdAsync(int id)
        {
            return await
            (
                from r in _context.reports
                join u in _context.users on r.created_by equals u.id into usersGroup
                from u in usersGroup.DefaultIfEmpty()
                where r.id == id && r.is_deleted == false
                select new ReportDTO
                {
                    id = r.id,
                    name = r.name,
                    module = r.module,
                    created_by = u != null ? u.id : null,
                    created_by_name = u != null && u.is_deleted == false ? u.name : null,
                    created_at = r.created_at,
                    filters_json = r.filters_json,
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<Report> AddReportAsync(Report report)
        {
            report.created_at = DateTime.UtcNow;
            report.is_deleted = false;
            report.deleted_at = null;
            _context.reports.Add(report);
            await _context.SaveChangesAsync();
            return report;
        }

        public async Task<Report> UpdateReportAsync(Report report)
        {
            var existingReport = await _context.reports.FirstOrDefaultAsync(r => r.id == report.id && r.is_deleted == false);
            if (existingReport == null)
            {
                Logger("Report not found. Unable to update report.");
                return null;
            }
            existingReport.name = report.name;
            existingReport.module = report.module;
            existingReport.created_by = report.created_by;
            await _context.SaveChangesAsync();
            return existingReport;
        }

        public async Task<Report> SoftDeleteReportAsync(Report report)
        {
            var existingReport = await _context.reports.FirstOrDefaultAsync(r => r.id == report.id && r.is_deleted == false);
            if (existingReport == null)
            {
                Logger("Report not found. Unable to delete report.");
                return null;
            }
            existingReport.is_deleted = true;
            existingReport.deleted_at = DateTime.UtcNow;
            _context.reports.Update(existingReport);
            await _context.SaveChangesAsync();
            return existingReport;
        }

        public async Task<Report> UndoSoftDeleteReportAsync(Report report)
        {
            var existingReport = await _context.reports.FirstOrDefaultAsync(r => r.id == report.id && r.is_deleted == true);
            if (existingReport == null)
            {
                Logger("Report not found. Unable to restore deleted report.");
                return null;
            }
            existingReport.is_deleted = false;
            existingReport.deleted_at = null;
            _context.reports.Update(existingReport);
            await _context.SaveChangesAsync();
            return existingReport;
        }
    }
}
