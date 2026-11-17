using System.Text;
using System.Text.Json.Nodes;

namespace RaceResultClient.Tests;

/// <summary>
/// Runs only against a real RaceResult server: set <c>RR_TEST_SERVER</c> (host, e.g. <c>192.168.1.20</c>; a
/// server at the venue, no login) and <c>RR_TEST_EVENT</c> (an event that may be changed: these tests save
/// its objects back, and add and remove a participant, bib 901). Skipped otherwise.
/// </summary>
public sealed class LiveFactAttribute : FactAttribute
{
    public LiveFactAttribute()
    {
        if (Live.Server is null || Live.Event is null)
            Skip = "Set RR_TEST_SERVER and RR_TEST_EVENT to run against a server.";
    }
}

public static class Live
{
    public static string? Server => Environment.GetEnvironmentVariable("RR_TEST_SERVER");
    public static string? Event => Environment.GetEnvironmentVariable("RR_TEST_EVENT");
    public static bool Https => Environment.GetEnvironmentVariable("RR_TEST_HTTPS") == "1";
}

/// <summary>Keeps the body of every answer, so the recorder can write what the server sent byte for byte.</summary>
public sealed class RecordingHandler() : DelegatingHandler(new HttpClientHandler())
{
    public string LastBody { get; private set; } = "";
    public string LastRequestBody { get; private set; } = "";

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        LastRequestBody = request.Content is null ? "" : await request.Content.ReadAsStringAsync(ct);
        var response = await base.SendAsync(request, ct);
        var bytes = await response.Content.ReadAsByteArrayAsync(ct);
        LastBody = Encoding.UTF8.GetString(bytes);
        var copy = new ByteArrayContent(bytes);
        foreach (var h in response.Content.Headers) copy.Headers.TryAddWithoutValidation(h.Key, h.Value);
        response.Content = copy;
        return response;
    }
}

public class LiveTests : IDisposable
{
    readonly RecordingHandler _rec = new();
    readonly ApiClient _api;
    readonly EventApiClient _ev;

    public LiveTests()
    {
        _api = new ApiClient(Live.Server ?? "localhost", Live.Https, "webapi/1.0", _rec);
        _ev = _api.ForEvent(Live.Event ?? "0");
    }

    public void Dispose() => _api.Dispose();

    /// <summary>
    /// Writes the server's answers to Fixtures/&lt;version&gt;/ when <c>RR_RECORD=1</c> (secrets scrubbed).
    /// Without it, only checks that every catalog call succeeds.
    /// </summary>
    [LiveFact]
    public async Task RecordFixtures()
    {
        var version = (await _api.General.GetAppVersionAsync()).Trim().Trim('"');
        var record = Environment.GetEnvironmentVariable("RR_RECORD") == "1";
        var dir = Path.Combine(Fixtures.Root, version);
        if (record) Directory.CreateDirectory(dir);
        var failed = new List<string>();
        var skipFile = Path.Combine(Fixtures.Root, "skip.txt");
        var skip = File.Exists(skipFile)
            ? File.ReadAllLines(skipFile).Where(l => l.Length > 0 && !l.StartsWith('#')).ToHashSet()
            : [];
        foreach (var e in Catalog.Fixed().Concat(await Catalog.DesignObjectsAsync(_ev)))
        {
            if (skip.Contains(e.Name)) continue;
            try { await e.Call(_ev); }
            catch (Exception ex) { failed.Add($"{e.Name}: {ex.Message}"); continue; }
            if (!record || string.IsNullOrWhiteSpace(_rec.LastBody)) continue;   // chipfile/get of an empty chip file is ""
            var node = JsonNode.Parse(_rec.LastBody);
            Fixtures.Scrub(node);
            await File.WriteAllTextAsync(Path.Combine(dir, Fixtures.FileName(e.Name) + ".json"),
                node?.ToJsonString(new System.Text.Json.JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }) ?? "null");
        }
        Assert.True(failed.Count == 0, string.Join("\n", failed));
    }

    /// <summary>Every design object, saved back unchanged, is unchanged on the server.</summary>
    [LiveFact]
    public async Task DesignObjectsSurviveSave()
    {
        var problems = new List<string>();
        async Task Check<T>(string label, Func<Task<T>> get, Func<T, Task> save)
        {
            var before = await get(); var b = _rec.LastBody;
            try { await save(before); }
            catch (Exception ex) { problems.Add($"{label}: save failed: {ex.Message}"); return; }
            await get(); var a = _rec.LastBody;
            var diff = Fixtures.Diff(JsonNode.Parse(b), JsonNode.Parse(a));
            if (diff.Count > 0) problems.Add($"{label}:\n    " + string.Join("\n    ", diff.Take(10)));
        }

        await Check("agegroups", () => _ev.AgeGroups.GetAsync(), x => _ev.AgeGroups.SaveAsync(x));
        await Check("contest 1", () => _ev.Contests.GetOneAsync(1), x => _ev.Contests.SaveAsync(x, x.Id));
        await Check("fields", () => _ev.CustomFields.GetAsync(), x => _ev.CustomFields.SaveAsync(x));
        await Check("entryfees", () => _ev.EntryFees.GetAsync(), x => _ev.EntryFees.SaveAsync(x));
        await Check("ranks", () => _ev.Rankings.GetAsync(), x => _ev.Rankings.SaveAsync(x));
        await Check("results", () => _ev.Results.GetAsync(), x => _ev.Results.SaveAsync(x));
        await Check("splits", () => _ev.Splits.GetAsync(), x => _ev.Splits.SaveAsync(x));
        foreach (var t in await _ev.TeamScores.GetAsync())
            await Check($"teamscore {t.Id}", () => _ev.TeamScores.GetOneAsync(t.Id), x => _ev.TeamScores.SaveAsync(x));
        foreach (var tp in await _ev.TimingPoints.GetAsync())
            await Check($"timingpoint {tp.Name}", () => _ev.TimingPoints.GetOneAsync(tp.Name), x => _ev.TimingPoints.SaveAsync(x, x.Name));
        await Check("userdefinedfields", () => _ev.UserDefinedFields.GetAsync(), x => _ev.UserDefinedFields.SetAsync(x));
        await Check("simpleapi", () => _ev.SimpleApi.GetAsync(), x => _ev.SimpleApi.SaveAllAsync(x));
        foreach (var t in new[] { "StartTimes", "FinishTimeLimits" })
            await Check($"grouptimes {t}", () => _ev.GroupTimes.GetAsync(t), x => _ev.GroupTimes.SaveAsync(t, x));
        foreach (var n in await _ev.Certificates.NamesAsync()) await Check($"certificate {n}", () => _ev.Certificates.GetAsync(n), x => _ev.Certificates.SaveAsync(x));
        foreach (var n in await _ev.CertificateSets.NamesAsync()) await Check($"certificateset {n}", () => _ev.CertificateSets.GetAsync(n), x => _ev.CertificateSets.SaveAsync(x));
        foreach (var n in await _ev.EmailTemplates.NamesAsync()) await Check($"emailtemplate {n}", () => _ev.EmailTemplates.GetAsync(n), x => _ev.EmailTemplates.SaveAsync(x));
        foreach (var n in await _ev.Kiosks.NamesAsync()) await Check($"kiosk {n}", () => _ev.Kiosks.GetAsync(n), x => _ev.Kiosks.SaveAsync(x));
        foreach (var n in await _ev.Labels.NamesAsync()) await Check($"label {n}", () => _ev.Labels.GetAsync(n), x => _ev.Labels.SaveAsync(x));
        foreach (var n in await _ev.Lists.NamesAsync()) await Check($"list {n}", () => _ev.Lists.GetAsync(n), x => _ev.Lists.SaveAsync(x));
        foreach (var n in await _ev.Registrations.NamesAsync()) await Check($"registration {n}", () => _ev.Registrations.GetAsync(n), x => _ev.Registrations.SaveAsync(x));
        foreach (var n in await _ev.Statistics.NamesAsync()) await Check($"statistics {n}", () => _ev.Statistics.GetAsync(n), x => _ev.Statistics.SaveAsync(x));

        Assert.True(problems.Count == 0, string.Join("\n", problems));
    }

    /// <summary>A new kiosk carries RFC 3339 dates with an offset ("…+01:00"); saving it must not escape the '+'.</summary>
    [LiveFact]
    public async Task NewKioskSurvivesSave()
    {
        const string name = "ClientTestKiosk";
        await _ev.Kiosks.NewAsync(name);
        try
        {
            var k = await _ev.Kiosks.GetAsync(name);
            await _ev.Kiosks.SaveAsync(k);
            Assert.DoesNotContain("\\u002B", _rec.LastRequestBody);
        }
        finally { await _ev.Kiosks.DeleteAsync(name); }
    }

    /// <summary>Every GetOne reads its item, whether the server answers with the item or a list of it.</summary>
    [LiveFact]
    public async Task GetOneReadsEitherShape()
    {
        var ag = (await _ev.AgeGroups.GetAsync())[0];
        Assert.Equal(ag.Id, (await _ev.AgeGroups.GetOneAsync(ag.Id)).Id);
        Assert.Equal(1, (await _ev.Contests.GetOneAsync(1)).Id);
        var cf = (await _ev.CustomFields.GetAsync())[0];
        Assert.Equal(cf.Id, (await _ev.CustomFields.GetOneAsync(cf.Id)).Id);
        var sp = (await _ev.Splits.GetAsync())[0];
        Assert.Equal(sp.Id, (await _ev.Splits.GetOneAsync(sp.Id)).Id);
        var tp = (await _ev.TimingPoints.GetAsync())[0];
        Assert.Equal(tp.Name, (await _ev.TimingPoints.GetOneAsync(tp.Name)).Name);
        foreach (var ts in await _ev.TeamScores.GetAsync())
            Assert.Equal(ts.Id, (await _ev.TeamScores.GetOneAsync(ts.Id)).Id);
        await Assert.ThrowsAsync<ApiException>(() => _ev.Contests.GetOneAsync(99999));
    }

    /// <summary>The endpoints taken from RaceResult's own JS client: what they write, they read back.</summary>
    [LiveFact]
    public async Task JsClientEndpointsWork()
    {
        // one setting: saved as text, read, deleted
        await _ev.Settings.SaveSettingAsync("ClientTestSetting", "hello");
        Assert.Equal("hello", await _ev.Settings.GetSettingAsync("ClientTestSetting"));
        await _ev.Settings.DeleteAsync("ClientTestSetting");
        Assert.Equal("", await _ev.Settings.GetSettingAsync("ClientTestSetting"));

        // the lookup table: saved, read, put back
        var table = await _ev.Table.GetAsync();
        try
        {
            await _ev.Table.SaveAsync([.. table, new TableValue { Index1 = "client", Index2 = "test", Value = "42" }]);
            Assert.Contains(await _ev.Table.GetAsync(), v => v.Index1 == "client" && v.Value == "42");
        }
        finally { await _ev.Table.SaveAsync(table); }
        Assert.Equal(table.Length, (await _ev.Table.GetAsync()).Length);

        // a presenter and a registration component: new, read, saved back unchanged, deleted
        await _ev.Presenters.NewAsync("ClientTestPresenter");
        try
        {
            var p = await _ev.Presenters.GetAsync("ClientTestPresenter"); var before = _rec.LastBody;
            await _ev.Presenters.SaveAsync(p);
            await _ev.Presenters.GetAsync("ClientTestPresenter");
            Assert.Empty(Fixtures.Diff(JsonNode.Parse(before), JsonNode.Parse(_rec.LastBody)));
        }
        finally { await _ev.Presenters.DeleteAsync("ClientTestPresenter"); }

        await _ev.RegistrationComponents.NewAsync("ClientTestComponent");
        try
        {
            var c = await _ev.RegistrationComponents.GetAsync("ClientTestComponent"); var before = _rec.LastBody;
            await _ev.RegistrationComponents.SaveAsync(c);
            await _ev.RegistrationComponents.GetAsync("ClientTestComponent");
            Assert.Empty(Fixtures.Diff(JsonNode.Parse(before), JsonNode.Parse(_rec.LastBody)));
        }
        finally { await _ev.RegistrationComponents.DeleteAsync("ClientTestComponent"); }

        // a trigger: created, deleted
        var id = await _ev.Triggers.CreateAsync(TriggerType.NewTime, fields: ["Bib"]);
        Assert.True(id > 0);
        await _ev.Triggers.DeleteAsync(id);

        // reads
        Assert.NotEmpty(await _ev.Statistics.ContestStatisticsAsync());
        Assert.True(await _ev.File.ServerTimeAsync() > 0);
        Assert.True(await _ev.File.ModJobIdAllSettingsAsync() > 0);
        Assert.Contains("_", await _ev.File.CustMd5Async());
        Assert.NotEmpty(await _api.General.GetCountriesAsync());
        Assert.Equal("DE", (await _api.General.GetCountryAsync("Germany")).Alpha2);
        Assert.NotEmpty(await _api.General.GetTranslationsAsync("de"));
    }

    /// <summary>times/add takes a passing by bib and by transponder; an unset DateTime must not fail the request.</summary>
    [LiveFact]
    public async Task TimesAddAcceptsPassings()
    {
        var me = Identifier.Bib(901);
        await _ev.Participants.NewAsync(bib: 901, contest: 1);
        try
        {
            var byBib = await _ev.Times.AddAsync([new PassingToProcess
            {
                Bib = 901, TimingPoint = (await _ev.TimingPoints.GetAsync())[0].Name, Time = 36000.25m,
                Passing = new Passing { DeviceId = "Test", DeviceName = "Test" }
            }]);
            Assert.Single(byBib);
            var byChip = await _ev.Times.AddAsync([new PassingToProcess
            {
                Passing = new Passing { Transponder = "ZZTEST1", UtcTime = DateTime.UtcNow, DeviceName = "Test" }
            }]);
            Assert.Single(byChip);
            // an unknown transponder makes RaceResult create a participant for it ("N.N.", bib 50001, …): remove it
            var chipRead = (await _ev.RawData.GetAsync(Identifier.Bib(0))).FirstOrDefault(r => r.Id == byChip[0].RawDataId);
            if (chipRead is { Pid: > 0 })
            {
                await _ev.RawData.DeleteAsync(Identifier.Pid(chipRead.Pid));
                var made = await _ev.Participants.GetFieldsAsync(Identifier.Pid(chipRead.Pid), ["Firstname"]);
                if (made.TryGetValue("Firstname", out var fn) && fn.GetString() == "N.N.")
                    await _ev.Participants.DeleteAsync(id: Identifier.Pid(chipRead.Pid));
            }
            var raw = await _ev.RawData.GetAsync(me);
            Assert.Contains(raw, r => r.Time == 36000.25m);
        }
        finally
        {
            await _ev.RawData.DeleteAsync(me);
            await _ev.Participants.DeleteAsync(id: me);
        }
    }
}
