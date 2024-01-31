using System.Net;

namespace Movies.Data;

public interface IResponse<T>
{
    T? Content { get; }
    string? Error { get; }
    bool IsError { get; }
    HttpStatusCode HttpStatusCode { get; }
    string? HttpReasonPhrase { get; }
    string? Raw { get; }
    Exception? Exception { get; }
    ResponseError ResponseError { get; }
}
