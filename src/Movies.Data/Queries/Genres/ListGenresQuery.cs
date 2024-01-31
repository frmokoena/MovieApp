using Dapper;
using Movies.Data.Entities;
using System.Data;
using System.Net;

namespace Movies.Data.Queries.Genres;

public class ListGenresQuery : IQuery<IResponse<IEnumerable<Genre>>>
{
    private const string Sql = @"
			SELECT
				GenreID,
				GenreName
			FROM
				Genres
            ORDER BY GenreName ASC";

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
