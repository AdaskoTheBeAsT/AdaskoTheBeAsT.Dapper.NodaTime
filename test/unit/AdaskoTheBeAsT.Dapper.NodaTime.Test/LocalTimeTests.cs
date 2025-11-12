using System.Linq;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Dapper;
using NodaTime;
using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.Test
{
    [Collection("DBTests")]
    public sealed class LocalTimeTests
    {
        private readonly string _connectionString;

        public LocalTimeTests(DatabaseFixture databaseFixture)
        {
            _connectionString = databaseFixture.ConnectionString;
            SqlMapper.AddTypeHandler(LocalTimeHandler.Default);
        }

        [Theory]
        [ClassData(typeof(DbVendorLibraryTestData))]
        public void Can_Write_And_Read_LocalTime_Stored_As_Time(DbVendorLibrary library)
        {
            using var connection = DbVendorLibraryConnectionProvider.Provide(library, _connectionString);
            var o = new TestObject { Value = new LocalTime(23, 59, 59, 999) };

            const string sql = "CREATE TABLE #T ([Value] time); INSERT INTO #T VALUES (@Value); SELECT * FROM #T";
            var t = connection.Query<TestObject>(sql, o).First();

            t.Value.Should().Be(o.Value);
        }

        [Theory]
        [ClassData(typeof(DbVendorLibraryTestData))]
        public void Can_Write_And_Read_LocalTime_Stored_As_DateTime(DbVendorLibrary library)
        {
            using var connection = DbVendorLibraryConnectionProvider.Provide(library, _connectionString);
            var o = new TestObject { Value = new LocalTime(23, 59, 59, 333) };

            const string sql = "CREATE TABLE #T ([Value] datetime); INSERT INTO #T VALUES (@Value); SELECT * FROM #T";
            var t = connection.Query<TestObject>(sql, o).First();

            t.Value.Should().Be(o.Value);
        }

        [Theory]
        [ClassData(typeof(DbVendorLibraryTestData))]
        public void Can_Write_And_Read_LocalTime_Stored_As_DateTime2(DbVendorLibrary library)
        {
            using var connection = DbVendorLibraryConnectionProvider.Provide(library, _connectionString);
            var o = new TestObject { Value = new LocalTime(23, 59, 59, 999) };

            const string sql = "CREATE TABLE #T ([Value] datetime2); INSERT INTO #T VALUES (@Value); SELECT * FROM #T";
            var t = connection.Query<TestObject>(sql, o).First();

            t.Value.Should().Be(o.Value);
        }

        [Theory]
        [ClassData(typeof(DbVendorLibraryTestData))]
        public void Can_Write_And_Read_LocalTime_With_Null_Value_Stored_As_Time(DbVendorLibrary library)
        {
            using var connection = DbVendorLibraryConnectionProvider.Provide(library, _connectionString);
            var o = new TestObject();

            const string sql = "CREATE TABLE #T ([Value] time NULL); INSERT INTO #T VALUES (@Value); SELECT * FROM #T";
            var t = connection.Query<TestObject>(sql, o).First();

            using (new AssertionScope())
            {
                t.Value.Should().BeNull();
                t.Value.Should().Be(o.Value);
            }
        }

        [Theory]
        [ClassData(typeof(DbVendorLibraryTestData))]
        public void Can_Write_And_Read_LocalTime_With_Null_Value_Stored_As_DateTime(DbVendorLibrary library)
        {
            using var connection = DbVendorLibraryConnectionProvider.Provide(library, _connectionString);
            var o = new TestObject();

            const string sql = "CREATE TABLE #T ([Value] datetime NULL); INSERT INTO #T VALUES (@Value); SELECT * FROM #T";
            var t = connection.Query<TestObject>(sql, o).First();

            using (new AssertionScope())
            {
                t.Value.Should().BeNull();
                t.Value.Should().Be(o.Value);
            }
        }

        [Theory]
        [ClassData(typeof(DbVendorLibraryTestData))]
        public void Can_Write_And_Read_LocalTime_With_Null_Value_Stored_As_DateTime2(DbVendorLibrary library)
        {
            using var connection = DbVendorLibraryConnectionProvider.Provide(library, _connectionString);
            var o = new TestObject();

            const string sql = "CREATE TABLE #T ([Value] datetime2 NULL); INSERT INTO #T VALUES (@Value); SELECT * FROM #T";
            var t = connection.Query<TestObject>(sql, o).First();

            using (new AssertionScope())
            {
                t.Value.Should().BeNull();
                t.Value.Should().Be(o.Value);
            }
        }

        [Fact]
        public void LocalTimeHandler_Parse_Should_Return_Same_LocalTime()
        {
            // Arrange
            var handler = LocalTimeHandler.Default;
            var localTime = LocalTime.FromHoursSinceMidnight(12);

            // Act
            var result = handler.Parse(localTime);

            // Assert
            result.Should().Be(localTime);
        }

        private sealed class TestObject
        {
            public LocalTime? Value { get; set; }
        }
    }
}
