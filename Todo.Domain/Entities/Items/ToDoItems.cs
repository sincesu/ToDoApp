using ToDo.Domain.Entities.Categories;
using ToDo.Domain.Entities.Users;

namespace ToDo.Domain.Entities.Items
{
    public class ToDoItems
    {
        public int id {  get; set; }

        public string content { get; set; } = string.Empty;

        public int priority { get; set; }

        public bool isDeleted { get; set; }

        public bool isCompleted { get; set; }
        
        public DateTime createdDate { get; set; }

        public DateTime? completedDate { get; set; }

        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        public int AppUserId { get; set; }

        public AppUser? AppUser { get; set; }
    }
}
