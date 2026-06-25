using ToDo.Application.DTOs.Attachment;

namespace ToDo.Application.Abstractions
{
    public interface IAttachmentService
    {
        Task<AttachmentDto> UploadAsync(AttachmentSaveDto dto);

        Task<AttachmentDto> GetByIdAsync(Guid id);

        Task DeleteAsync(Guid id);
    }
}
