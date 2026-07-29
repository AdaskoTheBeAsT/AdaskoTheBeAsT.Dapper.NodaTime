# AdaskoTheBeAsT.Dapper.NodaTime.SqlServer

[![NuGet](https://img.shields.io/nuget/v/AdaskoTheBeAsT.Dapper.NodaTime.SqlServer.svg)](https://www.nuget.org/packages/AdaskoTheBeAsT.Dapper.NodaTime.SqlServer/)

SQL Server dialect for [AdaskoTheBeAsT.Dapper.NodaTime](https://www.nuget.org/packages/AdaskoTheBeAsT.Dapper.NodaTime/).
It registers the NodaTime type handlers and shapes every parameter with the matching `System.Data.SqlDbType`
in addition to `DbType`.

Provider client: `Microsoft.Data.SqlClient`.

## Installation

```bash
dotnet add package AdaskoTheBeAsT.Dapper.NodaTime.SqlServer
```

## Usage

```csharp
using AdaskoTheBeAsT.Dapper.NodaTime.SqlServer;
using NodaTime;

SqlServerDapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb);
```

Call it once during startup. `DateTimeZoneProviders.Bcl` can be used instead of `Tzdb` when you need the
Windows registry time zone database.

The dialect itself is public as `SqlServerNodaTimeConfiguration`, so it can be passed to
`DapperNodaTimeSetup.Register` or subclassed.

## Parameter mapping

| NodaTime type | Value written | `DbType` | `SqlDbType` | Recommended column |
| --- | --- | --- | --- | --- |
| `Instant` | `DateTime` (UTC) | `DateTime2` | `DateTime2` | `DATETIME2` |
| `LocalDate` | `DateTime` at midnight | `Date` | `Date` | `DATE` |
| `LocalDateTime` | `DateTime` (unspecified) | `DateTime2` | `DateTime2` | `DATETIME2` |
| `LocalTime` | `TimeSpan` since midnight | `Time` | `Time` | `TIME` |
| `OffsetDateTime` | `DateTimeOffset` | `DateTimeOffset` | `DateTimeOffset` | `DATETIMEOFFSET` |
| `Offset` | `int` seconds | `Int32` | `Int` | `INT` |
| `Duration` | `long` nanoseconds | `Int64` | `BigInt` | `BIGINT` |
| `Period` | ISO8601 roundtrip string | `AnsiString` | `VarChar` | `VARCHAR(176)` |
| `CalendarSystem` | calendar id | `AnsiString` | `VarChar` | `VARCHAR(50)` |
| `DateTimeZone` | time zone id | `AnsiString` | `VarChar` | `VARCHAR(50)` |

SQL Server is the only supported provider that keeps the original UTC offset of an `OffsetDateTime`, thanks to
`DATETIMEOFFSET`.

## Recommended schema

```sql
CREATE TABLE Events (
    Id BIGINT PRIMARY KEY,
    CreatedAt DATETIME2 NOT NULL,          -- Instant
    ScheduledAt DATETIME2 NOT NULL,        -- LocalDateTime
    EventDate DATE NOT NULL,               -- LocalDate
    StartTime TIME NOT NULL,               -- LocalTime
    StartsAtWithOffset DATETIMEOFFSET NULL,-- OffsetDateTime
    UtcOffsetSeconds INT NULL,             -- Offset
    RunTimeNanoseconds BIGINT NULL,        -- Duration
    Recurrence VARCHAR(176) NULL,          -- Period
    Calendar VARCHAR(50) NULL,             -- CalendarSystem
    TimeZone VARCHAR(50) NULL);            -- DateTimeZone
```

## Example

```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using NodaTime;

using var connection = new SqlConnection(connectionString);

await connection.ExecuteAsync(
    "INSERT INTO Events (Id, CreatedAt, ScheduledAt) VALUES (@Id, @CreatedAt, @ScheduledAt)",
    new
    {
        Id = 1L,
        CreatedAt = SystemClock.Instance.GetCurrentInstant(),
        ScheduledAt = new LocalDateTime(2025, 11, 23, 10, 0),
    });
```

## Implementation notes

- Works with any `IDbDataParameter`. The native `SqlDbType` is applied through a cached compiled setter, so
  wrappers that expose a writable `SqlDbType` property (for example profiling or retry decorators) are also
  configured, while parameters without that property simply keep the portable `DbType`.
- Both `Microsoft.Data.SqlClient` and `System.Data.SqlClient` parameter types are handled by that mechanism.

## Target frameworks

`net10.0`, `net9.0`, `net8.0`, `net481`, `net48`, `net472`, `net471`, `net47`, `net462`.

## Links

- [Repository and full documentation](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.Dapper.NodaTime)
- [Upgrading from 5.x to 6.0](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.Dapper.NodaTime/blob/master/MIGRATION.md)
- [Core package readme](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.Dapper.NodaTime/blob/master/src/AdaskoTheBeAsT.Dapper.NodaTime/README.md)
- [MIT License](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.Dapper.NodaTime/blob/master/LICENSE)
