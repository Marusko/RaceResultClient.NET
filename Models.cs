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
    public int SendSMS { get; init; }
    public string Email { get; init; } = "";
    public string AccountNo { get; init; } = "";
    public string BranchNo { get; init; } = "";
    public string Bankname { get; init; } = "";
    public string AccountOwner { get; init; } = "";
    public string Iban { get; init; } = "";
    public string Bic { get; init; } = "";
    public string SepaMandate { get; init; } = "";
    public string Comment { get; init; } = "";
    public string Created { get; init; } = "";
    public string Modified { get; init; } = "";
    public string Uploaded { get; init; } = "";
    public string CreatedBy { get; init; } = "";
    public int ForeignId { get; init; }
    public string RecordPayGuid { get; init; } = "";
    public string ActivationEventId { get; init; } = "";
    public string Opjson { get; init; } = "";
    public string License { get; init; } = "";
    public bool ShowUnderscores { get; init; }
    public int GroupRegPos { get; init; }
    public int GroupId { get; init; }
    public string Password { get; init; } = "";
    public string Voucher { get; init; } = "";
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
    public int StartTransponder { get; init; }
    public int StartResult { get; init; }
    public decimal TimeDifference { get; init; }
    public int FinishResult { get; init; }
    public decimal FinishTimeLimit { get; init; }
    public int Laps { get; init; }
    public int MinResultId { get; init; }
    public decimal MinLapTime { get; init; }
    public int TimingMode { get; init; }
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

public record RaceTime(int Pid, int Result, decimal DecimalTime, string TimeText, string InfoText);

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
    public int Rssi { get; init; }
    public decimal Battery { get; init; }
    public int Temperature { get; init; }
    [JsonPropertyName("WUC")]
    public int WakeupCounter { get; init; }
    public byte LoopId { get; init; }
    public byte Channel { get; init; }
    public string InternalData { get; init; } = "";
    public int StatusFlags { get; init; }
    public string DeviceId { get; init; } = "";
    public string DeviceName { get; init; } = "";
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
    public int ResultId { get; init; }
    public decimal Time { get; init; }
    public string InfoText { get; init; } = "";
    public Passing? Passing { get; init; }
}

public record TimesAddResponseItem(
    int Status, decimal Time, int ResultId, string ResultName,
    int RawDataId, string TimingPoint,
    Dictionary<string, JsonElement>? Fields);

public record RawDataEntry
{
    public int Id { get; init; }
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

public record RawDataDistinctValues
{
    public string[]? DecoderId { get; init; }
    public int[]? OrderId { get; init; }
    public decimal[]? BatteryVoltage { get; init; }
    public int[]? Hits { get; init; }
    public int[]? Rssi { get; init; }
}

public record RawDataFilter
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int[]? Id { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int MinId { get; init; }
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
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[]? DeviceId { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[]? DeviceName { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[]? Transponder { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int[]? OrderId { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int[]? Hits { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int[]? Rssi { get; init; }
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
    public int Id { get; init; }
    public int ResultId { get; init; }
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
    public string GroupName { get; init; } = "";
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
    public int TypeOfSport { get; init; }
    public decimal Distance { get; init; }
    public string DistanceUnit { get; init; } = "";
    public int DistanceFrom { get; init; }
    public decimal TimeMin { get; init; }
    public decimal TimeMax { get; init; }
    public string Color { get; init; } = "";
    public int OrderPos { get; init; }
    public int SplitType { get; init; }
    public int SectorFrom { get; init; }
    public int SectorTo { get; init; }
    public string SpeedOrPace { get; init; } = "";
    public int TimeMode { get; init; }
    public string Label { get; init; } = "";
    public int SectorFrom2 { get; init; }
    public int SectorTo2 { get; init; }
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
    public string Placeholder { get; init; } = "";
    public string Label { get; init; } = "";
    public int OrderPos { get; init; }
    public int MinLen { get; init; }
    public int MaxLen { get; init; }
}

// ── Entry Fees ────────────────────────────────────────────────────────────────

public record EntryFee
{
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

public record EntryFeeItem(int Id, string Name, decimal Fee, string Field, decimal Tax, decimal Multiplication, int ContestId);

// ── Exporters ─────────────────────────────────────────────────────────────────

public record Exporter
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string Filter { get; init; } = "";
    public string TriggerTimingPoint { get; init; } = "";
    public string TriggerSplit { get; init; } = "";
    public int TriggerResultId { get; init; }
    public string DestinationType { get; init; } = "";
    public string Destination { get; init; } = "";
    public string Data { get; init; } = "";
    public int Mtb { get; init; }
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
    public int Id { get; init; }
    public int Bib { get; init; }
    public int PartId { get; init; }
    public DateTime DateTime { get; init; }
    public string FieldName { get; init; } = "";
    public JsonElement? OldValue { get; init; }
    public JsonElement? NewValue { get; init; }
    public string User { get; init; } = "";
    public string Application { get; init; } = "";
}

public record HistoryCount(int Pid, int Count);

public record HistoryFilter
{
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
    public int NotActivated { get; init; }
    public string EventLogo { get; set; } = "";
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
    public decimal TimeDifference { get; init; }
    public decimal FinishTimeLimit { get; init; }
    public string Comment { get; init; } = "";
    public string Filter { get; init; } = "";
}

public record TimingPoint
{
    public string Name { get; init; } = "";
    public int Type { get; init; }
    public int Ddt { get; init; }
    public int IgnoreIfTimeIn { get; init; }
    public decimal IgnoreBefore { get; init; }
    public decimal IgnoreAfter { get; init; }
    public int SubtractT0 { get; init; }
    public int IgnorePs { get; init; }
    public string Position { get; init; } = "";
    public int OrderPos { get; init; }
    public string Color { get; init; } = "";
}

public record TimingPointRule
{
    public int Id { get; init; }
    public string DecoderId { get; init; } = "";
    public string DecoderName { get; init; } = "";
    public byte LoopId { get; init; }
    public byte ChannelId { get; init; }
    public int OrderId { get; init; }
    public decimal MinTime { get; init; }
    public decimal MaxTime { get; init; }
    public int OrderPos { get; init; }
    public string TimingPoint { get; init; } = "";
}

public record TeamScore
{
    public int Id { get; init; }
    public int ResultId1 { get; init; }
    public int ResultId2 { get; init; }
    public int ResultId3 { get; init; }
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

public record SimpleApiItem(bool Disabled, string Key, string Url, string Label);

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

public record ContestStatisticsResult(int Id, int Male, int Female,
    [property: JsonPropertyName("isFormula")] bool IsFormula);

public record ContestStatistics
{
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
    int Pid,
    Dictionary<string, object?> Record,
    string[] Expressions);

public record UserDefinedField(string Name, string Expression, string Note, string Group);

public record OverwriteValue(int Id, int Pid, int ResultId, decimal Value);

// ── Archives ──────────────────────────────────────────────────────────────────

public record ArchiveMatch(int Id, string FirstName, string LastName, int Year);

public record ArchiveParticipant
{
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
    public int MfRank { get; init; }
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
    public int MfRank { get; init; }
    public int AgRank { get; init; }
    public int Bib { get; init; }
}
