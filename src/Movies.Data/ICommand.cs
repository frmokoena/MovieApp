using System.Data;

namespace Movies.Data;

public interface ICommand
{
    /// <summary>
    /// To enforce explicit transaction
    /// </summary>
    bool RequiresTransaction { get; }

    Task ExecuteAsync(
        IDbConnection connection, 
        IDbTransaction? transaction, 
        CancellationToken cancellationToken = default);
}

public interface ICommand<T>
{
    /// <summary>
    /// To enforce explicit transaction
    /// </summary>
    bool RequiresTransaction { get; }

    Task<T> ExecuteAsync(
        IDbConnection connection, 
        IDbTransaction? transaction, 
        CancellationToken cancellationToken = default);
}