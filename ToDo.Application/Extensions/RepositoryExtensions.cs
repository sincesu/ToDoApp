using ToDo.Application.Exceptions;
using ToDo.Application.Abstractions;

namespace ToDo.Application.Extensions
{
    public static class RepositoryExtensions
    {
        public static async Task<T> GetOrThrowAsync<T>
            (this IGenericRepository<T> repository,
            int id) where T : class
        {
            var entity = await repository.GetByIdAsync(id)
                ?? throw new NotFoundException
                ($"{typeof(T).Name} with {id} " +
                $"has not found list");

            return entity;
        }
    }
}
