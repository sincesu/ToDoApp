using ToDo.Application.DTOs.ToDo;

namespace ToDo.Application.DTOs.User
{
    public class UserTasksDto
    {
        public IEnumerable<ToDoItemsDto> Items { get; set; } = new HashSet<ToDoItemsDto>();
    }
}
