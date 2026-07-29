# AdaskoTheBeAsT.Dapper.NodaTime.Sqlite

[![NuGet](https://img.shields.io/nuget/v/AdaskoTheBeAsT.Dapper.NodaTime.Sqlite.svg)](https://www.nuget.org/packages/AdaskoTheBeAsT.Dapper.NodaTime.Sqlite/)

SQLite dialect for [AdaskoTheBeAsT.Dapper.NodaTime](https://www.nuget.org/packages/AdaskoTheBeAsT.Dapper.NodaTime/).
It registers the NodaTime type handlers and shapes every parameter with the matching `SqliteType` in addition
to `DbType`.

Provider client: `Microsoft.Data.Sqlite`.

## Installation

```bash
dotnet add package AdaskoTheBeAsT.Dapper.NodaTime.Sqlite
```

## Usage

```csharp
using AdaskoTheBeAsT.Dapper.NodaTime.Sqlite;
using NodaTime;

SqliteDapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb);
```

Call it once during startup. The dialect is public as `SqliteNodaTimeConfiguration` if you want to pass it to
`DapperNodaTimeSetup.Register` yourself.

## Parameter mapping

SQLite has no date/time storage class, so date-like values are stored as text and numeric values as integers.

| NodaTime type | Value written | `DbType` | `SqliteType` | Recommended column |
| --- | --- | --- | --- | --- |
| `Instant` | `DateTime` (UTC) | `DateTime2` | `Text` | `TEXT` |
| `LocalDate` | `DateTime` at midnight | `Date` | `Text` | `TEXT` |
| `LocalDateTime` | `DateTime` (unspecified) | `DateTime2` | `Text` | `TEXT` |
| `LocalTime` | `TimeSpan` since midnight | `Time` | `Text` | `TEXT` |
| `OffsetDateTime` | `DateTimeOffset` | `DateTimeOffset` | `Text` | `TEXT` |
| `Offset` | `int` seconds | `Int32` | `Integer` | `INTEGER` |
| `Duration` | `long` nanoseconds | `Int64` | `Integer` | `INTEGER` |
| `Period` | ISO8601 roundtrip string | `AnsiString` | `Text` | `TEXT` |
| `CalendarSystem` | calendar id | `AnsiString` | `Text` | `TEXT` |
| `DateTimeZone` | time zone id | `AnsiString` | `Text` | `TEXT` |

Because the values arrive back as strings, the handlers parse them with `CultureInfo.InvariantCulture` and
`DateTimeStyles.RoundtripKind`. Sorting and range filters in SQL work as long as the ISO layout written by
`Microsoft.Data.Sqlite` is preserved.

## Recommended schema

```sql
CREATE TABLE Events (
    Id INTEGER PRIMARY KEY,
    CreatedAt TEXT NOT NULL,        -- Instant
    ScheduledAt TEXT NOT NULL,      -- LocalDateTime
    EventDate TEXT NOT NULL,        -- LocalDate
    StartTime TEXT NOT NULL,        -- LocalTime
    StartsAtWithOffset TEXT NULL,   -- OffsetDateTime
    UtcOffsetSeconds INTEGER NULL,  -- Offset
    RunTimeNanoseconds INTEGER NULL,-- Duration
    Recurrence TEXT NULL,           -- Period
    Calendar TEXT NULL,             -- CalendarSystem
    TimeZone TEXT NULL);            -- DateTimeZone
```

## Example

```csharp
using Dapper;
using Microsoft.Data.Sqlite;
using NodaTime;

using var connection = new SqliteConnection("Data Source=events.db");

await connection.ExecuteAsync(
    "INSERT INTO Events (Id, CreatedAt, ScheduledAt) VALUES (@Id, @CreatedAt, @ScheduledAt)",
    new
    {
        Id = 1L,
        CreatedAt = SystemClock.Instance.GetCurrentInstant(),
        ScheduledAt = new LocalDateTime(2025, 11, 23, 10, 0),
    });
```

## Native library note

`Microsoft.Data.Sqlite` needs a native SQLite build at runtime. If your host application does not already
provide one, add a bundle package such as `SQLitePCLRaw.bundle_e_sqlite3`. On .NET Framework with
`AnyCPU`, prefer the `SQLitePCLRaw.bundle_*` packages: the 3.x `SQLitePCLRaw.lib.*` packages reject
`AnyCPU` builds and require an explicit platform or runtime identifier.

## Target frameworks

`net10.0`, `net9.0`, `net8.0`, `net481`, `net48`, `net472`, `net471`, `net47`, `net462`.

## Links

- [Repository and full documentation](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.Dapper.NodaTime)
- [Upgrading from 5.x to 6.0](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.Dapper.NodaTime/blob/master/MIGRATION.md)
- [Core package readme](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.Dapper.NodaTime/blob/master/src/AdaskoTheBeAsT.Dapper.NodaTime/README.md)
- [MIT License](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.Dapper.NodaTime/blob/master/LICENSE)
