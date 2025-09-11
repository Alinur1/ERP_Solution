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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPayrollById(int id)
        {
            var operation_GetPayrollById = await _iPayroll.GetPayrollByIdAsync(id);
            if (operation_GetPayrollById == null)
            {
                return BadRequest("Payroll not found.");
            }
            return Ok(operation_GetPayrollById);
        }

        [HttpPost]
        public async Task<IActionResult> AddPayroll(Payroll payroll)
        {
            try
            {
                var operation_AddPayroll = await _iPayroll.AddPayrollAsync(payroll);
                return Ok("Payroll added successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdatePayroll(Payroll payroll)
        {
            try
            {
                var operation_UpdatePayroll = await _iPayroll.UpdatePayrollAsync(payroll);
                return Ok("Payroll updated successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeletePayroll(Payroll payroll)
        {
            try
            {
                var operation_SoftDeletePayroll = await _iPayroll.SoftDeletePayrollAsync(payroll);
                return Ok("Payroll deleted successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }            
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeletePayroll(Payroll payroll)
        {
            try
            {
                var operation_UndoSoftDeletePayroll = await _iPayroll.UndoSoftDeletePayrollAsync(payroll);
                return Ok("Payroll restored successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
