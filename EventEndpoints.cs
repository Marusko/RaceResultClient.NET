using System.Text;
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

    public Task DeleteAsync(int id, int contest = 0, int set = 0, CancellationToken ct = default)
        => e.GetVoidAsync("agegroups/delete",
            new QueryParams().Add("id", id).Add("contest", contest).Add("set", set), ct);

    /// <summary>Saves age groups and returns the assigned IDs.</summary>
    public Task<int[]> SaveAsync(IEnumerable<AgeGroup> items, CancellationToken ct = default)
        => e.PostAsync<int[]>("agegroups/save", null, items.ToArray(), ct);

    /// <summary>Generates new age groups from templates.</summary>
    public Task<AgeGroup[]> GenerateAsync(string mode, int contest, int set, bool ageBase,
        string date, string lang = "", CancellationToken ct = default)
        => e.GetAsync<AgeGroup[]>("agegroups/generate",
            new QueryParams()
                .Add("mode", mode).Add("contest", contest).Add("set", set)
                .Add("ageBase", ageBase).Add("date", date).Add("lang", lang), ct);

    /// <summary>Reassigns age groups to participants.</summary>
    public Task ReassignAsync(int contest, Identifier id, int set = 0, bool addOnly = false,
        CancellationToken ct = default)
        => e.GetVoidAsync("agegroups/reassign",
            id.ApplyTo(new QueryParams()).Add("contest", contest).Add("set", set).Add("addOnly", addOnly), ct);
}

// ── BibRanges ─────────────────────────────────────────────────────────────────

public sealed class BibRangesEndpoints(EventApiClient e)
{
    public Task<byte[]> GetPdfAsync(CancellationToken ct = default)
        => e.GetBytesAsync("bibranges/pdf", ct: ct);

    public Task<BibRange[]> GetAsync(int contest = 0, int id = 0, CancellationToken ct = default)
        => e.GetAsync<BibRange[]>("bibranges/get",
            new QueryParams().Add("contest", contest).Add("id", id), ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("bibranges/delete", new QueryParams().Add("id", id), ct);

    public Task<int[]> SaveAsync(IEnumerable<BibRange> items, CancellationToken ct = default)
        => e.PostAsync<int[]>("bibranges/save", null, items.ToArray(), ct);
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

// ── CustomFields (server endpoint group: "fields") ────────────────────────────

public sealed class CustomFieldsEndpoints(EventApiClient e)
{
    public Task<CustomField[]> GetAsync(CancellationToken ct = default)
        => e.GetAsync<CustomField[]>("fields/get", ct: ct);

    public Task<CustomField> GetOneAsync(int id, CancellationToken ct = default)
        => e.GetAsync<CustomField>("fields/get", new QueryParams().Add("id", id), ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("fields/delete", new QueryParams().Add("id", id), ct);

    public Task<int[]> SaveAsync(IEnumerable<CustomField> items, CancellationToken ct = default)
        => e.PostAsync<int[]>("fields/save", null, items.ToArray(), ct);
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

    /// <summary>Creates a min/max/sum/count/avg transformation (pivot) of participant data.</summary>
    public Task<JsonElement[][]> TransformationAsync(
        string colField, IEnumerable<string> rowFields, string filter, string field,
        int mode, bool sortByValue, CancellationToken ct = default)
    {
        var q = new QueryParams()
            .Add("colField", colField)
            .AddArray("rowFields", rowFields)
            .Add("filter", filter)
            .Add("field", field)
            .AddAlways("mode", mode)
            .Add("sortByValue", sortByValue);
        return e.GetAsync<JsonElement[][]>("data/transformation", q, ct);
    }
}

// ── EntryFees ─────────────────────────────────────────────────────────────────

public sealed class EntryFeesEndpoints(EventApiClient e)
{
    public Task<byte[]> GetPdfAsync(CancellationToken ct = default)
        => e.GetBytesAsync("entryfees/pdf", ct: ct);

    public Task<EntryFee[]> GetAsync(int contest = 0, int id = 0, CancellationToken ct = default)
        => e.GetAsync<EntryFee[]>("entryfees/get",
            new QueryParams().Add("contest", contest).Add("id", id), ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("entryfees/delete", new QueryParams().Add("id", id), ct);

    public Task<int[]> SaveAsync(IEnumerable<EntryFee> items, CancellationToken ct = default)
        => e.PostAsync<int[]>("entryfees/save", null, items.ToArray(), ct);
}

// ── Exporters ─────────────────────────────────────────────────────────────────

public sealed class ExportersEndpoints(EventApiClient e)
{
    public Task<Exporter[]> GetAsync(CancellationToken ct = default)
        => e.GetAsync<Exporter[]>("exporters/get", ct: ct);

    public Task<Exporter> GetOneAsync(int id, CancellationToken ct = default)
        => e.GetAsync<Exporter>("exporters/get", new QueryParams().Add("id", id), ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("exporters/delete", new QueryParams().Add("id", id), ct);

    public Task<int> SaveAsync(Exporter item, CancellationToken ct = default)
        => e.PostAsync<int>("exporters/save", null, item, ct);

    public Task StartAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("exporters/start", new QueryParams().Add("id", id), ct);

    public Task StopAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("exporters/stop", new QueryParams().Add("id", id), ct);
}

// ── History ───────────────────────────────────────────────────────────────────

public sealed class HistoryEndpoints(EventApiClient e)
{
    public Task<HistoryEntry[]> GetAsync(Identifier identifier, CancellationToken ct = default)
        => e.GetAsync<HistoryEntry[]>("history/get", identifier.ApplyTo(new QueryParams()), ct);

    /// <summary>Extended (local) variant that posts a structured <see cref="HistoryFilter"/>.</summary>
    public Task<HistoryEntry[]> GetAsync(Identifier identifier, HistoryFilter filter,
        CancellationToken ct = default)
        => e.PostAsync<HistoryEntry[]>("history/get", identifier.ApplyTo(new QueryParams()), filter, ct);

    public Task<byte[]> ExcelExportAsync(Identifier identifier, string lang = "",
        CancellationToken ct = default)
        => e.GetBytesAsync("history/excelexport",
            identifier.ApplyTo(new QueryParams()).Add("lang", lang), ct);

    public Task<int> CountAsync(Identifier identifier, int contest = 0, string field = "",
        DateTime? dateFrom = null, DateTime? dateTo = null, string filter = "",
        CancellationToken ct = default)
        => e.GetAsync<int>("history/count", BuildFilter(identifier, contest, field, dateFrom, dateTo, filter), ct);

    public Task DeleteAsync(Identifier identifier, int contest = 0, string field = "",
        DateTime? dateFrom = null, DateTime? dateTo = null, string filter = "",
        CancellationToken ct = default)
        => e.GetVoidAsync("history/delete", BuildFilter(identifier, contest, field, dateFrom, dateTo, filter), ct);

    // Note: the server query key for the lower date bound is "dateForm" (matches go-webapi).
    private static QueryParams BuildFilter(Identifier identifier, int contest, string field,
        DateTime? dateFrom, DateTime? dateTo, string filter)
    {
        var q = identifier.ApplyTo(new QueryParams())
            .Add("contest", contest).Add("field", field).Add("filter", filter);
        if (dateFrom.HasValue) q.Add("dateForm", dateFrom.Value);
        if (dateTo.HasValue) q.Add("dateTo", dateTo.Value);
        return q;
    }
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

    /// <summary>Returns entry fees charged to the participants with the given bibs, keyed by bib.</summary>
    public Task<Dictionary<string, EntryFeeItem[]>> GetEntryFeesAsync(IEnumerable<int> bibs,
        CancellationToken ct = default)
        => e.GetAsync<Dictionary<string, EntryFeeItem[]>>("part/entryfee",
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

    /// <summary>Imports an entire SES file into the current event file.</summary>
    public Task<ImportResult> ImportSesAsync(byte[] file, string filter, string identity,
        bool addParticipants, bool updateParticipants, int contestFrom, int contestTo,
        int timesFrom, int timesTo, bool importRawData, CancellationToken ct = default)
        => e.PostAsync<ImportResult>("part/importses",
            new QueryParams()
                .Add("filter", filter).Add("identity", identity)
                .Add("addParticipants", addParticipants).Add("updateParticipants", updateParticipants)
                .Add("contestFrom", contestFrom).Add("contestTo", contestTo)
                .Add("timesFrom", timesFrom).Add("timesTo", timesTo)
                .Add("importRawData", importRawData),
            file, ct);

    /// <summary>Clears bank/payment information for matching participants.</summary>
    public Task ClearBankInformationAsync(Identifier id, int contest = 0,
        string filter = "", CancellationToken ct = default)
        => e.GetVoidAsync("part/clearbankinformation",
            id.ApplyTo(new QueryParams()).Add("contest", contest).Add("filter", filter), ct);
}

// ── Rankings (server endpoint group: "ranks") ─────────────────────────────────

public sealed class RankingsEndpoints(EventApiClient e)
{
    public Task<Ranking[]> GetAsync(CancellationToken ct = default)
        => e.GetAsync<Ranking[]>("ranks/get", ct: ct);

    public Task<Ranking> GetOneAsync(int id, CancellationToken ct = default)
        => e.GetAsync<Ranking>("ranks/get", new QueryParams().Add("id", id), ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("ranks/delete", new QueryParams().Add("id", id), ct);

    public Task<int[]> SaveAsync(IEnumerable<Ranking> items, CancellationToken ct = default)
        => e.PostAsync<int[]>("ranks/save", null, items.ToArray(), ct);
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

    public Task SetInvalidBatchAsync(string filter, RawDataFilter rdFilter, bool invalid,
        CancellationToken ct = default)
        => e.GetVoidAsync("rawdata/setinvalidbatch",
            new QueryParams()
                .Add("filter", filter)
                .Add("rdFilter", JsonSerializer.Serialize(rdFilter, JsonOptions.Default))
                .Add("invalid", invalid), ct);

    public Task DeleteByIdAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("rawdata/deleteid", new QueryParams().Add("id", id), ct);

    public Task DeleteAsync(Identifier id, string filter = "",
        RawDataFilter? rdFilter = null, CancellationToken ct = default)
    {
        var q = id.ApplyTo(new QueryParams()).Add("filter", filter);
        if (rdFilter is not null)
            q.Add("rdFilter", JsonSerializer.Serialize(rdFilter, JsonOptions.Default));
        return e.GetVoidAsync("rawdata/delete", q, ct);
    }

    /// <summary>Adds a manual raw data entry for the given participant.</summary>
    public Task AddManualAsync(string timingPoint, Identifier id, decimal time,
        bool addT0 = false, CancellationToken ct = default)
        => e.GetVoidAsync("rawdata/addmanual",
            id.ApplyTo(new QueryParams())
              .Add("timingPoint", timingPoint).AddAlways("time", time).Add("addT0", addT0), ct);

    public Task<RawDataWithAdditionalFields[]> GetAsync(Identifier id, string filter = "",
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
            q.Add("rdFilter", JsonSerializer.Serialize(rdFilter, JsonOptions.Default));
        if (addFields?.Length > 0)
            q.AddArray("addFields", addFields);
        return e.GetAsync<RawDataWithAdditionalFields[]>("rawdata/get", q, ct);
    }

    /// <summary>Returns raw data entries as a jagged array of selected fields.</summary>
    public Task<JsonElement[][]> ExportAsync(Identifier id, string filter,
        RawDataFilter? rdFilter, string[] fields,
        int firstRow = 0, int maxRows = 0, string sortBy = "",
        CancellationToken ct = default)
    {
        var q = id.ApplyTo(new QueryParams())
            .Add("filter", filter)
            .AddArray("fields", fields)
            .Add("firstRow", firstRow)
            .Add("maxRows", maxRows)
            .Add("sortBy", sortBy);
        if (rdFilter is not null)
            q.Add("rdFilter", JsonSerializer.Serialize(rdFilter, JsonOptions.Default));
        return e.GetAsync<JsonElement[][]>("rawdata/export", q, ct);
    }

    public Task<int> CountAsync(Identifier id, string filter = "",
        RawDataFilter? rdFilter = null, CancellationToken ct = default)
    {
        var q = id.ApplyTo(new QueryParams()).Add("filter", filter);
        if (rdFilter is not null)
            q.Add("rdFilter", JsonSerializer.Serialize(rdFilter, JsonOptions.Default));
        return e.GetAsync<int>("rawdata/count", q, ct);
    }

    /// <summary>Returns the list of unique values existing in the raw data.</summary>
    public Task<RawDataDistinctValues> DistinctValuesAsync(CancellationToken ct = default)
        => e.GetAsync<RawDataDistinctValues>("rawdata/distinctvalues", ct: ct);

    /// <summary>Copies raw data from one participant to another.</summary>
    public Task CopyAsync(Identifier from, Identifier to, CancellationToken ct = default)
        => e.GetVoidAsync("rawdata/copy",
            new QueryParams()
                .AddAlways(from.Key + "From", from.Value)
                .AddAlways(from.Key + "To", to.Value), ct);

    /// <summary>Swaps raw data between two participants.</summary>
    public Task SwapAsync(Identifier from, Identifier to, CancellationToken ct = default)
        => e.GetVoidAsync("rawdata/swap",
            new QueryParams()
                .AddAlways(from.Key + "1", from.Value)
                .AddAlways(to.Key + "2", to.Value), ct);
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
    /// <summary>Returns settings by name. Pass no names to retrieve an empty map.</summary>
    public Task<Dictionary<string, JsonElement>> GetAsync(CancellationToken ct = default,
        params string[] names)
    {
        // Mirrors go-webapi: no names means no request at all.
        if (names.Length == 0)
            return Task.FromResult(new Dictionary<string, JsonElement>());

        var q = new QueryParams();
        if (names.Length == 1) q.Add("name", names[0]);
        else q.Add("names", string.Join(",", names));
        return e.GetAsync<Dictionary<string, JsonElement>>("settings/getsettings", q, ct);
    }

    public async Task<JsonElement?> GetValueAsync(string name, CancellationToken ct = default)
    {
        var map = await GetAsync(ct, name);
        return map.TryGetValue(name, out var v) ? v : null;
    }

    /// <summary>Saves several settings in one call.</summary>
    public Task SaveAsync(IEnumerable<Setting> settings, CancellationToken ct = default)
        => e.PostVoidAsync("settings/savesettings", null, settings.ToArray(), ct);

    public Task SaveAsync(string name, object? value, int result = 0, int contest = 0,
        CancellationToken ct = default)
        => SaveAsync([new Setting { Name = name, Value = value, Result = result, Contest = contest }], ct);

    /// <summary>Saves a single setting value.</summary>
    public Task SaveValueAsync(string name, object? value, CancellationToken ct = default)
        => SaveAsync(name, value, ct: ct);

    /// <summary>Deletes a single setting (optionally linked to a contest/result).</summary>
    public Task DeleteAsync(string name, int contest = 0, int result = 0,
        CancellationToken ct = default)
        => e.GetVoidAsync("settings/delete",
            new QueryParams().Add("name", name).Add("contest", contest).Add("result", result), ct);

    /// <summary>Returns the names of settings matching the given prefix.</summary>
    public Task<string[]> NamesByPrefixAsync(string prefix, CancellationToken ct = default)
        => e.GetAsync<string[]>("settings/settingnamesbyprefix",
            new QueryParams().Add("prefix", prefix), ct);
}

// ── Splits ────────────────────────────────────────────────────────────────────

public sealed class SplitsEndpoints(EventApiClient e)
{
    public Task<Split[]> GetAsync(int contest = 0, CancellationToken ct = default)
        => e.GetAsync<Split[]>("splits/get", new QueryParams().Add("contest", contest), ct);

    public Task<Split> GetOneAsync(int id, CancellationToken ct = default)
        => e.GetAsync<Split>("splits/get", new QueryParams().Add("id", id), ct);

    public Task DeleteAsync(IEnumerable<int> ids, CancellationToken ct = default)
        => e.GetVoidAsync("splits/delete", new QueryParams().AddArray("id", ids), ct);

    public Task<int[]> SaveAsync(IEnumerable<Split> items, CancellationToken ct = default)
        => e.PostAsync<int[]>("splits/save", null, items.ToArray(), ct);
}

// ── TeamScores ────────────────────────────────────────────────────────────────

public sealed class TeamScoresEndpoints(EventApiClient e)
{
    public Task<TeamScore[]> GetAsync(CancellationToken ct = default)
        => e.GetAsync<TeamScore[]>("teamscores/get", ct: ct);

    public Task<TeamScore> GetOneAsync(int id, CancellationToken ct = default)
        => e.GetAsync<TeamScore>("teamscores/get", new QueryParams().Add("id", id), ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("teamscores/delete", new QueryParams().Add("id", id), ct);

    public Task SaveAsync(TeamScore item, CancellationToken ct = default)
        => e.PostVoidAsync("teamscores/save", null, item, ct);
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
        => e.GetVoidAsync("times/swap",
            new QueryParams()
                .AddAlways(from.Key + "1", from.Value)
                .AddAlways(to.Key + "2", to.Value), ct);

    public Task<byte[]> ExcelExportAsync(Identifier id, int result = 0,
        string lang = "", CancellationToken ct = default)
        => e.GetBytesAsync("times/excelexport",
            id.ApplyTo(new QueryParams()).Add("result", result).Add("lang", lang), ct);

    /// <summary>Creates single start times.</summary>
    public Task SingleStartAsync(int result, int contest, decimal firstTime, decimal interval,
        string sort = "", string filter = "", bool noHistory = false, CancellationToken ct = default)
        => e.GetVoidAsync("times/singlestart",
            new QueryParams()
                .Add("result", result).Add("contest", contest)
                .AddAlways("firstTime", firstTime).AddAlways("interval", interval)
                .Add("sort", sort).Add("filter", filter).Add("noHistory", noHistory), ct);

    /// <summary>Creates random times.</summary>
    public Task RandomTimesAsync(int result, int contest, decimal minTime, decimal maxTime,
        int offsetResult = 0, string filter = "", bool noHistory = false, CancellationToken ct = default)
        => e.GetVoidAsync("times/randomtimes",
            new QueryParams()
                .Add("result", result).Add("contest", contest)
                .AddAlways("minTime", minTime).AddAlways("maxTime", maxTime)
                .Add("offsetResult", offsetResult).Add("filter", filter).Add("noHistory", noHistory), ct);

    /// <summary>Copies times from one participant to another.</summary>
    public Task CopyAsync(Identifier from, Identifier to, bool overwriteExisting = false,
        CancellationToken ct = default)
        => e.GetVoidAsync("times/copy",
            new QueryParams()
                .AddAlways(from.Key + "From", from.Value)
                .AddAlways(from.Key + "To", to.Value)
                .Add("overwriteExisting", overwriteExisting), ct);

    /// <summary>Interpolates missing times.</summary>
    public Task InterpolateAsync(int destId, int helperId, int contest, int helpers,
        CancellationToken ct = default)
        => e.GetVoidAsync("times/interpolate",
            new QueryParams()
                .Add("destID", destId).Add("helperID", helperId)
                .Add("contest", contest).Add("helpers", helpers), ct);
}

// ── TimingPoints ──────────────────────────────────────────────────────────────

public sealed class TimingPointsEndpoints(EventApiClient e)
{
    public Task<TimingPoint[]> GetAsync(CancellationToken ct = default)
        => e.GetAsync<TimingPoint[]>("timingpoints/get", ct: ct);

    public Task<TimingPoint> GetOneAsync(string name, CancellationToken ct = default)
        => e.GetAsync<TimingPoint>("timingpoints/get", new QueryParams().Add("name", name), ct);

    public Task DeleteAsync(string name, CancellationToken ct = default)
        => e.GetVoidAsync("timingpoints/delete", new QueryParams().Add("name", name), ct);

    public Task SaveAsync(TimingPoint item, string oldName = "", CancellationToken ct = default)
        => e.PostVoidAsync("timingpoints/save", new QueryParams().Add("oldName", oldName), item, ct);
}

// ── TimingPointRules ──────────────────────────────────────────────────────────

public sealed class TimingPointRulesEndpoints(EventApiClient e)
{
    public Task<TimingPointRule[]> GetAsync(CancellationToken ct = default)
        => e.GetAsync<TimingPointRule[]>("timingpointrules/get", ct: ct);

    public Task<TimingPointRule> GetOneAsync(int id, CancellationToken ct = default)
        => e.GetAsync<TimingPointRule>("timingpointrules/get", new QueryParams().Add("id", id), ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("timingpointrules/delete", new QueryParams().Add("id", id), ct);

    public Task<int[]> SaveAsync(IEnumerable<TimingPointRule> items, CancellationToken ct = default)
        => e.PostAsync<int[]>("timingpointrules/save", null, items.ToArray(), ct);
}

// ── Vouchers ──────────────────────────────────────────────────────────────────

public sealed class VouchersEndpoints(EventApiClient e)
{
    public Task<Voucher[]> GetAsync(string code = "", CancellationToken ct = default)
        => e.GetAsync<Voucher[]>("vouchers/get", new QueryParams().Add("code", code), ct);

    public Task DeleteAsync(IEnumerable<int> ids, CancellationToken ct = default)
        => e.PostVoidAsync("vouchers/delete", null,
            Encoding.UTF8.GetBytes(string.Join(";", ids)), ct);

    public Task<int[]> SaveAsync(IEnumerable<Voucher> items, CancellationToken ct = default)
        => e.PostAsync<int[]>("vouchers/save", null, items.ToArray(), ct);

    /// <summary>Validates a voucher code (local convenience endpoint).</summary>
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

    public Task<int[]> SaveAsync(IEnumerable<WebHook> items, CancellationToken ct = default)
        => e.PostAsync<int[]>("webhooks/save", null, items.ToArray(), ct);
}

// ── SimpleApi ─────────────────────────────────────────────────────────────────

public sealed class SimpleApiEndpoints(EventApiClient e)
{
    public Task<SimpleApiItem[]> GetAsync(CancellationToken ct = default)
        => e.GetAsync<SimpleApiItem[]>("simpleapi/get", ct: ct);

    public Task DeleteAsync(string key, CancellationToken ct = default)
        => e.GetVoidAsync("simpleapi/delete", new QueryParams().Add("key", key), ct);

    public Task SaveAsync(IEnumerable<SimpleApiItem> items, CancellationToken ct = default)
        => e.PostVoidAsync("simpleapi/save", null, items.ToArray(), ct);

    public Task SaveAllAsync(IEnumerable<SimpleApiItem> items, CancellationToken ct = default)
        => e.PostVoidAsync("simpleapi/saveall", null, items.ToArray(), ct);
}

// ── Chat ──────────────────────────────────────────────────────────────────────

public sealed class ChatEndpoints(EventApiClient e)
{
    public Task<ChatMessage[]> GetMessagesAsync(int minId = 0, CancellationToken ct = default)
        => e.GetAsync<ChatMessage[]>("chat/getmessages", new QueryParams().Add("minID", minId), ct);

    /// <summary>Registers a user and returns the list of all chat users.</summary>
    public Task<string[]> GetUsersAsync(string username, CancellationToken ct = default)
        => e.GetAsync<string[]>("chat/getusers", new QueryParams().Add("username", username), ct);

    public Task PostMessageAsync(string username, string message, CancellationToken ct = default)
        => e.PostVoidAsync("chat/postmessage",
            new QueryParams().Add("username", username), Encoding.UTF8.GetBytes(message), ct);
}

// ── Statistics ────────────────────────────────────────────────────────────────

public sealed class StatisticsEndpoints(EventApiClient e)
{
    public Task<string[]> NamesAsync(CancellationToken ct = default)
        => e.GetAsync<string[]>("statistics/names", ct: ct);

    public Task<Statistics> GetAsync(string name, CancellationToken ct = default)
        => e.GetAsync<Statistics>("statistics/get", new QueryParams().Add("name", name), ct);

    public Task SaveAsync(Statistics item, CancellationToken ct = default)
        => e.PostVoidAsync("statistics/save", null, item, ct);

    public Task DeleteAsync(string name, CancellationToken ct = default)
        => e.GetVoidAsync("statistics/delete", new QueryParams().Add("name", name), ct);

    public Task CopyAsync(string name, string newName, CancellationToken ct = default)
        => e.GetVoidAsync("statistics/copy",
            new QueryParams().Add("name", name).Add("newName", newName), ct);

    public Task RenameAsync(string name, string newName, CancellationToken ct = default)
        => e.GetVoidAsync("statistics/rename",
            new QueryParams().Add("name", name).Add("newName", newName), ct);

    public Task NewAsync(string name, CancellationToken ct = default)
        => e.GetVoidAsync("statistics/new", new QueryParams().Add("name", name), ct);

    public Task<byte[]> CreateAsync(string name, string format, IEnumerable<int> contests,
        CancellationToken ct = default)
        => e.GetBytesAsync("statistics/create",
            new QueryParams().Add("name", name).Add("format", format).AddArray("contest", contests), ct);

    /// <summary>Computes an arbitrary statistic (pivot table).</summary>
    public Task<JsonElement[][]> ComputeAsync(string row, string col, string filter, string field,
        StatisticAggregation aggregation, CancellationToken ct = default)
        => e.GetAsync<JsonElement[][]>("statistics/statistics",
            new QueryParams()
                .Add("row", row).Add("col", col).Add("filter", filter).Add("field", field)
                .AddAlways("aggregation", (int)aggregation), ct);
}

// ── Forwarding ────────────────────────────────────────────────────────────────

public sealed class ForwardingEndpoints(EventApiClient e)
{
    public Task<bool> ActiveAsync(CancellationToken ct = default)
        => e.GetAsync<bool>("forwarding/active", ct: ct);

    public Task<ForwardingInfo> GetInfoAsync(CancellationToken ct = default)
        => e.GetAsync<ForwardingInfo>("forwarding/info", ct: ct);

    public Task StartAsync(string hostname, string eventId, string authToken,
        CancellationToken ct = default)
        => e.GetVoidAsync("forwarding/start",
            new QueryParams().Add("hostname", hostname).Add("eventid", eventId).Add("authToken", authToken), ct);

    public Task RestartAsync(CancellationToken ct = default)
        => e.GetVoidAsync("forwarding/restart", ct: ct);

    public Task StopAsync(CancellationToken ct = default)
        => e.GetVoidAsync("forwarding/stop", ct: ct);
}

// ── Registrations ─────────────────────────────────────────────────────────────

public sealed class RegistrationsEndpoints(EventApiClient e)
{
    /// <summary>Processes an online registration submission (local convenience endpoint).</summary>
    public Task<JsonElement> ProcessAsync(RegistrationRequest request,
        CancellationToken ct = default)
        => e.PostAsync<JsonElement>("registrations/process", null, request, ct);

    public Task<string[]> NamesAsync(CancellationToken ct = default)
        => e.GetAsync<string[]>("registrations/names", ct: ct);

    public Task<Registration> GetAsync(string name, CancellationToken ct = default)
        => e.GetAsync<Registration>("registrations/get", new QueryParams().Add("name", name), ct);

    public Task SaveAsync(Registration item, CancellationToken ct = default)
        => e.PostVoidAsync("registrations/save", null, item, ct);

    public Task DeleteAsync(string name, CancellationToken ct = default)
        => e.GetVoidAsync("registrations/delete", new QueryParams().Add("name", name), ct);

    public Task CopyAsync(string name, string newName, CancellationToken ct = default)
        => e.GetVoidAsync("registrations/copy",
            new QueryParams().Add("name", name).Add("newName", newName), ct);

    public Task RenameAsync(string name, string newName, CancellationToken ct = default)
        => e.GetVoidAsync("registrations/rename",
            new QueryParams().Add("name", name).Add("newName", newName), ct);

    public Task NewAsync(string name, bool group = false, CancellationToken ct = default)
        => e.GetVoidAsync("registrations/new",
            new QueryParams().Add("name", name).Add("group", group), ct);
}

// ── UserDefinedFields ─────────────────────────────────────────────────────────

public sealed class UserDefinedFieldsEndpoints(EventApiClient e)
{
    public Task<UserDefinedField[]> GetAsync(CancellationToken ct = default)
        => e.GetAsync<UserDefinedField[]>("userdefinedfields/get", ct: ct);

    /// <summary>Overwrites all user-defined fields.</summary>
    public Task SetAsync(IEnumerable<UserDefinedField> items, CancellationToken ct = default)
        => e.PostVoidAsync("userdefinedfields/set", null, items.ToArray(), ct);
}

// ── OverwriteValues ───────────────────────────────────────────────────────────

public sealed class OverwriteValuesEndpoints(EventApiClient e)
{
    /// <summary>Returns overwrite values for the participant (local convenience endpoint).</summary>
    public Task<OverwriteValue[]> GetAsync(Identifier id, int resultId = 0,
        CancellationToken ct = default)
        => e.GetAsync<OverwriteValue[]>("overwritevalues/get",
            id.ApplyTo(new QueryParams()).Add("resultID", resultId), ct);

    public Task<int> CountAsync(Identifier id, int result = 0, int contest = 0, string filter = "",
        CancellationToken ct = default)
        => e.GetAsync<int>("overwritevalues/count",
            id.ApplyTo(new QueryParams()).Add("result", result).Add("contest", contest).Add("filter", filter), ct);

    public Task SaveAsync(Identifier id, int result, decimal value, CancellationToken ct = default)
        => e.GetVoidAsync("overwritevalues/save",
            id.ApplyTo(new QueryParams()).Add("result", result).AddAlways("value", value), ct);

    public Task DeleteAsync(Identifier id, int result = 0, int contest = 0, string filter = "",
        CancellationToken ct = default)
        => e.GetVoidAsync("overwritevalues/delete",
            id.ApplyTo(new QueryParams()).Add("result", result).Add("contest", contest).Add("filter", filter), ct);
}

// ── File (event file management) ─────────────────────────────────────────────

public sealed class FileEndpoints(EventApiClient e)
{
    /// <summary>Downloads a copy of the entire event file.</summary>
    public Task<byte[]> GetFileAsync(CancellationToken ct = default)
        => e.GetBytesAsync("file/getfile", ct: ct);

    /// <summary>Downloads the event file (local convenience endpoint).</summary>
    public Task<byte[]> DownloadAsync(CancellationToken ct = default)
        => e.GetBytesAsync("file/download", ct: ct);

    /// <summary>Uploads the event file (local convenience endpoint).</summary>
    public Task UploadAsync(byte[] file, CancellationToken ct = default)
        => e.PostVoidAsync("file/upload", null, file, ct);

    /// <summary>Activates participants and returns the number of activated records.</summary>
    public Task<int> ActivateAsync(int bib = 0, string filter = "", int maxActivations = 0,
        CancellationToken ct = default)
        => e.GetAsync<int>("file/activate",
            new QueryParams().Add("bib", bib).Add("filter", filter).Add("maxActivations", maxActivations), ct);

    /// <summary>Returns the number of participants that are not activated.</summary>
    public Task<int> NotActivatedAsync(string filter = "", CancellationToken ct = default)
        => e.GetAsync<int>("file/notactivated", new QueryParams().Add("filter", filter), ct);

    /// <summary>Returns the version of the Sports Event Server.</summary>
    public Task<Version> SesVersionAsync(CancellationToken ct = default)
        => e.GetAsync<Version>("file/sesversion", ct: ct);

    /// <summary>Parses/checks an expression.</summary>
    public Task<string> CheckExpressionAsync(string expressions, bool returnTree = false,
        CancellationToken ct = default)
        => e.GetAsync<string>("file/checkexpression",
            new QueryParams().Add("expressions", expressions).Add("returnTree", returnTree), ct);

    /// <summary>Returns the modjobid of the file.</summary>
    public Task<int> ModJobIdAsync(CancellationToken ct = default)
        => e.GetAsync<int>("file/modjobid", ct: ct);

    /// <summary>Returns the normal modjobid and the settings modjobid of the file.</summary>
    public async Task<(int ModJobId, int SettingsModJobId)> ModJobIdsAsync(CancellationToken ct = default)
    {
        var s = await e.GetAsync<string>("file/modjobids", ct: ct);
        var arr = s.Split(';');
        if (arr.Length != 2) throw new ApiException("response invalid", 0);
        int.TryParse(arr[0], out var a);
        int.TryParse(arr[1], out var b);
        return (a, b);
    }

    /// <summary>Returns the filename of the event file.</summary>
    public Task<string> FilenameAsync(CancellationToken ct = default)
        => e.GetAsync<string>("file/filename", ct: ct);

    /// <summary>Returns the user ID of the event owner (online server only).</summary>
    public Task<int> OwnerAsync(CancellationToken ct = default)
        => e.GetAsync<int>("file/owner", ct: ct);

    /// <summary>Returns true if the current user owns the event (online server only).</summary>
    public Task<bool> IsOwnerAsync(CancellationToken ct = default)
        => e.GetAsync<bool>("file/isowner", ct: ct);

    /// <summary>Returns the user rights code for this event (online server only).</summary>
    public Task<string> RightsAsync(CancellationToken ct = default)
        => e.GetAsync<string>("file/rights", ct: ct);
}
