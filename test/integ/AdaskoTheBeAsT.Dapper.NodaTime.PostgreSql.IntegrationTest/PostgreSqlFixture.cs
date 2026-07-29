using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using AdaskoTheBeAsT.Dapper.NodaTime.IntegrationTests.Common;
using NodaTime;
using Npgsql;

namespace AdaskoTheBeAsT.Dapper.NodaTime.PostgreSql.IntegrationTest
{
    public sealed class PostgreSqlFixture
        : DockerDatabaseFixture
    {
        public PostgreSqlFixture()
            : base(
                "postgres:18",
                5432,
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["POSTGRES_PASSWORD"] = "NodaTime!Passw0rd",
                })
        {
        }

        public override DbConnection OpenConnection()
            => new NpgsqlConnection($"Host={Host};Port={MappedPort.ToString(CultureInfo.InvariantCulture)};Username=postgres;Password=NodaTime!Passw0rd;Database=postgres");

        protected override void ConfigureHandlers() => PostgreSqlDapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb);
    }
}
