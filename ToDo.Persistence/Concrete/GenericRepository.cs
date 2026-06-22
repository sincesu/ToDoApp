using Microsoft.EntityFrameworkCore;
using ToDo.Application.Abstractions;

namespace ToDo.Persistence.Concrete
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbset;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbset = _context.Set<T>();
        }

        //uptdate ve deletelerde entity'i efcore takip edebilsin diye asqueryable dönüyoz.
        //get işlemlerinde sadece okuma yapıyoz diye asnoTracking yapıyoz.
        public IQueryable<T> GetQueryable(bool asNoTracking=false)
        {
            var query = _dbset.AsQueryable();

            if (asNoTracking)
                query=query.AsNoTracking();

            return query; 
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbset.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbset.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbset.AddAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            _dbset.Update(entity);
        }

        public async Task DeleteAsync(T entity)
        {
            _dbset.Remove(entity);
        }
    }
}
