# AdaskoTheBeAsT.Dapper.NodaTime.Oracle

[![NuGet](https://img.shields.io/nuget/v/AdaskoTheBeAsT.Dapper.NodaTime.Oracle.svg)](https://www.nuget.org/packages/AdaskoTheBeAsT.Dapper.NodaTime.Oracle/)

Oracle dialect for [AdaskoTheBeAsT.Dapper.NodaTime](https://www.nuget.org/packages/AdaskoTheBeAsT.Dapper.NodaTime/).
It registers the NodaTime type handlers and shapes every parameter with the matching `OracleDbType`.

Provider client: `Oracle.ManagedDataAccess.Core` on .NET 8+, `Oracle.ManagedDataAccess` on .NET Framework.

## Installation

```bash
dotnet add package AdaskoTheBeAsT.Dapper.NodaTime.Oracle
```

## Usage

```csharp
using AdaskoTheBeAsT.Dapper.NodaTime.Oracle;
using NodaTime;

OracleDapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb);
```

Call it once during startup. The dialect is public as `OracleNodaTimeConfiguration` if you want to pass it to
`DapperNodaTimeSetup.Register` yourself.

## Parameter mapping

| NodaTime type | Value written | `OracleDbType` | Recommended column |
| --- | --- | --- | --- |
| `Instant` | `DateTime` (UTC) | `TimeStamp` | `TIMESTAMP` |
| `LocalDate` | `DateTime` at midnight | `Date` | `DATE` |
| `LocalDateTime` | `DateTime` (unspecified) | `TimeStamp` | `TIMESTAMP` |
| `LocalTime` | `TimeSpan` since midnight | `IntervalDS` | `INTERVAL DAY TO SECOND` |
| `OffsetDateTime` | `DateTimeOffset` | `TimeStampTZ` | `TIMESTAMP WITH TIME ZONE` |
| `Offset` | `int` seconds | `Int32` | `NUMBER(10)` |
| `Duration` | `long` nanoseconds | `Int64` | `NUMBER(19)` |
| `Period` | ISO8601 roundtrip string | `Varchar2` | `VARCHAR2(176)` |
| `CalendarSystem` | calendar id | `Varchar2` | `VARCHAR2(50)` |
| `DateTimeZone` | time zone id | `Varchar2` | `VARCHAR2(50)` |

Oracle specifics worth knowing:

- Oracle has no time-only type, so `LocalTime` is written as an `INTERVAL DAY TO SECOND` and read back from the
  returned `TimeSpan`.
- Oracle `DATE` carries a time component; `LocalDate` is written at midnight and only the date part is used.
- `TIMESTAMP WITH TIME ZONE` preserves the offset, so `OffsetDateTime` round-trips without normalization.
- Numeric values arrive as `decimal` from the provider; `Offset` and `Duration` handlers accept that.

## Recommended schema

```sql
CREATE TABLE Events (
    Id NUMBER(19) PRIMARY KEY,
    CreatedAt TIMESTAMP NOT NULL,                       -- Instant (UTC)
    ScheduledAt TIMESTAMP NOT NULL,                     -- LocalDateTime
    EventDate DATE NOT NULL,                            -- LocalDate
    StartTime INTERVAL DAY TO SECOND NOT NULL,          -- LocalTime
    StartsAtWithOffset TIMESTAMP WITH TIME ZONE NULL,   -- OffsetDateTime
    UtcOffsetSeconds NUMBER(10) NULL,                   -- Offset
    RunTimeNanoseconds NUMBER(19) NULL,                 -- Duration
    Recurrence VARCHAR2(176) NULL,                      -- Period
    Calendar VARCHAR2(50) NULL,                         -- CalendarSystem
    TimeZone VARCHAR2(50) NULL)                         -- DateTimeZone
```

## Example

```csharp
using Dapper;
using NodaTime;
using Oracle.ManagedDataAccess.Client;

using var connection = new OracleConnection(connectionString);

await connection.ExecuteAsync(
    "INSERT INTO Events (Id, CreatedAt, StartTime) VALUES (:Id, :CreatedAt, :StartTime)",
    new
    {
        Id = 1L,
        CreatedAt = SystemClock.Instance.GetCurrentInstant(),
        StartTime = new LocalTime(10, 0),
    });
```

Oracle uses `:` for bind parameters; keep `BindByName` in mind when a command has several parameters.

## Target frameworks

`net10.0`, `net9.0`, `net8.0`, `net481`, `net48`, `net472`.

## Links

- [Repository and full documentation](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.Dapper.NodaTime)
- [Upgrading from 5.x to 6.0](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.Dapper.NodaTime/blob/master/MIGRATION.md)
- [Core package readme](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.Dapper.NodaTime/blob/master/src/AdaskoTheBeAsT.Dapper.NodaTime/README.md)
- [MIT License](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.Dapper.NodaTime/blob/master/LICENSE)
