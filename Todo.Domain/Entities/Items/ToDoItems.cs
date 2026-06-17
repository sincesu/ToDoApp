using ToDo.Domain.Entities.Categories;
using ToDo.Domain.Entities.Common;
using ToDo.Domain.Entities.Users;
using ToDo.Domain.Enums;

namespace ToDo.Domain.Entities.Items
{
    public class ToDoItems : BaseEntity
    {
        public string content { get; set; } = string.Empty;

        public int priority { get; set; }

        public bool isDeleted { get; set; }

        public bool isCompleted { get; set; } = false;

        public DateTime createdDate { get; set; }

        public DateTime? completedDate { get; set; }

        public Guid CategoryId { get; set; }

        public Category? Category { get; set; }

        public Guid AppUserId { get; set; }

        public AppUser? AppUser { get; set; }

        public TaskState State { get; set; } = TaskState.Created;
    }
}
