using BlogApp.BL.Dtos.UserDtos;
using BlogApp.BL.Services.Interfaces;
using BlogApp.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IUserService _userManager) : ControllerBase
    {

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <remarks>
        /// Example request:
        /// 
        ///     POST /api/auth/register
        ///     {
        ///         "name": "John",
        ///         "surname": "Doe",
        ///         "userName": "johndoe",
        ///         "email": "john.doe@example.com",
        ///         "password": "SecurePassword123",
        ///         "confirmPassword": "SecurePassword123"
        ///     }
        /// </remarks>
        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register (RegisterDto dto)
        {
           await _userManager.RegisterAsync(dto);
            return NoContent();

        }

        /// <summary>
        /// Login user
        /// </summary>
        /// <remarks>
        /// Example request:
        /// 
        ///     POST /api/auth/login
        ///     {
        ///         "userName": "johndoe",
        ///         "password": "SecurePassword123"
        ///     }
        /// </remarks>
        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            return Ok(await _userManager.LoginAsync(dto));

        }
    }
}
