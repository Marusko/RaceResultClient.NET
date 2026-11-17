using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace RaceResultClient.Tests;

/// <summary>Where the recorded answers live, and how they are read and compared.</summary>
public static class Fixtures
{
    /// <summary>The Fixtures folder in the source tree (recording writes there, tests read from there).</summary>
    public static string Root { get; } = Path.Combine(SourceDir(), "Fixtures");

    static string SourceDir([CallerFilePath] string path = "") => Path.GetDirectoryName(path)!;

    /// <summary>Every recorded answer: (server version folder, fixture name, file).</summary>
    public static IEnumerable<object[]> All()
    {
        if (!Directory.Exists(Root)) yield break;
        foreach (var dir in Directory.GetDirectories(Root).Order())
            foreach (var file in Directory.GetFiles(dir, "*.json").Order())
                yield return [Path.GetFileName(dir), Path.GetFileNameWithoutExtension(file)];
    }

    public static string PathOf(string version, string name) => Path.Combine(Root, version, name + ".json");

    /// <summary>A fixture name as a file name ("lists.get[Online|Final]" → "lists.get[Online_Final]").</summary>
    public static string FileName(string name)
        => string.Concat(name.Select(c => Path.GetInvalidFileNameChars().Contains(c) || c == '/' ? '_' : c));

    /// <summary>The library's own JSON settings, with a field the type does not know made an error.</summary>
    public static JsonSerializerOptions Strict { get; } = new(JsonOptions.Default)
    {
        UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow
    };

    /// <summary>Values RaceResult keeps secret, replaced before a fixture is written to the repository.</summary>
    static readonly HashSet<string> Secrets = new(StringComparer.OrdinalIgnoreCase)
    {
        "Key", "ChangeKeySalt", "TestModeKey", "Password", "SMTPPassword", "SMTPUser", "ApiKey", "Token",
        "AuthToken", "Secret", "ClientSecret", "PW", "User",
    };

    /// <summary>A bank account number (IBAN), wherever it is written: a formula, a template's text, …</summary>
    static readonly System.Text.RegularExpressions.Regex Iban =
        new(@"\b[A-Z]{2}\d{2}(?: ?[A-Z0-9]{4}){2,7}(?: ?[A-Z0-9]{1,3})?\b");

    /// <summary>
    /// Replaces secret string values (by field name) and IBANs (anywhere) with x's of the same length.
    /// Free text can hold anything else personal: read a new fixture before committing it, and list an
    /// object that should not be recorded in Fixtures/skip.txt.
    /// </summary>
    public static void Scrub(JsonNode? node)
    {
        switch (node)
        {
            case JsonObject o:
                foreach (var (k, v) in o.ToList())
                {
                    if (v is JsonValue jv && jv.TryGetValue<string>(out var s) && s.Length > 0)
                        o[k] = Secrets.Contains(k) ? new string('x', s.Length) : Iban.Replace(s, m => new string('X', m.Length));
                    else Scrub(v);
                }
                break;
            case JsonArray a:
                foreach (var v in a) Scrub(v);
                break;
        }
    }

    /// <summary>
    /// Differences between what the server sent and what the library writes back: a field the library
    /// leaves out where the server wrote null is the same, numbers compare by value.
    /// </summary>
    public static List<string> Diff(JsonNode? server, JsonNode? ours, string path = "$", bool extraFieldsAllowed = false)
    {
        var acc = new List<string>();
        Walk(server, ours, path, acc, extraFieldsAllowed);
        return acc;
    }

    static void Walk(JsonNode? a, JsonNode? b, string p, List<string> acc, bool extra)
    {
        switch (a)
        {
            case null when b is null:
                return;
            case JsonObject ao when b is JsonObject bo:
                var bk = new Dictionary<string, JsonNode?>(StringComparer.OrdinalIgnoreCase);
                foreach (var (k, v) in bo) bk[k] = v;
                foreach (var (k, v) in ao)
                {
                    if (!bk.Remove(k, out var bv)) { if (v is not null) acc.Add($"{p}.{k}: {Short(v)} → (left out)"); continue; }
                    Walk(v, bv, $"{p}.{k}", acc, extra);
                }
                foreach (var (k, v) in bk) if (v is not null && !extra) acc.Add($"{p}.{k}: (absent) → {Short(v)}");
                return;
            case JsonArray aa when b is JsonArray ba:
                if (aa.Count != ba.Count) { acc.Add($"{p}: {aa.Count} items → {ba.Count}"); return; }
                for (var i = 0; i < aa.Count; i++) Walk(aa[i], ba[i], $"{p}[{i}]", acc, extra);
                return;
            case JsonValue av when b is JsonValue bv:
                if (av.GetValueKind() == JsonValueKind.Number && bv.GetValueKind() == JsonValueKind.Number)
                {
                    if (av.GetValue<decimal>() != bv.GetValue<decimal>()) acc.Add($"{p}: {Short(a)} → {Short(b)}");
                    return;
                }
                if (!JsonNode.DeepEquals(av, bv)) acc.Add($"{p}: {Short(a)} → {Short(b)}");
                return;
            default:
                acc.Add($"{p}: {Short(a)} → {Short(b)}");
                return;
        }
    }

    static string Short(JsonNode? n)
    {
        var s = n?.ToJsonString() ?? "null";
        return s.Length > 70 ? s[..70] + "…" : s;
    }
}
