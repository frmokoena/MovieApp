using Dapper;
using Movies.Data.Entities;
using System.Data;
using System.Net;

namespace Movies.Data.Commands.Genres;

public class AddGenreCommand(Genre entity) : ICommand<IResponse<Genre>>
{
    private const string Sql = @"
			INSERT INTO Genres(GenreName)
            OUTPUT INSERTED.*
			VALUES(@genreName)";

    private readonly Genre _entity = entity;

    public bool RequiresTransaction => false;

    public async Task<IResponse<Genre>> ExecuteAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var inserted = await connection.QuerySingleAsync<Genre>(Sql, new
            {
                genreName = _entity.GenreName
            }, transaction);

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
                    reason: "Entity exists");
            }

            return new Response<Genre>(
                raw: null,
                HttpStatusCode.InternalServerError,
                reason: "Failed to save entity");
        }
    }
}