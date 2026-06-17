using ToDo.Application.DTOs.ToDo;
using ToDo.Application.DTOs.User;

namespace ToDo.Application.DTOs.Comment
{
    public class CommentSaveDto
    {
        public required string Content { get; set; }
        public Guid ToDoItemsId { get; set; }
    }
}
