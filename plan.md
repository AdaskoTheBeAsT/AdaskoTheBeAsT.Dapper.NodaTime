# Plan: Multi-Database Support for AdaskoTheBeAsT.Dapper.NodaTime

Make the library work correctly on **SQL Server, PostgreSQL, MySQL, SQLite, and Oracle**
by introducing **per-vendor NuGet packages** (mirroring the structure of
`AdaskoTheBeAsT.Identity.Dapper`), while keeping the NodaTime↔CLR conversion logic shared
in a single core package.

## Decisions (locked)

| Decision | Choice |
| --- | --- |
| Packaging | **Per-vendor packages** (core + `.SqlServer`/`.PostgreSql`/`.MySql`/`.Sqlite`/`.Oracle`) |
| Default behavior | **Explicit dialect selection required** — no implicit SQL Server default (breaking change, major version bump) |
| Integration tests | **All 5 vendors** (Testcontainers for SQL Server/PostgreSQL/MySQL/Oracle; in-process file/in-memory DB for SQLite) |

---

## 1. Why the library is SQL Server-only today

Every type handler ends `SetValue` by calling:

```csharp
parameter.SetSqlDbType(SqlDbType.DateTime2); // System.Data.SqlDbType
```

`DbDataParameterExtensions.SetSqlDbType` (in core) uses **reflection to set a `SqlDbType`
property that exists only on SQL Server parameters** (`Microsoft.Data.SqlClient.SqlParameter`
and `System.Data.SqlClient.SqlParameter`). On `NpgsqlParameter`, `MySqlParameter`,
`SqliteParameter`, and `OracleParameter` that property does not exist, so the call throws
`InvalidOperationException: Property 'SqlDbType' not found ...`.

Key insight: the **NodaTime↔CLR conversion is portable** (it produces `DateTime`,
`DateTimeOffset`, `TimeSpan`, `long`, `int`, `string`). Only the **parameter type hint**
(and, for a few types, the exact value normalization) is vendor-specific. That is the only
thing that must become pluggable.

### Handlers and what they currently produce

| Handler | `parameter.Value` (CLR) | Current SqlDbType |
| --- | --- | --- |
| `InstantHandler` | `DateTime` (UTC) via `ToDateTimeUtc()` | `DateTime2` |
| `LocalDateTimeHandler` | `DateTime` (Unspecified) via `ToDateTimeUnspecified()` | `DateTime2` |
| `LocalDateHandler` | `DateTime` (Unspecified midnight) | `Date` |
| `LocalTimeHandler` | `TimeSpan` (`FromTicks(TickOfDay)`) | `Time` |
| `OffsetDateTimeHandler` | `DateTimeOffset` via `ToDateTimeOffset()` | `DateTimeOffset` |
| `OffsetHandler` | `int` (`Seconds`) | `Int` |
| `DurationHandler` | `long` (`ToInt64Nanoseconds()`) | `BigInt` |
| `PeriodHandler` | `string` (ISO roundtrip, ≤176) | `VarChar` |
| `CalendarSystemHandler` | `string` (`Id`, ≤50) | `VarChar` |
| `DateTimeZoneHandler` | `string` (`Id`, ≤50) | `VarChar` |

---

## 2. Reference structure to mirror (Identity.Dapper)

- `src/<...>.<Vendor>/` — one project per vendor; each references the native ADO.NET provider.
- `db/<Vendor>/*.sql` — per-vendor schema scripts.
- `test/integ/<Vendor>.IntegrationTest/` — real DB via **Testcontainers** + **dbup**; shared
  scenarios in `test/integ/Common/` compiled into each project via a `Compile Include` glob.
- `*.slnf` solution filters for single-vendor subsets (e.g. `MySQL.slnf`, `WithoutSqlDb.slnf`).
- Integration tests target a single modern TFM (`net10.0`); unit tests keep the full TFM span.

---

## 3. Target repository layout

```
src/
  AdaskoTheBeAsT.Dapper.NodaTime/                 # CORE: handlers + abstraction + DbType base config
  AdaskoTheBeAsT.Dapper.NodaTime.SqlServer/       # SqlDbType (client-agnostic, keeps reflection helper)
  AdaskoTheBeAsT.Dapper.NodaTime.PostgreSql/      # Npgsql -> NpgsqlDbType
  AdaskoTheBeAsT.Dapper.NodaTime.MySql/           # MySqlConnector -> MySqlDbType
  AdaskoTheBeAsT.Dapper.NodaTime.Sqlite/          # Microsoft.Data.Sqlite -> SqliteType
  AdaskoTheBeAsT.Dapper.NodaTime.Oracle/          # Oracle.ManagedDataAccess.Core -> OracleDbType
db/
  SqlServer/  PostgreSQL/  MySql/  SQLite/  Oracle/    # CREATE TABLE scripts per vendor (dbup)
test/
  unit/
    AdaskoTheBeAsT.Dapper.NodaTime.Test/          # provider-agnostic Parse/round-trip-logic unit tests
  integ/
    Common/                                        # shared value sets + round-trip scenario base
    AdaskoTheBeAsT.Dapper.NodaTime.SqlServer.IntegrationTest/
    AdaskoTheBeAsT.Dapper.NodaTime.PostgreSql.IntegrationTest/
    AdaskoTheBeAsT.Dapper.NodaTime.MySql.IntegrationTest/
    AdaskoTheBeAsT.Dapper.NodaTime.Sqlite.IntegrationTest/
    AdaskoTheBeAsT.Dapper.NodaTime.Oracle.IntegrationTest/
```

---

## 4. Core refactor: a pluggable vendor configuration

### 4.1 Abstraction

Introduce one interface owning the full `SetValue` per NodaTime type (so a vendor can adjust
both the **value** and the **type hint**), plus a base class with portable `System.Data.DbType`
defaults.

```csharp
// Core
public interface INodaTimeTypeHandlerConfiguration
{
    void SetInstant(IDbDataParameter p, Instant value);
    void SetLocalDate(IDbDataParameter p, LocalDate value);
    void SetLocalDateTime(IDbDataParameter p, LocalDateTime value);
    void SetLocalTime(IDbDataParameter p, LocalTime value);
    void SetOffsetDateTime(IDbDataParameter p, OffsetDateTime value);
    void SetOffset(IDbDataParameter p, Offset value);
    void SetDuration(IDbDataParameter p, Duration value);
    void SetPeriod(IDbDataParameter p, Period? value);
    void SetCalendarSystem(IDbDataParameter p, CalendarSystem? value);
    void SetDateTimeZone(IDbDataParameter p, DateTimeZone? value);
}
```

```csharp
// Core: portable defaults via System.Data.DbType (works on every ADO.NET provider)
public abstract class NodaTimeTypeHandlerConfigurationBase : INodaTimeTypeHandlerConfiguration
{
    public virtual void SetInstant(IDbDataParameter p, Instant value)
    { p.Value = value.ToDateTimeUtc(); p.DbType = DbType.DateTime2; }

    public virtual void SetLocalDateTime(IDbDataParameter p, LocalDateTime value)
    { p.Value = value.ToDateTimeUnspecified(); p.DbType = DbType.DateTime2; }

    public virtual void SetLocalDate(IDbDataParameter p, LocalDate value)
    { p.Value = value.AtMidnight().ToDateTimeUnspecified(); p.DbType = DbType.Date; }

    public virtual void SetLocalTime(IDbDataParameter p, LocalTime value)
    { p.Value = TimeSpan.FromTicks(value.TickOfDay); p.DbType = DbType.Time; }

    public virtual void SetOffsetDateTime(IDbDataParameter p, OffsetDateTime value)
    { p.Value = value.ToDateTimeOffset(); p.DbType = DbType.DateTimeOffset; }

    public virtual void SetOffset(IDbDataParameter p, Offset value)
    { p.Value = value.Seconds; p.DbType = DbType.Int32; }

    public virtual void SetDuration(IDbDataParameter p, Duration value)
    { p.Value = value.ToInt64Nanoseconds(); p.DbType = DbType.Int64; }

    public virtual void SetPeriod(IDbDataParameter p, Period? value)
    { p.Value = value is null ? DBNull.Value : PeriodPattern.Roundtrip.Format(value); p.DbType = DbType.AnsiString; }

    public virtual void SetCalendarSystem(IDbDataParameter p, CalendarSystem? value)
    { p.Value = value is null ? DBNull.Value : value.Id; p.DbType = DbType.AnsiString; }

    public virtual void SetDateTimeZone(IDbDataParameter p, DateTimeZone? value)
    { p.Value = value is null ? DBNull.Value : value.Id; p.DbType = DbType.AnsiString; }
}
```

### 4.2 Handlers become thin and config-driven

Handlers no longer hold a static `Default` singleton with a hard-coded `SqlDbType`. They take
the configuration and delegate `SetValue`. `Parse` stays shared in core (extended for tolerance —
see §6).

```csharp
public sealed class InstantHandler : SqlMapper.TypeHandler<Instant>
{
    private readonly INodaTimeTypeHandlerConfiguration _config;
    public InstantHandler(INodaTimeTypeHandlerConfiguration config) => _config = config;

    public override void SetValue(IDbDataParameter parameter, Instant value)
        => _config.SetInstant(parameter, value);

    public override Instant Parse(object value) { /* shared, see §6 */ }
}
```

### 4.3 Registration

Core exposes a registration that accepts the configuration; each vendor package exposes a
one-liner that supplies its own configuration.

```csharp
// Core
public static class DapperNodaTimeSetup
{
    public static void Register(IDateTimeZoneProvider provider, INodaTimeTypeHandlerConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(config);
        SqlMapper.AddTypeHandler(new InstantHandler(config));
        SqlMapper.AddTypeHandler(new LocalDateHandler(config));
        // ... all 10 handlers ...
        SqlMapper.AddTypeHandler(new DateTimeZoneHandler(provider, config));
    }
}
```

```csharp
// AdaskoTheBeAsT.Dapper.NodaTime.PostgreSql
public static class PostgreSqlDapperNodaTimeSetup
{
    public static void Register(IDateTimeZoneProvider provider)
        => DapperNodaTimeSetup.Register(provider, new PostgreSqlNodaTimeConfiguration());
}
```

Consumer code after upgrade:

```csharp
// before (SQL Server implicit):  DapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb);
// after:                         PostgreSqlDapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb);
```

### 4.4 Graceful native-type application

Each vendor config sets the portable `DbType` first, then upgrades to the precise native enum
**only when the parameter is the expected native type** (so a mismatch degrades instead of
throwing):

```csharp
protected static void TrySetNative<TParam>(IDbDataParameter p, Action<TParam> set)
    where TParam : class { if (p is TParam np) set(np); }
```

---

## 5. Per-type → per-vendor mapping matrix (the core technical contract)

Legend: **value** = CLR object assigned to `parameter.Value`; **native** = precise provider
enum applied on top of the portable `DbType`.

### Instant (UTC instant)
| Vendor | Column | Value | Native type |
| --- | --- | --- | --- |
| SQL Server | `datetime2` | `DateTime` (Utc) | `SqlDbType.DateTime2` |
| PostgreSQL | `timestamptz` | `DateTime` (Kind=Utc) | `NpgsqlDbType.TimestampTz` |
| MySQL | `DATETIME(6)` (UTC) | `DateTime` (Utc) | `MySqlDbType.DateTime` |
| SQLite | `TEXT` (ISO8601) | `DateTime` (Utc) | `SqliteType.Text` |
| Oracle | `TIMESTAMP` (UTC) | `DateTime` (Utc) | `OracleDbType.TimeStamp` |

### LocalDateTime (no zone)
| Vendor | Column | Value | Native type |
| --- | --- | --- | --- |
| SQL Server | `datetime2` | `DateTime` (Unspecified) | `SqlDbType.DateTime2` |
| PostgreSQL | `timestamp` | `DateTime` (Kind=Unspecified) | `NpgsqlDbType.Timestamp` |
| MySQL | `DATETIME(6)` | `DateTime` (Unspecified) | `MySqlDbType.DateTime` |
| SQLite | `TEXT` | `DateTime` (Unspecified) | `SqliteType.Text` |
| Oracle | `TIMESTAMP` | `DateTime` (Unspecified) | `OracleDbType.TimeStamp` |

### LocalDate
| Vendor | Column | Value | Native type |
| --- | --- | --- | --- |
| SQL Server | `date` | `DateTime` midnight | `SqlDbType.Date` |
| PostgreSQL | `date` | `DateTime` midnight | `NpgsqlDbType.Date` |
| MySQL | `DATE` | `DateTime` midnight | `MySqlDbType.Date` |
| SQLite | `TEXT` | `DateTime` midnight | `SqliteType.Text` |
| Oracle | `DATE` | `DateTime` midnight | `OracleDbType.Date` |

### LocalTime
| Vendor | Column | Value | Native type | Note |
| --- | --- | --- | --- | --- |
| SQL Server | `time` | `TimeSpan` | `SqlDbType.Time` | |
| PostgreSQL | `time` | `TimeSpan` | `NpgsqlDbType.Time` | **must** force `Time` — Npgsql maps a bare `TimeSpan` to `interval`, not `time` |
| MySQL | `TIME(6)` | `TimeSpan` | `MySqlDbType.Time` | |
| SQLite | `TEXT` | `TimeSpan` | `SqliteType.Text` | |
| Oracle | `INTERVAL DAY TO SECOND` | `TimeSpan` | `OracleDbType.IntervalDS` | Oracle has no pure TIME type |

### OffsetDateTime
| Vendor | Column | Value | Native type | Note |
| --- | --- | --- | --- | --- |
| SQL Server | `datetimeoffset` | `DateTimeOffset` | `SqlDbType.DateTimeOffset` | offset preserved |
| PostgreSQL | `timestamptz` | `DateTimeOffset.ToUniversalTime()` | `NpgsqlDbType.TimestampTz` | **offset NOT stored** (PG stores the instant); Npgsql ≥6 rejects non-UTC `DateTimeOffset` |
| MySQL | `DATETIME(6)` (UTC) | `DateTimeOffset.UtcDateTime` | `MySqlDbType.DateTime` | **offset lost** (no offset type in MySQL) |
| SQLite | `TEXT` (ISO8601 ±offset) | `DateTimeOffset` | `SqliteType.Text` | offset preserved (text) |
| Oracle | `TIMESTAMP WITH TIME ZONE` | `DateTimeOffset` | `OracleDbType.TimeStampTZ` | offset preserved |

> Offset-preservation note: for PostgreSQL/MySQL, applications needing the original offset must
> persist `OffsetDateTime` as text (a documented limitation), or store the `Offset` separately.

### Offset / Duration (numeric — fully portable)
| NodaTime | Value | SQL Server | PostgreSQL | MySQL | SQLite | Oracle |
| --- | --- | --- | --- | --- | --- | --- |
| `Offset` | `int` seconds | `int` | `integer` | `INT` | `INTEGER` | `NUMBER(10)` |
| `Duration` | `long` nanos | `bigint` | `bigint` | `BIGINT` | `INTEGER` | `NUMBER(19)` |

### Period / CalendarSystem / DateTimeZone (string — portable)
| NodaTime | Value | SQL Server | PostgreSQL | MySQL | SQLite | Oracle |
| --- | --- | --- | --- | --- | --- | --- |
| `Period` | ISO string ≤176 | `varchar(176)` | `text`/`varchar(176)` | `VARCHAR(176)` | `TEXT` | `VARCHAR2(176)` |
| `CalendarSystem` | `Id` ≤50 | `varchar(50)` | `text` | `VARCHAR(50)` | `TEXT` | `VARCHAR2(50)` |
| `DateTimeZone` | `Id` ≤50 | `varchar(50)` | `text` | `VARCHAR(50)` | `TEXT` | `VARCHAR2(50)` |

---

## 6. `Parse` robustness (read path)

`Parse` stays shared in core but must tolerate every CLR type the various providers return:

- **SQLite** returns date/time and offset columns as **`string`** → add ISO8601 string parsing
  to `InstantHandler`, `LocalDateHandler`, `LocalDateTimeHandler`, `LocalTimeHandler`,
  `OffsetDateTimeHandler` (`Period`/`CalendarSystem`/`DateTimeZone` already accept `string`).
- **Oracle** `INTERVAL DAY TO SECOND` round-trips as `TimeSpan` (already handled by `LocalTime`),
  and `OracleTimeStampTZ`/`OracleDate` are returned as `DateTime`/`DateTimeOffset` by the managed
  provider (verify in integ tests).
- **MySQL/PostgreSQL** return `DateTime`/`DateTimeOffset`/`TimeSpan` (already handled).
- Numeric widening: ensure `Parse` accepts `int`/`long`/`decimal` where Oracle `NUMBER` may
  surface as `decimal` (add `decimal`→`long`/`int` handling for `Duration`/`Offset`).

---

## 7. Per-vendor package details

| Package | ADO.NET dependency (suggested) | Native parameter | Notes |
| --- | --- | --- | --- |
| `.SqlServer` | none forced (keep client-agnostic) | `SqlParameter.SqlDbType` | reuse existing reflection-based `SetSqlDbType` so it supports **both** `Microsoft.Data.SqlClient` and `System.Data.SqlClient` |
| `.PostgreSql` | `Npgsql` | `NpgsqlParameter.NpgsqlDbType` | direct reference |
| `.MySql` | `MySqlConnector` | `MySqlParameter.MySqlDbType` | prefer `MySqlConnector` over `MySql.Data` |
| `.Sqlite` | `Microsoft.Data.Sqlite` | `SqliteParameter.SqliteType` | embedded; `DbType` mostly advisory |
| `.Oracle` | `Oracle.ManagedDataAccess.Core` | `OracleParameter.OracleDbType` | `.Core` for modern TFMs |

### Dependencies and TFMs

- **Core** (`AdaskoTheBeAsT.Dapper.NodaTime`): depends only on `Dapper` + `NodaTime`
  (no `SqlClient`). Keep full TFM span:
  `net10.0;net9.0;net8.0;net481;net48;net472;net471;net47;net462`.
  (`System.Data.SqlDbType`/`DbType` live in the BCL, so no extra package is needed.)
- **Vendor packages**: TFM span limited by the native provider. Target
  `net10.0;net9.0;net8.0;netstandard2.0` (netstandard2.0 covers .NET Framework consumers).
  `.SqlServer` may keep the wider span since it has no forced native dependency.
- Each vendor `.csproj` mirrors core packaging metadata (license, README, symbols, deterministic
  build) and adds `<ProjectReference>` to core + the native `<PackageReference>`.

---

## 8. Test strategy

### 8.1 Unit tests (`test/unit`)
- Keep **provider-agnostic** tests: every handler's `Parse` for each accepted input CLR type,
  plus value-shaping logic via a fake `IDbDataParameter` asserting `Value`/`DbType`.
- Remove SQL-Server-specific round-trip tests (`CREATE TABLE #T ...`) from unit — move them to
  the SqlServer integration project.
- The existing `DbVendorLibrary` / `DbVendorLibraryConnectionProvider` / `DbVendorLibraryTestData`
  scaffolding is superseded by per-vendor integration projects (delete or repurpose).

### 8.2 Integration tests (`test/integ`, one project per vendor)
Pattern lifted from Identity.Dapper:
- `test/integ/Directory.Build.props` adds the shared test deps and compiles `Common/**/*.cs`
  into each vendor project via `Compile Include`.
- `Common/` holds the canonical NodaTime sample values and an abstract round-trip scenario set
  (one case per type + null cases) parameterized over a connection factory and the vendor's
  column DDL.
- Each vendor project provides: a **Testcontainers fixture** (xUnit `IAsyncLifetime` + collection
  fixture), a connection factory, the vendor `Register(...)` call, and the per-vendor DDL/scripts.

| Vendor | Container image (Testcontainers) | dbup package |
| --- | --- | --- |
| SQL Server | `mcr.microsoft.com/mssql/server:2022-latest` (`Testcontainers.MsSql`) | `dbup-sqlserver` |
| PostgreSQL | `postgres:18` (`Testcontainers.PostgreSql`) | `dbup-postgresql` |
| MySQL | `mysql:8.4` (`Testcontainers.MySql`) | `dbup-mysql` |
| Oracle | `gvenzl/oracle-free:23-slim` (`Testcontainers.Oracle`) | `dbup-oracle` |
| SQLite | **no container** — temp-file / `:memory:` via `Microsoft.Data.Sqlite` | inline DDL or `dbup-sqlite` |

Fixture sketch (mirrors `DatabaseWithGuidIdFixture` in the reference):

```csharp
public sealed class PostgreSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container =
        new PostgreSqlBuilder("postgres:18").Build();

    public string ConnectionString { get; private set; } = "";

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        ConnectionString = _container.GetConnectionString();
        PostgreSqlDapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb);
        // dbup: create per-type test tables from db/PostgreSQL/*.sql
    }

    public Task DisposeAsync() => _container.DisposeAsync().AsTask();
}
```

- Each type gets a round-trip assertion against **every column type** valid for that vendor
  (e.g. `Instant` vs `timestamptz`/`timestamp`/`datetime2`), plus null-value cases — preserving
  the breadth of the current SQL Server tests but per vendor.

---

## 9. CI/CD changes

Current `azure-pipelines.yml`: single `windows-2022` job, unit tests only, packs & pushes NuGet
on tags. Changes:

1. **Build/unit/pack job** (Windows, unchanged): builds all `src` + unit tests; packs **all six**
   packages (core + 5 vendors) and pushes on `refs/tags/v*`.
2. **Integration job(s)** on **`ubuntu-latest`** (Linux containers are simplest for
   MSSQL/PostgreSQL/MySQL/Oracle). Use a **matrix over vendors** so each DB runs independently:
   - per-vendor `dotnet test test/integ/<Vendor>.IntegrationTest`.
   - SQLite needs no Docker; Oracle image is large (allow longer timeout / optional gating).
3. Add a **GitHub Actions** workflow (the `.github/workflows` folder is currently empty) with the
   same vendor matrix as a portable companion to Azure Pipelines.
4. Add `.slnf` filters: `WithoutDb.slnf` (build/unit only) and one per vendor (e.g. `MySql.slnf`)
   for fast local subsets — mirrors `MySQL.slnf`/`WithoutSqlDb.slnf` in the reference.

---

## 10. Backward compatibility & versioning

This is intentionally a **breaking** release (explicit dialect selection):

- `DapperNodaTimeSetup.Register(IDateTimeZoneProvider)` (SQL Server-implicit) is **removed**;
  the new core overload requires an `INodaTimeTypeHandlerConfiguration`.
- Static `XxxHandler.Default` singletons are **removed** (handlers now require a configuration).
- **Major version bump** (e.g. `2.0.0`). README + a `MIGRATION.md` documenting:
  - add the vendor package (`AdaskoTheBeAsT.Dapper.NodaTime.SqlServer`, etc.),
  - replace `DapperNodaTimeSetup.Register(Tzdb)` with `<Vendor>DapperNodaTimeSetup.Register(Tzdb)`,
  - replace any `SqlMapper.AddTypeHandler(InstantHandler.Default)` with the vendor `Register` call
    (or `new InstantHandler(config)`).
- Update README "Supported Types & SQL Mappings" into the per-vendor matrix from §5.

---

## 11. Solution / build wiring

- Add the 5 new `src` projects and 5 new `test/integ` projects to `AdaskoTheBeAsT.Dapper.NodaTime.slnx`.
- Add `test/integ/Directory.Build.props` (Testcontainers/dbup + `Common` compile glob).
- Keep root `Directory.Build.props` analyzers; vendor packages inherit them.
- Ensure `GeneratePackageOnBuild` + packaging metadata on every vendor project.

---

## 12. Phased task breakdown

### Phase 0 — Core abstraction (no behavior change yet)
- [ ] Add `INodaTimeTypeHandlerConfiguration` + `NodaTimeTypeHandlerConfigurationBase` (DbType defaults).
- [ ] Refactor 10 handlers to take the configuration; delegate `SetValue`.
- [ ] Extend `Parse` for `string`/`decimal` inputs (§6).
- [ ] New `DapperNodaTimeSetup.Register(provider, config)`; remove implicit SQL Server registration and `Default` singletons.
- [ ] Move `DbDataParameterExtensions.SetSqlDbType` usage out of core (into `.SqlServer`).
- [ ] Unit tests for base config + Parse paths (fake `IDbDataParameter`).

### Phase 1 — Vendor packages
- [ ] `.SqlServer` (reflection-based `SqlDbType`, client-agnostic) + `SqlServerDapperNodaTimeSetup`.
- [ ] `.PostgreSql` (Npgsql; override Instant/LocalDateTime/LocalTime/OffsetDateTime).
- [ ] `.MySql` (MySqlConnector; override datetime/time, document offset loss).
- [ ] `.Sqlite` (Microsoft.Data.Sqlite; text-based).
- [ ] `.Oracle` (Oracle.ManagedDataAccess.Core; INTERVAL DS for time, TimeStampTZ for offset).

### Phase 2 — Integration tests (all 5)
- [ ] `test/integ/Directory.Build.props` + `Common` round-trip scenario base + sample values.
- [ ] `db/<Vendor>/*.sql` per-type tables.
- [ ] One integration project per vendor with Testcontainers fixture (SQLite in-process).
- [ ] Port current SQL Server round-trip coverage into `.SqlServer.IntegrationTest`.

### Phase 3 — CI, packaging, docs
- [ ] Azure Pipelines: pack all 6 packages; add Linux integration matrix job.
- [ ] Add GitHub Actions vendor matrix workflow.
- [ ] Add `.slnf` filters; update `.slnx`.
- [ ] README per-vendor matrix + `MIGRATION.md`; bump major version.

---

## 13. Risks & open questions

- **PostgreSQL `OffsetDateTime`**: `timestamptz` stores an instant, not an offset; Npgsql ≥6
  rejects non-UTC `DateTimeOffset`. Decide policy: normalize to UTC (lossy, default) vs. text.
- **MySQL `OffsetDateTime`**: no offset type → store UTC (lossy) vs. text. Decide default.
- **Oracle `LocalTime`**: no TIME type → `INTERVAL DAY TO SECOND` chosen; confirm round-trip via
  the managed provider returns `TimeSpan`.
- **Oracle container** is heavy (slow CI pulls/startup); consider an opt-in/nightly gate.
- **`MySqlConnector` vs `MySql.Data`**: plan assumes `MySqlConnector`; confirm preference.
- **SQLite** is embedded, not containerized — confirm this satisfies the "all 5 via Testcontainers"
  intent (SQLite still part of the matrix, just in-process).
- **Net Framework TFMs** for vendor packages reach .NET Framework only through `netstandard2.0`;
  confirm that is acceptable (vs. explicit `net462` targets).
