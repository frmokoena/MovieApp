using Dapper;
using Movies.Data.Entities;
using System.Data;
using System.Net;

namespace Movies.Data.Commands.Characters;

public class DeleteCharacterCommand(Character entity) : ICommand<IResponse<Character>>
{
    private const string Sql = @"
			DELETE
				Characters
            OUTPUT DELETED.*
			WHERE
				MovieID = @movieID AND ActorID = @actorID";

    private readonly Character _entity = entity;

    public bool RequiresTransaction => false;

    public async Task<IResponse<Character>> ExecuteAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var deleted = await connection.QuerySingleAsync<Character>(Sql, new
            {
                movieID = _entity.MovieID,
                actorID = _entity.ActorID
            }, transaction);

            return new Response<Character>(
                deleted,
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
                    reason: "Entity exists");
            }

            if (ex.Message?.Contains("REFERENCE constraint", StringComparison.InvariantCultureIgnoreCase) ?? false)
            {
                // REFERENCE constraint
                return new Response<Character>(
                    raw: null,
                    HttpStatusCode.Conflict,
                    reason: "Cant not delete. Entity is has been used");
            }

            return new Response<Character>(
                raw: null,
                HttpStatusCode.InternalServerError,
                reason: "Failed to save entity");
        }
    }
}