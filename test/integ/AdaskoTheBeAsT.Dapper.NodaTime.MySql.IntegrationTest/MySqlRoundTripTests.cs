using System.Data.Common;
using AdaskoTheBeAsT.Dapper.NodaTime.IntegrationTests.Common;
using NodaTime;
using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.MySql.IntegrationTest
{
    [Collection("mysql")]
    public sealed class MySqlRoundTripTests
        : NodaTimeRoundTripTests
    {
        private readonly MySqlFixture _fixture;

        public MySqlRoundTripTests(MySqlFixture fixture) => _fixture = fixture;

        protected override DbConnection OpenConnection() => _fixture.OpenConnection();

        protected override OffsetDateTime NormalizeOffsetDateTime(OffsetDateTime value) => value.ToInstant().WithOffset(Offset.Zero);
    }
}
