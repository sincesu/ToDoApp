namespace ToDo.Application.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> CommitAsync();
    }
}
