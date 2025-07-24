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

        [HttpGet("id")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var operation_GetEmployeeById = await _iEmployee.GetEmployeeByIdAsync(id);
            if (operation_GetEmployeeById == null)
            {
                return NotFound("Employee not found.");
            }
            return Ok(operation_GetEmployeeById);
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee(Employee emp)
        {
            var operation_AddEmployee = await _iEmployee.AddEmployeeAsync(emp);
            if (operation_AddEmployee == null)
            {
                return NotFound("Same employee cannot be added in the same department.");
            }
            return Ok("Employee added successfully.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateEmployee(Employee emp)
        {
            var operation_UpdateEmployee = await _iEmployee.UpdateEmployeeAsync(emp);
            if (operation_UpdateEmployee == null)
            {
                return NotFound("Employee not found. Unable to update employee information.");
            }
            return Ok("Employee information updated successfully.");
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteEmployee(Employee emp)
        {
            var operation_SoftDeleteEmployee = await _iEmployee.SoftDeleteEmployeeAsync(emp);
            if (operation_SoftDeleteEmployee == null)
            {
                return NotFound("Employee not found. Unable to delete employee information.");
            }
            return Ok("Employee information deleted successfully.");
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteEmployee(Employee emp)
        {
            var operation_UndoSoftDeleteEmployee = await _iEmployee.UndoSoftDeleteEmployeeAsync(emp);
            if (operation_UndoSoftDeleteEmployee == null)
            {
                return NotFound("Employee not found. Unable to restore deleted employee information.");
            }
            return Ok("Employee information restored successfully.");
        }
    }
}
