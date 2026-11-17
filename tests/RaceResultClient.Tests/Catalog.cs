using System.Text.Json;

namespace RaceResultClient.Tests;

/// <summary>
/// The read endpoints whose answers are kept as fixtures: the fixture's name, the type the library
/// reads it into, and the call that fetches it. One list serves both the recorder (live) and the
/// offline tests, so a fixture is always read into the type its endpoint returns.
/// </summary>
public static class Catalog
{
    public sealed record Entry(string Name, Type Type, Func<EventApiClient, Task> Call);

    static Entry E<T>(string name, Func<EventApiClient, Task<T>> call) => new(name, typeof(T), call);

    /// <summary>Endpoints read the same way on every event.</summary>
    public static IEnumerable<Entry> Fixed() =>
    [
        E("agegroups.get", ev => ev.AgeGroups.GetAsync()),
        E("bibranges.get", ev => ev.BibRanges.GetAsync()),
        E("contests.get", ev => ev.Contests.GetAsync()),
        E("contests.getone", ev => ev.Contests.GetOneAsync(1)),
        E("fields.get", ev => ev.CustomFields.GetAsync()),
        E("entryfees.get", ev => ev.EntryFees.GetAsync()),
        E("exporters.get", ev => ev.Exporters.GetAsync()),
        E("ranks.get", ev => ev.Rankings.GetAsync()),
        E("results.get", ev => ev.Results.GetAsync()),
        E("splits.get", ev => ev.Splits.GetAsync()),
        E("teamscores.get", ev => ev.TeamScores.GetAsync()),
        E("timingpoints.get", ev => ev.TimingPoints.GetAsync()),
        E("timingpointrules.get", ev => ev.TimingPointRules.GetAsync()),
        E("rawdatarules.get", ev => ev.RawDataRules.GetAsync()),
        E("userdefinedfields.get", ev => ev.UserDefinedFields.GetAsync()),
        E("simpleapi.get", ev => ev.SimpleApi.GetAsync()),
        E("vouchers.get", ev => ev.Vouchers.GetAsync()),
        E("webhooks.get", ev => ev.WebHooks.GetAsync()),
        E("chipfile.get", ev => ev.ChipFile.GetAsync()),
        E("chat.getmessages", ev => ev.Chat.GetMessagesAsync()),
        E("grouptimes.get[StartTimes]", ev => ev.GroupTimes.GetAsync("StartTimes")),
        E("grouptimes.get[FinishTimeLimits]", ev => ev.GroupTimes.GetAsync("FinishTimeLimits")),
        E("backup.info", ev => ev.Backup.InfoAsync()),
        E("forwarding.info", ev => ev.Forwarding.GetInfoAsync()),
        E("file.sesversion", ev => ev.File.SesVersionAsync()),
        E("file.rights", ev => ev.File.RightsAsync()),
        E("rawdata.distinctvalues", ev => ev.RawData.DistinctValuesAsync()),
        E("rawdata.get", ev => ev.RawData.GetAsync(Identifier.Bib(0), addFields: ["Bib", "Lastname"])),
        E("history.get", ev => ev.History.GetAsync(Identifier.Bib(0))),
        E("data.list", ev => ev.Data.ListAsync(["ID", "Bib", "Firstname", "Lastname", "Contest", "Sex", "Status"])),
        E("settings.getsettings", ev => ev.Settings.GetAsync(default, "EventName", "EventDate", "EventLocation")),
        E("settings.settingnamesbyprefix", ev => ev.Settings.NamesByPrefixAsync("Event")),
        E("participants.getentryfees", async ev =>
        {
            var bib = (await ev.Data.ListAsync(["Bib"], limitTo: 1))[0][0].GetInt32();
            return await ev.Participants.GetEntryFeesAsync([bib]);
        }),
        E("times.get", async ev =>
        {
            var bib = (await ev.Data.ListAsync(["Bib"], limitTo: 1))[0][0].GetInt32();
            return await ev.Times.GetAsync(Identifier.Bib(bib));
        }),
        E("emailtemplates.preview", async ev =>
            await ev.EmailTemplates.PreviewAsync((await ev.EmailTemplates.NamesAsync())[0])),

        // what RaceResult's own JS client reads, go-webapi does not
        E("file.plugins", ev => ev.File.PlugInsAsync()),
        E("table.get", ev => ev.Table.GetAsync()),
        E("statistics.conteststatistic", ev => ev.Statistics.ContestStatisticsAsync()),
        E("statistics.resultstatistic", ev => ev.Statistics.ResultStatisticsAsync()),
        E("statistics.splitsreadrates", ev => ev.Statistics.SplitsReadRatesAsync()),
        E("statistics.splitsminmax", ev => ev.Statistics.SplitsMinMaxAsync()),
        E("settings.geocode", ev => ev.Settings.GeoCodeAsync()),
        E("websites.get", ev => ev.Websites.GetAsync()),
        E("websites.getpublishedlists", ev => ev.Websites.GetPublishedListsAsync()),
        E("invoices.get", ev => ev.Invoices.GetAsync()),
        E("invoices.template.get", ev => ev.Invoices.GetTemplateAsync()),
        E("invoices.template.default", ev => ev.Invoices.GetDefaultTemplateAsync()),
        E("history.getids", ev => ev.History.GetIdsAsync(new HistoryFilter(), limit: 5)),
        E("part.search", ev => ev.Participants.SearchAsync("Test", ["Bib", "Lastname"])),
        E("rawdata.getintervals", async ev => await ev.RawData.GetIntervalsAsync((await ev.TimingPoints.GetAsync())[0].Name)),
        E("information.countries", ev => ev.Client.GetAsync<JsonElement[][]>(null, "information/countries")),
        E("information.country", ev => ev.Client.General.GetCountryAsync("Germany")),
    ];

    /// <summary>The design objects, one fixture per object the event has.</summary>
    public static async Task<IEnumerable<Entry>> DesignObjectsAsync(EventApiClient ev)
    {
        var list = new List<Entry>();
        foreach (var n in await ev.Certificates.NamesAsync()) list.Add(E($"certificates.get[{n}]", e => e.Certificates.GetAsync(n)));
        foreach (var n in await ev.CertificateSets.NamesAsync()) list.Add(E($"certificatesets.get[{n}]", e => e.CertificateSets.GetAsync(n)));
        foreach (var n in await ev.EmailTemplates.NamesAsync()) list.Add(E($"emailtemplates.get[{n}]", e => e.EmailTemplates.GetAsync(n)));
        foreach (var n in await ev.Kiosks.NamesAsync()) list.Add(E($"kiosks.get[{n}]", e => e.Kiosks.GetAsync(n)));
        foreach (var n in await ev.Labels.NamesAsync()) list.Add(E($"labels.get[{n}]", e => e.Labels.GetAsync(n)));
        foreach (var n in await ev.Lists.NamesAsync()) list.Add(E($"lists.get[{n}]", e => e.Lists.GetAsync(n)));
        foreach (var n in await ev.Registrations.NamesAsync()) list.Add(E($"registrations.get[{n}]", e => e.Registrations.GetAsync(n)));
        foreach (var n in await ev.Statistics.NamesAsync()) list.Add(E($"statistics.get[{n}]", e => e.Statistics.GetAsync(n)));
        foreach (var n in await ev.Presenters.NamesAsync()) list.Add(E($"presenters.get[{n}]", e => e.Presenters.GetAsync(n)));
        foreach (var n in await ev.RegistrationComponents.NamesAsync()) list.Add(E($"registrations.components.get[{n}]", e => e.RegistrationComponents.GetAsync(n)));
        return list;
    }

    /// <summary>The type a fixture is read into, by its name (the part before any "[…]").</summary>
    public static Type TypeOf(string fixtureName)
    {
        var key = fixtureName.Split('[')[0];
        var fixedEntry = Fixed().FirstOrDefault(x => x.Name.Split('[')[0] == key);
        if (fixedEntry is not null) return fixedEntry.Type;
        return key switch
        {
            "certificates.get" => typeof(Certificate),
            "certificatesets.get" => typeof(CertificateSet),
            "emailtemplates.get" => typeof(EmailTemplate),
            "kiosks.get" => typeof(Kiosk),
            "labels.get" => typeof(Label),
            "lists.get" => typeof(List),
            "registrations.get" => typeof(Registration),
            "statistics.get" => typeof(Statistics),
            "presenters.get" => typeof(Presenter),
            "registrations.components.get" => typeof(RegistrationComponent),
            _ => throw new InvalidOperationException($"No type for fixture {fixtureName}")
        };
    }

    /// <summary>Types whose answer is a map or raw JSON: nothing to check field by field.</summary>
    public static bool IsOpen(Type t) =>
        t == typeof(string) || t == typeof(JsonElement) || t == typeof(JsonElement[][])
        || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>) && t.GetGenericArguments()[1] == typeof(JsonElement));
}
