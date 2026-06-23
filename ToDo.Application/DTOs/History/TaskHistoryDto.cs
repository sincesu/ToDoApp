namespace ToDo.Application.DTOs.History
{
    public class TaskHistoryDto
    {
        public Guid Id { get; set; }
        public Guid ToDoItemId { get; set; }
        public string OldState { get; set; } = string.Empty;
        public string NewState { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
        public Guid ChangeByUserId { get; set; }
    }
}
