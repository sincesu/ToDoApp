
using ToDo.Domain.Enums;

namespace ToDo.Application.DTOs.Filter
{
    public class ToDoFilterDto
    {
        public Guid? CategoryId { get; set; }
        public int? priority { get; set; }
        public string? searchText { get; set; }
        public Guid? userId { get; set; }
        public TaskState? taskState { get; set; }
    }
}
