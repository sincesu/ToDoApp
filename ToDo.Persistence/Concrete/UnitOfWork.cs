
using ToDo.Application.Abstractions;
using ToDo.Domain.Entities.Categories;
using ToDo.Domain.Entities.Comments;
using ToDo.Domain.Entities.Users;

namespace ToDo.Persistence.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        // (Repository) burada private olarak saklıyoruz
        private ToDoRepository? _toDoRepository;
        private GenericRepository<Category>? _category;
        private GenericRepository<AppUser>? _user;
        private GenericRepository<Comment>? _comment;
        
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        // Eğer dışarıdan ToDoRepository'ye erişilmek istenirse, ilk kez çağrıldığında ayağa kaldırıyoruz (Lazy Loading)
        public IToDoRepository ToDoRepository
        {
            get { return _toDoRepository ??= new ToDoRepository(_context); }
        }

        public IGenericRepository<Category> Category
        {
            get { return _category ??= new GenericRepository<Category>(_context);  }
        }

        public IGenericRepository<AppUser> AppUser
        {
            get { return _user ??= new GenericRepository<AppUser>(_context); }
        }

        public IGenericRepository<Comment> Comment
        {
            get { return _comment ??= new GenericRepository<Comment>(_context); }
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
