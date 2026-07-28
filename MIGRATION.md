# Migration to 6.0

Version 6.0 splits the single `AdaskoTheBeAsT.Dapper.NodaTime` package into a provider-agnostic core plus one
package per database dialect. The version number jumps from 5.x to 6.0 because the public API and the
parameter shaping behaviour both change in ways that require code edits.

## Table of contents

- [Why the major bump](#why-the-major-bump)
- [Step 1: install a provider package](#step-1-install-a-provider-package)
- [Step 2: replace the setup call](#step-2-replace-the-setup-call)
- [Step 3: replace handler singletons](#step-3-replace-handler-singletons)
- [Step 4: check your columns](#step-4-check-your-columns)
- [Breaking change reference](#breaking-change-reference)
- [Non-breaking improvements](#non-breaking-improvements)
- [Troubleshooting](#troubleshooting)

## Why the major bump

In 5.x the single package always applied SQL Server semantics: every handler set `SqlDbType` on the parameter,
so PostgreSQL, MySQL, SQLite and Oracle users silently got whatever `DbType` fallback the provider derived.
Fixing that means the dialect has to be chosen explicitly, and that changes the public API:

1. **The dialect is now a required, explicit decision.** `DapperNodaTimeSetup.Register` takes an
   `INodaTimeTypeHandlerConfiguration`, and each provider package ships one.
2. **Handlers are no longer singletons.** They need a configuration instance, so the parameterless
   `XxxHandler.Default` fields could not be kept.
3. **SQL Server specifics left the core package.** `DbDataParameterExtensions` (`SetSqlDbType`) moved into
   `AdaskoTheBeAsT.Dapper.NodaTime.SqlServer` and became internal.
4. **`Period` is now registered.** `Register` in 5.x created handlers for nine types and skipped
   `PeriodHandler`; 6.0 registers all ten.
5. **PostgreSQL and MySQL now normalize `OffsetDateTime` to UTC** to match the column types they use, so values
   read back can differ from 5.x behaviour.

## Step 1: install a provider package

```bash
# before
dotnet add package AdaskoTheBeAsT.Dapper.NodaTime

# after - pick exactly one; it brings the core package with it
dotnet add package AdaskoTheBeAsT.Dapper.NodaTime.SqlServer
dotnet add package AdaskoTheBeAsT.Dapper.NodaTime.PostgreSql
dotnet add package AdaskoTheBeAsT.Dapper.NodaTime.MySql
dotnet add package AdaskoTheBeAsT.Dapper.NodaTime.Sqlite
dotnet add package AdaskoTheBeAsT.Dapper.NodaTime.Oracle
```

Keeping a direct reference to the core package is fine, but it no longer registers anything on its own.

## Step 2: replace the setup call

```csharp
// 5.x
using AdaskoTheBeAsT.Dapper.NodaTime;

DapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb);
```

```csharp
// 6.0 - SQL Server keeps the previous behaviour
using AdaskoTheBeAsT.Dapper.NodaTime.SqlServer;

SqlServerDapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb);
```

| 5.x call | 6.0 replacement |
| --- | --- |
| `DapperNodaTimeSetup.Register(provider)` on SQL Server | `SqlServerDapperNodaTimeSetup.Register(provider)` |
| `DapperNodaTimeSetup.Register(provider)` on PostgreSQL | `PostgreSqlDapperNodaTimeSetup.Register(provider)` |
| `DapperNodaTimeSetup.Register(provider)` on MySQL/MariaDB | `MySqlDapperNodaTimeSetup.Register(provider)` |
| `DapperNodaTimeSetup.Register(provider)` on SQLite | `SqliteDapperNodaTimeSetup.Register(provider)` |
| `DapperNodaTimeSetup.Register(provider)` on Oracle | `OracleDapperNodaTimeSetup.Register(provider)` |

Calling the core overload directly still works when you supply a dialect:

```csharp
DapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb, new SqlServerNodaTimeConfiguration());
```

## Step 3: replace handler singletons

```csharp
// 5.x
SqlMapper.AddTypeHandler(InstantHandler.Default);
SqlMapper.AddTypeHandler(DateTimeZoneHandler.Default(DateTimeZoneProviders.Tzdb));
```

```csharp
// 6.0
var configuration = new SqlServerNodaTimeConfiguration();
SqlMapper.AddTypeHandler(new InstantHandler(configuration));
SqlMapper.AddTypeHandler(new DateTimeZoneHandler(DateTimeZoneProviders.Tzdb, configuration));
```

If you customized parameter shaping by wrapping the handlers, derive from
`NodaTimeTypeHandlerConfigurationBase` instead and override only the `SetXxx` methods you care about:

```csharp
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

## Step 4: check your columns

SQL Server column types are unchanged from 5.x. On the other providers the written types are now explicit, so
verify your schema against the dialect you selected:

| NodaTime type | SQL Server | PostgreSQL | MySQL | SQLite | Oracle |
| --- | --- | --- | --- | --- | --- |
| `Instant` | `DATETIME2` | `TIMESTAMPTZ` | `DATETIME(6)` | `TEXT` | `TIMESTAMP` |
| `LocalDate` | `DATE` | `DATE` | `DATE` | `TEXT` | `DATE` |
| `LocalDateTime` | `DATETIME2` | `TIMESTAMP` | `DATETIME(6)` | `TEXT` | `TIMESTAMP` |
| `LocalTime` | `TIME` | `TIME` | `TIME(6)` | `TEXT` | `INTERVAL DAY TO SECOND` |
| `OffsetDateTime` | `DATETIMEOFFSET` | `TIMESTAMPTZ` (UTC) | `DATETIME(6)` (UTC) | `TEXT` | `TIMESTAMP WITH TIME ZONE` |
| `Offset` | `INT` | `INTEGER` | `INT` | `INTEGER` | `NUMBER(10)` |
| `Duration` | `BIGINT` | `BIGINT` | `BIGINT` | `INTEGER` | `NUMBER(19)` |
| `Period` | `VARCHAR(176)` | `VARCHAR(176)` | `VARCHAR(176)` | `TEXT` | `VARCHAR2(176)` |
| `CalendarSystem` / `DateTimeZone` | `VARCHAR(50)` | `VARCHAR(50)` | `VARCHAR(50)` | `TEXT` | `VARCHAR2(50)` |

Two things to watch for:

- **MySQL fractional seconds.** Plain `DATETIME` / `TIME` truncate to whole seconds. Use `DATETIME(6)` and
  `TIME(6)` if you need sub-second round-trips.
- **`OffsetDateTime` on PostgreSQL and MySQL** is stored as UTC, so the offset does not survive the round-trip.
  Persist the `Offset` in its own column when the original offset is part of your domain.

## Breaking change reference

| 5.x | 6.0 |
| --- | --- |
| `DapperNodaTimeSetup.Register(IDateTimeZoneProvider)` | removed; use a provider setup class or `Register(IDateTimeZoneProvider, INodaTimeTypeHandlerConfiguration)` |
| `InstantHandler.Default` and the other `Default` singletons | removed; construct handlers with a configuration |
| Handlers had private constructors | handlers have public constructors taking `INodaTimeTypeHandlerConfiguration` |
| `IDbDataParameter.SetSqlDbType` extension in the core package | moved to the SqlServer package and made internal |
| `PeriodHandler` not registered by `Register` | registered together with the other handlers |
| Core package emitted `SqlDbType` for every provider | core emits portable `DbType` only; provider packages add native types |
| Single package for all databases | core + `SqlServer`, `PostgreSql`, `MySql`, `Sqlite`, `Oracle` |

## Non-breaking improvements

- **More accepted read values.** In addition to the native NodaTime types, handlers now read
  `string` (ISO), `decimal`, `int`/`long`, and `DateOnly`/`TimeOnly` on .NET 8+. This makes SQLite text columns,
  Oracle numeric columns and Npgsql's `DateOnly`/`TimeOnly` mapping work out of the box.
- **Wider target frameworks.** Added `netstandard2.1` and `net462`, `net47`, `net471`, `net472`, `net48`,
  `net481` next to `netstandard2.0`, `net8.0`, `net9.0` and `net10.0` (the Oracle package covers
  `net472`, `net48`, `net481` and .NET 8+).
- **Dependency version ranges.** `Dapper [2.1.79,3.0.0)` and `NodaTime [3.3.3,4.0.0)` instead of pinned
  versions, so you control the patch level.
- **Native provider types everywhere.** `SqlDbType`, `NpgsqlDbType`, `MySqlDbType`, `SqliteType` and
  `OracleDbType` are applied by the matching package.
- **Clear failures.** Unsupported CLR values and `null`/`DBNull` throw `System.Data.DataException` naming the
  offending type instead of producing a wrong value.

## Troubleshooting

| Symptom | Cause and fix |
| --- | --- |
| `CS7036: There is no argument given that corresponds to the required parameter 'configuration'` | You still call the 5.x `Register(provider)` overload. Switch to the provider setup class. |
| `CS0117: 'InstantHandler' does not contain a definition for 'Default'` | The singletons are gone. Use `new InstantHandler(configuration)`. |
| `CS1061: 'IDbDataParameter' has no method 'SetSqlDbType'` | The extension is internal to the SqlServer package now. Use `SqlServerNodaTimeConfiguration`. |
| `DataException: Cannot convert System.String to NodaTime.X` on SQLite | Register through `SqliteDapperNodaTimeSetup` so text values are parsed. |
| `OffsetDateTime` comes back with offset zero on PostgreSQL/MySQL | Expected; store the `Offset` separately. |
| Sub-second values truncated on MySQL | Widen the columns to `DATETIME(6)` / `TIME(6)`. |

Full documentation is in the [README](README.md), and each package has its own readme under
[`src/`](src) describing only that dialect.
