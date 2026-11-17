using System.Text.Json;
using System.Text.Json.Nodes;

namespace RaceResultClient.Tests;

/// <summary>
/// Real answers of a RaceResult server, recorded by <see cref="LiveTests.RecordFixtures"/>: each must load
/// into the library's type with no field left over, and writing it back must give the same JSON.
/// </summary>
public class FixtureTests
{
    public static IEnumerable<object[]> All() => Fixtures.All();

    [Fact]
    public void FixturesArePresent() => Assert.NotEmpty(Fixtures.All());

    /// <summary>Every field the server sends is a field of the model, and every value fits its type.</summary>
    [Theory]
    [MemberData(nameof(All))]
    public void LoadsWithNoUnknownField(string version, string name)
    {
        var type = Catalog.TypeOf(name);
        if (Catalog.IsOpen(type)) return;
        var json = File.ReadAllText(Fixtures.PathOf(version, name));
        try { JsonSerializer.Deserialize(json, type, Fixtures.Strict); }
        catch (JsonException ex) { Assert.Fail($"{version}/{name} as {type.Name}: {ex.Message}"); }
    }

    /// <summary>The strict check does catch a field the model lacks.</summary>
    [Fact]
    public void StrictRejectsUnknownField()
        => Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<TimingPoint>("""{"Name":"FINISH","NoSuchField":1}""", Fixtures.Strict));

    /// <summary>
    /// Read and written back, nothing the server sent is lost or changed (null may be left out, numbers
    /// compare by value). Fields a newer server added may be written where an older one sent none.
    /// </summary>
    [Theory]
    [MemberData(nameof(All))]
    public void WritesBackWhatItRead(string version, string name)
    {
        var type = Catalog.TypeOf(name);
        if (Catalog.IsOpen(type)) return;
        var json = File.ReadAllText(Fixtures.PathOf(version, name));
        var value = JsonSerializer.Deserialize(json, type, JsonOptions.Default);
        var written = JsonSerializer.Serialize(value, type, JsonOptions.Default);
        var diff = Fixtures.Diff(JsonNode.Parse(json), JsonNode.Parse(written), extraFieldsAllowed: true);
        Assert.True(diff.Count == 0, $"{version}/{name} as {type.Name}:\n  " + string.Join("\n  ", diff.Take(20)));
    }
}
