using System.Collections.Generic;
using System.Data.Common;
using AdaskoTheBeAsT.Dapper.NodaTime.IntegrationTests.Common;
using Microsoft.Data.SqlClient;
using NodaTime;
using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.SqlServer.IntegrationTest
{
    public sealed class SqlServerFixture : DockerDatabaseFixture
    {
        public SqlServerFixture()
            : base("mcr.microsoft.com/mssql/server:2022-latest", 1433, new Dictionary<string, string> { ["ACCEPT_EULA"] = "Y", ["MSSQL_SA_PASSWORD"] = "NodaTime!Passw0rd" }) { }
        protected override string SchemaFileName => "001_CreateNodaTimeRoundTrip.sql";
        public override DbConnection OpenConnection() => new SqlConnection($"Server={Host},{MappedPort};User ID=sa;Password=NodaTime!Passw0rd;TrustServerCertificate=True;Encrypt=False");
        protected override void ConfigureHandlers() => SqlServerDapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb);
    }
    [CollectionDefinition("sqlserver")] public sealed class SqlServerCollection : ICollectionFixture<SqlServerFixture> { }
    [Collection("sqlserver")] public sealed class SqlServerRoundTripTests : NodaTimeRoundTripTests { private readonly SqlServerFixture _fixture; public SqlServerRoundTripTests(SqlServerFixture fixture) => _fixture = fixture; protected override DbConnection OpenConnection() => _fixture.OpenConnection(); }
}
