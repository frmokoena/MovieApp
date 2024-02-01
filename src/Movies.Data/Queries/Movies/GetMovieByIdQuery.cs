using Dapper;
using Movies.Data.Entities;
using System.Data;
using System.Net;

namespace Movies.Data.Queries.Movies;

public class GetMovieByIdQuery(int id) : IQuery<IResponse<Movie?>>
{
    private const string Sql = @"
        SELECT 
            m.MovieID,
            m.MovieName,
            m.ReleaseYear,
            m.Genre,
            m.Version,
            g.GenreID, 
            g.GenreName,
            t.ActorID0,  
            t.MovieID,            
            t.CharacterName,
            t.ActorID,             
            t.ActorDOB,           
            t.ActorName         
        FROM Movies m
        INNER JOIN Genres g ON m.Genre = g.GenreID
        LEFT JOIN (
            SELECT 
                a.ActorDOB,
                a.ActorID, 
                a.ActorName,
                c.CharacterName,
                c.ActorID AS ActorID0, 
                c.MovieID
            FROM Characters c
            INNER JOIN Actors a ON c.ActorID = a.ActorID
        )  t ON m.MovieID = t.MovieID
        WHERE m.MovieID = @id
        ORDER BY m.MovieID, g.GenreID, t.ActorID0, t.MovieID";

    private readonly int _id = id;

    public async Task<IResponse<Movie?>> ExecuteAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken = default)
    {
        IDictionary<int, Movie> movies = new Dictionary<int, Movie>();

        var list = await connection.QueryAsync<Movie, Genre, Character, Actor, Movie>(
            Sql,
            (movie, genre, character, actor) =>
            {
                if (!movies.TryGetValue(movie.MovieID, out Movie? m))
                {
                    m = movie;
                    m.MovieGenre = genre;
                    movies.Add(movie.MovieID, m);
                }

                m.Characters ??= new List<Character>();

                if (character?.MovieID > 0)
                {
                    character.ActorID = actor.ActorID;
                    character.Actor = actor;
                    m.Characters.Add(character);
                }

                return m;
            },
            transaction: transaction,
            param: new { id = _id },
            splitOn: "GenreID,ActorID0,ActorID");

        var entity = movies.FirstOrDefault().Value;

        if (entity == null)
        {
            return new Response<Movie?>(
                raw: null,
                HttpStatusCode.NotFound,
                reason: "Entity not found");
        }

        var result = new Response<Movie?>(
            entity,
            raw: string.Empty,
            HttpStatusCode.OK);

        return result;
    }
}