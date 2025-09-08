using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErpBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartments _iDept;
        public DepartmentController(IDepartments iDept)
        {
            _iDept = iDept;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDepartment()
        {
            var operation_GetAllDepartment = await _iDept.GetAllDepartmentsAsync();
            return Ok(operation_GetAllDepartment);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            var operation_GetDepartmentById = await _iDept.GetDepartmentByIdAsync(id);
            if (operation_GetDepartmentById == null)
            {
                return BadRequest("Department not found.");
            }
            return Ok(operation_GetDepartmentById);
        }

        [HttpPost]
        public async Task<IActionResult> AddDepartment(Department dept)
        {
            try
            {
                var operation_AddDepartment = await _iDept.AddDepartmentAsync(dept);
                return Ok("Department added successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateDepartment(Department department)
        {
            try
            {
                var operation_UpdateDepartment = await _iDept.UpdateDepartmentAsync(department);
                return Ok("Department information updated successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteDepartment(Department department)
        {
            try
            {
                var operation_SoftDeleteDepartment = await _iDept.SoftDeleteDepartmentAsync(department);
                return Ok("Department information deleted successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteDepartment(Department department)
        {
            try
            {
                var operation_UndoSoftDeleteDepartment = await _iDept.UndoSoftDeleteDepartmentAsync(department);
                return Ok("Department information restored successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("deleted-departments")]
        public async Task<IActionResult> GetAllDeletedDepartments()
        {
            var operation_GetAllDeletedDepartments = await _iDept.GetAllDeletedDepartmentsAsync();
            return Ok(operation_GetAllDeletedDepartments);
        }
    }
}
