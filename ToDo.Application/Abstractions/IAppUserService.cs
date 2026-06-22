using ToDo.Application.DTOs.User;

namespace ToDo.Application.Abstractions
{
    public interface IAppUserService
    {
        public Task<IEnumerable<AppUserDto>> GetAllUsersAsync();

        public Task<IEnumerable<UserTasksDto>> GetAllTasksAsync();

        public Task AddUserAsync(AppUserSaveDto dto);

        public Task<string> LoginAsync(LoginDto dto);

        public Task<AppUserDto> GetByUserIdAsync(Guid id);

        public Task UpdateAsync(Guid id, AppUserUpdateDto dto);

        public Task DeleteAsync(Guid id);
    }
}
