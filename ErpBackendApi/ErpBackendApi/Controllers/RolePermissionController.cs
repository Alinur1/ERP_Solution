using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.BLL.Services;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErpBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolePermissionController : ControllerBase
    {
        private readonly IRolePermissions _iRolePermissions;
        public RolePermissionController(IRolePermissions iRolePermissions)
        {
            _iRolePermissions = iRolePermissions;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRolePermission()
        {
            var operation_GetAllRolePermission = await _iRolePermissions.GetAllRolePermissionAsync();
            return Ok(operation_GetAllRolePermission);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRolePermissionById(int id)
        {
            var operation_GetRolePermissionById = await _iRolePermissions.GetRolePermissionAsyncById(id);
            if(operation_GetRolePermissionById == null)
            {
                return NotFound("Role permission not found for this id.");
            }
            return Ok(operation_GetRolePermissionById);
        }

        [HttpPost]
        public async Task<IActionResult> AddRolePermission([FromBody] List<RolePermission> rolePermissions)
        {
            var addedRolePermissions = new List<RolePermission>();

            foreach (var rolePermission in rolePermissions)
            {
                var operation_AddRolePermission = await _iRolePermissions.AddRolePermissionAsync(rolePermission);
                if(operation_AddRolePermission == null)
                {
                    return NotFound("The feature for the same role already exists.");
                }
                addedRolePermissions.Add(rolePermission);
            }
            return Ok("Role permission added successfully.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRolePermission(RolePermission rolePermission)
        {
            var operation_UpdateRolePermission = await _iRolePermissions.UpdateRolePermissionAsync(rolePermission);
            if(operation_UpdateRolePermission == null)
            {
                return NotFound("Unable to update role permission.");
            }
            return Ok("Role permission updated successfully.");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRolePermission(int id)
        {
            var operation_DeleteRolePermission = await _iRolePermissions.DeleteRolePermissionAsync(id);
            if(operation_DeleteRolePermission == null)
            {
                return NotFound("Unable to delete role permission.");
            }
            return Ok("Role permission deleted successfully.");
        }
    }
}
