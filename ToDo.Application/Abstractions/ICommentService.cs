using ToDo.Application.DTOs.Comment;

namespace ToDo.Application.Abstractions
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentDto>> GetAllCommentsForTask(Guid taskId);

        Task<IEnumerable<CommentDto>> GetAllMyComments();

        Task<IEnumerable<CommentDto>> GetCommentsForUser(Guid UserId);

        Task <CommentDto> GetCommentForId(Guid id);

        Task AddComment(CommentSaveDto dto);

        Task UpdateComment(Guid id, CommentUpdateDto dto);

        Task DeleteComment(Guid id);
    }
}
