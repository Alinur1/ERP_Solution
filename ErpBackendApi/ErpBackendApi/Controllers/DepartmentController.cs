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
                return NotFound("Department not found.");
            }
            return Ok(operation_GetDepartmentById);
        }

        [HttpPost]
        public async Task<IActionResult> AddDepartment(Department dept)
        {
            var operation_AddDepartment = await _iDept.AddDepartmentAsync(dept);
            if (operation_AddDepartment == null)
            {
                return NotFound("Unable to add department. Same department name already exists.");
            }
            return Ok("Department added successfully.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDepartment(Department department)
        {
            var operation_UpdateDepartment = await _iDept.UpdateDepartmentAsync(department);
            if (operation_UpdateDepartment == null)
            {
                return NotFound("Unable to update department information. Department not found.");
            }
            return Ok("Department information updated successfully.");
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteDepartment(Department department)
        {
            var operation_SoftDeleteDepartment = await _iDept.SoftDeleteDepartmentAsync(department);
            if (operation_SoftDeleteDepartment == null)
            {
                return NotFound("Unable to delete department information. Department not found.");
            }
            return Ok("Department information deleted successfully.");
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteDepartment(Department department)
        {
            var operation_UndoSoftDeleteDepartment = await _iDept.UndoSoftDeleteDepartmentAsync(department);
            if (operation_UndoSoftDeleteDepartment == null)
            {
                return NotFound("Unable to restore deleted department information. Department not found.");
            }
            return Ok("Department information restored successfully.");
        }
    }
}
