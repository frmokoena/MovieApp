using System.Data;

namespace Movies.Data;

/// <summary>
/// Create connection
/// Note:
///     This is just a wrapping provider, consumer is responsible for the connection lifetime
/// </summary>
public interface IDbConnectionProvider
{
    IDbConnection Connect();
    Task<IDbConnection> ConnectAsync(CancellationToken cancellationToken = default);
}
