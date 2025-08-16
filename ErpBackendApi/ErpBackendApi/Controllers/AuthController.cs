using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.BLL.Services;
using ErpBackendApi.DAL.Models;
using ErpBackendApi.Utilities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ErpBackendApi.DAL.DTOs;

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
            var operation_Register = await _iUsers.AddUserAsync(user);
            if (operation_Register == null)
            {
                return NotFound("An user with this email already exists.");
            }
            return Ok("User created Successfully.");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO request, [FromServices] JwtHelper jwtHelper)
        {
            User existingUser = null;

            if (!string.IsNullOrEmpty(request.Email))
            {
                existingUser = await _iUsers.ValidateUserByEmailAsync(request.Email, request.Password);
            }
            else if (!string.IsNullOrEmpty(request.Phone))
            {
                existingUser = await _iUsers.ValidateUserByPhoneAsync(request.Phone, request.Password);
            }

            if (existingUser == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var token = jwtHelper.GenerateToken(existingUser);
            return Ok(new
            {
                message = "Login successful",
                token,
                user = new { existingUser.id, existingUser.name, existingUser.email, existingUser.phone }
            });
        }

    }
}
