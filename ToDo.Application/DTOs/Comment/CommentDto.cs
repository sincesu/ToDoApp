
using ToDo.Application.DTOs.Attachment;

namespace ToDo.Application.DTOs.Comment
{
    public class CommentDto
    {
        public Guid id { get; set; }
        public required string Content { get; set; }
        public required string userName { get; set; }
        public DateTime dateTime { get; set; }
        public IEnumerable<AttachmentDto> Attachments { get; set; } = new HashSet<AttachmentDto>();
    }
}
