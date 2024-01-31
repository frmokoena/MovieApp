using Microsoft.Data.SqlClient;
using System.Data;

namespace Movies.Data;

internal class DbConnectionProvider : IDbConnectionProvider
{
    private readonly string _connectionString;

    /// <summary>
    /// Initialize a new instance of <see cref="DbConnectionProvider"/> class
    /// </summary>
    /// <param name="connectionString"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public DbConnectionProvider(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString), "ConnectingString can not be empty");
        }
        _connectionString = connectionString;
    }

    public IDbConnection Connect()
    {
        var connection = new SqlConnection(_connectionString);
        connection.Open();
        return connection;
    }

    public async Task<IDbConnection> ConnectAsync(CancellationToken cancellationToken = default)
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
