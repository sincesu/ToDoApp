using Microsoft.AspNetCore.Http;

namespace ToDo.Application.DTOs.Attachment
{
    public class AttachmentSaveDto
    {
        public required IFormFile File { get; set; }

        public Guid? ToDoItemId { get; set; }

        public Guid? CommentId { get; set; }
    }
}
