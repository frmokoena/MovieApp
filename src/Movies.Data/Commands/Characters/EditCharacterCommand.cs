using Dapper;
using Movies.Data.Entities;
using System.Data;
using System.Net;

namespace Movies.Data.Commands.Characters;

public class EditCharacterCommand(Character entity) : ICommand<IResponse<Character>>
{
    private const string Sql = @"
    UPDATE Characters SET CharacterName = @characterName
    OUTPUT INSERTED.*
    WHERE MovieID = @movieID AND ActorID = @actorID AND Version = @version";

    private readonly Character _entity = entity;

    public bool RequiresTransaction => false;

    public async Task<IResponse<Character>> ExecuteAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var updated = await connection.QuerySingleAsync<Character>(Sql, new
            {
                movieID = _entity.MovieID,
                actorID = _entity.ActorID,
                version = _entity.Version,
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
                    reason: "Entity with the same genre name exists");
            }

            return new Response<Character>(
                raw: null,
                HttpStatusCode.InternalServerError,
                reason: "Failed to save entity");
        }
    }
}
