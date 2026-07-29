using System.Data.Common;
using AdaskoTheBeAsT.Dapper.NodaTime.IntegrationTests.Common;
using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.SqlServer.IntegrationTest
{
    [Collection("sqlserver")]
    public sealed class SqlServerRoundTripTests
        : NodaTimeRoundTripTests
    {
        private readonly SqlServerFixture _fixture;

        public SqlServerRoundTripTests(SqlServerFixture fixture) => _fixture = fixture;

        protected override DbConnection OpenConnection() => _fixture.OpenConnection();
    }
}
