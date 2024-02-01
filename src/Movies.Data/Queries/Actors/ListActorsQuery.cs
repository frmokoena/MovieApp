using Dapper;
using Movies.Data.Entities;
using System.Data;
using System.Net;

namespace Movies.Data.Queries.Actors;

public class ListActorsQuery : IQuery<IResponse<IEnumerable<Actor>>>
{
    private const string Sql = @"
			SELECT
				ActorID,
				ActorName,
                ActorDOB
			FROM
				Actors
            ORDER BY ActorName ASC";

    public async Task<IResponse<IEnumerable<Actor>>> ExecuteAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken = default)
    {
        var list = await connection.QueryAsync<Actor>(Sql, transaction: transaction);

        var result = new Response<IEnumerable<Actor>>(
            list,
            raw: string.Empty,
            HttpStatusCode.OK);

        return result;
    }
}