using ToDo.Domain.Entities.Common;
using ToDo.Domain.Entities.Comments;
using ToDo.Domain.Entities.Items;

namespace ToDo.Domain.Entities.Users
{
    public class AppUser : BaseEntity
    {
        public string name { get; set; } = string.Empty;

        public string password {  get; set; } = string.Empty;

        public string role { get; set; } = string.Empty;

        public string email { get; set; } = string.Empty;

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

        public ICollection<ToDoItems> ToDoItems { get; set; } = new HashSet<ToDoItems>();
    }
}
