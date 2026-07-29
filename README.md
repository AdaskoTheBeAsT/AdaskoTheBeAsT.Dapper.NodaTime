# AdaskoTheBeAsT.Dapper.NodaTime

> 🚀 Seamless NodaTime integration for Dapper - because dates and times should just work.

[![CI](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.Dapper.NodaTime/actions/workflows/ci.yml/badge.svg)](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.Dapper.NodaTime/actions/workflows/ci.yml)
[![NuGet Version](https://img.shields.io/nuget/v/AdaskoTheBeAsT.Dapper.NodaTime.svg?style=flat)](https://www.nuget.org/packages/AdaskoTheBeAsT.Dapper.NodaTime/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/AdaskoTheBeAsT.Dapper.NodaTime)](https://www.nuget.org/packages/AdaskoTheBeAsT.Dapper.NodaTime/)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=AdaskoTheBeAsT_AdaskoTheBeAsT.Dapper.NodaTime&metric=alert_status)](https://sonarcloud.io/dashboard?id=AdaskoTheBeAsT_AdaskoTheBeAsT.Dapper.NodaTime)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=AdaskoTheBeAsT_AdaskoTheBeAsT.Dapper.NodaTime&metric=coverage)](https://sonarcloud.io/dashboard?id=AdaskoTheBeAsT_AdaskoTheBeAsT.Dapper.NodaTime)
[![CodeFactor](https://www.codefactor.io/repository/github/adaskothebeast/adaskothebeast.dapper.nodatime/badge)](https://www.codefactor.io/repository/github/adaskothebeast/adaskothebeast.dapper.nodatime)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## Table of Contents

- [Why This Library?](#why-this-library)
- [Packages](#packages)
- [What's New in 6.0](#whats-new-in-60)
- [Quick Start](#quick-start)
- [Supported Types](#supported-types)
- [What Each Handler Reads](#what-each-handler-reads)
- [Recommended Schema](#recommended-schema)
- [Period Format](#period-format)
- [Custom Configuration](#custom-configuration)
- [Known Limitations](#known-limitations)
- [Framework Support](#framework-support)
- [Building and Testing](#building-and-testing)
- [Contributing](#contributing)
- [Credits](#credits)
- [License](#license)

## Why This Library?

Working with dates and times is hard. Working with them across database boundaries is harder. This library bridges the gap between [Dapper](https://github.com/DapperLib/Dapper)'s simplicity and [NodaTime](https://nodatime.org/)'s correctness, giving you:

- ✅ **Type-safe** date/time round-trips across your data layer
- ✅ **Explicit database dialects** - one package per provider, no guessing
- ✅ **Native parameter types** (`SqlDbType`, `NpgsqlDbType`, `MySqlDbType`, `SqliteType`, `OracleDbType`), not just `DbType`
- ✅ **Integration-tested** against real SQL Server, PostgreSQL, MySQL, Oracle and SQLite instances (Testcontainers), with 100% line and branch coverage
- ✅ **Broad TFM span** - .NET Framework 4.6.2+ through .NET 10, plus netstandard2.0/2.1
- ✅ **Actively maintained** continuation of the original Dapper-NodaTime project

## Packages

Install exactly one provider package; it pulls the core package in transitively.

| Package | Provider client | NuGet |
| --- | --- | --- |
| [`AdaskoTheBeAsT.Dapper.NodaTime`](src/AdaskoTheBeAsT.Dapper.NodaTime/README.md) | none (core handlers only) | [![NuGet](https://img.shields.io/nuget/v/AdaskoTheBeAsT.Dapper.NodaTime.svg)](https://www.nuget.org/packages/AdaskoTheBeAsT.Dapper.NodaTime/) |
| [`...SqlServer`](src/AdaskoTheBeAsT.Dapper.NodaTime.SqlServer/README.md) | `Microsoft.Data.SqlClient` | [![NuGet](https://img.shields.io/nuget/v/AdaskoTheBeAsT.Dapper.NodaTime.SqlServer.svg)](https://www.nuget.org/packages/AdaskoTheBeAsT.Dapper.NodaTime.SqlServer/) |
| [`...PostgreSql`](src/AdaskoTheBeAsT.Dapper.NodaTime.PostgreSql/README.md) | `Npgsql` | [![NuGet](https://img.shields.io/nuget/v/AdaskoTheBeAsT.Dapper.NodaTime.PostgreSql.svg)](https://www.nuget.org/packages/AdaskoTheBeAsT.Dapper.NodaTime.PostgreSql/) |
| [`...MySql`](src/AdaskoTheBeAsT.Dapper.NodaTime.MySql/README.md) | `MySqlConnector` | [![NuGet](https://img.shields.io/nuget/v/AdaskoTheBeAsT.Dapper.NodaTime.MySql.svg)](https://www.nuget.org/packages/AdaskoTheBeAsT.Dapper.NodaTime.MySql/) |
| [`...Sqlite`](src/AdaskoTheBeAsT.Dapper.NodaTime.Sqlite/README.md) | `Microsoft.Data.Sqlite` | [![NuGet](https://img.shields.io/nuget/v/AdaskoTheBeAsT.Dapper.NodaTime.Sqlite.svg)](https://www.nuget.org/packages/AdaskoTheBeAsT.Dapper.NodaTime.Sqlite/) |
| [`...Oracle`](src/AdaskoTheBeAsT.Dapper.NodaTime.Oracle/README.md) | `Oracle.ManagedDataAccess[.Core]` | [![NuGet](https://img.shields.io/nuget/v/AdaskoTheBeAsT.Dapper.NodaTime.Oracle.svg)](https://www.nuget.org/packages/AdaskoTheBeAsT.Dapper.NodaTime.Oracle/) |

Upgrading from 5.x? See [MIGRATION.md](MIGRATION.md).

## What's New in 6.0

Until 5.x this was a single package that always shaped parameters the SQL Server way: every handler set a
`SqlDbType`, so PostgreSQL, MySQL, SQLite and Oracle users got whatever `DbType` their provider happened to infer.
Making the dialect an explicit choice is a breaking API change, hence the major version bump to **6.0.0**.

### Why the version jumped to 6.0

| Change | Impact |
| --- | --- |
| 🧩 **One package per database** - core + `SqlServer`, `PostgreSql`, `MySql`, `Sqlite`, `Oracle` | Install a provider package instead of the single old package |
| ⚙️ **The dialect is explicit** - `DapperNodaTimeSetup.Register` now needs an `INodaTimeTypeHandlerConfiguration` | The old `Register(provider)` overload is gone; call `XxxDapperNodaTimeSetup.Register(provider)` |
| 🏗️ **Handlers take a configuration** | The `XxxHandler.Default` singletons were removed |
| 🧼 **SQL Server specifics left the core** - `SetSqlDbType` moved into the SqlServer package and is internal | Core now emits portable `DbType` only |
| 🕰️ **`Period` is registered** - 5.x shipped `PeriodHandler` but never wired it up | `Period` properties now round-trip out of the box |
| 🌍 **PostgreSQL and MySQL normalize `OffsetDateTime` to UTC** | Matches the column types those providers use; store `Offset` separately if you need the original |

### What you also get

- 🎯 **Native provider parameter types** - `SqlDbType`, `NpgsqlDbType`, `MySqlDbType`, `SqliteType`, `OracleDbType`
- 📚 **Wider framework reach** - added `netstandard2.1` and `net462`-`net481` next to `netstandard2.0` and .NET 8/9/10
- 🔓 **Dependency version ranges** - `Dapper [2.1.79,3.0.0)` and `NodaTime [3.3.3,4.0.0)` instead of pinned versions
- 🧪 **More tolerant reads** - ISO strings, `decimal`, `int`/`long`, and `DateOnly`/`TimeOnly` on .NET 8+
- ✅ **100% line and branch coverage**, with integration tests against real databases for all five providers
- 📦 **Per-package documentation** so each NuGet page describes only its own dialect

Step-by-step upgrade instructions, a full breaking-change table and troubleshooting are in
[MIGRATION.md](MIGRATION.md).

## Quick Start

### 1. Install

```bash
dotnet add package AdaskoTheBeAsT.Dapper.NodaTime.SqlServer
```

```powershell
Install-Package AdaskoTheBeAsT.Dapper.NodaTime.SqlServer
```

### 2. Register once at startup

```csharp
using AdaskoTheBeAsT.Dapper.NodaTime.SqlServer;
using NodaTime;

SqlServerDapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb);
```

One call registers handlers for all supported NodaTime types. Other providers expose the same shape:
`PostgreSqlDapperNodaTimeSetup.Register`, `MySqlDapperNodaTimeSetup.Register`,
`SqliteDapperNodaTimeSetup.Register`, `OracleDapperNodaTimeSetup.Register`.

Pick the time zone provider that matches your deployment:

```csharp
SqlServerDapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb); // IANA, recommended
SqlServerDapperNodaTimeSetup.Register(DateTimeZoneProviders.Bcl);  // Windows registry
```

### 3. Use NodaTime types directly

```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using NodaTime;

public sealed class Event
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public LocalDateTime ScheduledAt { get; set; }
    public Instant CreatedAt { get; set; }
    public Duration Duration { get; set; }
}

using var connection = new SqlConnection(connectionString);

await connection.ExecuteAsync(
    "INSERT INTO Events (Name, ScheduledAt, CreatedAt, Duration) VALUES (@Name, @ScheduledAt, @CreatedAt, @Duration)",
    new Event
    {
        Name = "Team Meeting",
        ScheduledAt = new LocalDateTime(2025, 11, 23, 10, 0),
        CreatedAt = SystemClock.Instance.GetCurrentInstant(),
        Duration = Duration.FromHours(1),
    });

var events = await connection.QueryAsync<Event>(
    "SELECT Id, Name, ScheduledAt, CreatedAt, Duration FROM Events WHERE ScheduledAt > @date",
    new { date = new LocalDateTime(2025, 11, 22, 9, 0) });
```

## Supported Types

Column types below are what each provider package writes parameters as (native provider type in parentheses).

| NodaTime type | SQL Server | PostgreSQL | MySQL | SQLite | Oracle |
| --- | --- | --- | --- | --- | --- |
| `Instant` | `datetime2` (`DateTime2`) | `timestamptz` (`TimestampTz`) | `DATETIME(6)` UTC (`DateTime`) | `TEXT` (`Text`) | `TIMESTAMP` UTC (`TimeStamp`) |
| `LocalDate` | `date` (`Date`) | `date` (`Date`) | `DATE` (`Date`) | `TEXT` (`Text`) | `DATE` (`Date`) |
| `LocalDateTime` | `datetime2` (`DateTime2`) | `timestamp` (`Timestamp`) | `DATETIME(6)` (`DateTime`) | `TEXT` (`Text`) | `TIMESTAMP` (`TimeStamp`) |
| `LocalTime` | `time` (`Time`) | `time` (`Time`) | `TIME(6)` (`Time`) | `TEXT` (`Text`) | `INTERVAL DAY TO SECOND` (`IntervalDS`) |
| `OffsetDateTime` | `datetimeoffset` (`DateTimeOffset`) | `timestamptz` UTC (`TimestampTz`) | `DATETIME(6)` UTC (`DateTime`) | `TEXT` (`Text`) | `TIMESTAMP WITH TIME ZONE` (`TimeStampTZ`) |
| `Offset` | `int` seconds (`Int`) | `integer` seconds (`Integer`) | `INT` seconds (`Int32`) | `INTEGER` seconds (`Integer`) | `NUMBER(10)` seconds (`Int32`) |
| `Duration` | `bigint` nanoseconds (`BigInt`) | `bigint` nanoseconds (`Bigint`) | `BIGINT` nanoseconds (`Int64`) | `INTEGER` nanoseconds (`Integer`) | `NUMBER(19)` nanoseconds (`Int64`) |
| `Period` | `varchar(176)` (`VarChar`) | `varchar(176)` (`Varchar`) | `VARCHAR(176)` (`VarChar`) | `TEXT` (`Text`) | `VARCHAR2(176)` (`Varchar2`) |
| `CalendarSystem` | `varchar(50)` id (`VarChar`) | `varchar(50)` id (`Varchar`) | `VARCHAR(50)` id (`VarChar`) | `TEXT` id (`Text`) | `VARCHAR2(50)` id (`Varchar2`) |
| `DateTimeZone` | `varchar(50)` id (`VarChar`) | `varchar(50)` id (`Varchar`) | `VARCHAR(50)` id (`VarChar`) | `TEXT` id (`Text`) | `VARCHAR2(50)` id (`Varchar2`) |

## What Each Handler Reads

Reading is deliberately permissive, so the same model works across providers and legacy schemas.

| NodaTime type | Accepted database values |
| --- | --- |
| `Instant` | `Instant`, `DateTime`, `DateTimeOffset`, ISO string |
| `LocalDate` | `LocalDate`, `DateTime`, `DateOnly` (.NET 8+), ISO string |
| `LocalDateTime` | `LocalDateTime`, `DateTime`, ISO string |
| `LocalTime` | `LocalTime`, `TimeSpan`, `DateTime`, `TimeOnly` (.NET 8+), ISO string |
| `OffsetDateTime` | `OffsetDateTime`, `DateTimeOffset`, `DateTime` (treated as UTC), ISO string |
| `Offset` | `Offset`, `int`, `long`, `decimal` (seconds) |
| `Duration` | `Duration`, `long`, `int`, `decimal` (nanoseconds) |
| `Period` | `Period`, ISO8601 roundtrip string |
| `CalendarSystem` | `CalendarSystem`, calendar id string |
| `DateTimeZone` | `DateTimeZone`, time zone id string |

Anything else throws a `DataException` with the offending CLR type, instead of silently producing a wrong value.

## Recommended Schema

<details>
<summary>SQL Server</summary>

```sql
CREATE TABLE NodaTimeRoundTrip (
    Id BIGINT PRIMARY KEY,
    InstantValue DATETIME2 NULL,
    LocalDateValue DATE NULL,
    LocalDateTimeValue DATETIME2 NULL,
    LocalTimeValue TIME NULL,
    OffsetDateTimeValue DATETIMEOFFSET NULL,
    OffsetValue INT NULL,
    DurationValue BIGINT NULL,
    PeriodValue VARCHAR(176) NULL,
    CalendarValue VARCHAR(50) NULL,
    ZoneValue VARCHAR(50) NULL);
```
</details>

<details>
<summary>PostgreSQL</summary>

```sql
CREATE TABLE NodaTimeRoundTrip (
    Id BIGINT PRIMARY KEY,
    InstantValue TIMESTAMPTZ NULL,
    LocalDateValue DATE NULL,
    LocalDateTimeValue TIMESTAMP NULL,
    LocalTimeValue TIME NULL,
    OffsetDateTimeValue TIMESTAMPTZ NULL,
    OffsetValue INTEGER NULL,
    DurationValue BIGINT NULL,
    PeriodValue VARCHAR(176) NULL,
    CalendarValue VARCHAR(50) NULL,
    ZoneValue VARCHAR(50) NULL);
```
</details>

<details>
<summary>MySQL</summary>

```sql
CREATE TABLE NodaTimeRoundTrip (
    Id BIGINT PRIMARY KEY,
    InstantValue DATETIME(6) NULL,
    LocalDateValue DATE NULL,
    LocalDateTimeValue DATETIME(6) NULL,
    LocalTimeValue TIME(6) NULL,
    OffsetDateTimeValue DATETIME(6) NULL,
    OffsetValue INT NULL,
    DurationValue BIGINT NULL,
    PeriodValue VARCHAR(176) NULL,
    CalendarValue VARCHAR(50) NULL,
    ZoneValue VARCHAR(50) NULL);
```
</details>

<details>
<summary>SQLite</summary>

```sql
CREATE TABLE NodaTimeRoundTrip (
    Id INTEGER PRIMARY KEY,
    InstantValue TEXT NULL,
    LocalDateValue TEXT NULL,
    LocalDateTimeValue TEXT NULL,
    LocalTimeValue TEXT NULL,
    OffsetDateTimeValue TEXT NULL,
    OffsetValue INTEGER NULL,
    DurationValue INTEGER NULL,
    PeriodValue TEXT NULL,
    CalendarValue TEXT NULL,
    ZoneValue TEXT NULL);
```
</details>

<details>
<summary>Oracle</summary>

```sql
CREATE TABLE NodaTimeRoundTrip (
    Id NUMBER(19) PRIMARY KEY,
    InstantValue TIMESTAMP NULL,
    LocalDateValue DATE NULL,
    LocalDateTimeValue TIMESTAMP NULL,
    LocalTimeValue INTERVAL DAY TO SECOND NULL,
    OffsetDateTimeValue TIMESTAMP WITH TIME ZONE NULL,
    OffsetValue NUMBER(10) NULL,
    DurationValue NUMBER(19) NULL,
    PeriodValue VARCHAR2(176) NULL,
    CalendarValue VARCHAR2(50) NULL,
    ZoneValue VARCHAR2(50) NULL);
```
</details>

The exact scripts used by the integration suite live in [`db/`](db).

## Period Format

`Period` is stored with NodaTime's ISO8601 roundtrip pattern, which needs at most **176 characters**:

```
P1Y2M3W4DT5H6M7S
```

means 1 year, 2 months, 3 weeks, 4 days, 5 hours, 6 minutes, 7 seconds. The worst case (every component at
`long.MinValue` / `int.MinValue`) is what drives the `VARCHAR(176)` recommendation.

## Custom Configuration

Every provider package is a thin `INodaTimeTypeHandlerConfiguration` implementation. To tweak how parameters are
shaped, derive from `NodaTimeTypeHandlerConfigurationBase`, override only what you need, and register it directly:

```csharp
using System.Data;
using AdaskoTheBeAsT.Dapper.NodaTime;
using NodaTime;
using NodaTime.Text;

public sealed class InstantAsTextConfiguration : NodaTimeTypeHandlerConfigurationBase
{
    public override void SetInstant(IDbDataParameter parameter, Instant value)
    {
        parameter.Value = InstantPattern.ExtendedIso.Format(value);
        parameter.DbType = DbType.AnsiString;
    }
}

DapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb, new InstantAsTextConfiguration());
```

`TrySetNative<TParameter>` is available to `protected` subclasses for applying provider-specific types only when the
parameter actually belongs to that provider.

## Known Limitations

- **`ZonedDateTime` is not supported directly.** Persist the parts and compose it in your domain layer:

  ```csharp
  public sealed class EventWithZone
  {
      public LocalDateTime LocalDateTime { get; set; }
      public DateTimeZone TimeZone { get; set; } = DateTimeZone.Utc;
      public CalendarSystem Calendar { get; set; } = CalendarSystem.Iso;

      public ZonedDateTime ToZonedDateTime() =>
          LocalDateTime.InZoneStrictly(TimeZone).WithCalendar(Calendar);
  }
  ```

- **PostgreSQL and MySQL normalize `OffsetDateTime` to UTC**, because neither has an offset-preserving column type.
  Store the `Offset` in a separate column when the original offset matters.
- **Oracle maps `LocalTime` to `INTERVAL DAY TO SECOND`**, since Oracle has no time-only type.
- **SQLite stores date/time values as `TEXT`**, so ordering and comparisons rely on the ISO format being preserved.

## Framework Support

| Package | Target frameworks |
| --- | --- |
| Core | `net10.0`, `net9.0`, `net8.0`, `netstandard2.1`, `netstandard2.0`, `net481`, `net48`, `net472`, `net471`, `net47`, `net462` |
| SqlServer, PostgreSql, MySql, Sqlite | `net10.0`, `net9.0`, `net8.0`, `net481`, `net48`, `net472`, `net471`, `net47`, `net462` |
| Oracle | `net10.0`, `net9.0`, `net8.0`, `net481`, `net48`, `net472` |

## Building and Testing

```powershell
dotnet build AdaskoTheBeAsT.Dapper.NodaTime.slnx

# unit tests (all target frameworks)
dotnet test test/unit/AdaskoTheBeAsT.Dapper.NodaTime.SqlServer.Test

# integration tests (Docker required, except SQLite)
dotnet test test/integ/AdaskoTheBeAsT.Dapper.NodaTime.Sqlite.IntegrationTest
dotnet test test/integ/AdaskoTheBeAsT.Dapper.NodaTime.PostgreSql.IntegrationTest
```

Integration tests spin up real databases with [Testcontainers](https://dotnet.testcontainers.org/)
(SQL Server, PostgreSQL, MySQL, Oracle); SQLite runs in-process. Solution filters
`WithoutDb.slnf` and `SqlServer.slnf` are available when you want a subset.

## Contributing

Contributions are welcome!

1. Fork the repository
2. Create a feature branch
3. Make your change **with tests**
4. Ensure `dotnet build` and the test suites are green
5. Open a pull request

## Credits

- **Original author**: [Matt Johnson-Pint](https://github.com/mj1856) ([Dapper-NodaTime](https://github.com/mj1856/Dapper-NodaTime))
- **Current maintainer**: Adam "AdaskoTheBeAsT" Pluciński

## License

[MIT License](LICENSE).

---

**Need help?** Open an [issue](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.Dapper.NodaTime/issues) or browse the [NodaTime docs](https://nodatime.org/).
