using ToDo.Application.DTOs.Category;
using ToDo.Application.DTOs.User;

namespace ToDo.Application.DTOs.ToDo
{
    public class ToDoItemsDto
    {
        public int id { get; set; }

        public required string content {  get; set; }

        public int priority { get; set; }

        public bool isCompleted { get; set; }   

        public DateTime? completedDate {  get; set; }

        public CategoryDto? Category { get; set; }

        public AppUserDto? AppUser {  get; set; }
    }
}
