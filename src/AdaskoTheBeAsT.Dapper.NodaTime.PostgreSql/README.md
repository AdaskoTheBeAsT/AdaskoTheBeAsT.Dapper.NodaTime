# AdaskoTheBeAsT.Dapper.NodaTime.PostgreSql

[![NuGet](https://img.shields.io/nuget/v/AdaskoTheBeAsT.Dapper.NodaTime.PostgreSql.svg)](https://www.nuget.org/packages/AdaskoTheBeAsT.Dapper.NodaTime.PostgreSql/)

PostgreSQL dialect for [AdaskoTheBeAsT.Dapper.NodaTime](https://www.nuget.org/packages/AdaskoTheBeAsT.Dapper.NodaTime/).
It registers the NodaTime type handlers and shapes every parameter with the matching `NpgsqlDbType` in
addition to `DbType`.

Provider client: `Npgsql`.

## Installation

```bash
dotnet add package AdaskoTheBeAsT.Dapper.NodaTime.PostgreSql
```

## Usage

```csharp
using AdaskoTheBeAsT.Dapper.NodaTime.PostgreSql;
using NodaTime;

PostgreSqlDapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb);
```

Call it once during startup. The dialect is public as `PostgreSqlNodaTimeConfiguration` if you want to pass it
to `DapperNodaTimeSetup.Register` yourself.

## Parameter mapping

| NodaTime type | Value written | `DbType` | `NpgsqlDbType` | Recommended column |
| --- | --- | --- | --- | --- |
| `Instant` | `DateTime` (UTC) | `DateTime2` | `TimestampTz` | `TIMESTAMPTZ` |
| `LocalDate` | `DateTime` at midnight | `Date` | `Date` | `DATE` |
| `LocalDateTime` | `DateTime` (unspecified) | `DateTime2` | `Timestamp` | `TIMESTAMP` |
| `LocalTime` | `TimeSpan` since midnight | `Time` | `Time` | `TIME` |
| `OffsetDateTime` | `DateTimeOffset` converted to UTC | `DateTimeOffset` | `TimestampTz` | `TIMESTAMPTZ` |
| `Offset` | `int` seconds | `Int32` | `Integer` | `INTEGER` |
| `Duration` | `long` nanoseconds | `Int64` | `Bigint` | `BIGINT` |
| `Period` | ISO8601 roundtrip string | `AnsiString` | `Varchar` | `VARCHAR(176)` |
| `CalendarSystem` | calendar id | `AnsiString` | `Varchar` | `VARCHAR(50)` |
| `DateTimeZone` | time zone id | `AnsiString` | `Varchar` | `VARCHAR(50)` |

## Reading notes

Npgsql returns `DateOnly` for `date` columns and `TimeOnly` for `time` columns on modern targets; both are
handled, so `LocalDate` and `LocalTime` round-trip without extra configuration.

## Offset handling

`TIMESTAMPTZ` stores an instant, not an offset. `OffsetDateTime` values are therefore normalized to UTC on the
way in, and read back with offset zero. When the original offset is part of your domain, persist it separately:

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
    CreatedAt TIMESTAMPTZ NOT NULL,     -- Instant
    ScheduledAt TIMESTAMP NOT NULL,     -- LocalDateTime
    EventDate DATE NOT NULL,            -- LocalDate
    StartTime TIME NOT NULL,            -- LocalTime
    StartsAtUtc TIMESTAMPTZ NULL,       -- OffsetDateTime (UTC normalized)
    UtcOffsetSeconds INTEGER NULL,      -- Offset
    RunTimeNanoseconds BIGINT NULL,     -- Duration
    Recurrence VARCHAR(176) NULL,       -- Period
    Calendar VARCHAR(50) NULL,          -- CalendarSystem
    TimeZone VARCHAR(50) NULL);         -- DateTimeZone
```

## Example

```csharp
using Dapper;
using NodaTime;
using Npgsql;

using var connection = new NpgsqlConnection(connectionString);

await connection.ExecuteAsync(
    "INSERT INTO Events (Id, CreatedAt, EventDate, StartTime) VALUES (@Id, @CreatedAt, @EventDate, @StartTime)",
    new
    {
        Id = 1L,
        CreatedAt = SystemClock.Instance.GetCurrentInstant(),
        EventDate = new LocalDate(2025, 11, 23),
        StartTime = new LocalTime(10, 0),
    });
```

## Target frameworks

`net10.0`, `net9.0`, `net8.0`, `net481`, `net48`, `net472`, `net471`, `net47`, `net462`.
`Npgsql` 10.x is used on .NET 8+ and `Npgsql` 8.x on .NET Framework targets.

## Links

- [Repository and full documentation](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.Dapper.NodaTime)
- [Upgrading from 5.x to 6.0](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.Dapper.NodaTime/blob/master/MIGRATION.md)
- [Core package readme](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.Dapper.NodaTime/blob/master/src/AdaskoTheBeAsT.Dapper.NodaTime/README.md)
- [MIT License](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.Dapper.NodaTime/blob/master/LICENSE)
