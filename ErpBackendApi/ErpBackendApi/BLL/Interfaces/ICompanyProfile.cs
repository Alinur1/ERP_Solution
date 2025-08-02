using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface ICompanyProfile
    {
        Task<IEnumerable<CompanyProfile>> GetCompanyProfileAsync();
        Task<CompanyProfile> AddCompanyProfileAsync(CompanyProfile companyProfile);
        Task<CompanyProfile> UpdateCompanyProfileAsync(CompanyProfile companyProfile);
        Task<CompanyProfile> SoftDeleteCompanyProfileAsync(CompanyProfile companyProfile);
        Task<CompanyProfile> UndoSoftDeleteCompanyProfileAsync(CompanyProfile companyProfile);
    }
}
