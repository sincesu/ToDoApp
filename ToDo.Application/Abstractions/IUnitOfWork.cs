using ToDo.Domain.Entities.Users;
using ToDo.Domain.Entities.Categories;

namespace ToDo.Application.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        IToDoRepository ToDoRepository { get; }
        
        IGenericRepository<Category> Category {  get; }

        IGenericRepository<AppUser> AppUser { get; }
         
        Task<int> CommitAsync();
    }
}
