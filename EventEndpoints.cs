using System.Text.Json;

namespace RaceResultClient;

// ── AgeGroups ─────────────────────────────────────────────────────────────────

public sealed class AgeGroupsEndpoints(EventApiClient e)
{
    public Task<byte[]> GetPdfAsync(CancellationToken ct = default)
        => e.GetBytesAsync("agegroups/pdf", ct: ct);

    public Task<AgeGroup[]> GetAsync(int contest = 0, int set = 0, string name = "",
        CancellationToken ct = default)
        => e.GetAsync<AgeGroup[]>("agegroups/get",
            new QueryParams().Add("contest", contest).Add("set", set).Add("name", name), ct);

    public Task<AgeGroup> GetOneAsync(int id, CancellationToken ct = default)
        => e.GetAsync<AgeGroup>("agegroups/get", new QueryParams().Add("id", id), ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("agegroups/delete", new QueryParams().Add("id", id), ct);

    public Task<int> SaveAsync(AgeGroup item, int oldId = 0, CancellationToken ct = default)
        => e.PostAsync<int>("agegroups/save", new QueryParams().Add("oldID", oldId), item, ct);
}

// ── BibRanges ─────────────────────────────────────────────────────────────────

public sealed class BibRangesEndpoints(EventApiClient e)
{
    public Task<BibRange[]> GetAsync(CancellationToken ct = default)
        => e.GetAsync<BibRange[]>("bibranges/get", ct: ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("bibranges/delete", new QueryParams().Add("id", id), ct);

    public Task<int> SaveAsync(BibRange item, int oldId = 0, CancellationToken ct = default)
        => e.PostAsync<int>("bibranges/save", new QueryParams().Add("oldID", oldId), item, ct);
}

// ── Contests ──────────────────────────────────────────────────────────────────

public sealed class ContestsEndpoints(EventApiClient e)
{
    public Task<byte[]> GetPdfAsync(CancellationToken ct = default)
        => e.GetBytesAsync("contests/pdf", ct: ct);

    public Task<Contest[]> GetAsync(CancellationToken ct = default)
        => e.GetAsync<Contest[]>("contests/get", ct: ct);

    public Task<Contest> GetOneAsync(int id, CancellationToken ct = default)
        => e.GetAsync<Contest>("contests/get", new QueryParams().Add("id", id), ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("contests/delete", new QueryParams().Add("id", id), ct);

    public Task<int> SaveAsync(Contest item, int oldId = 0, CancellationToken ct = default)
        => e.PostAsync<int>("contests/save", new QueryParams().Add("oldID", oldId), item, ct);
}

// ── CustomFields ──────────────────────────────────────────────────────────────

public sealed class CustomFieldsEndpoints(EventApiClient e)
{
    public Task<CustomField[]> GetAsync(CancellationToken ct = default)
        => e.GetAsync<CustomField[]>("customfields/get", ct: ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("customfields/delete", new QueryParams().Add("id", id), ct);

    public Task<int> SaveAsync(CustomField item, int oldId = 0, CancellationToken ct = default)
        => e.PostAsync<int>("customfields/save", new QueryParams().Add("oldID", oldId), item, ct);
}

// ── Data ──────────────────────────────────────────────────────────────────────

public sealed class DataEndpoints(EventApiClient e)
{
    /// <summary>Returns the number of participants matching <paramref name="filter"/>.</summary>
    public Task<int> CountAsync(string filter = "", CancellationToken ct = default)
        => e.GetAsync<int>("data/count", new QueryParams().Add("filter", filter), ct);

    /// <summary>
    /// Returns arbitrary participant fields as a jagged array.
    /// Each inner array corresponds to one participant row.
    /// </summary>
    public Task<JsonElement[][]> ListAsync(
        IEnumerable<string> fields, string filter = "",
        IEnumerable<string>? sort = null,
        int limitFrom = 0, int limitTo = 0,
        IEnumerable<string>? groups = null,
        string multiplierField = "", string selectorResult = "",
        CancellationToken ct = default)
    {
        var q = new QueryParams()
            .AddArray("fields", fields)
            .Add("filter", filter)
            .AddArray("sort", sort ?? [])
            .Add("limitFrom", limitFrom)
            .Add("limitTo", limitTo)
            .AddArray("groups", groups ?? [])
            .Add("multiplierField", multiplierField)
            .Add("selectorResult", selectorResult)
            .Add("listFormat", "JSON");
        return e.GetAsync<JsonElement[][]>("data/list", q, ct);
    }
}

// ── EntryFees ─────────────────────────────────────────────────────────────────

public sealed class EntryFeesEndpoints(EventApiClient e)
{
    public Task<EntryFee[]> GetAsync(CancellationToken ct = default)
        => e.GetAsync<EntryFee[]>("entryfees/get", ct: ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("entryfees/delete", new QueryParams().Add("id", id), ct);

    public Task<int> SaveAsync(EntryFee item, int oldId = 0, CancellationToken ct = default)
        => e.PostAsync<int>("entryfees/save", new QueryParams().Add("oldID", oldId), item, ct);
}

// ── Exporters ─────────────────────────────────────────────────────────────────

public sealed class ExportersEndpoints(EventApiClient e)
{
    public Task<Exporter[]> GetAsync(CancellationToken ct = default)
        => e.GetAsync<Exporter[]>("exporters/get", ct: ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("exporters/delete", new QueryParams().Add("id", id), ct);

    public Task<int> SaveAsync(Exporter item, int oldId = 0, CancellationToken ct = default)
        => e.PostAsync<int>("exporters/save", new QueryParams().Add("oldID", oldId), item, ct);

    public Task StartAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("exporters/start", new QueryParams().Add("id", id), ct);

    public Task StopAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("exporters/stop", new QueryParams().Add("id", id), ct);
}

// ── History ───────────────────────────────────────────────────────────────────

public sealed class HistoryEndpoints(EventApiClient e)
{
    public Task<HistoryEntry[]> GetAsync(Identifier identifier, HistoryFilter? filter = null,
        CancellationToken ct = default)
    {
        var q = identifier.ApplyTo(new QueryParams());
        return filter is null
            ? e.GetAsync<HistoryEntry[]>("history/get", q, ct)
            : e.PostAsync<HistoryEntry[]>("history/get", q, filter, ct);
    }

    public Task<int> CountAsync(Identifier identifier, CancellationToken ct = default)
        => e.GetAsync<int>("history/count", identifier.ApplyTo(new QueryParams()), ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("history/delete", new QueryParams().Add("id", id), ct);
}

// ── Participants ──────────────────────────────────────────────────────────────

public sealed class ParticipantsEndpoints(EventApiClient e)
{
    /// <summary>Returns selected fields for one participant as a key-value map.</summary>
    public Task<Dictionary<string, JsonElement>> GetFieldsAsync(Identifier id,
        IEnumerable<string> fields, CancellationToken ct = default)
        => e.GetAsync<Dictionary<string, JsonElement>>("part/getfields",
            id.ApplyTo(new QueryParams()).AddArray("fields", fields), ct);

    /// <summary>
    /// Simulates applying <paramref name="changes"/> without persisting them,
    /// and returns the resulting field values.
    /// </summary>
    public Task<Dictionary<string, JsonElement>> GetFieldsWithChangesAsync(
        Identifier id, IEnumerable<string> fields,
        Dictionary<string, object?> changes, CancellationToken ct = default)
        => e.PostAsync<Dictionary<string, JsonElement>>("part/getfieldswithchanges",
            id.ApplyTo(new QueryParams()).AddArray("fields", fields), changes, ct);

    /// <summary>Evaluates an expression and persists the result in a field.</summary>
    public Task SaveExpressionAsync(Identifier id, string field, string expression,
        bool noHistory = false, CancellationToken ct = default)
        => e.GetVoidAsync("part/saveexpression",
            id.ApplyTo(new QueryParams())
              .Add("field", field).Add("expression", expression).Add("noHistory", noHistory),
            ct);

    /// <summary>Saves multiple values for possibly different participants in one call.</summary>
    public Task SaveValueArrayAsync(IEnumerable<SaveValueArrayItem> items,
        bool noHistory = false, CancellationToken ct = default)
        => e.PostVoidAsync("part/savevaluearray",
            new QueryParams().Add("noHistory", noHistory), items.ToArray(), ct);

    /// <summary>Saves multiple fields for a single participant.</summary>
    public Task SaveFieldsAsync(Identifier id, Dictionary<string, object?> values,
        bool noHistory = false, CancellationToken ct = default)
        => e.PostVoidAsync("part/savefields",
            id.ApplyTo(new QueryParams()).Add("noHistory", noHistory), values, ct);

    /// <summary>Adds or updates participants from a batch of field maps.</summary>
    public Task SaveAsync(IEnumerable<Dictionary<string, object?>> participants,
        bool noHistory = false, CancellationToken ct = default)
        => e.PostVoidAsync("part/savefields",
            new QueryParams().Add("noHistory", noHistory), participants.ToArray(), ct);

    /// <summary>Deletes participants matching the given criteria.</summary>
    public Task DeleteAsync(string filter = "", Identifier? id = null, int contest = 0,
        CancellationToken ct = default)
    {
        var q = new QueryParams().Add("filter", filter).Add("contest", contest);
        if (id.HasValue) id.Value.ApplyTo(q);
        return e.GetVoidAsync("part/delete", q, ct);
    }

    /// <summary>Creates a new participant and returns their assigned bib and PID.</summary>
    public Task<ParticipantNewResponse> NewAsync(int bib = 0, int contest = 0,
        bool firstFree = false, CancellationToken ct = default)
        => e.GetAsync<ParticipantNewResponse>("part/new",
            new QueryParams()
                .Add("bib", bib).Add("contest", contest)
                .Add("firstfree", firstFree).Add("v2", true), ct);

    /// <summary>Returns entry fees charged to the participants with the given bibs.</summary>
    public Task<EntryFeeItem[]> GetEntryFeesAsync(IEnumerable<int> bibs,
        CancellationToken ct = default)
        => e.GetAsync<EntryFeeItem[]>("part/entryfee",
            new QueryParams().AddArray("bibs", bibs), ct);

    /// <summary>Creates blank participants for bib numbers in [from, to].</summary>
    public Task CreateBlanksAsync(int from, int to, int contest = 0,
        bool skipExcluded = false, CancellationToken ct = default)
        => e.GetVoidAsync("part/createblanks",
            new QueryParams()
                .Add("from", from).Add("to", to)
                .Add("contest", contest).Add("skipExcluded", skipExcluded), ct);

    /// <summary>Swaps the bibs of two participants.</summary>
    public Task SwapBibsAsync(int bib1, int bib2, CancellationToken ct = default)
        => e.GetVoidAsync("part/swapbibs",
            new QueryParams().Add("bib1", bib1).Add("bib2", bib2), ct);

    /// <summary>Reassigns bib numbers to all (or filtered) participants.</summary>
    public Task ResetBibsAsync(string sort = "", int firstBib = 1, bool ranges = false,
        string filter = "", bool noHistory = false, CancellationToken ct = default)
        => e.GetVoidAsync("part/resetbibs",
            new QueryParams()
                .Add("sort", sort).Add("firstBib", firstBib)
                .Add("ranges", ranges).Add("filter", filter).Add("noHistory", noHistory), ct);

    /// <summary>Applies a data-manipulation expression to all matching participants.</summary>
    public Task DataManipulationAsync(Dictionary<string, string> values,
        string filter = "", bool noHistory = false, CancellationToken ct = default)
        => e.PostVoidAsync("part/datamanipulation",
            new QueryParams().Add("filter", filter).Add("noHistory", noHistory), values, ct);

    /// <summary>Returns a currently unused bib number.</summary>
    public Task<int> FreeBibAsync(bool maxBibPlusOne = false, int contest = 0,
        int preferred = 0, CancellationToken ct = default)
        => e.GetAsync<int>("part/freebib",
            new QueryParams()
                .Add("maxBibPlus1", maxBibPlusOne)
                .Add("contest", contest).Add("preferred", preferred), ct);

    /// <summary>Returns frequently used club names matching a wildcard pattern.</summary>
    public Task<string[]> FrequentClubsAsync(string wildcard, int maxNumber = 10,
        CancellationToken ct = default)
        => e.GetAsync<string[]>("part/frequentclubs",
            new QueryParams().Add("wildcard", wildcard).Add("maxNumber", maxNumber), ct);

    /// <summary>Imports participants from a CSV/XLS/XLSX file.</summary>
    public Task<ImportResult> ImportAsync(byte[] file,
        bool addParticipants = true, bool updateParticipants = true,
        int colHandling = 0, int identityColumns = 0, string lang = "",
        CancellationToken ct = default)
        => e.PostAsync<ImportResult>("part/import",
            new QueryParams()
                .Add("addParticipants", addParticipants)
                .Add("updateParticipants", updateParticipants)
                .Add("colHandling", colHandling)
                .Add("identityColumns", identityColumns)
                .Add("lang", lang),
            file, ct);

    /// <summary>Clears bank/payment information for matching participants.</summary>
    public Task ClearBankInformationAsync(Identifier id, int contest = 0,
        string filter = "", CancellationToken ct = default)
        => e.GetVoidAsync("part/clearbankinformation",
            id.ApplyTo(new QueryParams()).Add("contest", contest).Add("filter", filter), ct);
}

// ── Rankings ──────────────────────────────────────────────────────────────────

public sealed class RankingsEndpoints(EventApiClient e)
{
    public Task<Ranking[]> GetAsync(CancellationToken ct = default)
        => e.GetAsync<Ranking[]>("rankings/get", ct: ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("rankings/delete", new QueryParams().Add("id", id), ct);

    public Task<int> SaveAsync(Ranking item, int oldId = 0, CancellationToken ct = default)
        => e.PostAsync<int>("rankings/save", new QueryParams().Add("oldID", oldId), item, ct);
}

// ── RawData ───────────────────────────────────────────────────────────────────

public sealed class RawDataEndpoints(EventApiClient e)
{
    public Task<byte[]> ExcelExportAsync(Identifier id, string lang = "",
        CancellationToken ct = default)
        => e.GetBytesAsync("rawdata/excelexport",
            id.ApplyTo(new QueryParams()).Add("lang", lang), ct);

    public Task SetInvalidAsync(int id, bool invalid, CancellationToken ct = default)
        => e.GetVoidAsync("rawdata/setinvalid",
            new QueryParams().Add("id", id).Add("invalid", invalid), ct);

    public Task DeleteByIdAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("rawdata/deleteid", new QueryParams().Add("id", id), ct);

    public Task DeleteAsync(Identifier id, string filter = "",
        RawDataFilter? rdFilter = null, CancellationToken ct = default)
    {
        var q = id.ApplyTo(new QueryParams()).Add("filter", filter);
        if (rdFilter is not null)
            q.Add("rdFilter", JsonSerializer.Serialize(rdFilter));
        return e.GetVoidAsync("rawdata/delete", q, ct);
    }

    public Task AddManualAsync(int bib, double time, string point, CancellationToken ct = default)
        => e.GetVoidAsync("rawdata/addmanual",
            new QueryParams().Add("bib", bib).Add("time", time).Add("timingpoint", point), ct);

    public Task<RawDataEntry[]> GetAsync(Identifier id, string filter = "",
        RawDataFilter? rdFilter = null, string[]? addFields = null,
        int firstRow = 0, int maxRows = 0, string sortBy = "",
        CancellationToken ct = default)
    {
        var q = id.ApplyTo(new QueryParams())
            .Add("filter", filter)
            .Add("firstRow", firstRow)
            .Add("maxRows", maxRows)
            .Add("sortBy", sortBy);
        if (rdFilter is not null)
            q.Add("rdFilter", JsonSerializer.Serialize(rdFilter));
        if (addFields?.Length > 0)
            q.AddArray("addFields", addFields);
        return e.GetAsync<RawDataEntry[]>("rawdata/get", q, ct);
    }

    public Task<int> CountAsync(Identifier id, string filter = "",
        CancellationToken ct = default)
        => e.GetAsync<int>("rawdata/count",
            id.ApplyTo(new QueryParams()).Add("filter", filter), ct);
}

// ── Results ───────────────────────────────────────────────────────────────────

public sealed class ResultsEndpoints(EventApiClient e)
{
    public Task<Result[]> GetAsync(string name = "", bool onlyFormulas = false,
        bool onlyNoFormulas = false, CancellationToken ct = default)
        => e.GetAsync<Result[]>("results/get",
            new QueryParams()
                .Add("name", name)
                .Add("onlyFormulas", onlyFormulas)
                .Add("onlyNoFormulas", onlyNoFormulas), ct);

    public Task<Result> GetOneAsync(int id, CancellationToken ct = default)
        => e.GetAsync<Result>("results/get", new QueryParams().Add("id", id), ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("results/delete", new QueryParams().Add("id", id), ct);

    public Task SaveAsync(IEnumerable<Result> items, CancellationToken ct = default)
        => e.PostVoidAsync("results/save", null, items.ToArray(), ct);
}

// ── Settings ──────────────────────────────────────────────────────────────────

public sealed class SettingsEndpoints(EventApiClient e)
{
    /// <summary>Returns settings by name. Pass no names to retrieve all settings.</summary>
    public Task<Dictionary<string, JsonElement>> GetAsync(CancellationToken ct = default,
        params string[] names)
    {
        var q = new QueryParams();
        if (names.Length == 1) q.Add("name", names[0]);
        else if (names.Length > 1) q.Add("names", string.Join(",", names));
        return e.GetAsync<Dictionary<string, JsonElement>>("settings/getsettings", q, ct);
    }

    public async Task<JsonElement?> GetValueAsync(string name, CancellationToken ct = default)
    {
        var map = await GetAsync(ct, name);
        return map.TryGetValue(name, out var v) ? v : null;
    }

    public Task SaveAsync(string name, object? value, int result = 0, int contest = 0,
        CancellationToken ct = default)
        => e.PostVoidAsync("settings/savesettings", null,
            new[] { new { Name = name, Value = value, Result = result, Contest = contest } }, ct);
}

// ── Splits ────────────────────────────────────────────────────────────────────

public sealed class SplitsEndpoints(EventApiClient e)
{
    public Task<Split[]> GetAsync(int contest = 0, CancellationToken ct = default)
        => e.GetAsync<Split[]>("splits/get", new QueryParams().Add("contest", contest), ct);

    public Task<Split> GetOneAsync(int id, CancellationToken ct = default)
        => e.GetAsync<Split>("splits/get", new QueryParams().Add("id", id), ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("splits/delete", new QueryParams().Add("id", id), ct);

    public Task<int> SaveAsync(Split item, int oldId = 0, CancellationToken ct = default)
        => e.PostAsync<int>("splits/save", new QueryParams().Add("oldID", oldId), item, ct);
}

// ── TeamScores ────────────────────────────────────────────────────────────────

public sealed class TeamScoresEndpoints(EventApiClient e)
{
    public Task<TeamScore[]> GetAsync(CancellationToken ct = default)
        => e.GetAsync<TeamScore[]>("teamscores/get", ct: ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("teamscores/delete", new QueryParams().Add("id", id), ct);

    public Task<int> SaveAsync(TeamScore item, int oldId = 0, CancellationToken ct = default)
        => e.PostAsync<int>("teamscores/save", new QueryParams().Add("oldID", oldId), item, ct);
}

// ── Times ─────────────────────────────────────────────────────────────────────

public sealed class TimesEndpoints(EventApiClient e)
{
    public Task<RaceTime[]> GetAsync(Identifier id, int result = 0,
        CancellationToken ct = default)
        => e.GetAsync<RaceTime[]>("times/get",
            id.ApplyTo(new QueryParams()).Add("result", result), ct);

    public Task<int> CountAsync(Identifier id, int contest = 0, int result = 0,
        string filter = "", CancellationToken ct = default)
        => e.GetAsync<int>("times/count",
            id.ApplyTo(new QueryParams())
              .Add("contest", contest).Add("result", result).Add("filter", filter), ct);

    /// <summary>
    /// Submits one or more passings for timing. Returns processing results per passing.
    /// </summary>
    public Task<TimesAddResponseItem[]> AddAsync(IEnumerable<Passing> passings,
        string[]? returnFields = null, int contestFilter = 0,
        bool ignoreBibToBibAssign = false, CancellationToken ct = default)
    {
        var q = new QueryParams()
            .Add("contestFilter", contestFilter)
            .Add("ignoreBibToBibAssign", ignoreBibToBibAssign);
        if (returnFields?.Length > 0) q.AddArray("returnFields", returnFields);
        return e.PostAsync<TimesAddResponseItem[]>("times/add", q, passings.ToArray(), ct);
    }

    public Task DeleteAsync(Identifier id, int contest = 0, int result = 0,
        string filter = "", string filterInfo = "", CancellationToken ct = default)
        => e.GetVoidAsync("times/delete",
            id.ApplyTo(new QueryParams())
              .Add("contest", contest).Add("result", result)
              .Add("filter", filter).Add("filterInfo", filterInfo), ct);

    public Task SwapAsync(Identifier from, Identifier to, CancellationToken ct = default)
    {
        var q = new QueryParams()
            .Add(from.Key + "1", from.Value.ToString())
            .Add(to.Key + "2", to.Value.ToString());
        return e.GetVoidAsync("times/swap", q, ct);
    }

    public Task<byte[]> ExcelExportAsync(Identifier id, int result = 0,
        string lang = "", CancellationToken ct = default)
        => e.GetBytesAsync("times/excelexport",
            id.ApplyTo(new QueryParams()).Add("result", result).Add("lang", lang), ct);
}

// ── TimingPoints ──────────────────────────────────────────────────────────────

public sealed class TimingPointsEndpoints(EventApiClient e)
{
    public Task<TimingPoint[]> GetAsync(CancellationToken ct = default)
        => e.GetAsync<TimingPoint[]>("timingpoints/get", ct: ct);

    public Task DeleteAsync(string name, CancellationToken ct = default)
        => e.GetVoidAsync("timingpoints/delete", new QueryParams().Add("name", name), ct);

    public Task SaveAsync(IEnumerable<TimingPoint> items, CancellationToken ct = default)
        => e.PostVoidAsync("timingpoints/save", null, items.ToArray(), ct);
}

// ── TimingPointRules ──────────────────────────────────────────────────────────

public sealed class TimingPointRulesEndpoints(EventApiClient e)
{
    public Task<TimingPointRule[]> GetAsync(CancellationToken ct = default)
        => e.GetAsync<TimingPointRule[]>("timingpointrules/get", ct: ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("timingpointrules/delete", new QueryParams().Add("id", id), ct);

    public Task<int> SaveAsync(TimingPointRule item, int oldId = 0, CancellationToken ct = default)
        => e.PostAsync<int>("timingpointrules/save",
            new QueryParams().Add("oldID", oldId), item, ct);
}

// ── Vouchers ──────────────────────────────────────────────────────────────────

public sealed class VouchersEndpoints(EventApiClient e)
{
    public Task<Voucher[]> GetAsync(CancellationToken ct = default)
        => e.GetAsync<Voucher[]>("vouchers/get", ct: ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("vouchers/delete", new QueryParams().Add("id", id), ct);

    public Task<int> SaveAsync(Voucher item, int oldId = 0, CancellationToken ct = default)
        => e.PostAsync<int>("vouchers/save", new QueryParams().Add("oldID", oldId), item, ct);

    public Task<string> ValidateAsync(string code, int contest = 0, int bib = 0,
        CancellationToken ct = default)
        => e.GetAsync<string>("vouchers/validate",
            new QueryParams().Add("code", code).Add("contest", contest).Add("bib", bib), ct);
}

// ── WebHooks ──────────────────────────────────────────────────────────────────

public sealed class WebHooksEndpoints(EventApiClient e)
{
    public Task<WebHook[]> GetAsync(CancellationToken ct = default)
        => e.GetAsync<WebHook[]>("webhooks/get", ct: ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("webhooks/delete", new QueryParams().Add("id", id), ct);

    public Task<int> SaveAsync(WebHook item, int oldId = 0, CancellationToken ct = default)
        => e.PostAsync<int>("webhooks/save", new QueryParams().Add("oldID", oldId), item, ct);
}

// ── SimpleApi ─────────────────────────────────────────────────────────────────

public sealed class SimpleApiEndpoints(EventApiClient e)
{
    public Task<SimpleApiItem[]> GetAsync(CancellationToken ct = default)
        => e.GetAsync<SimpleApiItem[]>("simpleapi/get", ct: ct);

    public Task SaveAsync(IEnumerable<SimpleApiItem> items, CancellationToken ct = default)
        => e.PostVoidAsync("simpleapi/save", null, items.ToArray(), ct);
}

// ── Chat ──────────────────────────────────────────────────────────────────────

public sealed class ChatEndpoints(EventApiClient e)
{
    public Task<ChatMessage[]> GetAsync(int afterId = 0, CancellationToken ct = default)
        => e.GetAsync<ChatMessage[]>("chat/get", new QueryParams().Add("afterID", afterId), ct);

    public Task SendAsync(string message, CancellationToken ct = default)
        => e.PostVoidAsync("chat/send", null, new { message }, ct);
}

// ── Statistics ────────────────────────────────────────────────────────────────

public sealed class StatisticsEndpoints(EventApiClient e)
{
    public Task<ContestStatistics[]> GetAsync(CancellationToken ct = default)
        => e.GetAsync<ContestStatistics[]>("statistics/get", ct: ct);
}

// ── Forwarding ────────────────────────────────────────────────────────────────

public sealed class ForwardingEndpoints(EventApiClient e)
{
    public Task<ForwardingInfo> GetInfoAsync(CancellationToken ct = default)
        => e.GetAsync<ForwardingInfo>("forwarding/info", ct: ct);

    public Task StartAsync(CancellationToken ct = default)
        => e.GetVoidAsync("forwarding/start", ct: ct);

    public Task StopAsync(CancellationToken ct = default)
        => e.GetVoidAsync("forwarding/stop", ct: ct);
}

// ── Registrations ─────────────────────────────────────────────────────────────

public sealed class RegistrationsEndpoints(EventApiClient e)
{
    public Task<JsonElement> ProcessAsync(RegistrationRequest request,
        CancellationToken ct = default)
        => e.PostAsync<JsonElement>("registrations/process", null, request, ct);
}

// ── UserDefinedFields ─────────────────────────────────────────────────────────

public sealed class UserDefinedFieldsEndpoints(EventApiClient e)
{
    public Task<UserDefinedField[]> GetAsync(CancellationToken ct = default)
        => e.GetAsync<UserDefinedField[]>("udfields/get", ct: ct);

    public Task SaveAsync(IEnumerable<UserDefinedField> items, CancellationToken ct = default)
        => e.PostVoidAsync("udfields/save", null, items.ToArray(), ct);
}

// ── OverwriteValues ───────────────────────────────────────────────────────────

public sealed class OverwriteValuesEndpoints(EventApiClient e)
{
    public Task<OverwriteValue[]> GetAsync(Identifier id, int resultId = 0,
        CancellationToken ct = default)
        => e.GetAsync<OverwriteValue[]>("overwritevalues/get",
            id.ApplyTo(new QueryParams()).Add("resultID", resultId), ct);

    public Task SaveAsync(OverwriteValue item, CancellationToken ct = default)
        => e.PostVoidAsync("overwritevalues/save", null, item, ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("overwritevalues/delete", new QueryParams().Add("id", id), ct);
}

// ── File (event file management) ─────────────────────────────────────────────

public sealed class FileEndpoints(EventApiClient e)
{
    public Task<byte[]> DownloadAsync(CancellationToken ct = default)
        => e.GetBytesAsync("file/download", ct: ct);

    public Task UploadAsync(byte[] file, CancellationToken ct = default)
        => e.PostVoidAsync("file/upload", null, file, ct);
}