using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;


namespace RaceResultClient;

/// <summary>
/// Core HTTP client for the RaceResult Web API.
/// Handles authentication, request building, and error propagation.
/// </summary>
public sealed class ApiClient : IDisposable
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;
    private string _sessionId = "0";

    /// <summary>
    /// Optional factory for custom exceptions. Receives the error message and HTTP status code.
    /// </summary>
    public Func<string, int, Exception>? ErrorFactory { get; set; }

    public ApiClient(string server, bool useHttps = true, string userAgent = "webapi/1.0")
    {
        var scheme = useHttps ? "https" : "http";
        _baseUrl = $"{scheme}://{server}";

        _http = new HttpClient();
        _http.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
    }

    // ── Session ──────────────────────────────────────────────────────────────

    internal void SetSession(string sessionId) => _sessionId = sessionId;
    public string SessionId => _sessionId;

    // ── Endpoint group factories ─────────────────────────────────────────────

    public PublicEndpoints Public => new(this);
    public GeneralEndpoints General => new(this);

    public EventApiClient ForEvent(string eventId) => new(eventId, this);

    // ── HTTP primitives ───────────────────────────────────────────────────────

    internal async Task<byte[]> GetBytesAsync(string? eventId, string cmd,
        QueryParams? query = null, CancellationToken ct = default)
    {
        var url = BuildUrl(eventId, cmd, query);
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        return await SendAsync(request, ct);
    }

    internal async Task<T> GetAsync<T>(string? eventId, string cmd,
        QueryParams? query = null, CancellationToken ct = default)
    {
        var bytes = await GetBytesAsync(eventId, cmd, query, ct);
        return Deserialize<T>(bytes);
    }

    internal async Task<byte[]> PostBytesAsync(string? eventId, string cmd,
        QueryParams? query, object? body, string contentType = "application/json",
        CancellationToken ct = default)
    {
        var url = BuildUrl(eventId, cmd, query);
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = BuildContent(body, contentType);
        return await SendAsync(request, ct);
    }

    internal async Task<T> PostAsync<T>(string? eventId, string cmd,
        QueryParams? query, object? body, string contentType = "application/json",
        CancellationToken ct = default)
    {
        var bytes = await PostBytesAsync(eventId, cmd, query, body, contentType, ct);
        return Deserialize<T>(bytes);
    }

    // fire-and-forget variants (discard response body)
    internal Task GetVoidAsync(string? eventId, string cmd,
        QueryParams? query = null, CancellationToken ct = default)
        => GetBytesAsync(eventId, cmd, query, ct);

    internal Task PostVoidAsync(string? eventId, string cmd,
        QueryParams? query, object? body, string contentType = "application/json",
        CancellationToken ct = default)
        => PostBytesAsync(eventId, cmd, query, body, contentType, ct);

    // ── Internals ─────────────────────────────────────────────────────────────

    private async Task<byte[]> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _sessionId);

        using var response = await _http.SendAsync(request, ct);
        var bytes = await response.Content.ReadAsByteArrayAsync(ct);

        if (response.StatusCode == HttpStatusCode.OK)
            return bytes;

        // Try to extract a structured error message from the JSON body
        var message = TryParseErrorMessage(bytes) ?? response.ReasonPhrase ?? "Unknown error";
        var statusCode = (int)response.StatusCode;

        throw ErrorFactory?.Invoke(message, statusCode)
              ?? new ApiException(message, statusCode);
    }

    private static string? TryParseErrorMessage(byte[] bytes)
    {
        try
        {
            using var doc = JsonDocument.Parse(bytes);
            if (doc.RootElement.TryGetProperty("Error", out var errorProp))
                return errorProp.GetString();
        }
        catch { /* fall through */ }
        return bytes.Length > 0 ? Encoding.UTF8.GetString(bytes) : null;
    }

    private string BuildUrl(string? eventId, string cmd, QueryParams? query)
    {
        var sb = new StringBuilder(_baseUrl);
        if (!string.IsNullOrEmpty(eventId))
            sb.Append("/_").Append(eventId);
        sb.Append("/api/").Append(cmd);
        if (query is { Count: > 0 })
            sb.Append('?').Append(query.ToQueryString());
        return sb.ToString();
    }

    private static HttpContent? BuildContent(object? body, string contentType)
    {
        if (body is null) return null;

        return body switch
        {
            byte[] bytes => new ByteArrayContent(bytes) { Headers = { ContentType = MediaTypeHeaderValue.Parse(contentType) } },
            string s => new StringContent(s, Encoding.UTF8, contentType),
            HttpContent hc => hc,
            _ => new StringContent(JsonSerializer.Serialize(body, JsonOptions.Default), Encoding.UTF8, "application/json")
        };
    }

    private static T Deserialize<T>(byte[] bytes)
    {
        // Plain string shortcut (avoids having to unwrap a JSON string)
        if (typeof(T) == typeof(string))
            return (T)(object)Encoding.UTF8.GetString(bytes);

        return JsonSerializer.Deserialize<T>(bytes, JsonOptions.Default)
               ?? throw new JsonException($"Deserialization returned null for type {typeof(T).Name}");
    }

    public void Dispose() => _http.Dispose();
}