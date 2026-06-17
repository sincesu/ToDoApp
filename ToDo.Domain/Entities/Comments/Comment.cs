using ToDo.Domain.Entities.Common;
using ToDo.Domain.Entities.Items;
using ToDo.Domain.Entities.Users;

namespace ToDo.Domain.Entities.Comments
{
    public class Comment : BaseEntity
    {
        public required string content { get; set; }

        public DateTime dateTime { get; set; }


        public Guid ToDoItemsId { get; set; }

        public required ToDoItems ToDoItems { get; set; }

        
        public Guid AppUserId { get; set; }

        public required AppUser AppUser { get; set; }
    }
}
