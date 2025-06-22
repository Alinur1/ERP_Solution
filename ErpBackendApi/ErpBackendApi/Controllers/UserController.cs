using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Mvc;

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
            var operation_AddUser = await _iUsers.AddUserAsync(user);
            if (operation_AddUser == null)
            {
                return NotFound("An user with this email already exists.");
            }
            return Ok("User created Successfully.");
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser(User user)
        {
            var operation_UpdateUser = await _iUsers.UpdateUserAsync(user);
            if (operation_UpdateUser == null)
            {
                return NotFound("User not found or the user is deleted.");
            }
            return Ok("User updated successfully.");
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
    }
}
