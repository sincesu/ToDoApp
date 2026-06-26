
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Todo.Domain.Entities;
using ToDo.Application.Abstractions;
using ToDo.Application.DTOs.Attachment;
using ToDo.Application.Exceptions;
using ToDo.Application.Extensions;
using ToDo.Domain.Entities.Comments;
using ToDo.Domain.Entities.Items;

namespace ToDo.Application.Services.Attachments
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<FileAttachment> _attachmentRepository;
        private readonly IGenericRepository<ToDoItems> _toDoRepository;
        private readonly IGenericRepository<Comment> _commentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStorageService _storageService;

        public AttachmentService(IMapper mapper
            , IUnitOfWork unitOfWork
            , IGenericRepository<FileAttachment> attachmentRepository
            , IGenericRepository<ToDoItems> toDoRepository
            , IGenericRepository<Comment> commentRepository
            , IHttpContextAccessor httpContextAccessor
            , IStorageService storageService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _attachmentRepository = attachmentRepository;
            _toDoRepository = toDoRepository;
            _commentRepository = commentRepository;
            _httpContextAccessor = httpContextAccessor;
            _storageService = storageService;
        }

        public async Task<AttachmentDto> UploadAsync(AttachmentSaveDto dto)
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            bool isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

            if (dto.ToDoItemId != null)
            {
                var item = await _toDoRepository.GetQueryable()
                    .FirstOrDefaultAsync(x => x.id == dto.ToDoItemId)
                    ?? throw new NotFoundException("The specified task could not be found.");

                if (!isAdmin)
                    throw new UnAuthorizedAccessException("Only administrators are allowed to add attachments to tasks.");
            }

            if (dto.CommentId != null)
            {
                var comment = await _commentRepository.GetQueryable()
                    .FirstOrDefaultAsync(x => x.id == dto.CommentId)
                    ?? throw new NotFoundException("The specified comment could not be found.");

                if (!isAdmin && comment.AppUserId != currentUserId)
                    throw new UnAuthorizedAccessException("Only the author of the comment is authorized to add attachments.");
            }
            string uploadedFilePath = await _storageService.UploadFileAsync(dto.File);
            
            var attachment = CreateAttachmentEntity(dto, uploadedFilePath);
            
            await _attachmentRepository.AddAsync(attachment);
            await _unitOfWork.CommitAsync();
            
            return _mapper.Map<AttachmentDto>(attachment);

        }

        public async Task<AttachmentDto> GetByIdAsync(Guid id)
        {
            var attachment = await _attachmentRepository.GetQueryable()
                .FirstOrDefaultAsync(x => x.id == id)
                ?? throw new NotFoundException("The requested attachment could not be found.");

            return _mapper.Map<AttachmentDto>(attachment);
        }

        public async Task DeleteAsync(Guid id)
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            bool isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

            var attachment = await _attachmentRepository.GetQueryable()
                .FirstOrDefaultAsync(x => x.id == id)
                ?? throw new NotFoundException("The attachment to be deleted could not be found.");

            if (!isAdmin)
            {
                if (attachment.ToDoItemId != null) //ek taska aitse direk hata
                    throw new UnAuthorizedAccessException("You do not have permission to delete this attachment");

                if (attachment.CommentId != null)
                {
                    var comment = await _commentRepository.GetQueryable()
                        .FirstOrDefaultAsync(x => x.id == attachment.CommentId);

                    if (comment == null || comment.AppUserId != currentUserId)
                        throw new UnAuthorizedAccessException("You do not have permission to delete this attachment.");
                }
            }
            await _storageService.DeleteFileAsync(attachment.filePath);

            attachment.isDeleted = true;

            await _unitOfWork.CommitAsync();
        }

        private FileAttachment CreateAttachmentEntity(AttachmentSaveDto dto, string uploadedFilePath)
        {
            return new FileAttachment
            {
                id = Guid.NewGuid(),
                originalFileName = dto.File.FileName,
                newFileName = Path.GetFileName(uploadedFilePath),
                filePath = uploadedFilePath,
                contentType = dto.File.ContentType,
                fileSize = dto.File.Length,
                UploadedAt = DateTime.Now,
                ToDoItemId = dto.ToDoItemId,
                CommentId = dto.CommentId,
                isDeleted = false
            };
        }
    }
}