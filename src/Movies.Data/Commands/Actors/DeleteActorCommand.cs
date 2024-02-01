using Dapper;
using Movies.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Data.Commands.Actors;

public class DeleteActorCommand(int id) : ICommand<IResponse<Actor>>
{
    private const string Sql = @"
			DELETE
				Actors
            OUTPUT DELETED.*
			WHERE
				ActorID = @id";

    private readonly int _id = id;

    public bool RequiresTransaction => false;

    public async Task<IResponse<Actor>> ExecuteAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var deleted = await connection.QuerySingleAsync<Actor>(Sql, new
            {
                id = _id
            }, transaction);

            return new Response<Actor>(
                deleted,
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

            if (ex.Message?.Contains("REFERENCE constraint", StringComparison.InvariantCultureIgnoreCase) ?? false)
            {
                // REFERENCE constraint
                return new Response<Actor>(
                    raw: null,
                    HttpStatusCode.Conflict,
                    reason: "Cant not delete. Entity is has been used");
            }

            return new Response<Actor>(
                raw: null,
                HttpStatusCode.InternalServerError,
                reason: "Failed to save entity");
        }
    }
}