# AdaskoTheBeAsT.Dapper.NodaTime

[![NuGet](https://img.shields.io/nuget/v/AdaskoTheBeAsT.Dapper.NodaTime.svg)](https://www.nuget.org/packages/AdaskoTheBeAsT.Dapper.NodaTime/)

Core [Dapper](https://github.com/DapperLib/Dapper) type handlers for [NodaTime](https://nodatime.org/).
This package is provider agnostic: it contains the handlers, the configuration abstraction and portable
`DbType` defaults, and depends only on `Dapper` and `NodaTime`.

For a ready-to-use database dialect, install one of the provider packages instead
(`...SqlServer`, `...PostgreSql`, `...MySql`, `...Sqlite`, `...Oracle`) - each of them references this package.

## Installation

```bash
dotnet add package AdaskoTheBeAsT.Dapper.NodaTime
```

## What this package provides

| Type | Purpose |
| --- | --- |
| `DapperNodaTimeSetup` | Registers all handlers with `Dapper.SqlMapper` in one call |
| `INodaTimeTypeHandlerConfiguration` | Contract describing how each NodaTime value is written to an `IDbDataParameter` |
| `NodaTimeTypeHandlerConfigurationBase` | Portable `DbType`-only implementation used as the base of every provider dialect |
| `InstantHandler`, `LocalDateHandler`, `LocalDateTimeHandler`, `LocalTimeHandler`, `OffsetDateTimeHandler`, `OffsetHandler`, `DurationHandler`, `PeriodHandler`, `CalendarSystemHandler`, `DateTimeZoneHandler` | The `SqlMapper.TypeHandler<T>` implementations |

## Usage

`Register` requires an explicit configuration, so the dialect is always a conscious decision:

```csharp
using AdaskoTheBeAsT.Dapper.NodaTime;
using NodaTime;

DapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb, new MyConfiguration());
```

Provider packages wrap exactly that call, e.g. `SqlServerDapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb)`.

## Writing values (portable defaults)

`NodaTimeTypeHandlerConfigurationBase` shapes parameters using only `System.Data.DbType`:

| NodaTime type | Parameter value | `DbType` |
| --- | --- | --- |
| `Instant` | `DateTime` (UTC) | `DateTime2` |
| `LocalDate` | `DateTime` at midnight, unspecified kind | `Date` |
| `LocalDateTime` | `DateTime`, unspecified kind | `DateTime2` |
| `LocalTime` | `TimeSpan` since midnight | `Time` |
| `OffsetDateTime` | `DateTimeOffset` | `DateTimeOffset` |
| `Offset` | `int` seconds | `Int32` |
| `Duration` | `long` nanoseconds | `Int64` |
| `Period` | ISO8601 roundtrip string or `DBNull` | `AnsiString` |
| `CalendarSystem` | calendar id string or `DBNull` | `AnsiString` |
| `DateTimeZone` | time zone id string or `DBNull` | `AnsiString` |

## Reading values

Handlers accept several database representations so the same model works across providers and legacy schemas:

| NodaTime type | Accepted values |
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

`null` / `DBNull` and unsupported CLR types throw `System.Data.DataException` with a descriptive message.
Strings are parsed with `CultureInfo.InvariantCulture` and `DateTimeStyles.RoundtripKind`.

## Custom dialects

Derive from `NodaTimeTypeHandlerConfigurationBase` and override only what differs:

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
```

`protected static TrySetNative<TParameter>(IDbDataParameter parameter, Action<TParameter> set)` applies a
provider-specific parameter type only when the parameter really is of that provider type, which keeps the
configuration usable with wrappers, profilers and mocks.

## Not supported

`ZonedDateTime` has no handler. Persist `LocalDateTime` + `DateTimeZone` (+ `CalendarSystem`) and compose it
in your domain layer.

## Target frameworks

`net10.0`, `net9.0`, `net8.0`, `netstandard2.1`, `netstandard2.0`, `net481`, `net48`, `net472`, `net471`, `net47`, `net462`.

## Links

- [Repository and full documentation](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.Dapper.NodaTime)
- [Upgrading from 5.x to 6.0](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.Dapper.NodaTime/blob/master/MIGRATION.md)
- [MIT License](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.Dapper.NodaTime/blob/master/LICENSE)
