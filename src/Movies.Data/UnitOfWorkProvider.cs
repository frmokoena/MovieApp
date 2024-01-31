using System.Data;

namespace Movies.Data;

/// <summary>
/// Initialize a new instance of <see cref="UnitOfWorkProvider"/> class
/// </summary>
/// <param name="connectionProvider"></param>
public class UnitOfWorkProvider(IDbConnectionProvider connectionProvider) : IUnitOfWorkProvider
{
    private readonly IDbConnectionProvider _connectionProvider = connectionProvider;

    public IUnitOfWork Create(
        bool transactional = false, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        var conn = _connectionProvider.Connect();

        return new UnitOfWork(conn, transactional, isolationLevel);
    }

    public async Task<IUnitOfWork> CreateAsync(
        bool transactional = false,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default)
    {
        var conn = await _connectionProvider.ConnectAsync(cancellationToken);

        return new UnitOfWork(conn, transactional, isolationLevel);
    }
}
