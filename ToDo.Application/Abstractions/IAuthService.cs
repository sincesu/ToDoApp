using ToDo.Application.DTOs.Auth;
using ToDo.Application.DTOs.User;

namespace ToDo.Application.Abstractions
{
    public interface IAuthService
    {
        public Task RegisterAsync(AppUserSaveDto dto);

        public Task<TokenResponseDto> LoginAsync(LoginDto dto);

        public Task LogoutAsync();

        public Task<TokenResponseDto> RefreshTokenLoginAsync(RefreshTokenDto dto);
    }
}
