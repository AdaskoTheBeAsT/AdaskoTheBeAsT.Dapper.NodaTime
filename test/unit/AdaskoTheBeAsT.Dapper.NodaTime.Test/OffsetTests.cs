using System.Linq;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Dapper;
using NodaTime;
using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.Test
{
    [Collection("DBTests")]
    public sealed class OffsetTests
    {
        private readonly string _connectionString;

        public OffsetTests(DatabaseFixture databaseFixture)
        {
            _connectionString = databaseFixture.ConnectionString;
            SqlMapper.AddTypeHandler(OffsetHandler.Default);
        }

        [Theory]
        [ClassData(typeof(DbVendorLibraryTestData))]
        public void Can_Write_And_Read_Offset_Stored_As_Int(DbVendorLibrary library)
        {
            using var connection = DbVendorLibraryConnectionProvider.Provide(library, _connectionString);
            var o = new TestObject { Value = Offset.FromHours(18) };

            const string sql = "CREATE TABLE #T ([Value] int); INSERT INTO #T VALUES (@Value); SELECT * FROM #T";
            var t = connection.Query<TestObject>(sql, o).First();

            t.Value.Should().Be(o.Value);
        }

        [Theory]
        [ClassData(typeof(DbVendorLibraryTestData))]
        public void Can_Write_And_Read_Offset_With_Null_Value_Stored_As_Int(DbVendorLibrary library)
        {
            using var connection = DbVendorLibraryConnectionProvider.Provide(library, _connectionString);
            var o = new TestObject();

            const string sql = "CREATE TABLE #T ([Value] int NULL); INSERT INTO #T VALUES (@Value); SELECT * FROM #T";
            var t = connection.Query<TestObject>(sql, o).First();

            using (new AssertionScope())
            {
                t.Value.Should().BeNull();
                t.Value.Should().Be(o.Value);
            }
        }

        [Fact]
        public void OffsetHandler_Parse_Should_Return_Same_Offset()
        {
            // Arrange
            var handler = OffsetHandler.Default;
            var offset = Offset.FromHours(12);

            // Act
            var result = handler.Parse(offset);

            // Assert
            result.Should().Be(offset);
        }

        private sealed class TestObject
        {
            public Offset? Value { get; set; }
        }
    }
}
