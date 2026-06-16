
namespace ToDo.Application.DTOs.ToDo
{
    public class ToDoItemsSaveDto
    {
        public required string content { get; set; }

        public int priority { get; set; }

        public Guid CategoryId { get; set; }
    }
}
