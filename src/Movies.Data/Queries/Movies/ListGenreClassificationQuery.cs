using Dapper;
using Movies.Data.Entities;
using System.Data;
using System.Net;

namespace Movies.Data.Queries.Movies;

public class ListGenreClassificationQuery : IQuery<IResponse<IEnumerable<Genre>>>
{
    private const string Sql = @"
    SELECT t.GenreID, t.GenreName
    FROM (
        SELECT DISTINCT m.Genre AS GenreID, g.GenreName
        FROM Movies m
        INNER JOIN Genres g ON m.Genre = g.GenreID
    ) t
    ORDER BY t.GenreName ASC";

    public async Task<IResponse<IEnumerable<Genre>>> ExecuteAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken = default)
    {
        var list = await connection.QueryAsync<Genre>(Sql, transaction: transaction);

        var result = new Response<IEnumerable<Genre>>(
            list,
            raw: string.Empty,
            HttpStatusCode.OK);

        return result;
    }
}