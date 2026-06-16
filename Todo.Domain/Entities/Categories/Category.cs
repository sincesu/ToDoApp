
namespace ToDo.Domain.Entities.Categories
{
    public class Category
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public bool isDeleted { get; set; }
    }
}
