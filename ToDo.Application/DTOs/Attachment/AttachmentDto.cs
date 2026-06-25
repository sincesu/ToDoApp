
namespace ToDo.Application.DTOs.Attachment
{
    public class AttachmentDto
    {
        public Guid id { get; set; }
        
        public required string OriginalFileName { get; set; }
        
        public required string FilePath { get; set; } // Tıklayıp indirmesi/görmesi için
    }
}