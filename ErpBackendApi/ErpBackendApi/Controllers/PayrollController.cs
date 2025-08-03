using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErpBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayrollController : ControllerBase
    {
        private readonly IPayrolls _iPayroll;
        public PayrollController(IPayrolls iPayroll)
        {
            _iPayroll = iPayroll;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPayroll()
        {
            var operation_GetAllPayroll = await _iPayroll.GetAllPayrollAsync();
            return Ok(operation_GetAllPayroll);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetPayrollById(int id)
        {
            var operation_GetPayrollById = await _iPayroll.GetPayrollByIdAsync(id);
            if (operation_GetPayrollById == null)
            {
                return NotFound("Payroll not found.");
            }
            return Ok(operation_GetPayrollById);
        }

        [HttpPost]
        public async Task<IActionResult> AddPayroll(Payroll payroll)
        {
            var operation_AddPayroll = await _iPayroll.AddPayrollAsync(payroll);
            if (operation_AddPayroll == null)
            {
                return NotFound("Same employee cannot have more than one payroll.");
            }
            return Ok("Payroll added successfully.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePayroll(Payroll payroll)
        {
            var operation_UpdatePayroll = await _iPayroll.UpdatePayrollAsync(payroll);
            if (operation_UpdatePayroll == null)
            {
                return NotFound("Unable to update payroll. Payroll not found.");
            }
            return Ok("Payroll updated successfully.");
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeletePayroll(Payroll payroll)
        {
            var operation_SoftDeletePayroll = await _iPayroll.SoftDeletePayrollAsync(payroll);
            if (operation_SoftDeletePayroll == null)
            {
                return NotFound("Unable to delete payroll. Payroll not found.");
            }
            return Ok("Payroll deleted successfully.");
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeletePayroll(Payroll payroll)
        {
            var operation_UndoSoftDeletePayroll = await _iPayroll.UndoSoftDeletePayrollAsync(payroll);
            if (operation_UndoSoftDeletePayroll == null)
            {
                return NotFound("Unable to restore payroll. Payroll not found.");
            }
            return Ok("Payroll restored successfully.");
        }
    }
}
