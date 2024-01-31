using Dapper;
using Movies.Data.Entities;
using System.Data;
using System.Net;
using static Dapper.SqlMapper;

namespace Movies.Data.Commands.Genres;

public class DeleteGenreCommand(int id) : ICommand<IResponse<Genre>>
{
    private const string Sql = @"
			DELETE
				Genres
            OUTPUT DELETED.*
			WHERE
				GenreID = @id";

    private readonly int _id = id;

    public bool RequiresTransaction => false;

    public async Task<IResponse<Genre>> ExecuteAsync(
        IDbConnection connection, 
        IDbTransaction? transaction, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var deleted = await connection.QuerySingleAsync<Genre>(Sql, new
            {
                id = _id
            }, transaction);

            return new Response<Genre>(
                deleted,
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

            if(ex.Message?.Contains("REFERENCE constraint", StringComparison.InvariantCultureIgnoreCase) ?? false)
            {
                // REFERENCE constraint
                return new Response<Genre>(
                    raw: null,
                    HttpStatusCode.Conflict,
                    reason: "Cant not delete. Entity is has been used");
            }

            return new Response<Genre>(
                raw: null,
                HttpStatusCode.InternalServerError,
                reason: "Failed to save entity");
        }
    }
}