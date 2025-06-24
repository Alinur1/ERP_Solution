using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static ErpBackendApi.Helper.LoggerClass;

namespace ErpBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoles _iUserRoles;
        public UserRoleController(IUserRoles iUserRoles)
        {
            _iUserRoles = iUserRoles;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUserRoles()
        {
            var operation_GetAllUserRoles = await _iUserRoles.GetAllUserRolesAsync();
            return Ok(operation_GetAllUserRoles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserRolesById(int id)
        {
            var operation_GetUserRolesById = await _iUserRoles.GetUserRoleByIdAsync(id);
            if (operation_GetUserRolesById == null)
            {
                return NotFound("User role not found.");
            }
            return Ok(operation_GetUserRolesById);
        }

        [HttpPost]
        public async Task<IActionResult> AssignUserRole(UserRole userRole)
        {
            var operation_AssignUserRole = await _iUserRoles.AssignUserRoleAsync(userRole);
            if (operation_AssignUserRole == null)
            {
                Logger("An error occurred at UserRoleController in AssignUserRole");
                return NotFound("Something went wrong. Please try again.");
            }
            return Ok("Role assigned to the user successfully.");
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUserRole(UserRole userRole)
        {
            var operation_UpdateUserRole = await _iUserRoles.UpdateUserRoleAsync(userRole);
            if (operation_UpdateUserRole == null)
            {
                Logger("An error occurred at UserRoleController in UpdateUserRole");
                return NotFound("Something went wrong. Please try again.");
            }
            return Ok("User's role updated successfully.");
        }

        [HttpPut("delete")]
        public async Task<IActionResult> RemoveUserRole(UserRole userRole)
        {
            var operation_RemoveUserRole = await _iUserRoles.RemoveUserRoleAsync(userRole);
            if (operation_RemoveUserRole == null)
            {
                Logger("An error occurred at UserRoleController in RemoveUserRole");
                return NotFound("Something went wrong. Please try again.");
            }
            return Ok("User's role removed successfully.");
        }
    }
}
