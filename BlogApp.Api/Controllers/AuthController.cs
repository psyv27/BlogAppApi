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

        [HttpPost("[action]")]
        public async Task<IActionResult> Register (RegisterDto dto)
        {
           await _userManager.RegisterAsync(dto);
            return NoContent();

        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            return Ok(await _userManager.LoginAsync(dto));

        }
    }
}
