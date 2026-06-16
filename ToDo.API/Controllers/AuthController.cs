using Microsoft.AspNetCore.Mvc;
using ToDo.API.Filters;
using ToDo.Application.Abstractions;
using ToDo.Application.DTOs.User;

namespace ToDo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAppUserService _appUserService;

        public AuthController(IAppUserService appUserService)
        {
            _appUserService = appUserService;
        }

        [HttpPost("register")]
        [GuestOnly]
        public async Task<IActionResult> RegisterAsync(AppUserSaveDto dto)
        {
            await _appUserService.AddUserAsync(dto);
            return StatusCode(201, $"{dto.name} succesfully registered");
        }

        [HttpPost("login")]
        [GuestOnly]
        public async Task<IActionResult> LoginAsync(LoginDto dto)
        {
            var tokenString = await _appUserService.LoginAsync(dto);
            return Ok(new { token = tokenString });
        }
    }
}
