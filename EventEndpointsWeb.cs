using System.Text.Json;

namespace RaceResultClient;

// Endpoint groups RaceResult's own JavaScript client (raceresultJS.js, served by every RaceResult server)
// has and go-webapi does not. Paths and parameters are the JS client's; the models are go-model's.

// ── Table ─────────────────────────────────────────────────────────────────────

/// <summary>The event's lookup table (<c>table/…</c>), read in formulas with <c>TValue(…)</c>.</summary>
public sealed class TableEndpoints(EventApiClient e)
{
    public Task<TableValue[]> GetAsync(CancellationToken ct = default)
        => e.GetAsync<TableValue[]>("table/get", ct: ct);

    /// <summary>Replaces the table with these values.</summary>
    public Task SaveAsync(IEnumerable<TableValue> values, CancellationToken ct = default)
        => e.PostVoidAsync("table/save", null, values.ToArray(), ct);
}

// ── Presenters ────────────────────────────────────────────────────────────────

public sealed class PresentersEndpoints(EventApiClient e)
{
    public Task<string[]> NamesAsync(CancellationToken ct = default)
        => e.GetAsync<string[]>("presenters/names", ct: ct);

    public Task<Presenter> GetAsync(string name, CancellationToken ct = default)
        => e.GetAsync<Presenter>("presenters/get", new QueryParams().Add("name", name), ct);

    public Task SaveAsync(Presenter item, CancellationToken ct = default)
        => e.PostVoidAsync("presenters/save", null, item, ct);

    public Task DeleteAsync(string name, CancellationToken ct = default)
        => e.GetVoidAsync("presenters/delete", new QueryParams().Add("name", name), ct);

    public Task CopyAsync(string name, string newName, CancellationToken ct = default)
        => e.GetVoidAsync("presenters/copy", new QueryParams().Add("name", name).Add("newName", newName), ct);

    public Task RenameAsync(string name, string newName, CancellationToken ct = default)
        => e.GetVoidAsync("presenters/rename", new QueryParams().Add("name", name).Add("newName", newName), ct);

    public Task NewAsync(string name, CancellationToken ct = default)
        => e.GetVoidAsync("presenters/new", new QueryParams().Add("name", name), ct);
}

// ── Registration components ───────────────────────────────────────────────────

/// <summary>Reusable parts of registration forms (<c>registrations/components/…</c>).</summary>
public sealed class RegistrationComponentsEndpoints(EventApiClient e)
{
    public Task<string[]> NamesAsync(CancellationToken ct = default)
        => e.GetAsync<string[]>("registrations/components/names", ct: ct);

    /// <summary>A component; <paramref name="translate"/> resolves its texts into the event's languages.</summary>
    public Task<RegistrationComponent> GetAsync(string name, bool translate = false, CancellationToken ct = default)
        => e.GetAsync<RegistrationComponent>("registrations/components/get",
            new QueryParams().Add("name", name).Add("translate", translate), ct);

    public Task SaveAsync(RegistrationComponent item, bool translate = false, CancellationToken ct = default)
        => e.PostVoidAsync("registrations/components/save", new QueryParams().Add("translate", translate), item, ct);

    public Task DeleteAsync(string name, CancellationToken ct = default)
        => e.GetVoidAsync("registrations/components/delete", new QueryParams().Add("name", name), ct);

    public Task CopyAsync(string name, string newName, CancellationToken ct = default)
        => e.GetVoidAsync("registrations/components/copy", new QueryParams().Add("name", name).Add("newName", newName), ct);

    public Task RenameAsync(string name, string newName, CancellationToken ct = default)
        => e.GetVoidAsync("registrations/components/rename", new QueryParams().Add("name", name).Add("newName", newName), ct);

    public Task NewAsync(string name, CancellationToken ct = default)
        => e.GetVoidAsync("registrations/components/new", new QueryParams().Add("name", name), ct);
}

// ── Websites ──────────────────────────────────────────────────────────────────

/// <summary>The event's site on my.raceresult.com (<c>websites/…</c>).</summary>
public sealed class WebsitesEndpoints(EventApiClient e)
{
    /// <summary>A site; an event has one, named "default" (what RaceResult's pages read).</summary>
    public Task<Website> GetAsync(string name = "default", CancellationToken ct = default)
        => e.GetAsync<Website>("websites/get", new QueryParams().Add("name", name), ct);

    public Task SaveAsync(Website item, CancellationToken ct = default)
        => e.PostVoidAsync("websites/save", null, item, ct);

    /// <summary>The lists the site shows.</summary>
    public Task<string[]> GetPublishedListsAsync(CancellationToken ct = default)
        => e.GetAsync<string[]>("websites/getpublishedlists", ct: ct);

    /// <summary>The certificates the site offers.</summary>
    public Task<string[]> GetPublishedCertificatesAsync(CancellationToken ct = default)
        => e.GetAsync<string[]>("websites/getpublishedcertificates", ct: ct);
}

// ── Invoices ──────────────────────────────────────────────────────────────────

/// <summary>
/// The event's invoices (<c>invoices/…</c>). Making them takes the event's <see cref="InvoiceSettings"/>
/// (<see cref="SettingsEndpoints.GetInvoiceSettingsAsync"/>), as RaceResult's invoices page sends them.
/// </summary>
public sealed class InvoicesEndpoints(EventApiClient e)
{
    public Task<InvoiceWithSum[]> GetAsync(InvoiceFilter? filter = null, CancellationToken ct = default)
        => e.PostAsync<InvoiceWithSum[]>("invoices/get", null, filter ?? new InvoiceFilter(), ct);

    public Task<InvoiceWithDetails[]> GetWithDetailsAsync(InvoiceFilter? filter = null, CancellationToken ct = default)
        => e.PostAsync<InvoiceWithDetails[]>("invoices/getwithdetails", null, filter ?? new InvoiceFilter(), ct);

    /// <summary>The invoices' payment references, by invoice.</summary>
    public Task<Dictionary<string, JsonElement>> PayRefMapAsync(InvoiceFilter? filter = null, CancellationToken ct = default)
        => e.PostAsync<Dictionary<string, JsonElement>>("invoices/payrefmap", null, filter ?? new InvoiceFilter(), ct);

    /// <summary>The invoices <see cref="CreateAsync"/> would make for the participants matching the filter.</summary>
    public Task<InvoiceWithDetails[]> PreviewAsync(string filter, InvoiceSettings settings, CancellationToken ct = default)
        => e.PostAsync<InvoiceWithDetails[]>("invoices/preview", new QueryParams().Add("filter", filter), settings, ct);

    /// <summary>Makes the invoices for the participants matching the filter.</summary>
    public Task<JsonElement> CreateAsync(string filter, InvoiceSettings settings, CancellationToken ct = default)
        => e.PostAsync<JsonElement>("invoices/create", new QueryParams().Add("filter", filter), settings, ct);

    public Task<InvoiceWithDetails> DetailsAsync(int id, CancellationToken ct = default)
        => e.GetAsync<InvoiceWithDetails>("invoices/details", new QueryParams().AddAlways("id", id), ct);

    public Task DeleteAsync(IEnumerable<int> ids, CancellationToken ct = default)
        => e.GetVoidAsync("invoices/delete", new QueryParams().Add("id", string.Join(",", ids)), ct);

    /// <summary>Makes credit notes for these invoices.</summary>
    public Task ReverseAsync(IEnumerable<int> ids, InvoiceSettings settings, CancellationToken ct = default)
        => e.PostVoidAsync("invoices/reverse", new QueryParams().Add("id", string.Join(",", ids)), settings, ct);

    /// <summary>Updates these invoices' receiver data; their numbers and items remain.</summary>
    public Task RecreateAsync(IEnumerable<int> ids, InvoiceSettings settings, CancellationToken ct = default)
        => e.PostVoidAsync("invoices/recreate", new QueryParams().Add("id", string.Join(",", ids)), settings, ct);

    /// <summary>These invoices as a file (PDF; several in one).</summary>
    public Task<byte[]> DownloadAsync(IEnumerable<int> ids, CancellationToken ct = default)
        => e.GetBytesAsync("invoices/download", new QueryParams().Add("id", string.Join(",", ids)), ct);

    /// <summary>One invoice as shown in the browser.</summary>
    public Task<byte[]> ShowAsync(int id, CancellationToken ct = default)
        => e.GetBytesAsync("invoices/show", new QueryParams().AddAlways("id", id), ct);

    /// <summary>The invoices' layout: a certificate design.</summary>
    public Task<Certificate> GetTemplateAsync(CancellationToken ct = default)
        => e.GetAsync<Certificate>("invoices/template/get", ct: ct);

    public Task SaveTemplateAsync(Certificate template, CancellationToken ct = default)
        => e.PostVoidAsync("invoices/template/save", null, template, ct);

    /// <summary>RaceResult's default invoice layout.</summary>
    public Task<Certificate> GetDefaultTemplateAsync(CancellationToken ct = default)
        => e.GetAsync<Certificate>("invoices/template/default", ct: ct);
}

// ── Triggers ──────────────────────────────────────────────────────────────────

/// <summary>
/// Push subscriptions (<c>trigger/…</c>). RaceResult's own client reads them over a WebSocket
/// (<c>trigger/ws</c>), which this library does not open.
/// </summary>
public sealed class TriggersEndpoints(EventApiClient e)
{
    /// <summary>Creates a trigger and returns its id.</summary>
    public Task<int> CreateAsync(TriggerType type, int result = 0, int contest = 0, IEnumerable<string>? fields = null,
        string filter = "", string splitName = "", CancellationToken ct = default)
        => e.GetAsync<int>("trigger/create",
            new QueryParams().AddAlways("type", (int)type).Add("result", result).Add("contest", contest)
                .AddArray("fields", fields ?? []).Add("filter", filter).Add("splitname", splitName), ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("trigger/delete", new QueryParams().AddAlways("id", id), ct);
}
