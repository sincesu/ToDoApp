using ToDo.Domain.Entities.Users;
using ToDo.Domain.Entities.Categories;
using ToDo.Domain.Entities.Comments;

namespace ToDo.Application.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        IToDoRepository ToDoRepository { get; }
        
        IGenericRepository<Category> Category {  get; }

        IGenericRepository<AppUser> AppUser { get; }

        IGenericRepository<Comment> Comment { get; }
         
        Task<int> CommitAsync();
    }
}
