namespace RaceResultClient;

// ── Public endpoints (login, event management, user rights) ───────────────────

/// <summary>
/// Endpoint group for server-level operations: authentication, event lifecycle, user rights.
/// </summary>
public sealed class PublicEndpoints(ApiClient client)
{
    /// <summary>Authenticates using credentials, API key, or RR user token.</summary>
    public async Task LoginAsync(LoginOptions options, CancellationToken ct = default)
    {
        var form = new FormUrlEncodedContent(BuildLoginForm(options));
        var bytes = await client.PostBytesAsync(null, "public/login",
            query: null, body: form,
            contentType: "application/x-www-form-urlencoded", ct);
        client.SetSession(System.Text.Encoding.UTF8.GetString(bytes));
    }

    /// <summary>Terminates the current session.</summary>
    public Task LogoutAsync(CancellationToken ct = default)
        => client.GetVoidAsync(null, "public/logout", ct: ct);

    /// <summary>Returns a paged list of events for a given year and optional name filter.</summary>
    public Task<EventListItem[]> GetEventListAsync(int year, string filter = "",
        CancellationToken ct = default)
        => client.GetAsync<EventListItem[]>(null, "public/eventlist",
            new QueryParams()
                .Add("year", year)
                .Add("filter", filter)
                .Add("addsettings", "EventName,EventDate,EventDate2,EventLocation,EventCountry"),
            ct);

    /// <summary>Creates a new event and returns an <see cref="EventApiClient"/> for it.</summary>
    public async Task<EventApiClient> CreateEventAsync(string name, DateTime date,
        int country = 0, int copyOf = 0, int templateId = 0, int mode = 0, int laps = 0,
        CancellationToken ct = default)
    {
        var q = new QueryParams()
            .Add("name", name)
            .Add("date", date)
            .Add("country", country)
            .Add("copyOf", copyOf)
            .Add("templateID", templateId)
            .Add("mode", mode)
            .Add("laps", laps);

        var eventId = await client.GetAsync<string>(null, "public/createevent", q, ct);
        return client.ForEvent(eventId);
    }

    /// <summary>Permanently deletes an event. Use with care.</summary>
    public Task DeleteEventAsync(string eventId, CancellationToken ct = default)
        => client.GetVoidAsync(null, "public/deleteevent",
            new QueryParams().Add("eventID", eventId), ct);

    /// <summary>Returns the currently authenticated user's info.</summary>
    public Task<UserInfo> GetUserInfoAsync(CancellationToken ct = default)
        => client.GetAsync<UserInfo>(null, "public/userinfo", ct: ct);

    /// <summary>Lists users with access rights for an event.</summary>
    public Task<UserRight[]> GetUserRightsAsync(string eventId, CancellationToken ct = default)
        => client.GetAsync<UserRight[]>(null, "userrights/get",
            new QueryParams().Add("eventID", eventId), ct);

    /// <summary>Grants or updates access rights for a user on an event.</summary>
    public Task SaveUserRightsAsync(string eventId, string user, string rights,
        CancellationToken ct = default)
        => client.GetVoidAsync(null, "userrights/save",
            new QueryParams().Add("eventID", eventId).Add("user", user).Add("rights", rights), ct);

    /// <summary>Revokes access rights for a user on an event.</summary>
    public Task DeleteUserRightsAsync(string eventId, int userId, CancellationToken ct = default)
        => client.GetVoidAsync(null, "userrights/delete",
            new QueryParams().Add("eventID", eventId).Add("userID", userId), ct);

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static IEnumerable<KeyValuePair<string, string>> BuildLoginForm(LoginOptions o)
    {
        if (!string.IsNullOrEmpty(o.ApiKey))
            yield return new("apikey", o.ApiKey);
        if (!string.IsNullOrEmpty(o.User))
        {
            yield return new("user", o.User);
            yield return new("pw", o.Password ?? "");
        }
        if (!string.IsNullOrEmpty(o.SignInAs))
            yield return new("signinas", o.SignInAs);
        if (!string.IsNullOrEmpty(o.Totp))
            yield return new("totp", o.Totp);
        if (!string.IsNullOrEmpty(o.RrUserToken))
            yield return new("rruser_token", o.RrUserToken);
    }
}

/// <summary>Login credentials. Populate only the fields appropriate for your auth method.</summary>
public sealed class LoginOptions
{
    public string? User { get; set; }
    public string? Password { get; set; }
    public string? SignInAs { get; set; }
    public string? Totp { get; set; }
    public string? ApiKey { get; set; }
    public string? RrUserToken { get; set; }
}

// ── General endpoints (fonts, version, translation) ──────────────────────────

/// <summary>
/// Endpoint group for server-wide utility endpoints.
/// </summary>
public sealed class GeneralEndpoints(ApiClient client)
{
    /// <summary>Returns a list of font names supported by the server.</summary>
    public Task<string[]> GetFontsAsync(CancellationToken ct = default)
        => client.GetAsync<string[]>(null, "fonts", ct: ct);

    /// <summary>Returns the web server application version string.</summary>
    public Task<string> GetAppVersionAsync(CancellationToken ct = default)
        => client.GetAsync<string>(null, "appversion", ct: ct);

    /// <summary>
    /// Translates a batch of field names or expressions.
    /// </summary>
    /// <param name="items">Items to translate.</param>
    /// <param name="fromEnglish">If true, translates from English to <paramref name="lang"/>; otherwise reverse.</param>
    /// <param name="lang">Target (or source) language code.</param>
    public Task<string[]> TranslateAsync(IEnumerable<string> items, bool fromEnglish,
        string lang, CancellationToken ct = default)
        => client.PostAsync<string[]>(null, "translate2",
            new QueryParams().Add("fromEnglish", fromEnglish).Add("lang", lang),
            items.ToArray(), ct: ct);

    /// <summary>Retrieves a single translation text item by name and language.</summary>
    public Task<string> GetLangItemAsync(string name, string lang, CancellationToken ct = default)
        => client.GetAsync<string>(null, "getlangitem",
            new QueryParams().Add("name", name).Add("lang", lang), ct);
}