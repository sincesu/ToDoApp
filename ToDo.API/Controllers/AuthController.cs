using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDo.API.Filters;
using ToDo.Application.Abstractions;
using ToDo.Application.DTOs.Auth;
using ToDo.Application.DTOs.User;

namespace ToDo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [GuestOnly]
        public async Task<IActionResult> RegisterAsync(AppUserSaveDto dto)
        {
            await _authService.RegisterAsync(dto);
            return StatusCode(201, $"{dto.name} succesfully registered");
        }

        [HttpPost("login")]
        [GuestOnly]
        public async Task<IActionResult> LoginAsync(LoginDto dto)
        {
            var tokenResponse = await _authService.LoginAsync(dto);
            return Ok(tokenResponse);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> LogoutAsync()
        {
            await _authService.LogoutAsync();
            return Ok();
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenController(RefreshTokenDto dto)
        {
            var tokenResponse = await _authService.RefreshTokenLoginAsync(dto);
            return Ok(tokenResponse);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("adduser")]
        public async Task<IActionResult> AddUserAsync(AppUserSaveDto dto)
        {
            await _authService.RegisterAsync(dto);
            return StatusCode(201, $"{dto.name} has been created");
        }

    }
}
