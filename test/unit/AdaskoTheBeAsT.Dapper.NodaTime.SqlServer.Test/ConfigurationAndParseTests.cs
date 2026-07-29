using System;
using System.Data;
using AwesomeAssertions;
using Microsoft.Data.SqlClient;
using NodaTime;
using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.SqlServer.Test
{
    public sealed class ConfigurationAndParseTests
    {
        private readonly TestConfiguration _configuration = new();

        [Fact]
        public void BaseConfiguration_ShapesInstantAsUtcDateTime2()
        {
            var parameter = new SqlParameter();
            var value = Instant.FromUtc(2024, 1, 2, 3, 4);

            _configuration.SetInstant(parameter, value);

            parameter.DbType.Should().Be(DbType.DateTime2);
            parameter.Value.Should().Be(value.ToDateTimeUtc());
        }

        [Fact]
        public void BaseConfiguration_WritesNullStringValuesAsDbNull()
        {
            var parameter = new SqlParameter();

            _configuration.SetPeriod(parameter, value: null);

            parameter.DbType.Should().Be(DbType.AnsiString);
            parameter.Value.Should().Be(DBNull.Value);
        }

        [Fact]
        public void Handlers_ParseSqliteStyleStringsAndOracleNumbers()
        {
            new InstantHandler(_configuration).Parse("2024-01-02T03:04:05Z").Should().Be(Instant.FromUtc(2024, 1, 2, 3, 4, 5));
            new LocalDateHandler(_configuration).Parse("2024-01-02").Should().Be(new LocalDate(2024, 1, 2));
            new LocalDateTimeHandler(_configuration).Parse("2024-01-02T03:04:05").Should().Be(new LocalDateTime(2024, 1, 2, 3, 4, 5));
            new LocalTimeHandler(_configuration).Parse("03:04:05").Should().Be(new LocalTime(3, 4, 5));
            new OffsetDateTimeHandler(_configuration).Parse("2024-01-02T03:04:05+02:00").Offset.Should().Be(Offset.FromHours(2));
            new OffsetHandler(_configuration).Parse(3600m).Should().Be(Offset.FromHours(1));
            new DurationHandler(_configuration).Parse(123m).Should().Be(Duration.FromNanoseconds(123));
        }

#if NET8_0_OR_GREATER
        [Fact]
        public void Handlers_ParseDateOnlyAndTimeOnlyValues()
        {
            new LocalDateHandler(_configuration).Parse(new DateOnly(2024, 1, 2)).Should().Be(new LocalDate(2024, 1, 2));
            new LocalTimeHandler(_configuration).Parse(new TimeOnly(3, 4, 5)).Should().Be(new LocalTime(3, 4, 5));
        }
#endif

        [Fact]
        public void Register_RequiresAnExplicitConfiguration()
        {
            Action action = () => DapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        private sealed class TestConfiguration : NodaTimeTypeHandlerConfigurationBase;
    }
}
