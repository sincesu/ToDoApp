using ToDo.Application.DTOs.ToDo;
using ToDo.Application.DTOs.Filter;

namespace ToDo.Application.Abstractions
{
    public interface IToDoService
    {
        Task<IEnumerable<ToDoItemsDto>> GetItemsAsync(ToDoFilterDto? filter);

        Task <ToDoItemsDto?> GetByIdAsync(int id);

        Task AddAsync(ToDoItemsSaveDto item);

        Task ToMarkAsync(int id);

        Task DeleteAsync(int id);

        Task UpdateAsync(int id, ToDoUpdateDto dto);

        Task <IEnumerable<ToDoItemsDto>> GetCompletedItemsAsync(ToDoFilterDto? filter);
    }
}
