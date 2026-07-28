# AdaskoTheBeAsT.Dapper.NodaTime.MySql

[![NuGet](https://img.shields.io/nuget/v/AdaskoTheBeAsT.Dapper.NodaTime.MySql.svg)](https://www.nuget.org/packages/AdaskoTheBeAsT.Dapper.NodaTime.MySql/)

MySQL / MariaDB dialect for [AdaskoTheBeAsT.Dapper.NodaTime](https://www.nuget.org/packages/AdaskoTheBeAsT.Dapper.NodaTime/).
It registers the NodaTime type handlers and shapes every parameter with the matching `MySqlDbType` in addition
to `DbType`.

Provider client: `MySqlConnector`.

## Installation

```bash
dotnet add package AdaskoTheBeAsT.Dapper.NodaTime.MySql
```

## Usage

```csharp
using AdaskoTheBeAsT.Dapper.NodaTime.MySql;
using NodaTime;

MySqlDapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb);
```

Call it once during startup. The dialect is public as `MySqlNodaTimeConfiguration` if you want to pass it to
`DapperNodaTimeSetup.Register` yourself.

## Parameter mapping

| NodaTime type | Value written | `DbType` | `MySqlDbType` | Recommended column |
| --- | --- | --- | --- | --- |
| `Instant` | `DateTime` (UTC) | `DateTime2` | `DateTime` | `DATETIME(6)` |
| `LocalDate` | `DateTime` at midnight | `Date` | `Date` | `DATE` |
| `LocalDateTime` | `DateTime` (unspecified) | `DateTime2` | `DateTime` | `DATETIME(6)` |
| `LocalTime` | `TimeSpan` since midnight | `Time` | `Time` | `TIME(6)` |
| `OffsetDateTime` | `DateTime` (UTC part of the offset value) | `DateTime2` | `DateTime` | `DATETIME(6)` |
| `Offset` | `int` seconds | `Int32` | `Int32` | `INT` |
| `Duration` | `long` nanoseconds | `Int64` | `Int64` | `BIGINT` |
| `Period` | ISO8601 roundtrip string | `AnsiString` | `VarChar` | `VARCHAR(176)` |
| `CalendarSystem` | calendar id | `AnsiString` | `VarChar` | `VARCHAR(50)` |
| `DateTimeZone` | time zone id | `AnsiString` | `VarChar` | `VARCHAR(50)` |

Use the `(6)` fractional-second precision shown above; plain `DATETIME` / `TIME` truncate to whole seconds and
will break sub-second round-trips.

## Offset handling

MySQL has no offset-preserving column type, so `OffsetDateTime` is normalized to UTC when written and comes
back with offset zero. Persist the offset separately when it matters:

```csharp
public sealed class Reminder
{
    public LocalDateTime LocalDateTime { get; set; }
    public Offset Offset { get; set; }

    public OffsetDateTime ToOffsetDateTime() => new(LocalDateTime, Offset);
}
```

## Recommended schema

```sql
CREATE TABLE Events (
    Id BIGINT PRIMARY KEY,
    CreatedAt DATETIME(6) NOT NULL,     -- Instant (UTC)
    ScheduledAt DATETIME(6) NOT NULL,   -- LocalDateTime
    EventDate DATE NOT NULL,            -- LocalDate
    StartTime TIME(6) NOT NULL,         -- LocalTime
    StartsAtUtc DATETIME(6) NULL,       -- OffsetDateTime (UTC normalized)
    UtcOffsetSeconds INT NULL,          -- Offset
    RunTimeNanoseconds BIGINT NULL,     -- Duration
    Recurrence VARCHAR(176) NULL,       -- Period
    Calendar VARCHAR(50) NULL,          -- CalendarSystem
    TimeZone VARCHAR(50) NULL);         -- DateTimeZone
```

## Example

```csharp
using Dapper;
using MySqlConnector;
using NodaTime;

using var connection = new MySqlConnection(connectionString);

await connection.ExecuteAsync(
    "INSERT INTO Events (Id, CreatedAt, StartTime) VALUES (@Id, @CreatedAt, @StartTime)",
    new
    {
        Id = 1L,
        CreatedAt = SystemClock.Instance.GetCurrentInstant(),
        StartTime = new LocalTime(10, 0),
    });
```

## Target frameworks

`net10.0`, `net9.0`, `net8.0`, `net481`, `net48`, `net472`, `net471`, `net47`, `net462`.

## Links

- [Repository and full documentation](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.Dapper.NodaTime)
- [Upgrading from 5.x to 6.0](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.Dapper.NodaTime/blob/master/MIGRATION.md)
- [Core package readme](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.Dapper.NodaTime/blob/master/src/AdaskoTheBeAsT.Dapper.NodaTime/README.md)
- [MIT License](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.Dapper.NodaTime/blob/master/LICENSE)
