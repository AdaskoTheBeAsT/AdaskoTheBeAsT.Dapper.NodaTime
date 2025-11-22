# AdaskoTheBeAsT.Dapper.NodaTime

> 🚀 Seamless NodaTime integration for Dapper - because dates and times should just work.

[![NuGet Version](https://img.shields.io/nuget/v/AdaskoTheBeAsT.Dapper.NodaTime.svg?style=flat)](https://www.nuget.org/packages/AdaskoTheBeAsT.Dapper.NodaTime/)
![Nuget](https://img.shields.io/nuget/dt/AdaskoTheBeAsT.Dapper.NodaTime)
[![Build Status](https://adaskothebeast.visualstudio.com/AdaskoTheBeAsT.Dapper.NodaTime/_apis/build/status/AdaskoTheBeAsT.AdaskoTheBeAsT.Dapper.NodaTime?branchName=master)](https://adaskothebeast.visualstudio.com/AdaskoTheBeAsT.Dapper.NodaTime/_build/latest?definitionId=21&branchName=master)
![Azure DevOps coverage](https://img.shields.io/azure-devops/coverage/AdaskoTheBeAsT/AdaskoTheBeAsT.Dapper.NodaTime/21?style=plastic)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=AdaskoTheBeAsT_AdaskoTheBeAsT.Dapper.NodaTime&metric=alert_status)](https://sonarcloud.io/dashboard?id=AdaskoTheBeAsT_AdaskoTheBeAsT.Dapper.NodaTime)
[![CodeFactor](https://www.codefactor.io/repository/github/adaskothebeast/adaskothebeast.dapper.nodatime/badge)](https://www.codefactor.io/repository/github/adaskothebeast/adaskothebeast.dapper.nodatime)

## Why This Library?

Working with dates and times is hard. Working with them across database boundaries is harder. This library bridges the gap between [Dapper](https://github.com/DapperLib/Dapper)'s simplicity and [NodaTime](https://nodatime.org/)'s correctness, giving you:

- ✅ **Type-safe** date/time operations across your data layer
- ✅ **Zero configuration** for most common scenarios
- ✅ **Battle-tested** with comprehensive test coverage
- ✅ **Modern .NET** support (netstandard2.0, .NET 8, 9, 10)
- ✅ **Production-ready** continuation of the original Dapper-NodaTime project

## Quick Start

### Installation

```bash
dotnet add package AdaskoTheBeAsT.Dapper.NodaTime
```

Or via Package Manager:
```powershell
Install-Package AdaskoTheBeAsT.Dapper.NodaTime
```

### Setup (One Line!)

Add this to your startup code:

```csharp
using AdaskoTheBeAsT.Dapper.NodaTime;
using NodaTime;

// Register all NodaTime type handlers
DapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb);
```

**That's it!** Now Dapper automatically handles all NodaTime types.

### Usage Example

```csharp
using Dapper;
using NodaTime;

public class Event
{
    public int Id { get; set; }
    public string Name { get; set; }
    public LocalDateTime ScheduledAt { get; set; }
    public Instant CreatedAt { get; set; }
    public Duration Duration { get; set; }
}

// Query with NodaTime types - just works!
using var connection = new SqlConnection(connectionString);
var events = await connection.QueryAsync<Event>(
    "SELECT Id, Name, ScheduledAt, CreatedAt, Duration FROM Events WHERE ScheduledAt > @date",
    new { date = new LocalDateTime(2025, 11, 22, 9, 0) }
);

// Insert with NodaTime types - seamless!
await connection.ExecuteAsync(
    "INSERT INTO Events (Name, ScheduledAt, CreatedAt, Duration) VALUES (@Name, @ScheduledAt, @CreatedAt, @Duration)",
    new Event 
    { 
        Name = "Team Meeting",
        ScheduledAt = new LocalDateTime(2025, 11, 23, 10, 0),
        CreatedAt = SystemClock.Instance.GetCurrentInstant(),
        Duration = Duration.FromHours(1)
    }
);
```

## Supported Types & SQL Mappings

| NodaTime Type | SQL Server Types | Description |
|---------------|------------------|-------------|
| `Instant` | `datetime`, `datetime2`, `datetimeoffset` | A point on the global timeline |
| `LocalDateTime` | `datetime`, `datetime2` | Date and time without timezone |
| `LocalDate` | `date`, `datetime`, `datetime2` | Just the date part |
| `LocalTime` | `time`, `datetime`, `datetime2` | Just the time part |
| `OffsetDateTime` | `datetimeoffset` | Date/time with UTC offset |
| `Duration` | `bigint` | Elapsed time in nanoseconds |
| `Period` | `varchar(176)` | Human-readable period (ISO8601) |
| `Offset` | `int` | UTC offset in seconds |
| `DateTimeZone` | `varchar(50)` | Time zone identifier |
| `CalendarSystem` | `varchar(50)` | Calendar system identifier |

## Advanced Configuration

### Individual Handler Registration

If you prefer granular control:

```csharp
using Dapper;

SqlMapper.AddTypeHandler(LocalDateTimeHandler.Default);
SqlMapper.AddTypeHandler(InstantHandler.Default);
SqlMapper.AddTypeHandler(DurationHandler.Default);
// ... register only what you need
```

### Time Zone Providers

Choose the right provider for your use case:

```csharp
// IANA Time Zone Database (recommended for cross-platform)
DapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb);

// Windows Registry (for Windows-specific applications)
DapperNodaTimeSetup.Register(DateTimeZoneProviders.Bcl);
```

## Database Schema Recommendations

```sql
-- Recommended column types for best compatibility
CREATE TABLE Events (
    Id INT PRIMARY KEY,
    Name NVARCHAR(255),
    
    -- For timezone-aware moments
    CreatedAt DATETIMEOFFSET NOT NULL,
    
    -- For local date/time (no timezone)
    ScheduledAt DATETIME2 NOT NULL,
    
    -- For date-only fields
    EventDate DATE NOT NULL,
    
    -- For time-only fields
    StartTime TIME NOT NULL,
    
    -- For durations
    Duration BIGINT NOT NULL,
    
    -- For periods (human-readable)
    RecurrencePeriod VARCHAR(176) NULL,
    
    -- For timezone reference
    TimeZone VARCHAR(50) NOT NULL
);
```

## Period Format

Periods are stored using ISO8601 roundtrip format (max 176 characters):

```
P-2147483648Y-2147483648M-2147483648W-2147483648DT-9223372036854775808H-9223372036854775808M-9223372036854775808S-9223372036854775808s-9223372036854775808t-9223372036854775808n
```

Example: `P1Y2M3W4DT5H6M7S` represents 1 year, 2 months, 3 weeks, 4 days, 5 hours, 6 minutes, and 7 seconds.

## Known Limitations

- **`ZonedDateTime`** is not directly supported. However, you can store its components separately:
  ```csharp
  public class EventWithZone
  {
      public LocalDateTime LocalDateTime { get; set; }
      public DateTimeZone TimeZone { get; set; }
      public CalendarSystem Calendar { get; set; }
      
      // Compose ZonedDateTime in your application layer
      public ZonedDateTime ToZonedDateTime() => 
          LocalDateTime.InZoneStrictly(TimeZone).WithCalendar(Calendar);
  }
  ```

## Framework Support

- **.NET Standard 2.0** (compatible with .NET Framework 4.6.1+, .NET Core 2.0+)
- **.NET 8.0**
- **.NET 9.0**
- **.NET 10.0**

## Contributing

This project is a continuation of Matt Johnson-Pint's original [Dapper-NodaTime](https://github.com/mj1856/Dapper-NodaTime) project, kept alive and updated for modern .NET. Contributions are welcome!

1. Fork the repository
2. Create a feature branch
3. Make your changes with tests
4. Submit a pull request

## Credits

- **Original Author**: [Matt Johnson-Pint](https://github.com/mj1856)
- **Current Maintainer**: Adam "AdaskoTheBeAsT" Pluciński

## License

[MIT License](LICENSE) - see the LICENSE file for details.

---

**Need Help?** Open an issue on [GitHub](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.Dapper.NodaTime/issues) or check out the [NodaTime documentation](https://nodatime.org/).
