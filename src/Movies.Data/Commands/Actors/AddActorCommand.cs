using Dapper;
using Movies.Data.Entities;
using System.Data;
using System.Net;

namespace Movies.Data.Commands.Actors;

public class AddActorCommand(Actor entity) : ICommand<IResponse<Actor>>
{
    private const string Sql = @"
			INSERT INTO Actors(ActorName,ActorDOB)
            OUTPUT INSERTED.*
			VALUES(@actorName,@actorDOB)";

    private readonly Actor _entity = entity;

    public bool RequiresTransaction => false;

    public async Task<IResponse<Actor>> ExecuteAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var inserted = await connection.QuerySingleAsync<Actor>(Sql, new
            {
                actorName = _entity.ActorName,
                actorDOB = _entity.ActorDOB

            }, transaction);

            return new Response<Actor>(
                inserted,
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
                    reason: "Entity exists");
            }

            return new Response<Actor>(
                raw: null,
                HttpStatusCode.InternalServerError,
                reason: "Failed to save entity");
        }
    }
}