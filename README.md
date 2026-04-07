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
├── Apiclient.cs          # Core HTTP client (auth, request building, error handling)
├── EventApiClient.cs     # Per-event facade; entry point for all event-scoped endpoints
├── EventEndpoints.cs     # All event-scoped endpoint groups (participants, timing, etc.)
├── PublicAndGeneral.cs   # Server-level endpoints (login, event lifecycle, user rights)
├── Models.cs             # All request/response record types and enums
├── Supporting.cs         # QueryParams builder, ApiException, Identifier, JSON config
└── RaceResultClient.csproj
```

---

## Getting Started

### 1. Create a client

```csharp
using RaceResultClient;

var client = new ApiClient(server: "my-raceresult-server.com", useHttps: true);
```

### 2. Authenticate

Three authentication methods are supported:

```csharp
// Username + password
await client.Public.LoginAsync(new LoginOptions { User = "admin", Password = "secret" });

// API key
await client.Public.LoginAsync(new LoginOptions { ApiKey = "your-api-key" });

// RaceResult user token
await client.Public.LoginAsync(new LoginOptions { RrUserToken = "token" });
```

After login, the session token is stored internally and sent with every subsequent request as a `Bearer` token.

### 3. Get an event client

```csharp
// By known event ID
var ev = client.ForEvent("my-event-id");

// Or create a new event and get a client for it
var ev = await client.Public.CreateEventAsync("My Race 2025", new DateTime(2025, 6, 1));

// Or pick from the event list
var events = await client.Public.GetEventListAsync(year: 2025);
var ev = client.ForEvent(events[0].Id);
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
    new Passing {
        Transponder = "ABC123",
        UtcTime = DateTime.UtcNow
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

### Server-level (`client.Public`, `client.General`)

| Endpoint | Description |
|---|---|
| `Public.LoginAsync` | Authenticate and store session |
| `Public.LogoutAsync` | Terminate the current session |
| `Public.GetEventListAsync` | List events for a given year |
| `Public.CreateEventAsync` | Create a new event |
| `Public.DeleteEventAsync` | Permanently delete an event |
| `Public.GetUserInfoAsync` | Get info about the authenticated user |
| `Public.GetUserRightsAsync` | List user access rights for an event |
| `Public.SaveUserRightsAsync` | Grant/update user rights |
| `Public.DeleteUserRightsAsync` | Revoke user rights |
| `General.GetFontsAsync` | List supported font names |
| `General.GetAppVersionAsync` | Get server application version |
| `General.TranslateAsync` | Translate field names or expressions |
| `General.GetLangItemAsync` | Get a single translation string |

### Event-scoped (`ev.*`)

All groups below are properties on `EventApiClient`.

| Group | Description |
|---|---|
| `AgeGroups` | CRUD for age group definitions |
| `BibRanges` | CRUD for bib number ranges |
| `Chat` | Read and post event chat messages |
| `Contests` | CRUD for contest/category definitions |
| `CustomFields` | CRUD for custom participant fields |
| `Data` | Query participant data by field with filtering, sorting, and pagination |
| `EntryFees` | CRUD for entry fee tiers |
| `Exporters` | CRUD and start/stop for data exporters |
| `File` | Upload and download the event file |
| `Forwarding` | Manage and monitor live data forwarding |
| `History` | Read and delete participant field history |
| `OverwriteValues` | CRUD for result overwrite values |
| `Participants` | Full participant lifecycle: create, read, update, delete, import, bib management |
| `Rankings` | CRUD for ranking definitions |
| `RawData` | Read, filter, and delete raw timing data |
| `Registrations` | Process online registration submissions |
| `Results` | CRUD for result column definitions |
| `Settings` | Read and write event settings by name |
| `SimpleApi` | CRUD for Simple API endpoint configurations |
| `Splits` | CRUD for split/checkpoint definitions |
| `Statistics` | Read per-contest registration statistics |
| `TeamScores` | CRUD for team scoring definitions |
| `TimingPoints` | CRUD for timing point definitions |
| `TimingPointRules` | CRUD for timing point decoder routing rules |
| `Times` | Submit passings, read/delete times, swap times between participants |
| `UserDefinedFields` | CRUD for user-defined formula fields |
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

### `QueryParams` — Fluent query string builder

`QueryParams` is a fluent, type-safe builder used internally and exposed for raw calls:

```csharp
var q = new QueryParams()
    .Add("filter", "contest=1")
    .Add("limitFrom", 0)
    .Add("limitTo", 50)
    .AddArray("fields", new[] { "Firstname", "Lastname", "Bib" });
```

### `ApiException` — Error handling

Any non-200 HTTP response throws an `ApiException` carrying the status code and server error message:

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

### Raw / escape-hatch access

If you need to call an endpoint not yet surfaced by the library:

```csharp
byte[] raw = await ev.GetRawAsync("some/undocumented/endpoint");

// Multi-resource request in a single round-trip
var results = await ev.MultiRequestAsync(new[] { "contests/get", "splits/get" });
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
    new Passing
    {
        Transponder = "A1B2C3",
        UtcTime = DateTime.UtcNow,
        DeviceName = "Finish Line Reader"
    }
});

foreach (var r in responses)
    Console.WriteLine($"Bib {r.ResultId}: {r.Time} at {r.TimingPoint}");
```

### Batch-update participant fields

```csharp
await ev.Participants.SaveFieldsAsync(
    id: Identifier.Bib(42),
    values: new Dictionary<string, object?>
    {
        ["Status"] = 1,
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
- **All models** use C# `record` types with `init`-only properties, making them immutable and copy-friendly.
- **Serialization** uses `System.Text.Json` throughout, with case-insensitive deserialization and `JsonStringEnumConverter` registered globally.
- **`CancellationToken`** is a last optional parameter on every async method, defaulting to `default`.

---

## License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).
