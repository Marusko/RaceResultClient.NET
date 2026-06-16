using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace RaceResultClient;

// ── Query string builder ──────────────────────────────────────────────────────

/// <summary>
/// Fluent builder for URL query parameters.
/// Handles type-safe serialization of primitives, arrays, and dates.
/// </summary>
public sealed class QueryParams
{
    private readonly List<(string Key, string Value)> _params = new();

    public int Count => _params.Count;

    public QueryParams Add(string key, string? value)
    {
        if (!string.IsNullOrEmpty(value))
            _params.Add((key, value));
        return this;
    }

    public QueryParams Add(string key, int value) => value != 0 ? Add(key, value.ToString()) : this;
    public QueryParams Add(string key, bool value) => Add(key, value ? "true" : "false");
    public QueryParams Add(string key, double value) => value != 0 ? Add(key, value.ToString("G", CultureInfo.InvariantCulture)) : this;
    public QueryParams Add(string key, decimal value) => value != 0 ? Add(key, value.ToString(CultureInfo.InvariantCulture)) : this;
    public QueryParams Add(string key, DateTime value) => Add(key, value.ToString("o", CultureInfo.InvariantCulture));

    // Forced variants — emit the parameter even when the value is the type default.
    public QueryParams AddAlways(string key, int value) => Add(key, value.ToString());
    public QueryParams AddAlways(string key, decimal value) => Add(key, value.ToString(CultureInfo.InvariantCulture));
    public QueryParams AddAlways(string key, double value) => Add(key, value.ToString("G", CultureInfo.InvariantCulture));

    public QueryParams AddArray(string key, IEnumerable<string> values)
    {
        var arr = values.ToArray();
        return arr.Length > 0
            ? Add(key, JsonSerializer.Serialize(arr))
            : this;
    }

    public QueryParams AddArray(string key, IEnumerable<int> values)
    {
        var arr = values.ToArray();
        return arr.Length > 0
            ? Add(key, string.Join(",", arr))
            : this;
    }

    public string ToQueryString()
    {
        var sb = new StringBuilder();
        foreach (var (k, v) in _params)
        {
            if (sb.Length > 0) sb.Append('&');
            sb.Append(HttpUtility.UrlEncode(k)).Append('=').Append(HttpUtility.UrlEncode(v));
        }
        return sb.ToString();
    }
}

// ── API exception ─────────────────────────────────────────────────────────────

/// <summary>
/// Thrown when the API returns a non-200 response.
/// Carries the HTTP status code alongside the server's error message.
/// </summary>
public sealed class ApiException(string message, int statusCode) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}

// ── JSON configuration ────────────────────────────────────────────────────────

internal static class JsonOptions
{
    // NOTE: enums are serialized numerically by default to match the Go model
    // (encoding/json marshals integer enum types as their underlying int value).
    // The only string-serialized enums in the Go model (certificate.PageSize /
    // certificate.PageFormat) are individually annotated with a string converter.
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}

// ── Participant identifier (Bib vs PID) ───────────────────────────────────────

/// <summary>
/// Discriminated union for addressing a participant either by bib number or internal PID.
/// </summary>
public readonly struct Identifier
{
    public string Key { get; }
    public int Value { get; }

    private Identifier(string key, int value) { Key = key; Value = value; }

    /// <summary>Address participant by bib number.</summary>
    public static Identifier Bib(int bib) => new("bib", bib);

    /// <summary>Address participant by internal participant ID.</summary>
    public static Identifier Pid(int pid) => new("pid", pid);

    /// <summary>Address participant by internal ID.</summary>
    public static Identifier Id(int id) => new("id", id);

    internal QueryParams ApplyTo(QueryParams q) => q.AddAlways(Key, Value);
}