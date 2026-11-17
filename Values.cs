using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RaceResultClient;

// Value types for what go-model writes as text: its datetime.DateTime and date.Date, and the text
// fields whose values are a known set. Each one round-trips the text RaceResult sends, including a
// value this library does not know yet.

// ── datetime.DateTime ─────────────────────────────────────────────────────────

/// <summary>
/// go-model's <c>datetime.DateTime</c>: either a wall-clock time without a zone (written
/// <c>"2006-01-02"</c> or <c>"2006-01-02 15:04:05"</c>) or an instant with one (RFC 3339,
/// <c>"2006-01-02T15:04:05+01:00"</c>). <c>default</c> is "not set" and is written as <c>""</c>, as
/// are go-model's zero dates (0001-01-01 and Visual Basic's 1899-12-30).
/// A <see cref="DateTime"/> converts implicitly: <see cref="DateTimeKind.Unspecified"/> has no zone,
/// <see cref="DateTimeKind.Utc"/> and <see cref="DateTimeKind.Local"/> have theirs.
/// </summary>
[JsonConverter(typeof(RaceResultDateTimeConverter))]
public readonly struct RaceResultDateTime : IEquatable<RaceResultDateTime>, IComparable<RaceResultDateTime>
{
    private static readonly DateTime ZeroVb = new(1899, 12, 30);

    private readonly DateTime _wallClock;
    private readonly TimeSpan _offset;

    /// <summary>A wall-clock time without a zone (its <see cref="DateTime.Kind"/> is ignored).</summary>
    public static RaceResultDateTime FromWallClock(DateTime wallClock)
        => new(DateTime.SpecifyKind(wallClock, DateTimeKind.Unspecified), TimeSpan.Zero, hasZone: false);

    /// <summary>An instant with its zone's offset.</summary>
    public static RaceResultDateTime FromInstant(DateTimeOffset instant)
        => new(instant.DateTime, instant.Offset, hasZone: true);

    public RaceResultDateTime(DateTime value)
    {
        this = value.Kind == DateTimeKind.Unspecified
            ? FromWallClock(value)
            : FromInstant(new DateTimeOffset(value));
    }

    private RaceResultDateTime(DateTime wallClock, TimeSpan offset, bool hasZone)
    {
        _wallClock = DateTime.SpecifyKind(wallClock, DateTimeKind.Unspecified);
        _offset = offset;
        HasZone = hasZone;
    }

    public static implicit operator RaceResultDateTime(DateTime value) => new(value);
    public static implicit operator RaceResultDateTime(DateTimeOffset value) => FromInstant(value);

    /// <summary>Whether this is an instant with a zone, rather than a wall-clock time.</summary>
    public bool HasZone { get; }

    /// <summary>False for "not set" and go-model's zero dates.</summary>
    public bool IsSet => HasZone
        ? !(Utc == DateTime.MinValue || Utc == ZeroVb)
        : !(_wallClock == DateTime.MinValue || _wallClock == ZeroVb);

    /// <summary>The date and time as written (in its own zone when it has one), <see cref="DateTimeKind.Unspecified"/>.</summary>
    public DateTime WallClock => _wallClock;

    /// <summary>The zone's offset, or null without a zone.</summary>
    public TimeSpan? Offset => HasZone ? _offset : null;

    /// <summary>The instant, or null without a zone (the wall clock alone does not name one).</summary>
    public DateTimeOffset? Instant => HasZone ? new DateTimeOffset(_wallClock, _offset) : null;

    private DateTime Utc => DateTime.SpecifyKind(_wallClock - _offset, DateTimeKind.Utc);

    /// <summary>
    /// As go-model's <c>Before</c>/<c>After</c>: instants when both have a zone; otherwise wall clocks,
    /// the one without a zone read in the other's zone.
    /// </summary>
    public int CompareTo(RaceResultDateTime other)
        => HasZone && other.HasZone ? Utc.CompareTo(other.Utc) : _wallClock.CompareTo(other._wallClock);

    public bool Before(RaceResultDateTime other) => CompareTo(other) < 0;
    public bool After(RaceResultDateTime other) => CompareTo(other) > 0;

    public static bool operator <(RaceResultDateTime a, RaceResultDateTime b) => a.CompareTo(b) < 0;
    public static bool operator >(RaceResultDateTime a, RaceResultDateTime b) => a.CompareTo(b) > 0;
    public static bool operator <=(RaceResultDateTime a, RaceResultDateTime b) => a.CompareTo(b) <= 0;
    public static bool operator >=(RaceResultDateTime a, RaceResultDateTime b) => a.CompareTo(b) >= 0;

    /// <summary>Same text: every "not set" value is equal, the same instant in two zones is not.</summary>
    public bool Equals(RaceResultDateTime other) => ToString() == other.ToString();
    public override bool Equals(object? obj) => obj is RaceResultDateTime o && Equals(o);
    public override int GetHashCode() => ToString().GetHashCode();
    public static bool operator ==(RaceResultDateTime a, RaceResultDateTime b) => a.Equals(b);
    public static bool operator !=(RaceResultDateTime a, RaceResultDateTime b) => !a.Equals(b);

    /// <summary>go-model's text: <c>""</c>, <c>"2006-01-02"</c>, <c>"2006-01-02 15:04:05"</c> or RFC 3339.</summary>
    public override string ToString()
    {
        if (!IsSet) return "";
        if (HasZone)
            return _wallClock.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture) + FormatOffset(_offset);
        return _wallClock.TimeOfDay == TimeSpan.Zero
            ? _wallClock.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
            : _wallClock.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
    }

    internal static string FormatOffset(TimeSpan offset)
        => offset == TimeSpan.Zero
            ? "Z"
            : (offset < TimeSpan.Zero ? "-" : "+") + offset.Duration().ToString(@"hh\:mm", CultureInfo.InvariantCulture);

    /// <summary>Reads go-model's text; <c>""</c> is "not set".</summary>
    public static bool TryParse(string? s, out RaceResultDateTime value)
    {
        value = default;
        if (string.IsNullOrEmpty(s)) return true;

        if (s.Length is 10 or 19 && !s.Contains('T'))
        {
            if (!DateTime.TryParseExact(s, s.Length == 10 ? "yyyy-MM-dd" : "yyyy-MM-dd HH:mm:ss",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var wallClock))
                return false;
            value = FromWallClock(wallClock);
            return true;
        }

        if (DateTimeOffset.TryParseExact(s, ["yyyy-MM-dd'T'HH:mm:ssK", "yyyy-MM-dd'T'HH:mm:ss.FFFFFFFK"],
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var instant)
            && (s.EndsWith('Z') || s.EndsWith("z") || s[^6] is '+' or '-'))
        {
            value = FromInstant(instant);
            return true;
        }
        return false;
    }

    public static RaceResultDateTime Parse(string? s)
        => TryParse(s, out var value) ? value : throw new FormatException($"Not a RaceResult date/time: \"{s}\"");
}

public sealed class RaceResultDateTimeConverter : JsonConverter<RaceResultDateTime>
{
    public override RaceResultDateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return default;
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException($"A RaceResult date/time is text, not {reader.TokenType}.");
        var s = reader.GetString();
        return RaceResultDateTime.TryParse(s, out var value)
            ? value
            : throw new JsonException($"Not a RaceResult date/time: \"{s}\"");
    }

    public override void Write(Utf8JsonWriter writer, RaceResultDateTime value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString());
}

// ── date.Date ─────────────────────────────────────────────────────────────────

/// <summary>
/// go-model's <c>date.Date</c>: a calendar date, written <c>"2006-01-02"</c>. <c>default</c> is
/// "not set" and is written as <c>""</c>, as are go-model's zero dates (0001-01-01, 1899-12-30).
/// </summary>
[JsonConverter(typeof(RaceResultDateConverter))]
public readonly struct RaceResultDate : IEquatable<RaceResultDate>, IComparable<RaceResultDate>
{
    private static readonly DateOnly ZeroVb = new(1899, 12, 30);
    private readonly DateOnly _value;

    public RaceResultDate(DateOnly value) => _value = value;
    public RaceResultDate(int year, int month, int day) => _value = new DateOnly(year, month, day);

    public static implicit operator RaceResultDate(DateOnly value) => new(value);

    public bool IsSet => _value != DateOnly.MinValue && _value != ZeroVb;

    /// <summary>The date, or null when not set.</summary>
    public DateOnly? Value => IsSet ? _value : null;

    public int CompareTo(RaceResultDate other) => (Value ?? DateOnly.MinValue).CompareTo(other.Value ?? DateOnly.MinValue);
    public bool Equals(RaceResultDate other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is RaceResultDate o && Equals(o);
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(RaceResultDate a, RaceResultDate b) => a.Equals(b);
    public static bool operator !=(RaceResultDate a, RaceResultDate b) => !a.Equals(b);

    public override string ToString() => IsSet ? _value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "";

    /// <summary>Reads <c>"2006-01-02"</c> (anything after the date, such as a time, is ignored) or <c>"20060102"</c>; <c>""</c> is "not set".</summary>
    public static bool TryParse(string? s, out RaceResultDate value)
    {
        value = default;
        if (string.IsNullOrEmpty(s)) return true;
        var t = s.IndexOf('T');
        if (t >= 0) s = s[..t];
        if (s.Length > 10) s = s[..10];
        if (!DateOnly.TryParseExact(s, ["yyyy-MM-dd", "yyyyMMdd"], CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var date))
            return false;
        value = new RaceResultDate(date);
        return true;
    }

    public static RaceResultDate Parse(string? s)
        => TryParse(s, out var value) ? value : throw new FormatException($"Not a RaceResult date: \"{s}\"");
}

public sealed class RaceResultDateConverter : JsonConverter<RaceResultDate>
{
    public override RaceResultDate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return default;
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException($"A RaceResult date is text, not {reader.TokenType}.");
        var s = reader.GetString();
        return RaceResultDate.TryParse(s, out var value)
            ? value
            : throw new JsonException($"Not a RaceResult date: \"{s}\"");
    }

    public override void Write(Utf8JsonWriter writer, RaceResultDate value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString());
}

// ── text with a known set of values ──────────────────────────────────────────

/// <summary>
/// A text field whose values are a known set: the known values are static members, any other text
/// is kept as it is. A string converts implicitly; <see cref="Value"/> is the text.
/// </summary>
public interface IStringValue<TSelf> where TSelf : struct, IStringValue<TSelf>
{
    /// <summary>The text; never null (<c>default</c> is <c>""</c>).</summary>
    string Value { get; }

    static abstract TSelf From(string value);
}

public sealed class StringValueConverter<T> : JsonConverter<T> where T : struct, IStringValue<T>
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType switch
        {
            JsonTokenType.Null => default,
            JsonTokenType.String => T.From(reader.GetString() ?? ""),
            _ => throw new JsonException($"{typeof(T).Name} is text, not {reader.TokenType}.")
        };

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Value);
}

// ── an enum RaceResult stores as text ─────────────────────────────────────────

/// <summary>
/// An enum whose number RaceResult stores as text (<c>"4"</c>). 0 is stored as <c>""</c> (as
/// RaceResult's splits page writes "none"), and both <c>""</c> and <c>"0"</c> read as 0. A number this
/// library has no name for still round-trips.
/// </summary>
public sealed class NumberTextEnumConverter<T> : JsonConverter<T> where T : struct, Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        long n;
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return default;
            case JsonTokenType.Number:
                n = reader.GetInt64();
                break;
            case JsonTokenType.String:
                var s = reader.GetString();
                if (string.IsNullOrWhiteSpace(s)) return default;
                if (!long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out n))
                    throw new JsonException($"{typeof(T).Name}: \"{s}\" is not a number.");
                break;
            default:
                throw new JsonException($"{typeof(T).Name} is a number as text, not {reader.TokenType}.");
        }
        return (T)Enum.ToObject(typeof(T), n);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var n = Convert.ToInt64(value, CultureInfo.InvariantCulture);
        writer.WriteStringValue(n == 0 ? "" : n.ToString(CultureInfo.InvariantCulture));
    }
}
