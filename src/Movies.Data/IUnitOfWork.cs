namespace Movies.Data;

public interface IUnitOfWork : IDisposable
{
    Task<T> QueryAsync<T>(IQuery<T> query);
    Task ExecuteAsync(ICommand command, CancellationToken cancellationToken = default);
    Task<T> ExecuteAsync<T>(ICommand<T> command, CancellationToken cancellation = default);
    void Commit();
    void Rollback();
}
