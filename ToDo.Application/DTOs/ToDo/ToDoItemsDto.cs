using ToDo.Application.DTOs.Attachment;
using ToDo.Application.DTOs.Category;
using ToDo.Application.DTOs.User;
using ToDo.Domain.Enums;

namespace ToDo.Application.DTOs.ToDo
{
    public class ToDoItemsDto
    {
        public Guid id { get; set; }

        public required string content {  get; set; }

        public int priority { get; set; }

        public TaskState State { get; set; }

        public bool isCompleted { get; set; }   

        public DateTime? completedDate {  get; set; }

        public CategoryDto? Category { get; set; }

        public AppUserDto? AppUser {  get; set; }

        public IEnumerable<AttachmentDto> Attachments { get; set; } = new HashSet<AttachmentDto>();
    }
}
