using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.BLL.Services;
using ErpBackendApi.DAL.Models;
using ErpBackendApi.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ErpBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUsers _iUsers;
        public AuthController(IUsers iUsers)
        {
            _iUsers = iUsers;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            var operation_AddUser = await _iUsers.AddUserAsync(user);
            if (operation_AddUser == null)
            {
                return NotFound("An user with this email already exists.");
            }
            return Ok("User created Successfully.");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(User user, [FromServices] JwtHelper jwtHelper)
        {
            var ExistingUser = await _iUsers.ValidateUserAsync(user.email, user.password);
            if (ExistingUser == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var token = jwtHelper.GenerateToken(ExistingUser);
            return Ok(new
            {
                message = "Login successful",
                token,
                user = new { ExistingUser.id, ExistingUser.name, ExistingUser.email }
            });
        }
    }
}
