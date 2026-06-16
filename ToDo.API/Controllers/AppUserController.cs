using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDo.Application.Abstractions;
using ToDo.Application.DTOs.User;

namespace ToDo.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AppUserController : ControllerBase
    {
        private readonly IAppUserService _appUserService;

        public AppUserController(IAppUserService appUserService)
        {
            _appUserService = appUserService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _appUserService.GetAllUsersAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("adduser")]
        public async Task<IActionResult> AddUserAsync(AppUserSaveDto dto)
        {
            await _appUserService.AddUserAsync(dto);
            return StatusCode(201, $"{dto.name} has been created");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var dto = await _appUserService.GetByUserIdAsync(id);
            
            return Ok(dto);
        }

        [HttpPatch("update/{id:Guid}")]
        public async Task<IActionResult> UpdateAsync(Guid id, AppUserUpdateDto dto)
        {
            await _appUserService.UpdateAsync(id, dto);

            return Ok();
        }

        [HttpDelete("delete/{id:Guid}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _appUserService.DeleteAsync(id);
            return NoContent();
        }
    }
}
