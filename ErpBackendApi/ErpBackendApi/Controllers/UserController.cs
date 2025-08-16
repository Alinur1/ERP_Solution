using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using ZstdSharp.Unsafe;

namespace ErpBackendApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUsers _iUsers;
        public UserController(IUsers iUsers)
        {
            _iUsers = iUsers;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var operation_GetAllUser = await _iUsers.GetAllUsersAsync();
            return Ok(operation_GetAllUser);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var operation_GetUsersById = await _iUsers.GetUserByIdAsync(id);
            if (operation_GetUsersById == null)
            {
                return NotFound("User not found.");
            }
            return Ok(operation_GetUsersById);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(User user)
        {
            try
            {
                var operation_AddUser = await _iUsers.AddUserAsync(user);
                return Ok("User created Successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser(User user)
        {
            try
            {
                var operation_UpdateUser = await _iUsers.UpdateUserAsync(user);
                return Ok("User updated successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteUser(User user)
        {
            var operation_SoftDeleteUser = await _iUsers.SoftDeleteUserAsync(user);
            if (operation_SoftDeleteUser == null)
            {
                return NotFound("User not found or already deleted.");
            }
            return Ok("User deleted successfully.");
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteUser(User user)
        {
            var operation_UndoSoftDeleteUser = await _iUsers.UndoSoftDeleteUserAsync(user);
            if (operation_UndoSoftDeleteUser == null)
            {
                return NotFound("User not found.");
            }
            return Ok("User restored successfully.");
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(User user)
        {
            try
            {
                var operation_ChangePassword = await _iUsers.ChangePasswordAsync(user);
                return Ok("Password changed successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("deleted-users")]
        public async Task<IActionResult> GetAllDeletedUser()
        {
            var operation_GetAllDeletedUser = await _iUsers.GetAllDeletedUsersAsync();
            return Ok(operation_GetAllDeletedUser);
        }
    }
}
