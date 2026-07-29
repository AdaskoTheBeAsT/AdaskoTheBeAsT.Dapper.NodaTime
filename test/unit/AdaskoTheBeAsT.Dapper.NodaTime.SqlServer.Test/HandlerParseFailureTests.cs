using System;
using System.Data;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Microsoft.Data.SqlClient;
using Moq;
using NodaTime;
using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.SqlServer.Test
{
    public sealed class HandlerParseFailureTests
    {
        private readonly SqlServerNodaTimeConfiguration _configuration = new();

        [Fact]
        public void Handlers_ThrowForUnsupportedValueTypes()
        {
            var value = new object();
            var parsers = new Action[]
            {
                () => new InstantHandler(_configuration).Parse(value),
                () => new LocalDateHandler(_configuration).Parse(value),
                () => new LocalDateTimeHandler(_configuration).Parse(value),
                () => new LocalTimeHandler(_configuration).Parse(value),
                () => new OffsetDateTimeHandler(_configuration).Parse(value),
                () => new OffsetHandler(_configuration).Parse(value),
                () => new DurationHandler(_configuration).Parse(value),
                () => new PeriodHandler(_configuration).Parse(value),
                () => new CalendarSystemHandler(_configuration).Parse(value),
                () => new DateTimeZoneHandler(DateTimeZoneProviders.Tzdb, _configuration).Parse(value),
            };

            using var scope = new AssertionScope();
            foreach (var parse in parsers)
            {
                parse.Should().Throw<DataException>();
            }
        }

        [Fact]
        public void Handlers_ThrowForNullAndDbNullValues()
        {
            Action parseNull = () => new InstantHandler(_configuration).Parse(null!);
            Action parseDbNull = () => new LocalDateHandler(_configuration).Parse(DBNull.Value);

            using var scope = new AssertionScope();
            parseNull.Should().Throw<DataException>();
            parseDbNull.Should().Throw<DataException>();
        }

        [Fact]
        public void Handlers_ThrowForUnparsableStrings()
        {
            Action parseLocalDate = () => new LocalDateHandler(_configuration).Parse("not-a-date");
            Action parseLocalTime = () => new LocalTimeHandler(_configuration).Parse("not-a-time");
            Action parseOffsetDateTime = () => new OffsetDateTimeHandler(_configuration).Parse("not-a-date");
            Action parseInstant = () => new InstantHandler(_configuration).Parse("not-a-dateZ");

            using var scope = new AssertionScope();
            parseLocalDate.Should().Throw<DataException>();
            parseLocalTime.Should().Throw<DataException>();
            parseOffsetDateTime.Should().Throw<DataException>();
            parseInstant.Should().Throw<DataException>();
        }

        [Fact]
        public void LocalTimeHandler_ParsesDateTimeStringAsTimeOfDay()
        {
            var result = new LocalTimeHandler(_configuration).Parse("2024-01-02T03:04:05");

            result.Should().Be(new LocalTime(3, 4, 5));
        }

        [Fact]
        public void DurationHandler_ParsesInt32Nanoseconds()
        {
            var result = new DurationHandler(_configuration).Parse(123);

            result.Should().Be(Duration.FromNanoseconds(123));
        }

        [Fact]
        public void TrySetNative_ThrowsWhenActionIsNull()
        {
            Action action = () => TrySetNativeProbe.SetWithNullAction(new SqlParameter());

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SqlServerConfiguration_SkipsNativeTypeForParametersWithoutSqlDbType()
        {
            var parameter = new Mock<IDbDataParameter>(MockBehavior.Strict);
            parameter.SetupAllProperties();

            _configuration.SetDuration(parameter.Object, Duration.FromNanoseconds(5));

            using var scope = new AssertionScope();
            parameter.Object.DbType.Should().Be(DbType.Int64);
            parameter.Object.Value.Should().Be(5L);
        }

        private sealed class TrySetNativeProbe : NodaTimeTypeHandlerConfigurationBase
        {
            public static void SetWithNullAction(IDbDataParameter parameter) => TrySetNative<SqlParameter>(parameter, null!);
        }
    }
}
