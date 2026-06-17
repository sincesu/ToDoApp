
namespace ToDo.Application.DTOs.Comment
{
    public class CommentDto
    {
        public Guid id { get; set; }
        public required string Content { get; set; }
        public required string userName { get; set; }
        public DateTime dateTime { get; set; }
    }
}
