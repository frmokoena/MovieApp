using System.Data;

namespace Movies.Data;

public interface IQuery<T>
{
    Task<T> ExecuteAsync(
        IDbConnection connection, 
        IDbTransaction? transaction, 
        CancellationToken cancellationToken = default);
}
