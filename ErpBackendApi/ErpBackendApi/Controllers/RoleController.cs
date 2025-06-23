using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErpBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoles _iRoles;
        public RoleController(IRoles iRoles)
        {
            _iRoles = iRoles;
        }

        [HttpPost]
        public async Task<IActionResult> AddRoles(Role role)
        {
            var operation_AddRoles = await _iRoles.AddRoleAsync(role);
            if (operation_AddRoles == null)
            {
                return NotFound("This role already exists.");
            }
            return Ok("Role created successfully.");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var operation_GetAllRoles = await _iRoles.GetAllRolesAsync();
            return Ok(operation_GetAllRoles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            var operation_GetRoleById = await _iRoles.GetRoleByIdAsync(id);
            if(operation_GetRoleById == null)
            {
                return NotFound("Role not found.");
            }
            return Ok(operation_GetRoleById);
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteRole(Role role)
        {
            var operation_SoftDeleteRole = await _iRoles.SoftDeleteRoleAsync(role);
            if(operation_SoftDeleteRole == null)
            {
                return NotFound("Role not found or deleted already.");
            }
            return Ok("Role deleted successfully.");
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteRole(Role role)
        {
            var operation_UndoSoftDeleteRole = await _iRoles.UndoSoftDeleteRoleAsync(role);
            if(operation_UndoSoftDeleteRole == null)
            {
                return NotFound("Role not found.");
            }
            return Ok("Roles restored successfully.");
        }
    }
}
