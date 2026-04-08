using System.Text.Json.Serialization;

namespace RaceResultClient;

// ── Participants & Identity ───────────────────────────────────────────────────

public record Participant
{
    public int Id { get; init; }
    public int Bib { get; init; }
    public string Transponder1 { get; init; } = "";
    public string Transponder2 { get; init; } = "";
    public string RegNo { get; init; } = "";
    public string Title { get; init; } = "";
    public string Lastname { get; init; } = "";
    public string Firstname { get; init; } = "";
    public string Sex { get; init; } = "";
    public string DateOfBirth { get; init; } = "";
    public string Street { get; init; } = "";
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
    public int Status { get; init; }
    public int Booleans { get; init; }
    public decimal PaidEntryFee { get; init; }
    public string Phone { get; init; } = "";
    public string CellPhone { get; init; } = "";
    public string Email { get; init; } = "";
    public string Comment { get; init; } = "";
    public DateTime Created { get; init; }
    public DateTime Modified { get; init; }
    public string ForeignId { get; init; } = "";
    public string Language { get; init; } = "";
}

public record ParticipantNewResponse(int Id, int Bib);

public record SaveValueArrayItem(int Bib, int Pid, string FieldName, object? Value);

public record ImportResult(int Added, int Updated, int[] Pids);

// ── Contests & Age Groups ─────────────────────────────────────────────────────

public record Contest
{
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
    public int Laps { get; init; }
    public bool Inactive { get; init; }
    public string Sort1 { get; init; } = "";
    public string Sort2 { get; init; } = "";
    public string Sort3 { get; init; } = "";
    public string Sort4 { get; init; } = "";
    public bool SortDesc1 { get; init; }
    public bool SortDesc2 { get; init; }
    public bool SortDesc3 { get; init; }
    public bool SortDesc4 { get; init; }
}

public record AgeGroup
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string NameShort { get; init; } = "";
    public string DateStart { get; init; } = "";
    public string DateEnd { get; init; } = "";
    public int AgeFrom { get; init; }
    public int AgeTo { get; init; }
    public int ContestId { get; init; }
    public int AgSet { get; init; }
    public int OrderPos { get; init; }
    public string Sex { get; init; } = "";
}

// ── Timing & Raw Data ─────────────────────────────────────────────────────────

public record RaceTime(int Pid, int Result, decimal DecimalTime, string TimeText, string InfoText);

public record Passing
{
    public string Transponder { get; init; } = "";
    public int Hits { get; init; }
    public int Rssi { get; init; }
    public string DeviceId { get; init; } = "";
    public string DeviceName { get; init; } = "";
    public int OrderId { get; init; }
    public bool IsMarker { get; init; }
    public DateTime Received { get; init; }
    public DateTime UtcTime { get; init; }
}

public record PassingToProcess
{
    public int Bib { get; init; }
    public string TimingPoint { get; init; } = "";
    public int ResultId { get; init; }
    public decimal Time { get; init; }
    public string InfoText { get; init; } = "";
    public Passing? Passing { get; init; }
}

public record TimesAddResponseItem(
    int Status, decimal Time, int ResultId, string ResultName,
    int RawDataId, string TimingPoint,
    Dictionary<string, object?>? Fields);

public record RawDataEntry
{
    public int Id { get; init; }
    public int Pid { get; init; }
    public string TimingPoint { get; init; } = "";
    public int Result { get; init; }
    public decimal Time { get; init; }
    public bool Invalid { get; init; }
    public int Bib { get; init; }
}

public record RawDataFilter
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int[]? Id { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[]? TimingPoint { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public decimal MinTime { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public decimal MaxTime { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int[]? Result { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[]? Transponder { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool[]? IsMarker { get; init; }
}

// ── Results & Rankings ────────────────────────────────────────────────────────

public record Result
{
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
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string[] Group { get; init; } = [];
    public string[] Sort { get; init; } = [];
    public bool[] SortDesc { get; init; } = [];
    public bool UseTies { get; init; }
    public bool ContestSort { get; init; }
    public string Filter { get; init; } = "";
}

// ── Splits ────────────────────────────────────────────────────────────────────

public record Split
{
    public int Id { get; init; }
    public int Contest { get; init; }
    public string Name { get; init; } = "";
    public string TimingPoint { get; init; } = "";
    public string Backup { get; init; } = "";
    public decimal BackupOffset { get; init; }
    public decimal Distance { get; init; }
    public string DistanceUnit { get; init; } = "";
    public int OrderPos { get; init; }
    public int SplitType { get; init; }
    public string Color { get; init; } = "";
}

public static class SplitTypes
{
    public const int Split = 0;
    public const int Internal = 2;
    public const int Leg = 9;
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
    public int OrderPos { get; init; }
}

// ── Entry Fees ────────────────────────────────────────────────────────────────

public record EntryFee
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public int Contest { get; init; }
    public string DateStart { get; init; } = "";
    public string DateEnd { get; init; } = "";
    public decimal Fee { get; init; }
    public decimal Tax { get; init; }
    public int OrderPos { get; init; }
    public string Category { get; init; } = "";
}

public record EntryFeeItem(int Id, string Name, decimal Fee, string Field, decimal Tax, decimal Multiplication);

// ── Exporters ─────────────────────────────────────────────────────────────────

public record Exporter
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string Filter { get; init; } = "";
    public string TriggerTimingPoint { get; init; } = "";
    public string DestinationType { get; init; } = "";
    public string Destination { get; init; } = "";
    public string Data { get; init; } = "";
    public bool StartPaused { get; init; }
    public int OrderPos { get; init; }
}

// ── History ───────────────────────────────────────────────────────────────────

public record HistoryEntry
{
    public int Id { get; init; }
    public int Bib { get; init; }
    public int PartId { get; init; }
    public DateTime DateTime { get; init; }
    public string FieldName { get; init; } = "";
    public object? OldValue { get; init; }
    public object? NewValue { get; init; }
    public string User { get; init; } = "";
    public string Application { get; init; } = "";
}

public record HistoryFilter
{
    public int[]? Id { get; init; }
    public string[]? Field { get; init; }
    public string[]? Application { get; init; }
    public string[]? User { get; init; }
    public DateTime? From { get; init; }
    public DateTime? To { get; init; }
    public HistoryParticipantFilter? Participant { get; init; }
}

public record HistoryParticipantFilter(int[]? Id, int[]? Contest, string? Expression);

// ── Auth & User management ────────────────────────────────────────────────────

public record UserInfo(int CustNo, string UserName, string UserPic);

public record UserRight(int UserId, string UserName, string UserPic,
    Dictionary<string, string[]> Rights);

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

// ── Event list ────────────────────────────────────────────────────────────────

public record EventListItem
{
    public string Id { get; init; } = "";
    public int UserId { get; init; }
    public string UserName { get; init; } = "";
    public bool CheckedOut { get; init; }
    public int Participants { get; init; }
    public string EventName { get; init; } = "";
    public string EventDate { get; init; } = "";
    public int EventType { get; init; }
    public string EventLocation { get; init; } = "";
    public int EventCountry { get; init; }
    public bool RegActive { get; set; }
    public bool TestMode { get; set; }
}

// ── Vouchers ──────────────────────────────────────────────────────────────────

public enum VoucherType : byte
{
    Amount = 0, Percent = 1, FirstReg = 2, PrevReg = 3
}

public record Voucher
{
    public int Id { get; init; }
    public string Code { get; init; } = "";
    public VoucherType Type { get; init; }
    public decimal Amount { get; init; }
    public decimal Tax { get; init; }
    public int[]? Contests { get; init; }
    public string Category { get; init; } = "";
    public int Reusable { get; init; }
    public int UseCounter { get; init; }
    public string Remark { get; init; } = "";
}

// ── WebHooks ──────────────────────────────────────────────────────────────────

public enum WebHookType
{
    ParticipantNew = 0, ParticipantUpdated = 1, RawDataNew = 2,
    ModJobId = 3, ModJobIdSettings = 4
}

public record WebHook
{
    public int Id { get; init; }
    public bool Disabled { get; init; }
    public string Name { get; init; } = "";
    public WebHookType Type { get; init; }
    public string Url { get; init; } = "";
    public string[] Fields { get; init; } = [];
    public string Filter { get; init; } = "";
    public int OrderPos { get; init; }
}

// ── Misc ──────────────────────────────────────────────────────────────────────

public record BibRange
{
    public int Id { get; init; }
    public int BibStart { get; init; }
    public int BibEnd { get; init; }
    public int Contest { get; init; }
    public string Comment { get; init; } = "";
    public string Filter { get; init; } = "";
}

public record TimingPoint
{
    public string Name { get; init; } = "";
    public int Type { get; init; }
    public int OrderPos { get; init; }
    public string Color { get; init; } = "";
    public string Position { get; init; } = "";
}

public record TimingPointRule
{
    public int Id { get; init; }
    public string DecoderId { get; init; } = "";
    public string DecoderName { get; init; } = "";
    public int OrderId { get; init; }
    public decimal MinTime { get; init; }
    public decimal MaxTime { get; init; }
    public int OrderPos { get; init; }
    public string TimingPoint { get; init; } = "";
}

public record TeamScore
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public int ResultId1 { get; init; }
    public int ResultId2 { get; init; }
    public int ResultId3 { get; init; }
    public int ResultId4 { get; init; }
    public int MinTotal { get; init; }
    public int MaxTotal { get; init; }
    public string Filter { get; init; } = "";
    public string TimeFormat { get; init; } = "";
}

public record SimpleApiItem(bool Disabled, string Key, string Url, string Label);

public record ChatMessage(
    [property: JsonPropertyName("i")] int Id,
    [property: JsonPropertyName("u")] string UserName,
    [property: JsonPropertyName("d")] string Date,
    [property: JsonPropertyName("m")] string Message);

public record Version(int Major, int Minor, int Revision, string Tag, string Hash);

public record ChipFileEntry(string Transponder, string Identification);

public record ContestStatistics
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public int Male { get; init; }
    public int Female { get; init; }
    public int Finished { get; init; }
}

public record ForwardingInfo(int BytesSent, int BytesReceived);

public record RegistrationRequest
{
    public string DataToken { get; init; } = "";
    public string PaymentToken { get; init; } = "";
    public string RegName { get; init; } = "";
    public RegistrationRequestRecord[] Records { get; init; } = [];
}

public record RegistrationRequestRecord(
    int Pid,
    Dictionary<string, object?> Record,
    string[] Expressions);

public record UserDefinedField(string Name, string Expression, string Note, string Group);

public record OverwriteValue(int Id, int Pid, int ResultId, decimal Value);