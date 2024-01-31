using System.Data;

namespace Movies.Data;

public class UnitOfWork : IUnitOfWork
{
    private bool _disposed;
    private IDbConnection? _connection;
    private IDbTransaction? _transaction;
    private readonly CancellationToken _cancellationToken;

    /// <summary>
    /// Initialize a new instance of <see cref="UnitOfWork"/> class
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="transactional"></param>
    /// <param name="isolationLevel"></param>
    /// <param name="cancellationToken"></param>
    public UnitOfWork(
        IDbConnection connection,
        bool transactional = false,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default)
    {
        _connection = connection;
        _cancellationToken = cancellationToken;

        if (transactional)
        {
            // We begin our transaction
            // Don't forget to explicitly commit the transaction when done
            _transaction = connection.BeginTransaction(isolationLevel);
        }
    }

    public Task ExecuteAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        if (command.RequiresTransaction && _transaction == null)
        {
            throw new InvalidOperationException($"The command {command.GetType()} requires a transaction");
        }

        return command.ExecuteAsync(_connection!, _transaction, cancellationToken);
    }

    public Task<T> ExecuteAsync<T>(ICommand<T> command, CancellationToken cancellationToken = default)
    {
        if (command.RequiresTransaction && _transaction == null)
        {
            throw new InvalidOperationException($"The command {command.GetType()} requires a transaction");
        }

        return command.ExecuteAsync(_connection!, _transaction, cancellationToken);
    }

    public Task<T> QueryAsync<T>(IQuery<T> query)
    {
        return query.ExecuteAsync(_connection!, _transaction, _cancellationToken);
    }

    public void Commit() => _transaction?.Commit();

    public void Rollback() => _transaction?.Rollback();

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~UnitOfWork() => Dispose(false);

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }

        _transaction = null;
        _connection = null;

        _disposed = true;
    }
}
