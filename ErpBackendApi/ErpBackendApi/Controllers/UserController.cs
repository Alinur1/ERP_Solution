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
        public async Task<IActionResult> GetUsersById(int id)
        {
            var operation_GetUsersById = await _iUsers.GetUserByIdAsync(id);
            if(operation_GetUsersById == null)
            {
                return NotFound();
            }
            return Ok(operation_GetUsersById);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(User user)
        {
            var operation_AddUser = await _iUsers.AddUserAsync(user);
            if(operation_AddUser == null)
            {
                return NotFound();
            }
            return Ok(operation_AddUser);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser(User user)
        {
            var operation_UpdateUser = await _iUsers.UpdateUserAsync(user);
            if(operation_UpdateUser == null)
            {
                return NotFound();
            }
            return Ok(operation_UpdateUser);
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteUser(User user)
        {
            var operation_SoftDeleteUser = await _iUsers.SoftDeleteUserAsync(user);
            if(operation_SoftDeleteUser == null)
            {
                return NotFound();
            }
            return Ok(operation_SoftDeleteUser);
        }
    }
}
