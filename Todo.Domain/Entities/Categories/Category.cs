
using ToDo.Domain.Entities.Common;

namespace ToDo.Domain.Entities.Categories
{
    public class Category : BaseEntity
    {
        public string name { get; set; } = string.Empty;
    }
}
