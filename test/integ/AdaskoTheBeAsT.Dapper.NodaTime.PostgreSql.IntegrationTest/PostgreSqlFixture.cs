using System.Collections.Generic;
using System.Data.Common;
using AdaskoTheBeAsT.Dapper.NodaTime.IntegrationTests.Common;
using NodaTime;
using Npgsql;
using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.PostgreSql.IntegrationTest
{
    public sealed class PostgreSqlFixture : DockerDatabaseFixture
    {
        public PostgreSqlFixture() : base("postgres:18", 5432, new Dictionary<string, string> { ["POSTGRES_PASSWORD"] = "NodaTime!Passw0rd" }) { }
        protected override string SchemaFileName => "001_CreateNodaTimeRoundTrip.sql";
        public override DbConnection OpenConnection() => new NpgsqlConnection($"Host={Host};Port={MappedPort};Username=postgres;Password=NodaTime!Passw0rd;Database=postgres");
        protected override void ConfigureHandlers() => PostgreSqlDapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb);
    }
    [CollectionDefinition("postgresql")] public sealed class PostgreSqlCollection : ICollectionFixture<PostgreSqlFixture> { }
    [Collection("postgresql")] public sealed class PostgreSqlRoundTripTests : NodaTimeRoundTripTests { private readonly PostgreSqlFixture _fixture; public PostgreSqlRoundTripTests(PostgreSqlFixture fixture) => _fixture = fixture; protected override DbConnection OpenConnection() => _fixture.OpenConnection(); protected override OffsetDateTime NormalizeOffsetDateTime(OffsetDateTime value) => value.ToInstant().WithOffset(Offset.Zero); }
}
