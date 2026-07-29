using System.Data.Common;
using System.Threading.Tasks;
using AwesomeAssertions;
using Dapper;
using NodaTime;
using NodaTime.Text;
using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.IntegrationTests.Common
{
    public abstract class NodaTimeRoundTripTests
    {
        protected virtual string ParameterPrefix => "@";

        [Fact]
        public async Task All_supported_types_round_tripAsync()
        {
            var expected = TestRow.Create();
#if NET8_0_OR_GREATER
            await using var connection = OpenConnection();
#endif
#if NET462_OR_GREATER
            using var connection = OpenConnection();
#endif
            await connection.OpenAsync(TestCancellation.Token);
            await connection.ExecuteAsync("DELETE FROM NodaTimeRoundTrip");
            await connection.ExecuteAsync(
                $@"INSERT INTO NodaTimeRoundTrip
                (Id, InstantValue, LocalDateValue, LocalDateTimeValue, LocalTimeValue, OffsetDateTimeValue, OffsetValue, DurationValue, PeriodValue, CalendarValue, ZoneValue)
                VALUES
                ({ParameterPrefix}Id, {ParameterPrefix}InstantValue, {ParameterPrefix}LocalDateValue, {ParameterPrefix}LocalDateTimeValue, {ParameterPrefix}LocalTimeValue, {ParameterPrefix}OffsetDateTimeValue, {ParameterPrefix}OffsetValue, {ParameterPrefix}DurationValue, {ParameterPrefix}PeriodValue, {ParameterPrefix}CalendarValue, {ParameterPrefix}ZoneValue)",
                expected);

            var actual = await connection.QuerySingleAsync<TestRow>($"SELECT * FROM NodaTimeRoundTrip WHERE Id = {ParameterPrefix}Id", new { expected.Id });
            actual.InstantValue.Should().Be(expected.InstantValue);
            actual.LocalDateValue.Should().Be(expected.LocalDateValue);
            actual.LocalDateTimeValue.Should().Be(expected.LocalDateTimeValue);
            actual.LocalTimeValue.Should().Be(expected.LocalTimeValue);
            actual.OffsetDateTimeValue.Should().Be(NormalizeOffsetDateTime(expected.OffsetDateTimeValue!.Value));
            actual.OffsetValue.Should().Be(expected.OffsetValue);
            actual.DurationValue.Should().Be(expected.DurationValue);
            PeriodPattern.Roundtrip.Format(actual.PeriodValue!).Should().Be(PeriodPattern.Roundtrip.Format(expected.PeriodValue!));
            actual.CalendarValue!.Id.Should().Be(expected.CalendarValue!.Id);
            actual.ZoneValue!.Id.Should().Be(expected.ZoneValue!.Id);
        }

        [Fact]
        public async Task Nullable_reference_types_round_trip_as_nullAsync()
        {
            var row = new TestRow { Id = 2 };
#if NET8_0_OR_GREATER
            await using var connection = OpenConnection();
#endif
#if NET462_OR_GREATER
            using var connection = OpenConnection();
#endif
            await connection.OpenAsync(TestCancellation.Token);
            await connection.ExecuteAsync($"DELETE FROM NodaTimeRoundTrip WHERE Id = {ParameterPrefix}Id", row);
            await connection.ExecuteAsync($"INSERT INTO NodaTimeRoundTrip (Id, PeriodValue, CalendarValue, ZoneValue) VALUES ({ParameterPrefix}Id, {ParameterPrefix}PeriodValue, {ParameterPrefix}CalendarValue, {ParameterPrefix}ZoneValue)", row);

            var actual = await connection.QuerySingleAsync<TestRow>($"SELECT * FROM NodaTimeRoundTrip WHERE Id = {ParameterPrefix}Id", row);
            actual.PeriodValue.Should().BeNull();
            actual.CalendarValue.Should().BeNull();
            actual.ZoneValue.Should().BeNull();
        }

        protected abstract DbConnection OpenConnection();

        protected virtual OffsetDateTime NormalizeOffsetDateTime(OffsetDateTime value) => value;

        private sealed class TestRow
        {
            public long Id { get; set; }

            public Instant? InstantValue { get; set; }

            public LocalDate? LocalDateValue { get; set; }

            public LocalDateTime? LocalDateTimeValue { get; set; }

            public LocalTime? LocalTimeValue { get; set; }

            public OffsetDateTime? OffsetDateTimeValue { get; set; }

            public Offset? OffsetValue { get; set; }

            public Duration? DurationValue { get; set; }

            public Period? PeriodValue { get; set; }

            public CalendarSystem? CalendarValue { get; set; }

            public DateTimeZone? ZoneValue { get; set; }

            public static TestRow Create() => new()
            {
                Id = 1,
                InstantValue = Instant.FromUtc(2024, 1, 2, 3, 4, 5).PlusTicks(1234560),
                LocalDateValue = new LocalDate(2024, 1, 2),
                LocalDateTimeValue = new LocalDateTime(2024, 1, 2, 3, 4, 5).PlusTicks(1234560),
                LocalTimeValue = new LocalTime(3, 4, 5).PlusTicks(1234560),
                OffsetDateTimeValue = new OffsetDateTime(new LocalDateTime(2024, 1, 2, 3, 4, 5), Offset.FromHours(2)),
                OffsetValue = Offset.FromHours(2),
                DurationValue = Duration.FromNanoseconds(123456789),
                PeriodValue = Period.FromMonths(2) + Period.FromDays(3) + Period.FromHours(4),
                CalendarValue = CalendarSystem.Iso,
                ZoneValue = DateTimeZoneProviders.Tzdb["Europe/Warsaw"],
            };
        }
    }
}
