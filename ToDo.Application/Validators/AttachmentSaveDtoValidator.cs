
using FluentValidation;
using Microsoft.AspNetCore.Http;
using ToDo.Application.DTOs.Attachment;

namespace ToDo.Application.Validators
{
    public class AttachmentSaveDtoValidator : AbstractValidator<AttachmentSaveDto>
    {
        public AttachmentSaveDtoValidator()
        {
            RuleFor(x => x.File)
                .NotNull().WithMessage("Please select a file to upload.")
                .Must(BeAValidSize).WithMessage("File size cannot exceed 5 MB.")
                .Must(BeAValidFormat).WithMessage("Only .jpg, .png, and .pdf formats are supported.");
            RuleFor(x => x)
                .Must(IsValidType).WithMessage("Attachment must be linked to one target: either ToDoItem or Comment.");
        }

        private bool IsValidType(AttachmentSaveDto dto)
        {
            if (dto.CommentId == null && dto.ToDoItemId == null)
                return false;
            if (dto.CommentId != null && dto.ToDoItemId != null)
                return false;

            return true;
        }

        private bool BeAValidSize(IFormFile file)
        {
            if (file == null)
                return false;

            int maxSizeInBytes = 5 * 1024 * 1024;

            return file.Length <= maxSizeInBytes;
        }

        private bool BeAValidFormat(IFormFile file)
        {
            if (file == null)
                return false;

            var allowedFormats = new[] { "image/jpeg", "image/png", "application/pdf" };

            return allowedFormats.Contains(file.ContentType);
        }
    }
}
