using ToDo.Domain.Entities.Common;
using ToDo.Domain.Entities.Items;
using ToDo.Domain.Enums;

namespace ToDo.Domain.Entities.Histories
{
    public class TaskHistory : BaseEntity
    {
        public Guid ToDoItemId { get; set; }

        public ToDoItems? ToDoItem { get; set; }

        public TaskState OldState { get; set; }

        public TaskState NewState { get; set; }

        public DateTime ChangedAt { get; set; }

        public Guid ChangeByUserId { get; set; }

    }
}
