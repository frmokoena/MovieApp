using Dapper;
using Movies.Data.Entities;
using System.Data;
using System.Net;

namespace Movies.Data.Queries.Characters;

public class ListCharactersQuery : IQuery<IResponse<IEnumerable<Character>>>
{
    private const string Sql = @"
        SELECT 
            c.CharacterName,
            a.ActorID, 
            a.ActorName,
            a.ActorDOB,
            c.MovieID,
            m.MovieName,
            m.ReleaseYear
        FROM Characters c
        INNER JOIN Actors a ON c.ActorID = a.ActorID
        INNER JOIN Movies m ON c.MovieID = m.MovieID";

    public async Task<IResponse<IEnumerable<Character>>> ExecuteAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken = default)
    {
        var list = await connection.QueryAsync<Character, Actor, Movie, Character>(
            Sql,
            (character, actor, movie) => { character.Actor = actor; character.Movie = movie; return character; },
            transaction: transaction, splitOn: "ActorId,MovieID");

        var result = new Response<IEnumerable<Character>>(
            list,
            raw: string.Empty,
            HttpStatusCode.OK);

        return result;
    }
}
