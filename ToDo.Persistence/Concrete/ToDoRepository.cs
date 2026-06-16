using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Entities.Items;
using ToDo.Application.Abstractions;
using ToDo.Application.DTOs.Filter;

namespace ToDo.Persistence.Concrete
{
    public class ToDoRepository : GenericRepository<ToDoItems>, IToDoRepository
    {
        private readonly AppDbContext _context;

        public ToDoRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<ToDoItems> GetToDosWithCategory(ToDoFilterDto? filter)
        {
            var query = _context.ToDoItems
                .Include(x => x.Category)
                .AsQueryable();

            if (filter != null)
            {
                if (filter.CategoryId.HasValue)
                    query = query.Where(x => x.CategoryId == filter.CategoryId.Value);
                if (filter.priority.HasValue)
                    query = query.Where(x => x.priority == filter.priority.Value);
                if (!string.IsNullOrWhiteSpace(filter.searchText))
                    query = query.Where(x => x.content.Contains(filter.searchText));
                if (filter.userId.HasValue)
                    query = query.Where(x => x.AppUserId == filter.userId.Value);
                        //LIKE sorgusunu tetikliyor
            }

            return query;
        }
    }
}
