using System.Data.Common;
using AdaskoTheBeAsT.Dapper.NodaTime.IntegrationTests.Common;
using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.Oracle.IntegrationTest
{
    [Collection("oracle")]
    public sealed class OracleRoundTripTests
        : NodaTimeRoundTripTests
    {
        private readonly OracleFixture _fixture;

        public OracleRoundTripTests(OracleFixture fixture) => _fixture = fixture;

        protected override string ParameterPrefix => ":";

        protected override DbConnection OpenConnection() => _fixture.OpenConnection();
    }
}
