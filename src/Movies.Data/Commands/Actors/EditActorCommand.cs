using Dapper;
using Movies.Data.Entities;
using System.Data;
using System.Net;

namespace Movies.Data.Commands.Actors;

public class EditActorCommand(int id, Actor entity) : ICommand<IResponse<Actor>>
{
    private const string Sql = @"
    UPDATE Actors SET ActorName = @actorName, ActorDOB = @actorDOB
    OUTPUT INSERTED.*
    WHERE ActorID = @id AND Version = @version";

    private readonly int _id = id;
    private readonly Actor _entity = entity;

    public bool RequiresTransaction => false;

    public async Task<IResponse<Actor>> ExecuteAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken = default)
    {
        if (_id != _entity?.ActorID)
        {
            return new Response<Actor>(
                raw: null,
                HttpStatusCode.NotFound,
                reason: "Entity not found");
        }

        try
        {
            var updated = await connection.QuerySingleAsync<Actor>(Sql, new
            {
                id = _id,
                version = _entity.Version,
                actorName = _entity.ActorName,
                actorDOB = _entity.ActorDOB,
            }, transaction); ;

            return new Response<Actor>(
                updated,
                raw: string.Empty,
                HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            if (ex.Message?.Contains("duplicate", StringComparison.InvariantCultureIgnoreCase) ?? false)
            {
                return new Response<Actor>(
                    raw: null,
                    HttpStatusCode.Conflict,
                    reason: "Entity with the same genre name exists");
            }

            return new Response<Actor>(
                raw: null,
                HttpStatusCode.InternalServerError,
                reason: "Failed to save entity");
        }
    }
}
