using ToDo.Domain.Entities.Users;
using ToDo.Domain.Entities.Categories;
using ToDo.Domain.Entities.Comments;

namespace ToDo.Application.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> CommitAsync();
    }
}
