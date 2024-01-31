using Dapper;
using Movies.Data.Entities;
using System.Data;
using System.Net;

namespace Movies.Data.Commands.Genres;

public class EditGenreCommand(int id, Genre entity) : ICommand<IResponse<Genre>>
{
    private const string Sql = @"
    UPDATE Genres SET GenreName = @genreName
    OUTPUT INSERTED.*
    WHERE GenreID = @id";

    private readonly int _id = id;
    private readonly Genre _entity = entity;

    public bool RequiresTransaction => false;

    public async Task<IResponse<Genre>> ExecuteAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken = default)
    {
        if(_id != _entity?.GenreID)
        {
            return new Response<Genre>(
                raw: null,
                HttpStatusCode.NotFound,
                reason: "Entity not found");
        }

        try
        {
            var inserted = await connection.QuerySingleAsync<Genre>(Sql, new
            {
                id = _id,
                genreName = _entity.GenreName
            }, transaction); ;

            return new Response<Genre>(
                inserted,
                raw: string.Empty,
                HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            if (ex.Message?.Contains("duplicate", StringComparison.InvariantCultureIgnoreCase) ?? false)
            {
                return new Response<Genre>(
                    raw: null,
                    HttpStatusCode.Conflict,
                    reason: "Entity with the same genre name exists");
            }

            return new Response<Genre>(
                raw: null,
                HttpStatusCode.InternalServerError,
                reason: "Failed to save entity");
        }
    }
}
