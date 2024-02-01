using Dapper;
using Movies.Data.Entities;
using System.Data;
using System.Net;

namespace Movies.Data.Queries.Movies;

public class MovieListReportQuery(int? genre = null) : IQuery<IResponse<IEnumerable<Movie>>>
{
    private const string SqlByGenre = @"
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
        WHERE m.Genre = @genre
        ORDER BY m.MovieID, g.GenreID, t.ActorID0, t.MovieID";

    private const string SqlList = @"
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
        ORDER BY m.MovieID, g.GenreID, t.ActorID0, t.MovieID";

    private readonly int? _genre = genre;

    public async Task<IResponse<IEnumerable<Movie>>> ExecuteAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken = default)
    {
        IDictionary<int, Movie> entities = new Dictionary<int, Movie>();

        if (_genre != null)
        {
            var list = await connection.QueryAsync<Movie, Genre, Character, Actor, Movie>(
                SqlByGenre,
                (movie, genre, character, actor) =>
                {
                    if (!entities.TryGetValue(movie.MovieID, out Movie? m))
                    {
                        m = movie;
                        m.MovieGenre = genre;
                        entities.Add(movie.MovieID, m);
                    }

                    m.Characters ??= new List<Character>();

                    if (character?.MovieID != null)
                    {
                        character.ActorID = actor.ActorID;
                        character.Actor = actor;
                        m.Characters.Add(character);
                    }

                    return m;
                },
                transaction: transaction,
                param: new { genre = _genre },
                splitOn: "GenreID,ActorID0,ActorID");
        }
        else
        {
            var list = await connection.QueryAsync<Movie, Genre, Character, Actor, Movie>(
                SqlList,
                (movie, genre, character, actor) =>
                {
                    if (!entities.TryGetValue(movie.MovieID, out Movie? m))
                    {
                        m = movie;
                        m.MovieGenre = genre;
                        entities.Add(movie.MovieID, m);
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
                splitOn: "GenreID,ActorID0,ActorID");
        }

        var result = new Response<IEnumerable<Movie>>(
            entities.Values,
            raw: string.Empty,
            HttpStatusCode.OK);

        return result;
    }
}