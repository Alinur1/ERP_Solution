using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErpBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployees _iEmployee;
        public EmployeeController(IEmployees iEmployee)
        {
            _iEmployee = iEmployee;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var operation_GetAllEmployees = await _iEmployee.GetAllEmployeesAsync();
            return Ok(operation_GetAllEmployees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var operation_GetEmployeeById = await _iEmployee.GetEmployeeByIdAsync(id);
            if (operation_GetEmployeeById == null)
            {
                return BadRequest("Employee not found.");
            }
            return Ok(operation_GetEmployeeById);
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee(Employee emp)
        {
            try
            {
                var operation_AddEmployee = await _iEmployee.AddEmployeeAsync(emp);
                return Ok("Employee added successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateEmployee(Employee emp)
        {
            try
            {
                var operation_UpdateEmployee = await _iEmployee.UpdateEmployeeAsync(emp);
                return Ok("Employee information updated successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteEmployee(Employee emp)
        {
            try
            {
                var operation_SoftDeleteEmployee = await _iEmployee.SoftDeleteEmployeeAsync(emp);
                return Ok("Employee information deleted successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteEmployee(Employee emp)
        {
            try
            {
                var operation_UndoSoftDeleteEmployee = await _iEmployee.UndoSoftDeleteEmployeeAsync(emp);
                return Ok("Employee information restored successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
