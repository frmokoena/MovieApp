using Dapper;
using Movies.Data.Entities;
using System.Data;
using System.Net;

namespace Movies.Data.Queries.Movies;

public class MovieListReportQuery() : IQuery<IResponse<IEnumerable<Movie>>>
{
    private const string Sql = @"
        SELECT 
            m.MovieID, 
            m.MovieName, 
            g.GenreName,
            g.GenreID, 
            t.ActorID, 
            t.ActorDOB, 
            t.ActorName, 
            t.CharacterName, 
            t.MovieID, 
            t.ActorIDIN, 
            m.ReleaseYear
        FROM Movies  m
        INNER JOIN Genres g ON  g.GenreID = m.Genre
        LEFT JOIN 
        (
            SELECT c.ActorID, a.ActorDOB, a.ActorName, c.CharacterName, c.MovieID, a.ActorID AS ActorIDIN
            FROM Characters c
            INNER JOIN Actors a ON a.ActorID = c.ActorID
        )  t ON  t.MovieID = m.MovieID
        ORDER BY m.MovieID, g.GenreID, t.ActorID, t.MovieID";

    public async Task<IResponse<IEnumerable<Movie>>> ExecuteAsync(
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
                    movies.Add(movie.MovieID, m = movie);
                }

                m.Characters ??= new List<Character>();

                if (character != null)
                {
                    character.Actor = actor;
                    m.Characters.Add(character);
                }

                return m;
            },
            transaction: transaction);

        var entity = movies.Values;

        if (entity == null)
        {
            return new Response<IEnumerable<Movie>>(
                raw: null,
                HttpStatusCode.NotFound,
                reason: "Entity not found");
        }

        var result = new Response<IEnumerable<Movie>>(
            entity,
            raw: string.Empty,
            HttpStatusCode.OK);

        return result;
    }
}
