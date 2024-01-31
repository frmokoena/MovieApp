using Dapper;
using Movies.Data.Entities;
using System.Data;
using System.Net;

namespace Movies.Data.Queries.Genres;

public class GetGenreByIdQuery(int id) : IQuery<IResponse<Genre?>>
{
    private const string Sql = @"
			SELECT *
			FROM
				Genres
            WHERE GenreID = @id";

    private readonly int _id = id;

    public async Task<IResponse<Genre?>> ExecuteAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken = default)
    {
        var entity = await connection.QueryFirstOrDefaultAsync<Genre>(
            Sql,
            transaction: transaction,
            param: new { id = _id });

        if (entity == null)
        {
            return new Response<Genre?>(
                raw: null,
                HttpStatusCode.NotFound,
                reason: "Entity not found");
        }

        var result = new Response<Genre?>(
            entity,
            raw: string.Empty,
            HttpStatusCode.OK);

        return result;
    }
}