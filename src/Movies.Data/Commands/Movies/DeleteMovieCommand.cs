using Dapper;
using Movies.Data.Entities;
using System.Data;
using System.Net;

namespace Movies.Data.Commands.Movies;

public class DeleteMovieCommand(int id) : ICommand<IResponse<Movie>>
{
    private const string Sql = @"
			DELETE
				Movies
            OUTPUT DELETED.*
			WHERE
				MovieID = @id";

    private readonly int _id = id;

    public bool RequiresTransaction => false;

    public async Task<IResponse<Movie>> ExecuteAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var deleted = await connection.QuerySingleAsync<Movie>(Sql, new
            {
                id = _id
            }, transaction);

            return new Response<Movie>(
                deleted,
                raw: string.Empty,
                HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            if (ex.Message?.Contains("duplicate", StringComparison.InvariantCultureIgnoreCase) ?? false)
            {
                return new Response<Movie>(
                    raw: null,
                    HttpStatusCode.Conflict,
                    reason: "Entity exists");
            }

            if (ex.Message?.Contains("REFERENCE constraint", StringComparison.InvariantCultureIgnoreCase) ?? false)
            {
                // REFERENCE constraint
                return new Response<Movie>(
                    raw: null,
                    HttpStatusCode.Conflict,
                    reason: "Cant not delete. Entity is has been used");
            }

            return new Response<Movie>(
                raw: null,
                HttpStatusCode.InternalServerError,
                reason: "Failed to save entity");
        }
    }
}
