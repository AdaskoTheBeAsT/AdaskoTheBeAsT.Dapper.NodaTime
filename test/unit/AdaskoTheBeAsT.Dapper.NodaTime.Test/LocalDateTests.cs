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
    public sealed class LocalDateTests
    {
        private readonly string _connectionString;

        public LocalDateTests(DatabaseFixture databaseFixture)
        {
            _connectionString = databaseFixture.ConnectionString;
            SqlMapper.AddTypeHandler(LocalDateHandler.Default);
        }

        [Theory]
        [ClassData(typeof(DbVendorLibraryTestData))]
        public void Can_Write_And_Read_LocalDate_Stored_As_Date(DbVendorLibrary library)
        {
            using var connection = DbVendorLibraryConnectionProvider.Provide(library, _connectionString);
            var o = new TestObject { Value = new LocalDate(1234, 12, 31) };

            const string sql = "CREATE TABLE #T ([Value] date); INSERT INTO #T VALUES (@Value); SELECT * FROM #T";
            var t = connection.Query<TestObject>(sql, o).First();

            t.Value.Should().Be(o.Value);
        }

        [Theory]
        [ClassData(typeof(DbVendorLibraryTestData))]
        public void Can_Write_And_Read_LocalDate_Stored_As_DateTime(DbVendorLibrary library)
        {
            using var connection = DbVendorLibraryConnectionProvider.Provide(library, _connectionString);
            var o = new TestObject { Value = new LocalDate(1753, 12, 31) };

            const string sql = "CREATE TABLE #T ([Value] datetime); INSERT INTO #T VALUES (@Value); SELECT * FROM #T";
            var t = connection.Query<TestObject>(sql, o).First();

            t.Value.Should().Be(o.Value);
        }

        [Theory]
        [ClassData(typeof(DbVendorLibraryTestData))]
        public void Can_Write_And_Read_LocalDate_Stored_As_DateTime2(DbVendorLibrary library)
        {
            using var connection = DbVendorLibraryConnectionProvider.Provide(library, _connectionString);
            var o = new TestObject { Value = new LocalDate(1234, 12, 31) };

            const string sql = "CREATE TABLE #T ([Value] datetime2); INSERT INTO #T VALUES (@Value); SELECT * FROM #T";
            var t = connection.Query<TestObject>(sql, o).First();

            t.Value.Should().Be(o.Value);
        }

        [Theory]
        [ClassData(typeof(DbVendorLibraryTestData))]
        public void Can_Write_And_Read_LocalDate_With_Null_Value_Stored_As_Date(DbVendorLibrary library)
        {
            using var connection = DbVendorLibraryConnectionProvider.Provide(library, _connectionString);
            var o = new TestObject();

            const string sql = "CREATE TABLE #T ([Value] date NULL); INSERT INTO #T VALUES (@Value); SELECT * FROM #T";
            var t = connection.Query<TestObject>(sql, o).First();

            using (new AssertionScope())
            {
                t.Value.Should().BeNull();
                t.Value.Should().Be(o.Value);
            }
        }

        [Theory]
        [ClassData(typeof(DbVendorLibraryTestData))]
        public void Can_Write_And_Read_LocalDate_With_Null_Value_Stored_As_DateTime(DbVendorLibrary library)
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
        public void Can_Write_And_Read_LocalDate_With_Null_Value_Stored_As_DateTime2(DbVendorLibrary library)
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
        public void LocalDateHandler_Parse_Should_Return_Same_LocalDate()
        {
            // Arrange
            var handler = LocalDateHandler.Default;
            var localDate = LocalDate.FromDateTime(DateTime.Now);

            // Act
            var result = handler.Parse(localDate);

            // Assert
            result.Should().Be(localDate);
        }

        private sealed class TestObject
        {
            public LocalDate? Value { get; set; }
        }
    }
}
