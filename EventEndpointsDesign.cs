using System.Text;

namespace RaceResultClient;

// Endpoint groups added to mirror the official go-webapi surface.

// ── Archives ──────────────────────────────────────────────────────────────────

public sealed class ArchivesEndpoints(EventApiClient e)
{
    public Task<int> CreateNewRegNoAsync(CancellationToken ct = default)
        => e.GetAsync<int>("archives/createnewregno", ct: ct);

    public Task<ArchiveMatch[]> GetMatchesAsync(string prefix, int maxNumber,
        CancellationToken ct = default)
        => e.GetAsync<ArchiveMatch[]>("archives/getmatches",
            new QueryParams().Add("prefix", prefix).Add("maxNumber", maxNumber), ct);

    public Task<ArchiveParticipant> GetEntryAsync(int id, string regNo = "",
        CancellationToken ct = default)
        => e.GetAsync<ArchiveParticipant>("archives/getentry",
            new QueryParams().Add("id", id).Add("regNo", regNo), ct);

    public Task<ArchiveParticipationExt[]> GetParticipationsAsync(Identifier identifier,
        CancellationToken ct = default)
        => e.GetAsync<ArchiveParticipationExt[]>("archives/getparticipations",
            identifier.ApplyTo(new QueryParams()), ct);

    public Task<byte[]> DownloadAsync(CancellationToken ct = default)
        => e.GetBytesAsync("archives/download", ct: ct);

    public Task RemoveAsync(CancellationToken ct = default)
        => e.GetVoidAsync("archives/remove", ct: ct);

    public Task CreateAsync(CancellationToken ct = default)
        => e.GetVoidAsync("archives/create", ct: ct);

    public Task WriteAsync(CancellationToken ct = default)
        => e.GetVoidAsync("archives/write", ct: ct);

    public Task ImportAsync(byte[] archive, CancellationToken ct = default)
        => e.PostVoidAsync("archives/import", null, archive, ct);
}

// ── Backup ────────────────────────────────────────────────────────────────────

public sealed class BackupEndpoints(EventApiClient e)
{
    public Task<bool> ActiveAsync(CancellationToken ct = default)
        => e.GetAsync<bool>("backup/active", ct: ct);

    public Task StartAsync(string hostname, string filename, CancellationToken ct = default)
        => e.GetVoidAsync("backup/start",
            new QueryParams().Add("hostname", hostname).Add("filename", filename), ct);

    public Task RestartAsync(CancellationToken ct = default)
        => e.GetVoidAsync("backup/restart", ct: ct);

    public Task StopAsync(CancellationToken ct = default)
        => e.GetVoidAsync("backup/stop", ct: ct);

    public Task<ForwardingInfo> InfoAsync(CancellationToken ct = default)
        => e.GetAsync<ForwardingInfo>("backup/info", ct: ct);
}

// ── Certificates ──────────────────────────────────────────────────────────────

public sealed class CertificatesEndpoints(EventApiClient e)
{
    public Task<string[]> NamesAsync(CancellationToken ct = default)
        => e.GetAsync<string[]>("certificates/names", ct: ct);

    public Task<Certificate> GetAsync(string name, CancellationToken ct = default)
        => e.GetAsync<Certificate>("certificates/get", new QueryParams().Add("name", name), ct);

    public Task SaveAsync(Certificate item, CancellationToken ct = default)
        => e.PostVoidAsync("certificates/save", null, item, ct);

    public Task DeleteAsync(string name, CancellationToken ct = default)
        => e.GetVoidAsync("certificates/delete", new QueryParams().Add("name", name), ct);

    public Task CopyAsync(string name, string newName, CancellationToken ct = default)
        => e.GetVoidAsync("certificates/copy",
            new QueryParams().Add("name", name).Add("newName", newName), ct);

    public Task RenameAsync(string name, string newName, CancellationToken ct = default)
        => e.GetVoidAsync("certificates/rename",
            new QueryParams().Add("name", name).Add("newName", newName), ct);

    public Task NewAsync(string name, CancellationToken ct = default)
        => e.GetVoidAsync("certificates/new", new QueryParams().Add("name", name), ct);

    public Task<byte[]> ThumbnailAsync(string name, int maxWidth, int maxHeight,
        CancellationToken ct = default)
        => e.GetBytesAsync("certificates/thumbnail",
            new QueryParams().Add("name", name).Add("maxWidth", maxWidth).Add("maxHeight", maxHeight), ct);

    public Task<byte[]> PreviewJpgAsync(string name, int page, int dpi, string lang = "",
        CancellationToken ct = default)
        => e.GetBytesAsync("certificates/previewJPG",
            new QueryParams().Add("name", name).AddAlways("page", page).Add("dpi", dpi).Add("lang", lang), ct);

    public Task<byte[]> CreatePdfAsync(string name, int page, int bib, string lang = "",
        CancellationToken ct = default)
        => e.GetBytesAsync("certificates/create",
            new QueryParams().Add("name", name).AddAlways("page", page).Add("bib", bib)
                .Add("lang", lang).Add("format", "pdf"), ct);

    public Task<byte[]> CreateJpgAsync(string name, int page, int bib, int dpi, string lang = "",
        CancellationToken ct = default)
        => e.GetBytesAsync("certificates/create",
            new QueryParams().Add("name", name).AddAlways("page", page).Add("bib", bib)
                .Add("dpi", dpi).Add("lang", lang).Add("format", "jpg"), ct);
}

// ── CertificateSets ───────────────────────────────────────────────────────────

public sealed class CertificateSetsEndpoints(EventApiClient e)
{
    public Task<string[]> NamesAsync(CancellationToken ct = default)
        => e.GetAsync<string[]>("certificatesets/names", ct: ct);

    public Task<CertificateSet> GetAsync(string name, CancellationToken ct = default)
        => e.GetAsync<CertificateSet>("certificatesets/get", new QueryParams().Add("name", name), ct);

    public Task SaveAsync(CertificateSet item, CancellationToken ct = default)
        => e.PostVoidAsync("certificatesets/save", null, item, ct);

    public Task DeleteAsync(string name, CancellationToken ct = default)
        => e.GetVoidAsync("certificatesets/delete", new QueryParams().Add("name", name), ct);

    public Task CopyAsync(string name, string newName, CancellationToken ct = default)
        => e.GetVoidAsync("certificatesets/copy",
            new QueryParams().Add("name", name).Add("newName", newName), ct);

    public Task RenameAsync(string name, string newName, CancellationToken ct = default)
        => e.GetVoidAsync("certificatesets/rename",
            new QueryParams().Add("name", name).Add("newName", newName), ct);

    public Task NewAsync(string name, CancellationToken ct = default)
        => e.GetVoidAsync("certificatesets/new", new QueryParams().Add("name", name), ct);

    public Task<byte[]> CreateAsync(string name, IEnumerable<int> contests, string filter = "",
        string lang = "", CancellationToken ct = default)
        => e.GetBytesAsync("certificatesets/create",
            new QueryParams().Add("name", name).AddArray("contest", contests)
                .Add("filter", filter).Add("lang", lang), ct);

    public Task<int> CountAsync(string name, IEnumerable<int> contests, CancellationToken ct = default)
        => e.GetAsync<int>("certificatesets/count",
            new QueryParams().Add("name", name).AddArray("contest", contests), ct);
}

// ── ChipFile ──────────────────────────────────────────────────────────────────

public sealed class ChipFileEndpoints(EventApiClient e)
{
    /// <summary>Returns the entire chip file (parsed from the "transponder;identification" lines).</summary>
    public async Task<ChipFileEntry[]> GetAsync(CancellationToken ct = default)
    {
        var bytes = await e.GetBytesAsync("chipfile/get", ct: ct);
        var text = Encoding.UTF8.GetString(bytes);
        var list = new List<ChipFileEntry>();
        foreach (var line in text.Split("\r\n"))
        {
            var arr = line.Split(';');
            if (arr.Length != 2) continue;
            list.Add(new ChipFileEntry(arr[0], arr[1]));
        }
        return list.ToArray();
    }

    /// <summary>Saves a new chip file.</summary>
    public Task SaveAsync(IEnumerable<ChipFileEntry> items, CancellationToken ct = default)
    {
        var body = string.Join("\r\n", items.Select(i => $"{i.Transponder};{i.Identification}"));
        return e.PostVoidAsync("chipfile/save", null, Encoding.UTF8.GetBytes(body), ct);
    }

    public Task ClearAsync(CancellationToken ct = default)
        => e.GetVoidAsync("chipfile/clear", ct: ct);
}

// ── Dependencies ──────────────────────────────────────────────────────────────

public sealed class DependenciesEndpoints(EventApiClient e)
{
    public Task<string> ShowAsync(CancellationToken ct = default)
        => e.GetAsync<string>("dependencies/show", ct: ct);

    public Task<string> CircularReferencesAsync(CancellationToken ct = default)
        => e.GetAsync<string>("dependencies/circularreferences", ct: ct);
}

// ── EmailTemplates ────────────────────────────────────────────────────────────

public sealed class EmailTemplatesEndpoints(EventApiClient e)
{
    public Task<string[]> NamesAsync(CancellationToken ct = default)
        => e.GetAsync<string[]>("emailtemplates/names", ct: ct);

    public Task<EmailTemplate> GetAsync(string name, CancellationToken ct = default)
        => e.GetAsync<EmailTemplate>("emailtemplates/get", new QueryParams().Add("name", name), ct);

    public Task SaveAsync(EmailTemplate item, CancellationToken ct = default)
        => e.PostVoidAsync("emailtemplates/save", null, item, ct);

    public Task DeleteAsync(string name, CancellationToken ct = default)
        => e.GetVoidAsync("emailtemplates/delete", new QueryParams().Add("name", name), ct);

    public Task CopyAsync(string name, string newName, CancellationToken ct = default)
        => e.GetVoidAsync("emailtemplates/copy",
            new QueryParams().Add("name", name).Add("newName", newName), ct);

    public Task RenameAsync(string name, string newName, CancellationToken ct = default)
        => e.GetVoidAsync("emailtemplates/rename",
            new QueryParams().Add("name", name).Add("newName", newName), ct);

    public Task NewAsync(string name, CancellationToken ct = default)
        => e.GetVoidAsync("emailtemplates/new", new QueryParams().Add("name", name), ct);

    /// <summary>Generates previews of the email/message for matching participants.</summary>
    public Task<EmailPreview[]> PreviewAsync(string name, string filter = "", string lang = "",
        CancellationToken ct = default)
        => e.GetAsync<EmailPreview[]>("emailtemplates/preview",
            new QueryParams().Add("name", name).Add("filter", filter).Add("lang", lang), ct);

    /// <summary>Sends a pre-generated preview.</summary>
    public Task SendPreviewAsync(string name, EmailPreview preview, string lang = "",
        CancellationToken ct = default)
        => e.PostVoidAsync("emailtemplates/sendpreview",
            new QueryParams().Add("name", name).Add("lang", lang), preview, ct);

    /// <summary>Generates the previews and directly sends them.</summary>
    public Task SendAsync(string name, string filter = "", string lang = "",
        CancellationToken ct = default)
        => e.GetVoidAsync("emailtemplates/send",
            new QueryParams().Add("name", name).Add("filter", filter).Add("lang", lang), ct);
}

// ── GroupTimes ────────────────────────────────────────────────────────────────

public sealed class GroupTimesEndpoints(EventApiClient e)
{
    public Task<GroupTimes> GetAsync(string type, CancellationToken ct = default)
        => e.GetAsync<GroupTimes>("grouptimes/get", new QueryParams().Add("type", type), ct);

    public Task SaveAsync(string type, GroupTimes item, CancellationToken ct = default)
        => e.PostVoidAsync("grouptimes/save", new QueryParams().Add("type", type), item, ct);
}

// ── Information ────────────────────────────────────────────────────────────────

public sealed class InformationEndpoints(EventApiClient e)
{
    /// <summary>Returns frequent first names that have the given prefix.</summary>
    public Task<string[]> FrequentNamesAsync(string prefix, int maxNo,
        CancellationToken ct = default)
        => e.GetAsync<string[]>("information/frequentnames",
            new QueryParams().Add("prefix", prefix).Add("maxNo", maxNo), ct);

    /// <summary>Returns the gender associated with the given first name.</summary>
    public Task<string> GetSexAsync(string name, CancellationToken ct = default)
        => e.GetAsync<string>("information/getsex", new QueryParams().Add("name", name), ct);

    /// <summary>Adds a name to the database of first names.</summary>
    public Task AddFirstNameAsync(string name, string sex, CancellationToken ct = default)
        => e.GetVoidAsync("information/addfirstname",
            new QueryParams().Add("name", name).Add("sex", sex), ct);
}

// ── Kiosks ────────────────────────────────────────────────────────────────────

public sealed class KiosksEndpoints(EventApiClient e)
{
    public Task<string[]> NamesAsync(CancellationToken ct = default)
        => e.GetAsync<string[]>("kiosks/names", ct: ct);

    public Task<Kiosk> GetAsync(string name, CancellationToken ct = default)
        => e.GetAsync<Kiosk>("kiosks/get", new QueryParams().Add("name", name), ct);

    public Task SaveAsync(Kiosk item, CancellationToken ct = default)
        => e.PostVoidAsync("kiosks/save", null, item, ct);

    public Task DeleteAsync(string name, CancellationToken ct = default)
        => e.GetVoidAsync("kiosks/delete", new QueryParams().Add("name", name), ct);

    public Task CopyAsync(string name, string newName, CancellationToken ct = default)
        => e.GetVoidAsync("kiosks/copy",
            new QueryParams().Add("name", name).Add("newName", newName), ct);

    public Task RenameAsync(string name, string newName, CancellationToken ct = default)
        => e.GetVoidAsync("kiosks/rename",
            new QueryParams().Add("name", name).Add("newName", newName), ct);

    public Task NewAsync(string name, CancellationToken ct = default)
        => e.GetVoidAsync("kiosks/new", new QueryParams().Add("name", name), ct);
}

// ── Labels ────────────────────────────────────────────────────────────────────

public sealed class LabelsEndpoints(EventApiClient e)
{
    public Task<string[]> NamesAsync(CancellationToken ct = default)
        => e.GetAsync<string[]>("labels/names", ct: ct);

    public Task<Label> GetAsync(string name, CancellationToken ct = default)
        => e.GetAsync<Label>("labels/get", new QueryParams().Add("name", name), ct);

    public Task SaveAsync(Label item, CancellationToken ct = default)
        => e.PostVoidAsync("labels/save", null, item, ct);

    public Task DeleteAsync(string name, CancellationToken ct = default)
        => e.GetVoidAsync("labels/delete", new QueryParams().Add("name", name), ct);

    public Task CopyAsync(string name, string newName, CancellationToken ct = default)
        => e.GetVoidAsync("labels/copy",
            new QueryParams().Add("name", name).Add("newName", newName), ct);

    public Task RenameAsync(string name, string newName, CancellationToken ct = default)
        => e.GetVoidAsync("labels/rename",
            new QueryParams().Add("name", name).Add("newName", newName), ct);

    public Task NewAsync(string name, CancellationToken ct = default)
        => e.GetVoidAsync("labels/new", new QueryParams().Add("name", name), ct);

    public Task<byte[]> CreateAsync(string name, IEnumerable<int> contests, int startX, int startY,
        string lang = "", CancellationToken ct = default)
        => e.GetBytesAsync("labels/create",
            new QueryParams().Add("name", name).AddArray("contest", contests)
                .Add("startX", startX).Add("startY", startY).Add("lang", lang), ct);
}

// ── Lists ─────────────────────────────────────────────────────────────────────

public sealed class ListsEndpoints(EventApiClient e)
{
    public Task<string[]> NamesAsync(CancellationToken ct = default)
        => e.GetAsync<string[]>("lists/names", ct: ct);

    public Task DeleteAsync(string name, CancellationToken ct = default)
        => e.GetVoidAsync("lists/delete", new QueryParams().Add("name", name), ct);

    public Task CopyAsync(string name, string newName, CancellationToken ct = default)
        => e.GetVoidAsync("lists/copy",
            new QueryParams().Add("name", name).Add("newName", newName), ct);

    public Task RenameAsync(string name, string newName, CancellationToken ct = default)
        => e.GetVoidAsync("lists/rename",
            new QueryParams().Add("name", name).Add("newName", newName), ct);

    public Task NewAsync(string name, CancellationToken ct = default)
        => e.GetVoidAsync("lists/new", new QueryParams().Add("name", name), ct);

    public Task<List> GetAsync(string name, bool noTranslate = false, string lang = "",
        CancellationToken ct = default)
        => e.GetAsync<List>("lists/get",
            new QueryParams().Add("name", name).Add("noTranslate", noTranslate).Add("lang", lang), ct);

    public Task SaveAsync(List item, CancellationToken ct = default)
        => e.PostVoidAsync("lists/save", null, item, ct);

    public Task<byte[]> CreatePdfAsync(string name, IEnumerable<int> contests, string filter = "",
        string selectorResult = "", string lang = "", CancellationToken ct = default)
        => CreateAsync("pdf", name, contests, filter, selectorResult, lang, ct: ct);

    public Task<byte[]> CreateHtmlAsync(string name, IEnumerable<int> contests, string filter = "",
        string selectorResult = "", string lang = "", CancellationToken ct = default)
        => CreateAsync("html", name, contests, filter, selectorResult, lang, ct: ct);

    public Task<byte[]> CreateJsonAsync(string name, IEnumerable<int> contests, string filter = "",
        string selectorResult = "", string lang = "", CancellationToken ct = default)
        => CreateAsync("JSON", name, contests, filter, selectorResult, lang, ct: ct);

    public Task<byte[]> CreateNewspaperAsync(string name, IEnumerable<int> contests, string filter = "",
        string selectorResult = "", string lang = "", CancellationToken ct = default)
        => CreateAsync("newspaper", name, contests, filter, selectorResult, lang, ct: ct);

    public Task<byte[]> CreateXlsxAsync(string name, IEnumerable<int> contests, string filter = "",
        string selectorResult = "", string lang = "", CancellationToken ct = default)
        => CreateAsync("xlsx", name, contests, filter, selectorResult, lang, ct: ct);

    public Task<byte[]> CreateXmlAsync(string name, IEnumerable<int> contests, string charset = "",
        string filter = "", string selectorResult = "", string lang = "", CancellationToken ct = default)
        => CreateAsync("xml", name, contests, filter, selectorResult, lang, charset, ct: ct);

    public Task<byte[]> CreateCsvAsync(string name, IEnumerable<int> contests, string charset = "",
        string separator = "", string filter = "", string selectorResult = "", string lang = "",
        CancellationToken ct = default)
        => CreateAsync("csv", name, contests, filter, selectorResult, lang, charset, separator, ct);

    public Task<byte[]> CreateTextAsync(string name, IEnumerable<int> contests, string charset = "",
        string separator = "", string filter = "", string selectorResult = "", string lang = "",
        CancellationToken ct = default)
        => CreateAsync("text", name, contests, filter, selectorResult, lang, charset, separator, ct);

    private Task<byte[]> CreateAsync(string format, string name, IEnumerable<int> contests,
        string filter, string selectorResult, string lang, string charset = "",
        string separator = "", CancellationToken ct = default)
        => e.GetBytesAsync("lists/create",
            new QueryParams()
                .Add("name", name).Add("format", format)
                .Add("charset", charset).Add("separator", separator)
                .AddArray("contest", contests).Add("filter", filter)
                .Add("selectorResult", selectorResult).Add("lang", lang), ct);

    /// <summary>Returns the number of participants in the list which are not activated.</summary>
    public Task<int> ParticipantsNotActivatedAsync(string name, IEnumerable<int> contests,
        bool onlyWithUnderscores = false, CancellationToken ct = default)
        => e.GetAsync<int>("lists/participantsnotactivated",
            new QueryParams().Add("name", name).AddArray("contest", contests)
                .Add("onlyWithUnderscores", onlyWithUnderscores), ct);
}

// ── Pictures ──────────────────────────────────────────────────────────────────

public sealed class PicturesEndpoints(EventApiClient e)
{
    public Task<string[]> NamesAsync(string folder, CancellationToken ct = default)
        => e.GetAsync<string[]>("pictures/names", new QueryParams().Add("folder", folder), ct);

    public Task<byte[]> GetAsync(string name, CancellationToken ct = default)
        => e.GetBytesAsync("pictures/get", new QueryParams().Add("name", name), ct);

    public Task<byte[]> ThumbnailAsync(string name, int maxWidth, int maxHeight,
        CancellationToken ct = default)
        => e.GetBytesAsync("pictures/thumbnail",
            new QueryParams().Add("name", name).Add("maxWidth", maxWidth).Add("maxHeight", maxHeight), ct);

    public Task<string> InfoAsync(string name, CancellationToken ct = default)
        => e.GetAsync<string>("pictures/info", new QueryParams().Add("name", name), ct);

    public Task DeleteAsync(string name, CancellationToken ct = default)
        => e.GetVoidAsync("pictures/delete", new QueryParams().Add("name", name), ct);

    public Task ImportAsync(string folder, string name, byte[] content, CancellationToken ct = default)
        => e.PostVoidAsync("pictures/import",
            new QueryParams().Add("folder", folder).Add("name", name), content, ct);
}

// ── RawDataRules ──────────────────────────────────────────────────────────────

public sealed class RawDataRulesEndpoints(EventApiClient e)
{
    public Task<RawDataRule[]> GetAsync(int id = 0, int resultId = 0, CancellationToken ct = default)
        => e.GetAsync<RawDataRule[]>("rawdatarules/get",
            new QueryParams().Add("id", id).Add("resultID", resultId), ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => e.GetVoidAsync("rawdatarules/delete", new QueryParams().Add("id", id), ct);

    public Task<int[]> SaveAsync(IEnumerable<RawDataRule> items, CancellationToken ct = default)
        => e.PostAsync<int[]>("rawdatarules/save", null, items.ToArray(), ct);
}

// ── Synchronization ───────────────────────────────────────────────────────────

public sealed class SynchronizationEndpoints(EventApiClient e)
{
    /// <summary>Returns true if the file has status "checked out" (online server only).</summary>
    public Task<bool> IsCheckedOutAsync(CancellationToken ct = default)
        => e.GetAsync<bool>("synchronization/isCheckedOut", ct: ct);

    /// <summary>Sets the event status back to "checked in" (online server only).</summary>
    public Task SetCheckedInAsync(CancellationToken ct = default)
        => e.GetVoidAsync("synchronization/setCheckedIn", ct: ct);
}
