using System.Data.Common;
using AdaskoTheBeAsT.Dapper.NodaTime.IntegrationTests.Common;
using NodaTime;
using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.PostgreSql.IntegrationTest
{
    [Collection("postgresql")]
    public sealed class PostgreSqlRoundTripTests
        : NodaTimeRoundTripTests
    {
        private readonly PostgreSqlFixture _fixture;

        public PostgreSqlRoundTripTests(PostgreSqlFixture fixture) => _fixture = fixture;

        protected override DbConnection OpenConnection() => _fixture.OpenConnection();

        protected override OffsetDateTime NormalizeOffsetDateTime(OffsetDateTime value) => value.ToInstant().WithOffset(Offset.Zero);
    }
}
