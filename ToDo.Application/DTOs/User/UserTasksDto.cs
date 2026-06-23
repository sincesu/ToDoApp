using ToDo.Application.DTOs.ToDo;

namespace ToDo.Application.DTOs.User
{
    public class UserTasksDto
    {
        public ICollection<ToDoItemsDto> Items { get; set; } = new HashSet<ToDoItemsDto>();
    }
}
