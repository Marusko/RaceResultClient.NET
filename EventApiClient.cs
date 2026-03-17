using System.Text.Json;

namespace RaceResultClient;

/// <summary>
/// Per-event API facade. Obtain via <see cref="ApiClient.ForEvent"/> or
/// <see cref="Endpoints.PublicEndpoints.CreateEventAsync"/>.
/// All endpoint groups are created on-demand (no state beyond the event ID).
/// </summary>
public sealed class EventApiClient
{
    internal readonly string EventId;
    internal readonly ApiClient Client;

    internal EventApiClient(string eventId, ApiClient client)
    {
        EventId = eventId;
        Client = client;
    }

    // ── Endpoint group accessors (lazy, stateless) ───────────────────────────

    public AgeGroupsEndpoints AgeGroups => new(this);
    public BibRangesEndpoints BibRanges => new(this);
    public ContestsEndpoints Contests => new(this);
    public CustomFieldsEndpoints CustomFields => new(this);
    public DataEndpoints Data => new(this);
    public EntryFeesEndpoints EntryFees => new(this);
    public ExportersEndpoints Exporters => new(this);
    public HistoryEndpoints History => new(this);
    public ParticipantsEndpoints Participants => new(this);
    public RankingsEndpoints Rankings => new(this);
    public RawDataEndpoints RawData => new(this);
    public ResultsEndpoints Results => new(this);
    public SettingsEndpoints Settings => new(this);
    public SplitsEndpoints Splits => new(this);
    public TeamScoresEndpoints TeamScores => new(this);
    public TimesEndpoints Times => new(this);
    public TimingPointsEndpoints TimingPoints => new(this);
    public TimingPointRulesEndpoints TimingPointRules => new(this);
    public VouchersEndpoints Vouchers => new(this);
    public WebHooksEndpoints WebHooks => new(this);
    public SimpleApiEndpoints SimpleApi => new(this);
    public ChatEndpoints Chat => new(this);
    public StatisticsEndpoints Statistics => new(this);
    public ForwardingEndpoints Forwarding => new(this);
    public RegistrationsEndpoints Registrations => new(this);
    public UserDefinedFieldsEndpoints UserDefinedFields => new(this);
    public OverwriteValuesEndpoints OverwriteValues => new(this);
    public FileEndpoints File => new(this);

    // ── Low-level escape hatches ──────────────────────────────────────────────

    /// <summary>
    /// Executes a raw GET against an arbitrary endpoint path for this event.
    /// Useful for endpoints not yet surfaced by this library.
    /// </summary>
    public Task<byte[]> GetRawAsync(string requestUri, CancellationToken ct = default)
        => Client.GetBytesAsync(EventId, requestUri, ct: ct);

    /// <summary>
    /// Executes a raw POST against an arbitrary endpoint path for this event.
    /// </summary>
    public Task<byte[]> PostRawAsync(string requestUri, object? body, CancellationToken ct = default)
        => Client.PostBytesAsync(EventId, requestUri, query: null, body: body, ct: ct);

    /// <summary>
    /// Sends multiple resource requests in a single round-trip.
    /// Returns a dictionary keyed by resource name.
    /// </summary>
    public Task<Dictionary<string, JsonElement>> MultiRequestAsync(
        IEnumerable<string> requests, CancellationToken ct = default)
        => Client.PostAsync<Dictionary<string, JsonElement>>(
            EventId, "multirequest", query: null, body: requests.ToArray(), ct: ct);

    // ── Internal helpers used by endpoint classes ─────────────────────────────

    internal Task<byte[]> GetBytesAsync(string cmd, QueryParams? q = null, CancellationToken ct = default)
        => Client.GetBytesAsync(EventId, cmd, q, ct);

    internal Task<T> GetAsync<T>(string cmd, QueryParams? q = null, CancellationToken ct = default)
        => Client.GetAsync<T>(EventId, cmd, q, ct);

    internal Task GetVoidAsync(string cmd, QueryParams? q = null, CancellationToken ct = default)
        => Client.GetVoidAsync(EventId, cmd, q, ct);

    internal Task<T> PostAsync<T>(string cmd, QueryParams? q, object? body, CancellationToken ct = default)
        => Client.PostAsync<T>(EventId, cmd, q, body, ct: ct);

    internal Task PostVoidAsync(string cmd, QueryParams? q, object? body, CancellationToken ct = default)
        => Client.PostVoidAsync(EventId, cmd, q, body, ct: ct);
}