
namespace ToDo.Application.DTOs.Filter
{
    public class ToDoFilterDto
    {
        public int? CategoryId { get; set; }
        public int? priority { get; set; }
        public string? searchText { get; set; }
        public int? userId { get; set; }
    }
}
