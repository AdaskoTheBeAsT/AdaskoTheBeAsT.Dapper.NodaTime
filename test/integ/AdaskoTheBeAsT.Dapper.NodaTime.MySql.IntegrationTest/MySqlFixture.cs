using System.Collections.Generic;
using System.Data.Common;
using AdaskoTheBeAsT.Dapper.NodaTime.IntegrationTests.Common;
using MySqlConnector;
using NodaTime;
using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.MySql.IntegrationTest
{
    public sealed class MySqlFixture : DockerDatabaseFixture
    {
        public MySqlFixture() : base("mysql:8.4", 3306, new Dictionary<string, string> { ["MYSQL_ROOT_PASSWORD"] = "NodaTime!Passw0rd", ["MYSQL_DATABASE"] = "nodatime" }) { }
        protected override string SchemaFileName => "001_CreateNodaTimeRoundTrip.sql";
        public override DbConnection OpenConnection() => new MySqlConnection($"Server={Host};Port={MappedPort};User ID=root;Password=NodaTime!Passw0rd;Database=nodatime");
        protected override void ConfigureHandlers() => MySqlDapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb);
    }
    [CollectionDefinition("mysql")] public sealed class MySqlCollection : ICollectionFixture<MySqlFixture> { }
    [Collection("mysql")] public sealed class MySqlRoundTripTests : NodaTimeRoundTripTests { private readonly MySqlFixture _fixture; public MySqlRoundTripTests(MySqlFixture fixture) => _fixture = fixture; protected override DbConnection OpenConnection() => _fixture.OpenConnection(); protected override OffsetDateTime NormalizeOffsetDateTime(OffsetDateTime value) => value.ToInstant().WithOffset(Offset.Zero); }
}
