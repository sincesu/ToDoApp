using ToDo.Application.DTOs.User;

namespace ToDo.Application.Abstractions
{
    public interface IAppUserService
    {
        public Task<IEnumerable<AppUserDto>> GetAllUsersAsync();

        public Task AddUserAsync(AppUserSaveDto dto);

        public Task<string> LoginAsync(LoginDto dto);

        public Task<AppUserDto> GetByUserIdAsync(int id);

        public Task UpdateAsync(int id, AppUserUpdateDto dto);

        public Task DeleteAsync(int id);
    }
}
