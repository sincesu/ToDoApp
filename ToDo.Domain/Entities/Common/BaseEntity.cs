
namespace ToDo.Domain.Entities.Common
{
    public abstract class BaseEntity
    {
        public Guid id { get; set; } = Guid.NewGuid();

        public bool isDeleted { get; set; } = false;
    }
}
