
namespace ToDo.Application.DTOs.User
{
    public class AppUserSaveDto
    {
        public required string name { get; set; }

        public required string password { get; set; }

        public required string email { get; set; }
    }
}
