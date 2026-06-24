using ToDo.Domain.Entities.Comments;
using ToDo.Domain.Entities.Common;
using ToDo.Domain.Entities.Items;

namespace Todo.Domain.Entities
{
    public class FileAttachment : BaseEntity
    {
        public string originalFileName { get; set; } = string.Empty;
        public string newFileName { get; set; } = string.Empty;
        public string filePath { get; set; } = string.Empty;
        public string contentType {  get; set; } = string.Empty; //png,pdf vb.
        public long fileSize { get; set; }
        public DateTime UploadedAt {  get; set; }

        public Guid? ToDoItemId {  get; set; }
        public ToDoItems? ToDoItem {  get; set; }

        public Guid? CommentId { get; set; }
        public Comment? Comment { get; set; } 
    }
}
