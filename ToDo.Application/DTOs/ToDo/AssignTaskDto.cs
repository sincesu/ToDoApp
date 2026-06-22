using ToDo.Domain.Entities.Items;

namespace ToDo.Application.DTOs.ToDo
{
    public class AssignTaskDto
    {
        public Guid ToDoItemsId { get; set; }

        public Guid AssignedToUserId { get; set; }

        public ToDoItemsSaveDto? ToDoItems { get; set; }
    }
}
