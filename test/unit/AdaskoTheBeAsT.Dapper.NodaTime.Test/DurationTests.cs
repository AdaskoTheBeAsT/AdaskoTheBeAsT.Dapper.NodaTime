using System.Linq;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Dapper;
using NodaTime;
using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.Test
{
    [Collection("DBTests")]
    public sealed class DurationTests
    {
        private readonly string _connectionString;

        public DurationTests(DatabaseFixture databaseFixture)
        {
            _connectionString = databaseFixture.ConnectionString;
            SqlMapper.AddTypeHandler(DurationHandler.Default);
        }

        [Theory]
        [ClassData(typeof(DbVendorLibraryTestData))]
        public void Can_Write_And_Read_Duration_Stored_As_BigInt(DbVendorLibrary library)
        {
            using var connection = DbVendorLibraryConnectionProvider.Provide(library, _connectionString);
            var o = new TestObject { Value = Duration.FromDays(365) };

            const string sql = "CREATE TABLE #T ([Value] bigint); INSERT INTO #T VALUES (@Value); SELECT * FROM #T";
            var t = connection.Query<TestObject>(sql, o).First();

            t.Value.Should().Be(o.Value);
        }

        [Theory]
        [ClassData(typeof(DbVendorLibraryTestData))]
        public void Can_Write_And_Read_Duration_With_Null_Value_Stored_As_BigInt(DbVendorLibrary library)
        {
            using var connection = DbVendorLibraryConnectionProvider.Provide(library, _connectionString);
            var o = new TestObject();

            const string sql = "CREATE TABLE #T ([Value] bigint NULL); INSERT INTO #T VALUES (@Value); SELECT * FROM #T";
            var t = connection.Query<TestObject>(sql, o).First();

            using (new AssertionScope())
            {
                t.Value.Should().BeNull();
                t.Value.Should().Be(o.Value);
            }
        }

        [Fact]
        public void DurationHandler_Parse_Should_Return_Same_Duration()
        {
            // Arrange
            var handler = DurationHandler.Default;
            var duration = Duration.FromDays(10);

            // Act
            var result = handler.Parse(duration);

            // Assert
            result.Should().Be(duration);
        }

        private sealed class TestObject
        {
            public Duration? Value { get; set; }
        }
    }
}
