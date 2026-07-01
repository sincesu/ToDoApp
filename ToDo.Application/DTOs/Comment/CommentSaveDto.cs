using Microsoft.AspNetCore.Http;

namespace ToDo.Application.DTOs.Comment
{
    public class CommentSaveDto
    {
        public required string Content { get; set; }
        public Guid ToDoItemsId { get; set; }
        public IFormFile? File { get; set; }
    }
}
