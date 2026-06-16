
namespace ToDo.Domain.Entities.Users
{
    public class AppUser
    {
        public int id { get; set; }

        public string name { get; set; } = string.Empty;

        public string password {  get; set; } = string.Empty;

        public string role { get; set; } = string.Empty;

        public string email { get; set; } = string.Empty;

        public bool isDeleted { get; set; }
    }
}
