using System.Linq;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Dapper;
using NodaTime;
using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.Test
{
    [Collection("DBTests")]
    public sealed class PeriodTests
    {
        private readonly string _connectionString;

        public PeriodTests(DatabaseFixture databaseFixture)
        {
            _connectionString = databaseFixture.ConnectionString;
            SqlMapper.AddTypeHandler(PeriodHandler.Default);
        }

        [Theory]
        [ClassData(typeof(DbVendorLibraryTestData))]
        public void Can_Write_And_Read_Period_Stored_As_Varchar(DbVendorLibrary library)
        {
            using var connection = DbVendorLibraryConnectionProvider.Provide(library, _connectionString);
            var o = new TestObject
            {
                Value = new PeriodBuilder
                {
                    Years = int.MinValue,
                    Months = int.MinValue,
                    Weeks = int.MinValue,
                    Days = int.MinValue,
                    Hours = long.MinValue,
                    Minutes = long.MinValue,
                    Seconds = long.MinValue,
                    Milliseconds = long.MinValue,
                    Ticks = long.MinValue,
                    Nanoseconds = long.MinValue,
                }.Build(),
            };

            const string sql = "CREATE TABLE #T ([Value] varchar(176)); INSERT INTO #T VALUES (@Value); SELECT * FROM #T";
            var t = connection.Query<TestObject>(sql, o).First();

            t.Value.Should().Be(o.Value);
        }

        [Theory]
        [ClassData(typeof(DbVendorLibraryTestData))]
        public void Can_Write_And_Read_Period_With_Null_Value_Stored_As_Varchar(DbVendorLibrary library)
        {
            using var connection = DbVendorLibraryConnectionProvider.Provide(library, _connectionString);
            var o = new TestObject();

            const string sql = "CREATE TABLE #T ([Value] varchar(176) NULL); INSERT INTO #T VALUES (@Value); SELECT * FROM #T";
            var t = connection.Query<TestObject>(sql, o).First();

            using (new AssertionScope())
            {
                t.Value.Should().BeNull();
                t.Value.Should().Be(o.Value);
            }
        }

        [Fact]
        public void PeriodHandler_Parse_Should_Return_Same_Period()
        {
            // Arrange
            var handler = PeriodHandler.Default;
            var period = new PeriodBuilder
            {
                Years = 1,
                Months = 2,
                Weeks = 3,
                Days = 4,
                Hours = 5,
                Minutes = 6,
                Seconds = 7,
                Milliseconds = 8,
                Ticks = 9,
                Nanoseconds = 10,
            }.Build();

            // Act
            var result = handler.Parse(period);

            // Assert
            result.Should().Be(period);
        }

        private sealed class TestObject
        {
            public Period? Value { get; set; }
        }
    }
}
