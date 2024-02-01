using Dapper;
using Movies.Data.Entities;
using System.Data;
using System.Net;

namespace Movies.Data.Queries.Movies;

public class ListMoviesQuery : IQuery<IResponse<IEnumerable<Movie>>>
{
    private const string Sql = @"
			SELECT
				m.MovieID,
				m.MovieName,
                m.ReleaseYear,
                m.Genre,
                g.GenreID,
                g.GenreName
			FROM
				Movies m
            INNER JOIN Genres g on g.GenreID = m.Genre
            ORDER BY m.MovieName ASC";

    public async Task<IResponse<IEnumerable<Movie>>> ExecuteAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken = default)
    {
        var list = await connection.QueryAsync<Movie, Genre, Movie>(
            Sql,
            (movie, genre) => { movie.MovieGenre = genre; return movie; },
            transaction: transaction, splitOn: "GenreID");

        var result = new Response<IEnumerable<Movie>>(
            list,
            raw: string.Empty,
            HttpStatusCode.OK);

        return result;
    }
}