using System.Linq;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Dapper;
using NodaTime;
using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.Test
{
    [Collection("DBTests")]
    public sealed class DateTimeZoneTests
    {
        private readonly string _connectionString;

        public DateTimeZoneTests(DatabaseFixture databaseFixture)
        {
            _connectionString = databaseFixture.ConnectionString;
            SqlMapper.AddTypeHandler(DateTimeZoneHandler.Default(DateTimeZoneProviders.Tzdb));
        }

        [Theory]
        [ClassData(typeof(DbVendorLibraryTestData))]
        public void Can_Write_And_Read_DateTimeZone_Stored_As_Int(DbVendorLibrary library)
        {
            using var connection = DbVendorLibraryConnectionProvider.Provide(library, _connectionString);
            var o = new TestObject { Value = DateTimeZoneProviders.Tzdb["Europe/Warsaw"] };

            const string sql = "CREATE TABLE #T ([Value] varchar(50)); INSERT INTO #T VALUES (@Value); SELECT * FROM #T";
            var t = connection.Query<TestObject>(sql, o).First();

            using (new AssertionScope())
            {
                t.Value.Should().NotBeNull();
                t.Value!.Id.Should().Be(o.Value.Id);
            }
        }

        [Theory]
        [ClassData(typeof(DbVendorLibraryTestData))]
        public void Can_Write_And_Read_DateTimeZone_With_Null_Value_Stored_As_Int(DbVendorLibrary library)
        {
            using var connection = DbVendorLibraryConnectionProvider.Provide(library, _connectionString);
            var o = new TestObject();

            const string sql = "CREATE TABLE #T ([Value] varchar(50) NULL); INSERT INTO #T VALUES (@Value); SELECT * FROM #T";
            var t = connection.Query<TestObject>(sql, o).First();

            using (new AssertionScope())
            {
                t.Value.Should().BeNull();
                t.Value.Should().Be(o.Value);
            }
        }

        [Fact]
        public void DateTimeZoneHandler_Parse_Should_Return_Same_CalendarSystem()
        {
            // Arrange
            var handler = DateTimeZoneHandler.Default(DateTimeZoneProviders.Tzdb);
            var dateTimeZone = DateTimeZone.ForOffset(Offset.FromHours(1));

            // Act
            var result = handler.Parse(dateTimeZone);

            // Assert
            result.Should().Be(dateTimeZone);
        }

        private sealed class TestObject
        {
            public DateTimeZone? Value { get; set; }
        }
    }
}
