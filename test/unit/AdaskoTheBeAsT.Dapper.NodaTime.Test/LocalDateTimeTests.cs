using System;
using System.Linq;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Dapper;
using NodaTime;
using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.Test
{
    [Collection("DBTests")]
    public sealed class LocalDateTimeTests
    {
        private readonly string _connectionString;

        public LocalDateTimeTests(DatabaseFixture databaseFixture)
        {
            _connectionString = databaseFixture.ConnectionString;
            SqlMapper.AddTypeHandler(LocalDateTimeHandler.Default);
        }

        [Theory]
        [ClassData(typeof(DbVendorLibraryTestData))]
        public void Can_Write_And_Read_LocalDateTime_Stored_As_DateTime2(DbVendorLibrary library)
        {
            using var connection = DbVendorLibraryConnectionProvider.Provide(library, _connectionString);
            var o = new TestObject { Value = new LocalDateTime(1234, 12, 31, 1, 2, 3, 4) };

            const string sql = "CREATE TABLE #T ([Value] datetime2); INSERT INTO #T VALUES (@Value); SELECT * FROM #T";
            var t = connection.Query<TestObject>(sql, o).First();

            t.Value.Should().Be(o.Value);
        }

        [Theory]
        [ClassData(typeof(DbVendorLibraryTestData))]
        public void Can_Write_And_Read_LocalDateTime_Stored_As_DateTime(DbVendorLibrary library)
        {
            using var connection = DbVendorLibraryConnectionProvider.Provide(library, _connectionString);
            var o = new TestObject { Value = new LocalDateTime(1753, 12, 31, 1, 2, 3, 333) };

            const string sql = "CREATE TABLE #T ([Value] datetime); INSERT INTO #T VALUES (@Value); SELECT * FROM #T";
            var t = connection.Query<TestObject>(sql, o).First();

            t.Value.Should().Be(o.Value);
        }

        [Theory]
        [ClassData(typeof(DbVendorLibraryTestData))]
        public void Can_Write_And_Read_LocalDateTime_With_Null_Value_Stored_As_DateTime2(DbVendorLibrary library)
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

        [Theory]
        [ClassData(typeof(DbVendorLibraryTestData))]
        public void Can_Write_And_Read_LocalDateTime_With_Null_Value_Stored_As_DateTime(DbVendorLibrary library)
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

        [Fact]
        public void LocalDateTimeHandler_Parse_Should_Return_Same_LocalDateTime()
        {
            // Arrange
            var handler = LocalDateTimeHandler.Default;
            var localDateTime = LocalDateTime.FromDateTime(DateTime.Now);

            // Act
            var result = handler.Parse(localDateTime);

            // Assert
            result.Should().Be(localDateTime);
        }

        private sealed class TestObject
        {
            public LocalDateTime? Value { get; set; }
        }
    }
}
