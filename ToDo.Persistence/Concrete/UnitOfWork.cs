
using ToDo.Application.Abstractions;
using ToDo.Domain.Entities.Categories;
using ToDo.Domain.Entities.Comments;
using ToDo.Domain.Entities.Users;

namespace ToDo.Persistence.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        //tek noktadan savechanges çağırma
        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        //iş bitince db bağlantısını kapama
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
