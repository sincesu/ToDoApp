namespace ToDo.Application.DTOs.ToDo
{
    public class ToDoUpdateDto
    {
        public string? content { get; set; } = string.Empty;

        public int? priority { get; set; }

        public Guid? CategoryId { get; set; }
    }
}