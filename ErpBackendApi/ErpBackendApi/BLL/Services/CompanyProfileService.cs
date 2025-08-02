using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

//TODO: Make the company profile service compatible for one company only.

namespace ErpBackendApi.BLL.Services
{
    public class CompanyProfileService : ICompanyProfile
    {
        private readonly AppDataContext _context;
        public CompanyProfileService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CompanyProfile>> GetCompanyProfileAsync()
        {
            return await _context.company_profile.ToListAsync();
        }

        public async Task<CompanyProfile> AddCompanyProfileAsync(CompanyProfile companyProfile)
        {
            companyProfile.is_deleted = false;
            companyProfile.deleted_at = null;
            _context.company_profile.Add(companyProfile);
            await _context.SaveChangesAsync();
            return companyProfile;
        }

        public async Task<CompanyProfile> UpdateCompanyProfileAsync(CompanyProfile companyProfile)
        {
            var existingCompany = await _context.company_profile.FirstOrDefaultAsync(c => c.id == companyProfile.id && c.is_deleted == false);
            if (existingCompany == null)
            {
                Logger("Company not found. Company profile can't be updated.");
                return null;
            }
            existingCompany.company_name = companyProfile.company_name;
            existingCompany.address = companyProfile.address;
            existingCompany.email = companyProfile.email;
            existingCompany.phone = companyProfile.phone;
            existingCompany.tax_number = companyProfile.tax_number;
            existingCompany.logo = companyProfile.logo;
            await _context.SaveChangesAsync();
            return existingCompany;
        }

        public async Task<CompanyProfile> SoftDeleteCompanyProfileAsync(CompanyProfile companyProfile)
        {
            var existingCompany = await _context.company_profile.FirstOrDefaultAsync(c => c.id == companyProfile.id && c.is_deleted == false);
            if (existingCompany == null)
            {
                Logger("Company not found. Company profile can't be deleted.");
                return null;
            }
            existingCompany.is_deleted = true;
            existingCompany.deleted_at = DateTime.UtcNow;
            _context.company_profile.Update(existingCompany);
            await _context.SaveChangesAsync();
            return existingCompany;
        }

        public async Task<CompanyProfile> UndoSoftDeleteCompanyProfileAsync(CompanyProfile companyProfile)
        {
            var existingCompany = await _context.company_profile.FirstOrDefaultAsync(c => c.id == companyProfile.id && c.is_deleted == true);
            if (existingCompany == null)
            {
                Logger("Company not found. Company profile can't be restored.");
                return null;
            }
            existingCompany.is_deleted = false;
            existingCompany.deleted_at = null;
            _context.company_profile.Update(existingCompany);
            await _context.SaveChangesAsync();
            return existingCompany;
        }
    }
}
