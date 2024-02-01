using Dapper;
using Movies.Data.Entities;
using System.Data;
using System.Net;

namespace Movies.Data.Commands.Movies;

public class AddMovieCommand(Movie entity) : ICommand<IResponse<Movie>>
{
    private const string Sql = @"
			INSERT INTO Movies(MovieName,ReleaseYear,Genre)
            OUTPUT INSERTED.*
			VALUES(@movieName,@releaseYear,@genre)";

    private readonly Movie _entity = entity;

    public bool RequiresTransaction => false;

    public async Task<IResponse<Movie>> ExecuteAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var inserted = await connection.QuerySingleAsync<Movie>(Sql, new
            {
                movieName = _entity.MovieName,
                releaseYear = _entity.ReleaseYear,
                genre = _entity.Genre
            }, transaction);

            return new Response<Movie>(
                inserted,
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

            return new Response<Movie>(
                raw: null,
                HttpStatusCode.InternalServerError,
                reason: "Failed to save entity");
        }
    }
}
