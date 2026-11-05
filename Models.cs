using System.Security.Principal;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RaceResultClient;

// Field-type mapping notes (go-model → C#):
//   decimal.Decimal     → decimal          (marshals as a JSON number)
//   date.Date           → string           (marshals as "2006-01-02")
//   datetime.DateTime   → string           (marshals as e.g. "2006-01-02 15:04:05", non-ISO)
//   time.Time           → DateTime         (marshals as RFC3339)
//   variant.Variant     → JsonElement?     (arbitrary JSON value)
//   variant.VariantMap  → Dictionary<string, JsonElement>

// ── Participants & Identity ───────────────────────────────────────────────────

public record Participant
{
    [JsonPropertyName("ID")]
    public int Id { get; init; }
    public int Bib { get; init; }
    public string ForeignKey { get; init; } = "";
    public string Transponder1 { get; init; } = "";
    public string Transponder2 { get; init; } = "";
    public string RegNo { get; init; } = "";
    public string Title { get; init; } = "";
    public string Lastname { get; init; } = "";
    public string Firstname { get; init; } = "";
    public string Sex { get; init; } = "";
    public string DateOfBirth { get; init; } = "";
    public string Street { get; init; } = "";
    [JsonPropertyName("ZIP")]
    public string Zip { get; init; } = "";
    public string City { get; init; } = "";
    public string State2 { get; init; } = "";
    public string Country { get; init; } = "";
    public string Nation { get; init; } = "";
    public int AgeGroup1 { get; init; }
    public int AgeGroup2 { get; init; }
    public int AgeGroup3 { get; init; }
    public string Club { get; init; } = "";
    public int Contest { get; init; }
    public ParticipantStatus Status { get; init; }
    public int Booleans { get; init; }
    public decimal PaidEntryFee { get; init; }
    public string Phone { get; init; } = "";
    public string CellPhone { get; init; } = "";
    public int SendSMS { get; init; }
    public string Email { get; init; } = "";
    public string AccountNo { get; init; } = "";
    public string BranchNo { get; init; } = "";
    public string Bankname { get; init; } = "";
    public string AccountOwner { get; init; } = "";
    [JsonPropertyName("IBAN")]
    public string Iban { get; init; } = "";
    [JsonPropertyName("BIC")]
    public string Bic { get; init; } = "";
    [JsonPropertyName("SEPAMandate")]
    public string SepaMandate { get; init; } = "";
    public string Comment { get; init; } = "";
    public string Created { get; init; } = "";
    public string Modified { get; init; } = "";
    public string Uploaded { get; init; } = "";
    public string CreatedBy { get; init; } = "";
    [JsonPropertyName("ForeignID")]
    public int ForeignId { get; init; }
    [JsonPropertyName("RecordPayGUID")]
    public string RecordPayGuid { get; init; } = "";
    [JsonPropertyName("ActivationEventID")]
    public string ActivationEventId { get; init; } = "";
    [JsonPropertyName("OPJSON")]
    public string Opjson { get; init; } = "";
    public string License { get; init; } = "";
    public bool ShowUnderscores { get; init; }
    public int GroupRegPos { get; init; }
    [JsonPropertyName("GroupID")]
    public int GroupId { get; init; }
    public string Password { get; init; } = "";
    public string Voucher { get; init; } = "";
    public string Language { get; init; } = "";
}

public record ParticipantNewResponse([property: JsonPropertyName("ID")] int Id, int Bib);

public record SaveValueArrayItem(int Bib, [property: JsonPropertyName("PID")] int Pid, string FieldName,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] object? Value);

public record ImportResult(int Added, int Updated, [property: JsonPropertyName("PIDs")] int[] Pids);

public record ParticipantAndDummyCount(int Participants, int Dummies);

/// <summary>A participant's online-payment data (the parsed form of <see cref="Participant.Opjson"/>).</summary>
public record OpData
{
    public decimal EntryFees { get; init; }
    public decimal Processed { get; init; }
    public decimal Received { get; init; }
    public DateTime Date { get; init; }
    public OpDataPayment[]? Payments { get; init; }
}

public record OpDataPayment
{
    [JsonPropertyName("ID")]
    public int Id { get; init; }
    public DateTime Created { get; init; }
    public int Method { get; init; }
    public string Reference { get; init; } = "";
    public decimal Amount { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public decimal Fees { get; init; }
    public string Currency { get; init; } = "";
    public decimal Received { get; init; }
    public string Comment { get; init; } = "";
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Canceled { get; init; }
}

// ── Contests & Age Groups ─────────────────────────────────────────────────────

public record Contest
{
    [JsonPropertyName("ID")]
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string NameShort { get; init; } = "";
    public string Color { get; init; } = "";
    public string AgeStart { get; init; } = "";
    public string AgeEnd { get; init; } = "";
    public string Sex { get; init; } = "";
    public int Day { get; init; }
    public decimal StartTime { get; init; }
    public decimal Length { get; init; }
    public string LengthUnit { get; init; } = "";
    public string TimeFormat { get; init; } = "";
    public int TimeRounding { get; init; }
    public int StartTransponder { get; init; }
    public int StartResult { get; init; }
    public decimal TimeDifference { get; init; }
    public int FinishResult { get; init; }
    public decimal FinishTimeLimit { get; init; }
    public int Laps { get; init; }
    [JsonPropertyName("MinResultID")]
    public int MinResultId { get; init; }
    public decimal MinLapTime { get; init; }
    public ContestTimingMode TimingMode { get; init; }
    public string TimingModeFilter { get; init; } = "";
    public string Attributes { get; init; } = "";
    public double OrderPos { get; init; }
    public string Sort1 { get; init; } = "";
    public string Sort2 { get; init; } = "";
    public string Sort3 { get; init; } = "";
    public string Sort4 { get; init; } = "";
    public bool SortDesc1 { get; init; }
    public bool SortDesc2 { get; init; }
    public bool SortDesc3 { get; init; }
    public bool SortDesc4 { get; init; }
    public bool Inactive { get; init; }
}

public record AgeGroup
{
    [JsonPropertyName("ID")]
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string NameShort { get; init; } = "";
    public string DateStart { get; init; } = "";
    public string DateEnd { get; init; } = "";
    public int AgeFrom { get; init; }
    public int AgeTo { get; init; }
    public int Contest { get; init; }
    [JsonPropertyName("AGSet")]
    public int AgSet { get; init; }
    public int OrderPos { get; init; }
    public string Sex { get; init; } = "";
}

// ── Timing & Raw Data ─────────────────────────────────────────────────────────

public record RaceTime([property: JsonPropertyName("PID")] int Pid, int Result, decimal DecimalTime, string TimeText, string InfoText);

public record PassingPosition
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public double Altitude { get; init; }
    public string Flag { get; init; } = "";
}

public record Passing
{
    public string Transponder { get; init; } = "";
    public PassingPosition? Position { get; init; }
    public int Hits { get; init; }
    [JsonPropertyName("RSSI")]
    public int Rssi { get; init; }
    public decimal Battery { get; init; }
    public int Temperature { get; init; }
    [JsonPropertyName("WUC")]
    public int WakeupCounter { get; init; }
    [JsonPropertyName("LoopID")]
    public byte LoopId { get; init; }
    public byte Channel { get; init; }
    public string InternalData { get; init; } = "";
    public int StatusFlags { get; init; }
    [JsonPropertyName("DeviceID")]
    public string DeviceId { get; init; } = "";
    public string DeviceName { get; init; } = "";
    [JsonPropertyName("OrderID")]
    public int OrderId { get; init; }
    public int Port { get; init; }
    public bool IsMarker { get; init; }
    public int FileNo { get; init; }
    public int PassingNo { get; init; }
    public int Customer { get; init; }
    public DateTime Received { get; init; }
    [JsonPropertyName("UTCTime")]
    public DateTime UtcTime { get; init; }
}

public record PassingToProcess
{
    public int Bib { get; init; }
    public string TimingPoint { get; init; } = "";
    [JsonPropertyName("ResultID")]
    public int ResultId { get; init; }
    public decimal Time { get; init; }
    public string InfoText { get; init; } = "";
    public Passing? Passing { get; init; }
}

public record TimesAddResponseItem(
    int Status, decimal Time, [property: JsonPropertyName("ResultID")] int ResultId, string ResultName,
    [property: JsonPropertyName("RawDataID")] int RawDataId, string TimingPoint,
    Dictionary<string, JsonElement>? Fields);

public record RawDataEntry
{
    [JsonPropertyName("ID")]
    public int Id { get; init; }
    [JsonPropertyName("PID")]
    public int Pid { get; init; }
    public string TimingPoint { get; init; } = "";
    public int Result { get; init; }
    public decimal Time { get; init; }
    public bool Invalid { get; init; }
    public Passing? Passing { get; init; }
}

public record RawDataWithAdditionalFields : RawDataEntry
{
    public int Bib { get; init; }
    public Dictionary<string, JsonElement>? Fields { get; init; }
}

/// <summary>The short form of a raw-data entry, without the full <see cref="Passing"/>.</summary>
public record RawDataReduced
{
    public string TimingPoint { get; init; } = "";
    [JsonPropertyName("PID")]
    public int Pid { get; init; }
    public decimal Time { get; init; }
    public bool Invalid { get; init; }
    [JsonPropertyName("OrderID")]
    public int OrderId { get; init; }
    public int Result { get; init; }
    public bool IsMarker { get; init; }
    [JsonPropertyName("RSSI")]
    public int Rssi { get; init; }
}

public record RawDataDistinctValues
{
    [JsonPropertyName("DecoderID")]
    public string[]? DecoderId { get; init; }
    [JsonPropertyName("OrderID")]
    public int[]? OrderId { get; init; }
    public decimal[]? BatteryVoltage { get; init; }
    public int[]? Hits { get; init; }
    [JsonPropertyName("RSSI")]
    public int[]? Rssi { get; init; }
}

public record RawDataFilter
{
    [JsonPropertyName("ID")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int[]? Id { get; init; }
    [JsonPropertyName("MinID")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int MinId { get; init; }
    [JsonPropertyName("MaxID")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int MaxId { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[]? TimingPoint { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public decimal MinTime { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public decimal MaxTime { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int[]? Result { get; init; }
    [JsonPropertyName("DeviceID")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[]? DeviceId { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[]? DeviceName { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[]? Transponder { get; init; }
    [JsonPropertyName("OrderID")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int[]? OrderId { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int[]? Hits { get; init; }
    [JsonPropertyName("RSSI")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int[]? Rssi { get; init; }
    [JsonPropertyName("LoopID")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public byte[]? LoopId { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public byte[]? Channel { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal[]? Battery { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int[]? Port { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int[]? StatusFlags { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int[]? FileNo { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int[]? PassingNo { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool[]? IsMarker { get; init; }
}

public record RawDataRule
{
    [JsonPropertyName("ID")]
    public int Id { get; init; }
    [JsonPropertyName("ResultID")]
    public int ResultId { get; init; }
    [JsonPropertyName("ContestID")]
    public int ContestId { get; init; }
    public int Mode { get; init; }
    public int N { get; init; }
    public int Min { get; init; }
    public decimal MinOffset { get; init; }
    public int Max { get; init; }
    public decimal MaxOffset { get; init; }
    public int Ref { get; init; }
    public decimal RefOffset { get; init; }
}

// ── Results & Rankings ────────────────────────────────────────────────────────

public record Result
{
    [JsonPropertyName("ID")]
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string Formula { get; init; } = "";
    public string TimeFormat { get; init; } = "";
    public string Location { get; init; } = "";
    public int TimeRounding { get; init; }
    public string Group { get; init; } = "";
}

public record Ranking
{
    [JsonPropertyName("ID")]
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string[] Group { get; init; } = [];
    public string[] Sort { get; init; } = [];
    public bool[] SortDesc { get; init; } = [];
    public bool UseTies { get; init; }
    public bool ContestSort { get; init; }
    public string Filter { get; init; } = "";
    public string GroupName { get; init; } = "";
}

// ── Splits ────────────────────────────────────────────────────────────────────

public record Split
{
    [JsonPropertyName("ID")]
    public int Id { get; init; }
    public int Contest { get; init; }
    public string Name { get; init; } = "";
    public string TimingPoint { get; init; } = "";
    public string Backup { get; init; } = "";
    public decimal BackupOffset { get; init; }
    public TypeOfSport TypeOfSport { get; init; }
    public decimal Distance { get; init; }
    public string DistanceUnit { get; init; } = "";
    public SplitDistanceFrom DistanceFrom { get; init; }
    public decimal TimeMin { get; init; }
    public decimal TimeMax { get; init; }
    public string Color { get; init; } = "";
    public int OrderPos { get; init; }
    public SplitType SplitType { get; init; }
    public int SectorFrom { get; init; }
    public int SectorTo { get; init; }
    /// <summary>
    /// The speed or pace shown for this split. RaceResult stores one of
    /// <see cref="SplitSpeed"/>'s values here as text, so <c>((int)SplitSpeed.Kmh).ToString()</c>
    /// is the value for km/h.
    /// </summary>
    public string SpeedOrPace { get; init; } = "";
    public SplitTimeMode TimeMode { get; init; }
    public string Label { get; init; } = "";
    public int SectorFrom2 { get; init; }
    public int SectorTo2 { get; init; }
}

/// <summary><see cref="Split.SplitType"/> (go-model's <c>SplitType…</c> constants).</summary>
public enum SplitType
{
    Split = 0,
    Internal = 2,
    Leg = 9
}

/// <summary>
/// <see cref="Split.TimeMode"/>: what a split's <see cref="Split.TimeMin"/> and <see cref="Split.TimeMax"/>
/// are (go-model's <c>SplitTimeMode…</c> constants; RaceResult's splits page, "Min./Max.
/// Pace/Speed/Time"). "From Split …" is stored as a value of its own that is none of these, so a
/// value outside this enum is RaceResult's own encoding of a split reference: round-trip it unchanged.
/// </summary>
public enum SplitTimeMode
{
    /// <summary>Race Time: from the start.</summary>
    RaceTime = 0,
    /// <summary>Time of Day.</summary>
    TimeOfDay = -1,
    /// <summary>Time From Prev. Split.</summary>
    FromPreviousSplit = -2,
    /// <summary>A pace in minutes per kilometre.</summary>
    MinPerKm = -3,
    MinPerMile = -4,
    MinPer100m = -5,
    /// <summary>A speed in kilometres per hour.</summary>
    Kmh = -6,
    Mph = -7,
    MetresPerSecond = -8
}

/// <summary>
/// A split's speed or pace display (go-model's <c>SplitSpeed…</c> constants).
/// <see cref="Split.SpeedOrPace"/> holds one of these values as text.
/// </summary>
public enum SplitSpeed
{
    None = 0,
    MinPerKm = 1,
    MinPerMile = 2,
    MinPer100m = 3,
    Kmh = 4,
    Mph = 5,
    MetresPerSecond = 6
}

/// <summary>
/// <see cref="Split.DistanceFrom"/>: what a split's <see cref="Split.Distance"/> is measured from — the
/// start, the previous split, or another split by its <see cref="Split.Id"/> (RaceResult's "From Split …").
/// Use <see cref="SplitDistanceFromExtensions.FromSplit"/> and
/// <see cref="SplitDistanceFromExtensions.SplitId"/> for the split-reference form.
/// </summary>
public enum SplitDistanceFrom
{
    Start = 0,
    PreviousSplit = -1
}

/// <summary>The split-reference form of <see cref="SplitDistanceFrom"/>.</summary>
public static class SplitDistanceFromExtensions
{
    /// <summary>Measured from the split with this <see cref="Split.Id"/>.</summary>
    public static SplitDistanceFrom FromSplit(int splitId) => (SplitDistanceFrom)splitId;

    /// <summary>True when this value references another split rather than naming a fixed origin.</summary>
    public static bool IsSplitReference(this SplitDistanceFrom value) => (int)value > 0;

    /// <summary>The referenced <see cref="Split.Id"/>, or <c>null</c> for a fixed origin.</summary>
    public static int? SplitId(this SplitDistanceFrom value)
        => value.IsSplitReference() ? (int)value : null;
}

/// <summary><see cref="Split.DistanceUnit"/>: the units RaceResult's splits page offers.</summary>
public static class SplitDistanceUnits
{
    public const string Meters = "m";
    public const string Kilometers = "km";
    public const string Miles = "miles";
}

/// <summary>RaceResult's types of sport (<see cref="Split.TypeOfSport"/>, "Type of Sport (up to here)").</summary>
public enum TypeOfSport
{
    None = 0,
    Swimming = 4,
    Skiing = 5,
    Other = 10,
    Cycling = 11,
    Running = 100,
    TransitionFirstRead = 250,
    TransitionLastRead = 251
}

/// <summary><see cref="Contest.TimingMode"/>: whether the contest's results (and its splits) are gun or chip time.</summary>
public enum ContestTimingMode
{
    GunTime = 0,
    ChipTime = 1
}

/// <summary>A participant's <see cref="Participant.Status"/> (RaceResult's knowledge base; <c>StatusText</c> gives the name).</summary>
public enum ParticipantStatus
{
    Regular = 0,
    OutOfCompetition = 1,
    Disqualified = 2,
    DidNotFinish = 3,
    DidNotStart = 4,
    /// <summary>Did not show up (registered, never came to the start).</summary>
    NotPresent = 5
}

// ── Custom Fields ─────────────────────────────────────────────────────────────

public enum CustomFieldType
{
    Text = 0, DropDown = 1, YesNo = 2, Integer = 3,
    Decimal = 4, Date = 5, Currency = 6, Country = 7,
    Email = 8, CellPhone = 9, Transponder = 10
}

public record CustomField
{
    [JsonPropertyName("ID")]
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string AltName { get; init; } = "";
    public string Group { get; init; } = "";
    [JsonPropertyName("Type")]
    public CustomFieldType FieldType { get; init; }
    public bool Enabled { get; init; }
    public bool Mandatory { get; init; }
    public string Config { get; init; } = "";
    public string Default { get; init; } = "";
    public string Placeholder { get; init; } = "";
    public string Label { get; init; } = "";
    public int OrderPos { get; init; }
    public int MinLen { get; init; }
    public int MaxLen { get; init; }
}

// ── Entry Fees ────────────────────────────────────────────────────────────────

public record EntryFee
{
    [JsonPropertyName("ID")]
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public int Contest { get; init; }
    public string DateStart { get; init; } = "";
    public string DateEnd { get; init; } = "";
    public string RegStart { get; init; } = "";
    public string RegEnd { get; init; } = "";
    public string Field { get; init; } = "";
    public string Operator { get; init; } = "";
    public string Value { get; init; } = "";
    public decimal Fee { get; init; }
    public bool ShowAsBasicFee { get; init; }
    public bool IsMultiplicator { get; init; }
    public string Multiplication { get; init; } = "";
    public string Category { get; init; } = "";
    public decimal Tax { get; init; }
    public int OrderPos { get; init; }
}

public record EntryFeeItem([property: JsonPropertyName("ID")] int Id, string Name, decimal Fee, string Field, decimal Tax, decimal Multiplication, [property: JsonPropertyName("ContestID")] int ContestId);

// ── Exporters ─────────────────────────────────────────────────────────────────

public record Exporter
{
    [JsonPropertyName("ID")]
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string Filter { get; init; } = "";
    public string TriggerTimingPoint { get; init; } = "";
    public string TriggerSplit { get; init; } = "";
    [JsonPropertyName("TriggerResultID")]
    public int TriggerResultId { get; init; }
    public string DestinationType { get; init; } = "";
    public string Destination { get; init; } = "";
    public string Data { get; init; } = "";
    [JsonPropertyName("MTB")]
    public int Mtb { get; init; }
    [JsonPropertyName("MQL")]
    public int Mql { get; init; }
    public int ProcessingDelay { get; init; }
    public string LineEnding { get; init; } = "";
    public bool StartPaused { get; init; }
    public decimal IgnoreBefore { get; init; }
    public decimal IgnoreAfter { get; init; }
    public string Encoding { get; init; } = "";
    public string ConnectMsg { get; init; } = "";
    public int OrderPos { get; init; }
}

// ── History ───────────────────────────────────────────────────────────────────

public record HistoryEntry
{
    [JsonPropertyName("ID")]
    public int Id { get; init; }
    public int Bib { get; init; }
    [JsonPropertyName("PartID")]
    public int PartId { get; init; }
    public DateTime DateTime { get; init; }
    public string FieldName { get; init; } = "";
    public JsonElement? OldValue { get; init; }
    public JsonElement? NewValue { get; init; }
    public string User { get; init; } = "";
    public string Application { get; init; } = "";
}

public record HistoryCount([property: JsonPropertyName("PID")] int Pid, int Count);

public record HistoryFilter
{
    [JsonPropertyName("ID")]
    public int[]? Id { get; init; }
    public string[]? Field { get; init; }
    public string[]? OldValue { get; init; }
    public string[]? NewValue { get; init; }
    public string[]? Application { get; init; }
    public string[]? User { get; init; }
    public string? From { get; init; }
    public string? To { get; init; }
    public HistoryParticipantFilter? Participant { get; init; }
}

public record HistoryParticipantFilter([property: JsonPropertyName("ID")] int[]? Id, int[]? Contest, string? Expression);

// ── Auth & User management ────────────────────────────────────────────────────

public record UserInfo(int CustNo, string UserName, string UserPic);

/// <summary>A single setting value, optionally scoped to a contest and/or result.</summary>
public record Setting
{
    public string Name { get; init; } = "";
    // Go marshals a nil variant.Variant as an explicit null, so never omit this.
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public object? Value { get; init; }
    public int Result { get; init; }
    public int Contest { get; init; }
}

public record UserRight([property: JsonPropertyName("UserID")] int UserId, string UserName, string UserPic,
    Dictionary<string, string[]> Rights)
{
    /// <summary>
    /// Whether this user holds <paramref name="right"/>, written as <c>group</c> or <c>group.detail</c>
    /// (go-model's <c>UserRights.Has</c>). <c>"*"</c> as a group or a detail grants everything under it.
    /// </summary>
    public bool Has(string right) => UserRights.Has(Rights, right);
}

/// <summary>Reads a rights map the way go-model's <c>UserRights</c> does.</summary>
public static class UserRights
{
    /// <inheritdoc cref="UserRight.Has"/>
    public static bool Has(IReadOnlyDictionary<string, string[]>? rights, string right)
    {
        if (rights is null) return false;
        if (rights.ContainsKey("*")) return true;

        var parts = right.Split('.');
        if (!rights.TryGetValue(parts[0], out var details)) return false;
        if (parts.Length == 1) return true;

        return details is not null && details.Any(r => r == "*" || r == parts[1]);
    }
}

/// <summary>Login credentials. Populate only the fields appropriate for your auth method.</summary>
public sealed class LoginOptions
{
    public string? User { get; set; }
    public string? Password { get; set; }
    public string? SignInAs { get; set; }
    public string? Totp { get; set; }
    public string? ApiKey { get; set; }
    public string? RrUserToken { get; set; }
}

/// <summary>OAuth2 token returned by <c>public/tokenfromsession</c>.</summary>
public record OAuthToken
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = "";
    [JsonPropertyName("token_type")]
    public string TokenType { get; init; } = "";
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; init; } = "";
    [JsonPropertyName("expiry")]
    public DateTime? Expiry { get; init; }
}

// ── Event list ────────────────────────────────────────────────────────────────

public record EventListItem
{
    [JsonPropertyName("ID")]
    public string Id { get; init; } = "";
    [JsonPropertyName("UserID")]
    public int UserId { get; init; }
    public string UserName { get; init; } = "";
    public bool CheckedOut { get; init; }
    public int Participants { get; init; }
    public string EventName { get; init; } = "";
    public string EventDate { get; init; } = "";
    public string EventDate2 { get; init; } = "";
    public int EventType { get; init; }
    public string EventLocation { get; init; } = "";
    public int EventCountry { get; init; }
    public bool RegActive { get; set; }
    public bool TestMode { get; set; }
    public int NotActivated { get; init; }
    public string EventLogo { get; set; } = "";
}

/// <summary>An event on a RACE RESULT server at the venue (<c>local/eventlist</c>).</summary>
public record LocalEventListItem
{
    [JsonPropertyName("ID")]
    public string Id { get; init; } = "";
    /// <summary>The event file on the server (<c>.ses</c>).</summary>
    public string FileName { get; init; } = "";
    public string EventName { get; init; } = "";
    public string EventDate { get; init; } = "";
    public int EventType { get; init; }
    public bool CheckedOut { get; init; }
    public int Participants { get; init; }
    public int NotActivated { get; init; }
    public string EventLogo { get; init; } = "";
}

// ── Vouchers ──────────────────────────────────────────────────────────────────

public enum VoucherType : byte
{
    Amount = 0, Percent = 1, FirstReg = 2, PrevReg = 3
}

public record Voucher
{
    [JsonPropertyName("ID")]
    public int Id { get; init; }
    public string Code { get; init; } = "";
    public VoucherType Type { get; init; }
    public decimal Amount { get; init; }
    public decimal Tax { get; init; }
    public int[]? Contests { get; init; }
    public string Category { get; init; } = "";
    public string ValidUntil { get; init; } = "";
    public string ValidFrom { get; init; } = "";
    public int Reusable { get; init; }
    public int UseCounter { get; init; }
    public string Remark { get; init; } = "";
    public double OrderPos { get; init; }
}

// ── WebHooks ──────────────────────────────────────────────────────────────────

public enum WebHookType
{
    ParticipantNew = 0, ParticipantUpdated = 1, RawDataNew = 2,
    ModJobId = 3, ModJobIdSettings = 4
}

public record WebHook
{
    [JsonPropertyName("ID")]
    public int Id { get; init; }
    public bool Disabled { get; init; }
    public string Name { get; init; } = "";
    public WebHookType Type { get; init; }
    [JsonPropertyName("URL")]
    public string Url { get; init; } = "";
    public string[] Fields { get; init; } = [];
    public string Filter { get; init; } = "";
    public int OrderPos { get; init; }
}

/// <summary>The body a web hook posts to its <see cref="WebHook.Url"/>.</summary>
public record WebHookMessage
{
    [JsonPropertyName("EventID")]
    public string EventId { get; init; } = "";
    [JsonPropertyName("WebHookID")]
    public int WebHookId { get; init; }
    public DateTime TimeStamp { get; init; }
    public Dictionary<string, JsonElement>? Values { get; init; }
}

// ── Triggers (push subscriptions) ─────────────────────────────────────────────

public enum TriggerType
{
    NewTime = 0,
    NewParticipant = 1,
    NewChatMessage = 3,
    NewRawData = 4,
    Exporter = 5,
    NewRawDataSimple = 6,
    NewSplit = 7,
    ModJobIdSimple = 8,
    SettingValue = 9,
    NewRawDataV2 = 10
}

/// <summary>What a trigger subscribes to.</summary>
public record TriggerSettings
{
    public TriggerType TriggerType { get; init; }
    [JsonPropertyName("PID")]
    public int[]? Pid { get; init; }
    [JsonPropertyName("ResultID")]
    public int[]? ResultId { get; init; }
    [JsonPropertyName("ContestID")]
    public int[]? ContestId { get; init; }
    public string[]? SplitName { get; init; }
    [JsonPropertyName("ExporterID")]
    public int ExporterId { get; init; }
    public string[]? SettingNames { get; init; }
    public string Filter { get; init; } = "";
    public RawDataFilter? RawDataFilter { get; init; }
    public string[]? ReturnFields { get; init; }
}

// ── Misc ──────────────────────────────────────────────────────────────────────

public record BibRange
{
    [JsonPropertyName("ID")]
    public int Id { get; init; }
    public int BibStart { get; init; }
    public int BibEnd { get; init; }
    public int Contest { get; init; }
    public decimal TimeDifference { get; init; }
    public decimal FinishTimeLimit { get; init; }
    public string Comment { get; init; } = "";
    public string Filter { get; init; } = "";
    public bool Disabled { get; init; }
}

public record TimingPoint
{
    public string Name { get; init; } = "";
    public int Type { get; init; }
    [JsonPropertyName("DDT")]
    public int Ddt { get; init; }
    public int IgnoreIfTimeIn { get; init; }
    public decimal IgnoreBefore { get; init; }
    public decimal IgnoreAfter { get; init; }
    public int SubtractT0 { get; init; }
    [JsonPropertyName("IgnorePS")]
    public int IgnorePs { get; init; }
    public string Position { get; init; } = "";
    public int OrderPos { get; init; }
    public string Color { get; init; } = "";
}

public record TimingPointRule
{
    [JsonPropertyName("ID")]
    public int Id { get; init; }
    [JsonPropertyName("DecoderID")]
    public string DecoderId { get; init; } = "";
    public string DecoderName { get; init; } = "";
    [JsonPropertyName("LoopID")]
    public byte LoopId { get; init; }
    [JsonPropertyName("ChannelID")]
    public byte ChannelId { get; init; }
    [JsonPropertyName("OrderID")]
    public int OrderId { get; init; }
    public decimal MinTime { get; init; }
    public decimal MaxTime { get; init; }
    public int OrderPos { get; init; }
    public string TimingPoint { get; init; } = "";
}

public record TeamScore
{
    [JsonPropertyName("ID")]
    public int Id { get; init; }
    [JsonPropertyName("ResultID1")]
    public int ResultId1 { get; init; }
    [JsonPropertyName("ResultID2")]
    public int ResultId2 { get; init; }
    [JsonPropertyName("ResultID3")]
    public int ResultId3 { get; init; }
    [JsonPropertyName("ResultID4")]
    public int ResultId4 { get; init; }
    public int ResultMode1 { get; init; }
    public int ResultMode2 { get; init; }
    public int ResultMode3 { get; init; }
    public int ResultMode4 { get; init; }
    public bool SortDesc1 { get; init; }
    public bool SortDesc2 { get; init; }
    public bool SortDesc3 { get; init; }
    public bool RealTime { get; init; }
    public int MinTotal { get; init; }
    public int MaxTotal { get; init; }
    public int MinFemale { get; init; }
    public int MaxFemale { get; init; }
    public int MaxTeams { get; init; }
    public string Filter { get; init; } = "";
    public string TimeFormat { get; init; } = "";
    public int LapTimes { get; init; }
    public bool LapTimesLemans { get; init; }
    public bool LapTimesZeroStart { get; init; }
    public string Name { get; init; } = "";
    public string LapModeLocation { get; init; } = "";
    public string TeamSort { get; init; } = "";
    public string Assigning1 { get; init; } = "";
    public string Grouping1 { get; init; } = "";
    public string Assigning2 { get; init; } = "";
    public string Grouping2 { get; init; } = "";
    public string Assigning3 { get; init; } = "";
    public string Grouping3 { get; init; } = "";
    public string Assigning4 { get; init; } = "";
    public string Grouping4 { get; init; } = "";
    public bool UseTies { get; init; }
    public bool LapTimesSubtractT0 { get; init; }
    public bool LapTimesCountLemansAsLap { get; init; }
    public int LapTimesPenaltyTimeResult { get; init; }
    public int LapTimesPenaltyLapsResult { get; init; }
    public decimal LapTimesMinLapTime { get; init; }
    public decimal LapTimesIgnoreBefore { get; init; }
    public decimal LapTimesIgnoreAfter { get; init; }
}

public record SimpleApiItem(bool Disabled, string Key, [property: JsonPropertyName("URL")] string Url, string Label);

/// <summary>One row of the event file's generic lookup table.</summary>
public record TableValue
{
    [JsonPropertyName("ID")]
    public int Id { get; init; }
    public string Index1 { get; init; } = "";
    public string Index2 { get; init; } = "";
    public string Index3 { get; init; } = "";
    public string Index4 { get; init; } = "";
    public string Value { get; init; } = "";
}

public record ChatMessage(
    [property: JsonPropertyName("i")] int Id,
    [property: JsonPropertyName("u")] string UserName,
    [property: JsonPropertyName("d")] string Date,
    [property: JsonPropertyName("m")] string Message);

public record Version(
    [property: JsonPropertyName("major")] int Major,
    [property: JsonPropertyName("minor")] int Minor,
    [property: JsonPropertyName("revision")] int Revision,
    [property: JsonPropertyName("tag")] string Tag,
    [property: JsonPropertyName("hash")] string Hash);

public record ChipFileEntry(string Transponder, string Identification);

public record ContestStatisticsResult([property: JsonPropertyName("ID")] int Id, int Male, int Female,
    [property: JsonPropertyName("isFormula")] bool IsFormula);

public record ContestStatistics
{
    [JsonPropertyName("ID")]
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public int Male { get; init; }
    public int Female { get; init; }
    public int Adults { get; init; }
    public int Children { get; init; }
    public double MeanAge { get; init; }
    public int Finished { get; init; }
    public ContestStatisticsResult[]? Results { get; init; }
}

public record ForwardingInfo(int BytesSent, int BytesReceived);

public record GroupTimes
{
    public string Mode { get; init; } = "";
    public string WaveField { get; init; } = "";
    public GroupTime[] Items { get; init; } = [];
}

public record GroupTime
{
    [JsonPropertyName("ID")]
    public JsonElement? Id { get; init; }
    public decimal Time { get; init; }
    public JsonElement? Item { get; init; }
    public int Count { get; init; }
}

public record RegistrationRequest
{
    public string DataToken { get; init; } = "";
    public string PaymentToken { get; init; } = "";
    public string RegName { get; init; } = "";
    public RegistrationRequestRecord[] Records { get; init; } = [];
}

public record RegistrationRequestRecord(
    [property: JsonPropertyName("PID")] int Pid,
    Dictionary<string, object?> Record,
    string[] Expressions);

public record RegistrationRequestResponse
{
    public RegistrationRequestResponseRecord[] Records { get; init; } = [];
    public InvoicePaymentTerms? PaymentTerms { get; init; }
}

public record RegistrationRequestResponseRecord
{
    public Dictionary<string, JsonElement>? Expressions { get; init; }
    public EntryFeeItem[]? EntryFees { get; init; }
    public bool HasDuplicate { get; init; }
}

/// <summary>go-model's <c>invoice.PaymentTerms</c>.</summary>
public record InvoicePaymentTerms
{
    public int Method { get; init; }
    public string AccountOwner { get; init; } = "";
    [JsonPropertyName("IBAN")]
    public string Iban { get; init; } = "";
    [JsonPropertyName("BIC")]
    public string Bic { get; init; } = "";
    public string AccountNo { get; init; } = "";
    public string BranchNo { get; init; } = "";
    public string BankName { get; init; } = "";
    [JsonPropertyName("MandateID")]
    public string MandateId { get; init; } = "";
    public int DueDays { get; init; }
    public string Reference { get; init; } = "";
    public string Terms { get; init; } = "";
}

public record UserDefinedField(string Name, string Expression, string Note, string Group);

public record OverwriteValue([property: JsonPropertyName("ID")] int Id, [property: JsonPropertyName("PID")] int Pid, [property: JsonPropertyName("ResultID")] int ResultId, decimal Value);

// ── Archives ──────────────────────────────────────────────────────────────────

public record ArchiveMatch([property: JsonPropertyName("ID")] int Id, string FirstName, string LastName, int Year);

public record ArchiveEvent
{
    [JsonPropertyName("ID")]
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public string Date { get; init; } = "";
    public ArchiveContest[]? Contests { get; init; }
}

public record ArchiveContest([property: JsonPropertyName("ID")] int Id, string Name);

public record ArchiveParticipant
{
    [JsonPropertyName("ID")]
    public int Id { get; init; }
    public string Transponder1 { get; init; } = "";
    public string Transponder2 { get; init; } = "";
    public string RegNo { get; init; } = "";
    public string Title { get; init; } = "";
    public string Language { get; init; } = "";
    public string Lastname { get; init; } = "";
    public string Firstname { get; init; } = "";
    public string Sex { get; init; } = "";
    public string DateOfBirth { get; init; } = "";
    public string Street { get; init; } = "";
    [JsonPropertyName("ZIP")]
    public string Zip { get; init; } = "";
    public string State { get; init; } = "";
    public string City { get; init; } = "";
    public int Country { get; init; }
    public int Nation { get; init; }
    public string Club { get; init; } = "";
    public string License { get; init; } = "";
    public string Phone { get; init; } = "";
    public string CellPhone { get; init; } = "";
    public string Email { get; init; } = "";
    public Dictionary<string, JsonElement>? AddFields { get; init; }
    public ArchiveParticipation[]? Participations { get; init; }
}

public record ArchiveParticipation
{
    public string Event { get; init; } = "";
    public int Contest { get; init; }
    public string Time { get; init; } = "";
    public int TotRank { get; init; }
    [JsonPropertyName("MFRank")]
    public int MfRank { get; init; }
    [JsonPropertyName("AGRank")]
    public int AgRank { get; init; }
    public int Bib { get; init; }
}

public record ArchiveParticipationExt
{
    public string EventDate { get; init; } = "";
    public string EventName { get; init; } = "";
    public string ContestName { get; init; } = "";
    public string FinalTime { get; init; } = "";
    public int TotRank { get; init; }
    [JsonPropertyName("MFRank")]
    public int MfRank { get; init; }
    [JsonPropertyName("AGRank")]
    public int AgRank { get; init; }
    public int Bib { get; init; }
}
