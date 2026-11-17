# RaceResultClient.NET

A strongly-typed, async .NET client library for the [RaceResult](https://www.raceresult.com/) Web API. It wraps the entire HTTP surface — authentication, event management, participants, timing, results, and more — behind a clean, fluent C# API.

---

## Requirements

- .NET 10.0 or later
- A running RaceResult Web server (on-premise or cloud)

---

## Project Structure

```
RaceResultClient.NET/
├── Apiclient.cs              # Core HTTP client (auth, request building, error handling)
├── EventApiClient.cs         # Per-event facade; entry point for all event-scoped endpoints
├── EventEndpoints.cs         # Core event-scoped endpoint groups (participants, timing, etc.)
├── EventEndpointsDesign.cs   # Additional groups (archives, certificates, lists, labels, …)
├── PublicAndGeneral.cs       # Server-level endpoints (login, event lifecycle, user rights)
├── Models.cs                 # Core request/response record types, enums and RaceResult's codes
├── DesignModels.cs           # Design-object models (certificate, label, list, kiosk, …)
├── WebsiteModels.cs          # go-model's website (event site, participant view) and presenter models
├── PayModels.cs              # go-model's invoice and pay (online payment) models
├── Values.cs                 # RaceResultDateTime, RaceResultDate, string-backed value types, converters
├── Supporting.cs             # QueryParams builder, ApiException, Identifier, JSON config
└── RaceResultClient.csproj
```

This client mirrors the official Go libraries [go-webapi](https://github.com/raceresult/go-webapi)
and [go-model](https://github.com/raceresult/go-model): endpoint paths, query parameters and
model fields track those repositories. Enums serialize numerically (as in Go's `encoding/json`),
except the certificate page-size/format enums which serialize as strings.

Property names follow C# conventions (`Id`, `Url`, `Rssi`), but every property whose Go JSON name
differs carries a `[JsonPropertyName]` attribute (`ID`, `URL`, `RSSI`, …), so the JSON names on the
wire are the ones go-webapi sends and expects. The JSON is not byte-for-byte Go's, though: a `null`
property is left out where Go writes `null`, and a `DateTime` (Go's `time.Time`) carries up to seven
fractional digits where Go writes nine (go-model's own date types are written as Go writes them, see
[Dates](#dates)).

Two of RaceResult's own rules shape the JSON too. A `DateTime` always goes out with a zone, as Go's
`time.Time` needs: one of `DateTimeKind.Unspecified` — an unset `Passing.Received` among them — is
sent as UTC, else `times/add` refuses the whole request. And `+` and non-ASCII text are written as
they are, not `\u002B`: RaceResult tells a date-time string's format by its length in bytes, so
`"…+01:00"` escaped is "date time format not supported" (Registration and Kiosk `EnabledTo`).

---

## Getting Started

### 1. Create a client

```csharp
using RaceResultClient;

var client = new ApiClient(server: "my-raceresult-server.com", useHttps: true);

// Or with a handler of your own (one that checks where it may connect, a proxy, a test double);
// it is disposed with the client
var checkedClient = new ApiClient("my-raceresult-server.com", true, "webapi/1.0", new SocketsHttpHandler());
```

### 2. Authenticate

Three authentication methods are supported:

```csharp
// Username + password (optionally with a TOTP code, or signing in as another user)
await client.Public.LoginAsync(new LoginOptions { User = "admin", Password = "secret" });
await client.Public.LoginAsync(new LoginOptions { User = "admin", Password = "secret", Totp = "123456" });

// API key
await client.Public.LoginAsync(new LoginOptions { ApiKey = "your-api-key" });

// RaceResult user token
await client.Public.LoginAsync(new LoginOptions { RrUserToken = "token" });
```

After login, the session token is stored internally and sent with every subsequent request as a `Bearer` token.
A session obtained elsewhere can be used instead of logging in, and the request timeout — 30 seconds,
as in go-webapi — can be changed before the first request:

```csharp
client.SessionId = sessionIdFromElsewhere;   // go-webapi's SetSessionID
client.Timeout = TimeSpan.FromMinutes(2);    // go-webapi's SetTimeout
```

### 3. Get an event client

```csharp
// By known event ID
var ev = client.ForEvent("my-event-id");

// Or create a new event (online server only) and get a client for it
var ev = await client.Public.CreateEventAsync("My Race 2025", new DateTime(2025, 6, 1));

// Or pick from the event list
var events = await client.Public.GetEventListAsync(year: 2025);
var nextEvents = await client.Public.GetNextEventListAsync();
var pastEvents = await client.Public.GetPastEventListAsync();
var ev = client.ForEvent(events[0].Id);

// A RACE RESULT server at the venue: no login, its own event list (public/eventlist
// would ask it for a token it cannot make); event endpoints then work as usual
using var local = new ApiClient("192.168.1.20", useHttps: false);
var localEvents = await local.Local.GetEventListAsync();
var localEv = local.ForEvent(localEvents[0].Id);
```

### 4. Call endpoints

All endpoint groups are accessed as properties on `EventApiClient`:

```csharp
// Count registered participants
int count = await ev.Data.CountAsync();

// Get all contests
Contest[] contests = await ev.Contests.GetAsync();

// Create a new participant
var result = await ev.Participants.NewAsync(bib: 42, contest: 1);

// Submit a timing passing
await ev.Times.AddAsync(new[] {
    new PassingToProcess {
        Passing = new Passing { Transponder = "ABC123", UtcTime = DateTime.UtcNow }
    }
});
```

### 5. Dispose

`ApiClient` owns an `HttpClient` and implements `IDisposable`:

```csharp
using var client = new ApiClient("my-server.com");
// ...
```

---

## Endpoint Reference

### Server-level (`client.Public`, `client.General`, `client.Local`)

| Endpoint | Description |
|---|---|
| `Public.LoginAsync` | Authenticate and store session |
| `Public.LogoutAsync` | Terminate the current session |
| `Public.GetEventListAsync` | List events for a given year |
| `Public.GetNextEventListAsync` / `Public.GetPastEventListAsync` | List events from today on / up to today |
| `Public.CreateEventAsync` | Create a new event (online server only), optionally as a copy (`copyOf`, `copyMode`) |
| `Public.DeleteEventAsync` | Permanently delete an event |
| `Public.GetUserInfoAsync` | Get info about the authenticated user |
| `Public.TokenFromSessionAsync` | Exchange the session for an OAuth2 token |
| `Public.GetUserRightsAsync` | List user access rights for an event |
| `Public.SaveUserRightsAsync` | Grant/update user rights |
| `Public.DeleteUserRightsAsync` | Revoke user rights |
| `General.GetFontsAsync` | List supported font names |
| `General.GetAppVersionAsync` | Get server application version |
| `General.TranslateAsync` | Translate field names or expressions |
| `General.GetLangItemAsync` | Get a single translation string |
| `Local.GetEventListAsync` | All events on a RACE RESULT server at the venue (no login) |

### Event-scoped (`ev.*`)

All groups below are properties on `EventApiClient`.

| Group | Description |
|---|---|
| `AgeGroups` | CRUD, generate and reassign age group definitions |
| `Archives` | Archive lookup, create/write/import the event archive |
| `Backup` | Start/stop and monitor the backup process |
| `BibRanges` | CRUD for bib number ranges |
| `Certificates` | CRUD for certificate designs; render PDF/JPG/thumbnail |
| `CertificateSets` | CRUD for certificate sets; render and count |
| `Chat` | Read messages, register users, post messages |
| `ChipFile` | Read, save and clear the transponder chip file |
| `Contests` | CRUD for contest/category definitions |
| `CustomFields` | CRUD for custom participant fields |
| `Data` | Query and transform participant data with filtering, sorting, pagination |
| `Dependencies` | Inspect the field dependency tree and circular references |
| `EmailTemplates` | CRUD for email/SMS templates; preview and send |
| `EntryFees` | CRUD for entry fee tiers |
| `Exporters` | CRUD and start/stop for data exporters |
| `File` | File download/upload, activation, version, expression check, modjobid, ownership/rights |
| `Forwarding` | Manage and monitor live data forwarding |
| `GroupTimes` | Read and save group start times / finish time limits |
| `History` | Read, export and delete participant field history |
| `Information` | Frequent first names, gender lookup, adding first names |
| `Kiosks` | CRUD for kiosk configurations |
| `Labels` | CRUD for label designs; render PDF |
| `Lists` | CRUD for list designs; render PDF/HTML/XML/JSON/CSV/XLSX/… |
| `OverwriteValues` | Read, save, delete and count result overwrite values |
| `Participants` | Full participant lifecycle: create, read, update, delete, import (incl. SES), bib management |
| `Pictures` | Manage participant/event pictures and thumbnails |
| `Rankings` | CRUD for ranking definitions |
| `RawData` | Read, filter, export, copy/swap and delete raw timing data |
| `RawDataRules` | CRUD for raw data routing rules |
| `Registrations` | Process submissions; CRUD for registration forms |
| `Results` | CRUD for result column definitions |
| `Settings` | Read, write and delete event settings by name |
| `SimpleApi` | CRUD for Simple API endpoint configurations |
| `Splits` | CRUD for split/checkpoint definitions |
| `Statistics` | CRUD for statistic definitions; render and compute pivots |
| `Synchronization` | Check-out/check-in status (online server) |
| `TeamScores` | CRUD for team scoring definitions |
| `TimingPoints` | CRUD for timing point definitions |
| `TimingPointRules` | CRUD for timing point decoder routing rules |
| `Times` | Submit passings, read/delete/copy/swap times, single-start, random and interpolated times |
| `UserDefinedFields` | Read and overwrite user-defined formula fields |
| `Vouchers` | CRUD for discount vouchers and validation |
| `WebHooks` | CRUD for webhook configurations |

---

## Key Concepts

### `Identifier` — Bib vs. PID

Many endpoints accept either a bib number, an internal participant ID (PID), or a generic internal ID. The `Identifier` struct encodes this choice:

```csharp
// Address by bib number
await ev.Times.GetAsync(Identifier.Bib(101));

// Address by internal participant ID (PID)
await ev.Participants.GetFieldsAsync(Identifier.Pid(5432), new[] { "Firstname", "Lastname" });

// Address by internal ID
await ev.History.GetAsync(Identifier.Id(99));
```

`default(Identifier)` names nothing; an endpoint given one throws an `ArgumentException` before any
request is sent.

### RaceResult's codes

Where a model field or a parameter is a choice among RaceResult's own numbers, the library takes an
`enum`, not an `int`, so a caller keeps no RaceResult numbers of its own. Identifiers stay `int` — a
PID, a bib, a contest, a result or a split `Id` is a number, not a choice. The values are
RaceResult's go-model constants or what RaceResult's own pages write (checked against a splits
definition, a registration's payment methods, a kiosk and an event site set up by hand and exported):

| Enum | Field | Values |
|---|---|---|
| `SplitType` | `Split.SplitType` | `Split` 0, `Internal` 2, `Leg` 9 |
| `SplitTimeMode` | `Split.TimeMode` (what `TimeMin`/`TimeMax` are) | `RaceTime` 0, `TimeOfDay` -1, `FromPreviousSplit` -2, `MinPerKm` -3, `MinPerMile` -4, `MinPer100m` -5, `Kmh` -6, `Mph` -7, `MetresPerSecond` -8 |
| `SplitDistanceFrom` | `Split.DistanceFrom` | `Start` 0, `PreviousSplit` -1, or `SplitDistanceFromExtensions.FromSplit(id)` |
| `TypeOfSport` | `Split.TypeOfSport` | `None` 0, `Swimming` 4, `Skiing` 5, `Other` 10, `Cycling` 11, `Running` 100, `TransitionFirstRead` 250, `TransitionLastRead` 251 |
| `ContestTimingMode` | `Contest.TimingMode` | `GunTime` 0, `ChipTime` 1 |
| `ParticipantStatus` | `Participant.Status` | `Regular` 0, `OutOfCompetition` 1, `Disqualified` 2, `DidNotFinish` 3, `DidNotStart` 4, `NotPresent` 5 |
| `CustomFieldType` | `CustomField.FieldType` | `Text` 0 … `Transponder` 10, `Time` 11 |
| `VoucherType` | `Voucher.Type` | `Amount` 0, `Percent` 1, `FirstReg` 2, `PrevReg` 3 |
| `WebHookType` | `WebHook.Type` | `ParticipantNew` 0 … `ModJobIdSettings` 4 |
| `TriggerType` | `TriggerSettings.TriggerType` | `NewTime` 0 … `NewRawDataV2` 10 |
| `StatisticAggregation` | `Statistics.Aggregation`, `Statistics.ComputeAsync`, `Data.TransformationAsync` | `Count` 1, `Minimum` 2, `Maximum` 3, `Mean` 4, `Sum` 5 |
| `CertificateAlignment` / `CertificateVAlignment` | `CertificateElement.Alignment` / `.VAlignment` | `Left` 1, `Center` 2, `Right` 3 / `Top` 0, `Middle` 1, `Bottom` 2 |
| `KioskTransponderMode` | `Kiosk.TransponderMode` | `ErrorOnMultipleAssignment` 0, `DeleteExistingAssignment` 1, `MultipleAssignmentAllowed` 2 |
| `KioskAcceptedTransponders` | `Kiosk.AcceptedTransponders` | `Any` 0, `OnlyInChipFile` 1 |
| `WebsiteAlignment` | `Alignment` of the site's text, picture, field and compact-row elements | `None` 0, `Left` 1, `Center` 2, `Right` 3 |
| `WebsiteRegIfLoggedIn` | `WebsiteRegConfig.IfLoggedIn`, `WebsiteRegistration.IfLoggedIn` | `Always` 0, `IfLoggedIn` 1, `IfNotLoggedIn` 2 |
| `PaymentMethod` | `RegistrationPaymentMethod.Id`, `InvoicePaymentTerms.Method`, `PayPayment.Method`, `PayMethod.Id`, … | `NoPayment` 0, `CreditCardEur` 2, `MoneyTransferEuropean` 4, `CashAtEvent` 5, `PayPalEur` 7, `SepaDirectDebit` 16, … `OwnStripe` 31, … `OwnTazaPay` 42, named as RaceResult's registration settings list them |
| `PayState` | `PayPayment.PayState` | `Undefined` 0, `Pending` 1, `Underpaid` 2, `Paid` 3, `Overpaid` 4, `NoPayout` 5 |

The design enums (`PageSize`, `PageFormat`, `CertificateElementType`, `LabelAlignment`,
`ListShowAt`, `ListPageBreak`, `EmailTemplateType`, `CertificateSetType`, …) cover the same ground
for the design objects.

Where RaceResult stores a choice as text, the field is a string-backed value type: the known values
are static members (`SplitDistanceUnit.Kilometers`), a string converts implicitly
(`DistanceUnit = "km"`, `tab.Type == "home"`), `.Value` is the text, and text this library does not
know yet is kept as it is and written back unchanged (a plain C# enum would throw on it). RaceResult
adds such values often: four new event-site values in one month.

| Type | Field | Values |
|---|---|---|
| `SplitDistanceUnit` | `Split.DistanceUnit` | `Meters` "m", `Kilometers` "km", `Miles` "miles" |
| `WebsiteTabType`, `WebsiteShowInMenu`, `WebsiteElementType`, `WebsiteElementActive`, `WebsiteFieldDisplayMode`, `WebsiteChartType`, `WebsiteRankScope`, `WebsiteAdditionalFeatureType`, `WebsiteExternalContentSubType`, `WebsiteListsSubtype`, `WebsiteApplyContestColor` | the event site's `Type`, `ShowInMenu2`, `Active`, … | go-model's `website` constants |

`Split.SpeedOrPace` is text holding a number (`"4"`); it is typed as the `SplitSpeed` enum (`None` 0 …
`MetresPerSecond` 6) and still written as text.

### Dates

go-model writes two date types as text, and they are typed here:

| Type | go-model | Text | Fields |
|---|---|---|---|
| `RaceResultDateTime` | `datetime.DateTime` | `"2006-01-02"` or `"2006-01-02 15:04:05"` (a wall clock, no zone), or RFC 3339 `"2006-01-02T15:04:05+01:00"` (an instant) | `EnabledFrom`/`EnabledTo` of registrations, kiosks, presenters and site elements, `Participant.Created`, `Voucher.ValidFrom`, `EventListItem.EventDate`, `WebsiteTab.ActiveFrom`, … |
| `RaceResultDate` | `date.Date` | `"2006-01-02"` | `Participant.DateOfBirth`, `Contest.AgeStart`, `EntryFee.DateStart`, `Invoice.Date`, … |

`default` is "not set" (`IsSet` is false) and is written as `""`, as are go-model's zero dates
(0001-01-01 and 1899-12-30). A `DateTime` converts implicitly — `Unspecified` as a wall clock,
`Utc`/`Local` as an instant — and so does a `DateOnly` to `RaceResultDate`. `WallClock`, `Instant`
(null without a zone) and `RaceResultDate.Value` read them back. Comparison follows go-model: two
instants compare as instants, otherwise the wall clocks compare (`Before`, `After`, `<`, `>`).
Text that is not one of these forms throws a `JsonException`, as go-model's own parser fails on it.

Fields go-model types as `time.Time` stay `DateTime`. In a query string a `DateTime` is sent as
go-webapi sends a `time.Time`: RFC 3339 with the offset (`2025-06-01T00:00:00+02:00`; `Unspecified`
is local time).

```csharp
// Lap 2: 5 km from the previous split, between 5 s and 1 min after it, cycling
var lap = new Split
{
    Contest = 1, Name = "Lap2", TimingPoint = "FINISH",
    TypeOfSport = TypeOfSport.Cycling,
    Distance = 5, DistanceUnit = SplitDistanceUnit.Kilometers, SpeedOrPace = SplitSpeed.Kmh,
    DistanceFrom = SplitDistanceFrom.PreviousSplit,
    TimeMin = 5, TimeMax = 60, TimeMode = SplitTimeMode.FromPreviousSplit,
};
await ev.Splits.SaveAsync([lap]);

// Measured from another split instead
var fromSplit = lap with { DistanceFrom = SplitDistanceFromExtensions.FromSplit(13) };
int? origin = fromSplit.DistanceFrom.SplitId();   // 13
```

Every enum is backed by its RaceResult number and serializes as that number, so a value RaceResult
adds later still round-trips: `Split.TimeMode` also carries RaceResult's own encoding of
"From Split …" (a value of its own, outside `SplitTimeMode`), and reading and saving a split keeps
it unchanged.

### Models go-webapi has no endpoint for

go-model carries a few types that no go-webapi endpoint returns; this library ports them too, so a
caller that meets them elsewhere (a web hook's body, a trigger subscription, a participant's payment
JSON) does not have to write them itself:

| Type | Where it comes from |
|---|---|
| `WebHookMessage` | the body a web hook posts to its `WebHook.Url` |
| `TriggerSettings`, `TriggerType` | a push-trigger subscription |
| `OpData`, `OpDataPayment` | the parsed form of `Participant.Opjson` |
| `RegistrationRequestResponse`, `RegistrationRequestResponseRecord`, `InvoicePaymentTerms` | the answer to a `RegistrationRequest` (`Registrations.ProcessAsync` returns the raw `JsonElement`; deserialize it into these) |
| `RawDataReduced` | the short form of a raw-data entry, without the `Passing` |
| `TableValue` | a row of the event file's generic lookup table |
| `ParticipantAndDummyCount` | a participant count split into real and dummy records |
| `ArchiveEvent`, `ArchiveContest` | an archive's events and their contests |
| `Website`, `WebsiteTab`, `WebsiteElement`, the `Website…Config` records, `WebsiteRegistrationConfig`, `WebsiteTrackingData`, … | the event site (go-model's `website`): its tabs, the participant view's elements, its registration overview and tracking map |
| `Presenter`, `PresenterScreen`, `PresenterWindow`, … | a presenter's screens (go-model's `presenter`) |
| `Invoice`, `InvoiceWithSum`, `InvoiceWithDetails`, `InvoiceSettings`, `InvoiceFilter`, … | invoices (go-model's `invoice`) |
| `PayPayment`, `PayRegistration`, `PayMethod`, `PayCreateRequest`, `PayCaptureResponse`, … | RACE RESULT's online payment service (go-model's `pay`) |

A tab's or element's `Config` is the raw `JsonElement`; deserialize it into the record its `Type`
names (`tab.Config?.Deserialize<WebsiteTabListsConfig>()`). `InvoiceSettings` is stored as the
event's `Invoice…` settings (`InvoiceNumberScheme`, `InvoiceIssuerCompany`, …, `InvoiceExcludedEntryFees`
as `"1,2"`); `ev.Settings.GetInvoiceSettingsAsync()` and `SaveInvoiceSettingsAsync(settings)` read and
save them (not in go-webapi; `InvoiceSettings.FromSettings` / `ToSettings` convert). go-model's helpers are ported with them:
`WebsiteTab.IsVisibleInMenu(t)` / `IsWithinActiveTimeFrame(t)`, `Certificate.PageCount()`,
`GetWidth()`, `GetHeight()`, `GetHoles()`, `CertificatePageSize.Width()` / `Height()` and
`PageSize.ToMm(landscape)`.

`UserRight.Has("participants.edit")` reads a rights map the way go-model's `UserRights.Has` does,
`"*"` as a group or a detail included.

### `QueryParams` — Fluent query string builder

`QueryParams` is a fluent, type-safe builder used internally and exposed for raw calls. `Add` leaves
out an empty string and a numeric `0`; `AddAlways` sends the `0`:

```csharp
var q = new QueryParams()
    .Add("filter", "contest=1")
    .AddAlways("limitFrom", 0)
    .Add("limitTo", 50)
    .AddArray("fields", new[] { "Firstname", "Lastname", "Bib" });

byte[] raw = await ev.GetRawAsync("data/list?" + q.ToQueryString());
```

### `ApiException` — Error handling

Any non-200 HTTP response throws an `ApiException` carrying the status code and server error message
(unless an `ErrorFactory` is set, see below):

```csharp
try
{
    await ev.Participants.DeleteAsync(filter: "bib=999");
}
catch (ApiException ex)
{
    Console.WriteLine($"API error {ex.StatusCode}: {ex.Message}");
}
```

You can also inject a custom exception factory if your application uses its own exception hierarchy:

```csharp
client.ErrorFactory = (message, statusCode) => new MyAppException(message, statusCode);
```

### Raw access

If you need to call an endpoint not yet surfaced by the library:

```csharp
byte[] raw = await ev.GetRawAsync("some/undocumented/endpoint");
byte[] posted = await ev.PostRawAsync("some/undocumented/endpoint", new { Name = "x" });

// Several resources in one round trip (go-webapi's MultiRequest): the answer is keyed by request
Dictionary<string, JsonElement> many = await ev.MultiRequestAsync(requests);

// What RaceResult's web UI calls periodically while an event is open (not in go-webapi);
// it appears to keep the session alive. RaceResult answers 1.
int check = await ev.CheckAsync(lang: "en");
```

---

## Common Usage Examples

### Import participants from a file

```csharp
byte[] fileBytes = File.ReadAllBytes("participants.xlsx");

ImportResult result = await ev.Participants.ImportAsync(
    file: fileBytes,
    addParticipants: true,
    updateParticipants: true
);

Console.WriteLine($"Added: {result.Added}, Updated: {result.Updated}");
```

### Query participant data with field projection

```csharp
var rows = await ev.Data.ListAsync(
    fields: new[] { "Bib", "Firstname", "Lastname", "Contest" },
    filter: "status=0",
    sort: new[] { "Lastname" },
    limitFrom: 0,
    limitTo: 100
);

foreach (var row in rows)
    Console.WriteLine($"{row[0]} — {row[1]} {row[2]}");
```

### Submit a timing passing

```csharp
var responses = await ev.Times.AddAsync(new[]
{
    // A decoder's passing: RaceResult finds the participant by the transponder.
    new PassingToProcess
    {
        Passing = new Passing
        {
            Transponder = "A1B2C3",
            UtcTime = DateTime.UtcNow,
            DeviceName = "Finish Line Reader"
        }
    },
    // A time by bib and timing point, in seconds of the day; the raw data shows the device.
    new PassingToProcess
    {
        Bib = 42,
        TimingPoint = "Finish",
        Time = 36000.5m,
        Passing = new Passing { DeviceId = "Timy", DeviceName = "Finish line" }
    }
});

foreach (var r in responses)
    Console.WriteLine($"Raw data {r.RawDataId}: result {r.ResultId} at {r.TimingPoint}");
```

### Batch-update participant fields

```csharp
await ev.Participants.SaveFieldsAsync(
    id: Identifier.Bib(42),
    values: new Dictionary<string, object?>
    {
        ["Status"] = (int)ParticipantStatus.DidNotStart,
        ["Comment"] = "DNS"
    }
);
```

### Read and write event settings

```csharp
var settings = await ev.Settings.GetAsync(ct, "EventName", "EventDate");

await ev.Settings.SaveAsync("EventName", "My Updated Race Name");
```

---

## Architecture Notes

- **`ApiClient`** owns the `HttpClient` and session state. It is the root object and should be disposed when done.
- **`EventApiClient`** is a lightweight façade created via `client.ForEvent(id)`. It holds only an event ID string and a reference to `ApiClient` — no state of its own. Endpoint group objects (e.g. `ev.Participants`) are instantiated on each property access (no caching overhead, no shared state).
- **Models** are C# `record` types with `init`-only properties, copied with `with`. `LoginOptions` is a
  plain settable class.
- **Serialization** uses `System.Text.Json` throughout, with case-insensitive deserialization; `null`
  properties are left out when writing. Enums serialize as their numbers, as Go's `encoding/json`
  does; the certificate page-size/format enums are the exception and serialize as strings, matching
  go-model's own marshallers.
- **`CancellationToken`** is an optional parameter on every async method, defaulting to `default`. It
  is the last one everywhere except `Settings.GetAsync(ct, params names)`, where it comes first so the
  names can be a `params` list.

---

## License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).
