using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ToDo.Application.Abstractions;
using ToDo.Application.DTOs.Comment;
using ToDo.Application.Exceptions;
using ToDo.Application.Extensions;
using ToDo.Domain.Entities.Comments;

namespace ToDo.Application.Services.Comments
{
    public class CommentService : ICommentService
    {
        private readonly IToDoRepository _toDoRepository;
        private readonly IGenericRepository<Comment> _commentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CommentService(
            IHttpContextAccessor httpContextAccessor
            , IUnitOfWork unitOfWork
            , IMapper mapper
            , IGenericRepository<Comment> commentRepository
            , IToDoRepository toDoRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _commentRepository = commentRepository;
            _toDoRepository = toDoRepository;
        }


        public async Task<IEnumerable<CommentDto>> GetAllCommentsForTask(Guid taskId)
        {
            Guid userId = _httpContextAccessor.HttpContext.User.GetUserId();
            bool isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

            var query = _commentRepository.GetQueryable(true)
            .Where(x => x.ToDoItemsId == taskId);

            if (!isAdmin)
                query = query.Where(x => x.ToDoItems.AppUserId == userId);

            var commentsEntity = await query.ToListAsync();

            if (commentsEntity == null || !commentsEntity.Any())
                throw new NotFoundException($"Task with id {taskId} not found or you have no access to its comments");

            var comments = _mapper.Map<IEnumerable<CommentDto>>(commentsEntity);

            return comments;
        }

        public async Task<IEnumerable<CommentDto>> GetAllMyComments()
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();

            var entity = await _commentRepository.GetQueryable()
                .Where(x => x.AppUserId == currentUserId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CommentDto>>(entity);
        }


        public async Task<CommentDto> GetCommentForId(Guid id)
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            bool isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

            var comment = await _commentRepository.GetQueryable(true)
                .Include(x => x.AppUser)
                .Include(x => x.ToDoItems)
                .FirstOrDefaultAsync(x => x.id == id);

            if (comment == null)
                throw new NotFoundException($"Comment with id {id} not found");

            //kişiye ait olup olmadığını kontrol yeri
            if (!isAdmin)
            {
                bool isCommentOwner = comment.AppUserId == currentUserId;
                bool isTaskerOwner = comment.ToDoItems.AppUserId == currentUserId;

                if (!isCommentOwner && !isTaskerOwner)
                    throw new UnAuthorizedAccessException($"You are not authorized to access comment with id {id}");
            }

            return _mapper.Map<CommentDto>(comment);
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsForUser(Guid UserId)
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            bool isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

            if (!isAdmin && currentUserId != UserId)
                throw new UnAuthorizedAccessException($"You are not authorized to access comments for user with id {UserId}");

            var commentsEntity = await _commentRepository.GetQueryable(true)
                .Include(x => x.AppUser)
                .Include(x => x.ToDoItems)
                .Where(x =>x.AppUserId == UserId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CommentDto>>(commentsEntity);
        }

        public async Task AddComment(CommentSaveDto dto)
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            bool isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

            if (!isAdmin)
            {
                var isTaskOwner = await _toDoRepository.GetQueryable(true)
                .AnyAsync(x => x.id == dto.ToDoItemsId && x.AppUserId == currentUserId);
                if (!isTaskOwner)
                    throw new UnAuthorizedAccessException("You do not have permission to comment on this task");
            }
            else
            {
                var taskExists = await _toDoRepository.GetQueryable(true)
                .AnyAsync(x => x.id == dto.ToDoItemsId);
                if (!taskExists)
                    throw new NotFoundException("There is no such task in the database");
            }

            var entity = _mapper.Map<Comment>(dto);
            entity.AppUserId = currentUserId; //dto'da olmadığı için elle yapıyoruz.
            entity.dateTime = DateTime.Now;

            await _commentRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateComment(Guid id, CommentUpdateDto dto) //gelen id -> comment id'si
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            bool isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin");
        
            var comment = await _commentRepository.GetQueryable()
            .FirstOrDefaultAsync(x => x.id == id);

            if (comment == null)
                throw new NotFoundException($"Comment with id {id} not found");

            if (!isAdmin && comment.AppUserId != currentUserId)
                throw new UnAuthorizedAccessException($"You are not authorized to access comment with id {id}");
            
            _mapper.Map(dto, comment);
            comment.dateTime = DateTime.Now;

            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteComment(Guid id)
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            bool isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

            var comment = await _commentRepository.GetQueryable()
            .Include(x => x.ToDoItems)
            .FirstOrDefaultAsync(x => x.id == id);

            if (comment == null)
                throw new NotFoundException($"Comment with id {id} not found");

            if (!isAdmin)
            {
                bool isCommentOwner = comment.AppUserId == currentUserId;
                bool isTaskOwner = comment.ToDoItems.AppUserId == currentUserId;

                if (!isCommentOwner && !isTaskOwner)
                    throw new UnAuthorizedAccessException($"You are not authorized to delete comment with id {id}");
            }

            comment.isDeleted = true;

            await _unitOfWork.CommitAsync();
        }
    }
}