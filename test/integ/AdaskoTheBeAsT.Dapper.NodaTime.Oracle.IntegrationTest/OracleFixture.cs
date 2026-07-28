using System.Collections.Generic;
using System.Data.Common;
using AdaskoTheBeAsT.Dapper.NodaTime.IntegrationTests.Common;
using NodaTime;
using Oracle.ManagedDataAccess.Client;
using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.Oracle.IntegrationTest
{
    public sealed class OracleFixture : DockerDatabaseFixture
    {
        public OracleFixture() : base("gvenzl/oracle-free:23-slim", 1521, new Dictionary<string, string> { ["ORACLE_PASSWORD"] = "NodaTime!Passw0rd" }) { }
        protected override string SchemaFileName => "001_CreateNodaTimeRoundTrip.sql";
        public override DbConnection OpenConnection() => new OracleConnection($"User Id=system;Password=NodaTime!Passw0rd;Data Source={Host}:{MappedPort}/FREEPDB1");
        protected override void ConfigureHandlers() => OracleDapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb);
    }
    [CollectionDefinition("oracle")] public sealed class OracleCollection : ICollectionFixture<OracleFixture> { }
    [Collection("oracle")] public sealed class OracleRoundTripTests : NodaTimeRoundTripTests { private readonly OracleFixture _fixture; public OracleRoundTripTests(OracleFixture fixture) => _fixture = fixture; protected override DbConnection OpenConnection() => _fixture.OpenConnection(); protected override string ParameterPrefix => ":"; }
}
