using System.Data.Common;
using AdaskoTheBeAsT.Dapper.NodaTime.IntegrationTests.Common;
using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.Sqlite.IntegrationTest
{
    [Collection("sqlite")]
    public sealed class SqliteRoundTripTests
        : NodaTimeRoundTripTests
    {
        private readonly SqliteFixture _fixture;

        public SqliteRoundTripTests(SqliteFixture fixture) => _fixture = fixture;

        protected override DbConnection OpenConnection() => _fixture.OpenConnection();
    }
}
