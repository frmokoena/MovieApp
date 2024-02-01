using Dapper;
using Movies.Data.Entities;
using System.Data;
using System.Net;

namespace Movies.Data.Commands.Movies;

public class AddMovieCharacterCommand(int movieId, Character entity) : ICommand<IResponse<Character>>
{
    private const string Sql = @"
			INSERT INTO Characters(MovieID,ActorId,CharacterName)
            OUTPUT INSERTED.*
			VALUES(@movieId,@actorId,@characterName)";

    private readonly int _id = movieId;
    private readonly Character _entity = entity;

    public bool RequiresTransaction => false;

    public async Task<IResponse<Character>> ExecuteAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken = default)
    {
        if (_id != _entity?.MovieID)
        {
            return new Response<Character>(
                raw: null,
                HttpStatusCode.NotFound,
                reason: "Entity not found");
        }

        try
        {
            var updated = await connection.QuerySingleAsync<Character>(Sql, new
            {
                movieId = _entity.MovieID,
                actorId = _entity.ActorID,
                characterName = _entity.CharacterName
            }, transaction); ;

            return new Response<Character>(
                updated,
                raw: string.Empty,
                HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            if (ex.Message?.Contains("duplicate", StringComparison.InvariantCultureIgnoreCase) ?? false)
            {
                return new Response<Character>(
                    raw: null,
                    HttpStatusCode.Conflict,
                    reason: "Entity with the same characer exists");
            }

            return new Response<Character>(
                raw: null,
                HttpStatusCode.InternalServerError,
                reason: "Failed to save entity");
        }
    }
}
