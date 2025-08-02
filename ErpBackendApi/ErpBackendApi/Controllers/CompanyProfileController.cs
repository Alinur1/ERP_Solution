using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace ErpBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyProfileController : ControllerBase
    {
        private readonly ICompanyProfile _comp;
        public CompanyProfileController(ICompanyProfile comp)
        {
            _comp = comp;
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanyProfiles()
        {
            var operation_GetCompanyProfiles = await _comp.GetCompanyProfileAsync();
            if(operation_GetCompanyProfiles == null)
            {
                return NotFound("No active company profile found.");
            }
            return Ok(operation_GetCompanyProfiles);
        }

        [HttpPost]
        public async Task<IActionResult> AddCompanyProfile(CompanyProfile company)
        {
            var operation_AddCompanyProfile = await _comp.AddCompanyProfileAsync(company);
            if (operation_AddCompanyProfile == null)
            {
                return NotFound("Error! Unable to add company profile.");
            }
            return Ok("Company profile added successfully.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCompanyProfile(CompanyProfile company)
        {
            var operation_UpdateCompanyProfile = await _comp.UpdateCompanyProfileAsync(company);
            if (operation_UpdateCompanyProfile == null)
            {
                return NotFound("Company not found. Company profile can't be updated.");
            }
            return Ok("Company profile updated successfully.");
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteCompanyProfile(CompanyProfile company)
        {
            var operation_SoftDeleteCompanyProfile = await _comp.SoftDeleteCompanyProfileAsync(company);
            if (operation_SoftDeleteCompanyProfile == null)
            {
                return NotFound("Company not found. Company profile can't be deleted.");
            }
            return Ok("Company profile deleted successfully.");
        }

        [HttpPut("soft-delete")]
        public async Task<IActionResult> UndoSoftDeleteCompanyProfile(CompanyProfile company)
        {
            var operation_UndoSoftDeleteCompanyProfile = await _comp.UndoSoftDeleteCompanyProfileAsync(company);
            if (operation_UndoSoftDeleteCompanyProfile== null)
            {
                return NotFound("Company not found. Company profile can't be restored.");
            }
            return Ok("Company profile restored successfully.");
        }
    }
}
