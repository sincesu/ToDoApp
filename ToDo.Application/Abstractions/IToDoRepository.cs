using ToDo.Domain.Entities.Items;
using ToDo.Application.DTOs.Filter;

namespace ToDo.Application.Abstractions
{
    public interface IToDoRepository : IGenericRepository<ToDoItems>
    {
        IQueryable<ToDoItems> GetToDosWithCategory(ToDoFilterDto? filter);
    }
}
