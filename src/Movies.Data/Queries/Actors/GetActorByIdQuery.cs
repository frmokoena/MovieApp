using Dapper;
using Movies.Data.Entities;
using System.Data;
using System.Net;

namespace Movies.Data.Queries.Actors;

public class GetActorByIdQuery(int id) : IQuery<IResponse<Actor?>>
{
    private const string Sql = @"
			SELECT *
			FROM
				Actors
            WHERE ActorID = @id";

    private readonly int _id = id;

    public async Task<IResponse<Actor?>> ExecuteAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken = default)
    {
        var entity = await connection.QueryFirstOrDefaultAsync<Actor>(
            Sql,
            transaction: transaction,
            param: new { id = _id });

        if (entity == null)
        {
            return new Response<Actor?>(
                raw: null,
                HttpStatusCode.NotFound,
                reason: "Entity not found");
        }

        var result = new Response<Actor?>(
            entity,
            raw: string.Empty,
            HttpStatusCode.OK);

        return result;
    }
}