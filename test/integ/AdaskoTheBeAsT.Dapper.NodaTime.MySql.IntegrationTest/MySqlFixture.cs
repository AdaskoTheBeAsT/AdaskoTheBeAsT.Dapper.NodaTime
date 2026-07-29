using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using AdaskoTheBeAsT.Dapper.NodaTime.IntegrationTests.Common;
using MySqlConnector;
using NodaTime;

namespace AdaskoTheBeAsT.Dapper.NodaTime.MySql.IntegrationTest
{
    public sealed class MySqlFixture
        : DockerDatabaseFixture
    {
        public MySqlFixture()
            : base(
                "mysql:8.4",
                3306,
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["MYSQL_ROOT_PASSWORD"] = "NodaTime!Passw0rd",
                    ["MYSQL_DATABASE"] = "nodatime",
                })
        {
        }

        public override DbConnection OpenConnection()
            => new MySqlConnection($"Server={Host};Port={MappedPort.ToString(CultureInfo.InvariantCulture)};User ID=root;Password=NodaTime!Passw0rd;Database=nodatime");

        protected override void ConfigureHandlers() => MySqlDapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb);
    }
}
