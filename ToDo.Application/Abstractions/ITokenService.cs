using ToDo.Domain.Entities.Users;

namespace ToDo.Application.Abstractions
{
    public interface ITokenService
    {
        public string CreateToken(AppUser user);

        public string CreateRefreshToken();
    }
}
