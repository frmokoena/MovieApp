using System.Diagnostics;
using System.Text.Json;

namespace Movies.Data.Extensions;

public static class JsonStringExtentions
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    [DebuggerStepThrough]
    public static T? ToTypedObject<T>(this string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentNullException(nameof(json));
        }

        var typed = JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);
        return typed;
    }

    [DebuggerStepThrough]
    public static string ToJsonString(this object @object)
    {
        if (@object is string) throw new ArgumentException("value can not be string", nameof(@object));

        var json = JsonSerializer.Serialize(@object, _jsonSerializerOptions);
        return json;
    }
}