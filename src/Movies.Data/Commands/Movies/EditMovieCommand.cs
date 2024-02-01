using Dapper;
using Movies.Data.Entities;
using System.Data;
using System.Net;

namespace Movies.Data.Commands.Movies;

public class EditMovieCommand(int id, Movie entity) : ICommand<IResponse<Movie>>
{
    private const string Sql = @"
    UPDATE Movies SET MovieName = @movieName, ReleaseYear = @releaseYear, Genre = @genre
    OUTPUT INSERTED.*
    WHERE MovieID = @id AND Version = @version";

    private readonly int _id = id;
    private readonly Movie _entity = entity;

    public bool RequiresTransaction => false;

    public async Task<IResponse<Movie>> ExecuteAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken = default)
    {
        if (_id != _entity?.MovieID)
        {
            return new Response<Movie>(
                raw: null,
                HttpStatusCode.NotFound,
                reason: "Entity not found");
        }

        try
        {
            var updated = await connection.QuerySingleAsync<Movie>(Sql, new
            {
                id = _entity.MovieID,
                version = _entity.Version,
                movieName = _entity.MovieName,
                releaseYear = _entity.ReleaseYear,
                genre = _entity.Genre
            }, transaction); ;

            return new Response<Movie>(
                updated,
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
                    reason: "Entity with the same genre name exists");
            }

            return new Response<Movie>(
                raw: null,
                HttpStatusCode.InternalServerError,
                reason: "Failed to save entity");
        }
    }
}
