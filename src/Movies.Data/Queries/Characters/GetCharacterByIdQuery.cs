using Dapper;
using Movies.Data.Entities;
using System.Data;
using System.Net;

namespace Movies.Data.Queries.Characters;

public class GetCharacterByIdQuery(int movieID, int actorID) : IQuery<IResponse<Character?>>
{
    private const string Sql = @"
        SELECT 
            c.CharacterName,
            c.Version,
            a.ActorID, 
            a.ActorName,
            a.ActorDOB,
            c.MovieID,
            m.MovieName,
            m.ReleaseYear
        FROM Characters c
        INNER JOIN Actors a ON c.ActorID = a.ActorID
        INNER JOIN Movies m ON c.MovieID = m.MovieID
        WHERE c.MovieID = @movieID AND a.ActorID = @actorID";

    private readonly int _movieID = movieID;
    private readonly int _actorID = actorID;

    public async Task<IResponse<Character?>> ExecuteAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken = default)
    {
        var list = await connection.QueryAsync<Character, Actor, Movie, Character>(
            Sql,
            (character, actor, movie) => { character.Actor = actor; character.Movie = movie; return character; },
            transaction: transaction,
            param: new { movieID = _movieID, actorID = _actorID },
            splitOn: "ActorId,MovieID");

        var entity = list.FirstOrDefault();

        if (entity == null)
        {
            return new Response<Character?>(
                raw: null,
                HttpStatusCode.NotFound,
                reason: "Entity not found");
        }

        var result = new Response<Character?>(
            entity,
            raw: string.Empty,
            HttpStatusCode.OK);

        return result;
    }
}