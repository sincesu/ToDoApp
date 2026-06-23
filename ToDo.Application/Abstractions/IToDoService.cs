using ToDo.Application.DTOs.Filter;
using ToDo.Application.DTOs.History;
using ToDo.Application.DTOs.ToDo;

namespace ToDo.Application.Abstractions
{
    public interface IToDoService
    {
        Task<IEnumerable<ToDoItemsDto>> GetItemsAsync(ToDoFilterDto? filter);

        Task <ToDoItemsDto?> GetByIdAsync(Guid id);

        Task<IEnumerable<TaskHistoryDto>> GetTaskHistoriesAsync(Guid id);

        Task AssignTask(AssignTaskDto dto);

        Task AddAsync(ToDoItemsSaveDto item);

        Task UpdateState(Guid id, ChangeTaskStateDto dto);

        Task ToMarkForCompletedAsync(Guid id);

        Task UpdateAsync(Guid id, ToDoUpdateDto dto);
        
        Task DeleteAllCommentsOfTaskAsync(Guid id, bool savechanges = true);

        Task <IEnumerable<ToDoItemsDto>> GetCompletedItemsAsync(ToDoFilterDto? filter);
    }
}
