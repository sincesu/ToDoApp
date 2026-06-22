using ToDo.Application.DTOs.ToDo;
using ToDo.Application.DTOs.Filter;

namespace ToDo.Application.Abstractions
{
    public interface IToDoService
    {
        Task<IEnumerable<ToDoItemsDto>> GetItemsAsync(ToDoFilterDto? filter);

        Task <ToDoItemsDto?> GetByIdAsync(Guid id);

        Task AssignTask(AssignTaskDto dto);

        Task AddAsync(ToDoItemsSaveDto item);

        Task UpdateState(Guid id, ChangeTaskStateDto dto);

        Task ToMarkAsync(Guid id);

        Task UpdateAsync(Guid id, ToDoUpdateDto dto);
        
        Task DeleteAllCommentsOfTaskAsync(Guid id);

        Task <IEnumerable<ToDoItemsDto>> GetCompletedItemsAsync(ToDoFilterDto? filter);
    }
}
