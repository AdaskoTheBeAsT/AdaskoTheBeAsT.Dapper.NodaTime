using System.Linq;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Dapper;
using NodaTime;
using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.Test
{
    [Collection("DBTests")]
    public sealed class CalendarSystemTests
    {
        private readonly string _connectionString;

        public CalendarSystemTests(DatabaseFixture databaseFixture)
        {
            _connectionString = databaseFixture.ConnectionString;
            SqlMapper.AddTypeHandler(CalendarSystemHandler.Default);
        }

        [Theory]
        [ClassData(typeof(DbVendorLibraryTestData))]
        public void Can_Write_And_Read_CalendarSystem_Stored_As_Int(DbVendorLibrary library)
        {
            using var connection = DbVendorLibraryConnectionProvider.Provide(library, _connectionString);
            var o = new TestObject { Value = CalendarSystem.Iso };

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
        public void Can_Write_And_Read_CalendarSystem_With_Null_Value_Stored_As_Int(DbVendorLibrary library)
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
        public void CalendarSystemHandler_Parse_Should_Return_Same_CalendarSystem()
        {
            // Arrange
            var handler = CalendarSystemHandler.Default;
            var calendarSystem = CalendarSystem.Iso;

            // Act
            var result = handler.Parse(calendarSystem);

            // Assert
            result.Should().Be(calendarSystem);
        }

        private sealed class TestObject
        {
            public CalendarSystem? Value { get; set; }
        }
    }
}
