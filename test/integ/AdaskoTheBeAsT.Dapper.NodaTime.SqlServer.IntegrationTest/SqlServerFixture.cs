using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using AdaskoTheBeAsT.Dapper.NodaTime.IntegrationTests.Common;
using Microsoft.Data.SqlClient;
using NodaTime;

namespace AdaskoTheBeAsT.Dapper.NodaTime.SqlServer.IntegrationTest
{
    public sealed class SqlServerFixture
        : DockerDatabaseFixture
    {
        public SqlServerFixture()
            : base(
                "mcr.microsoft.com/mssql/server:2025-latest",
                1433,
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["ACCEPT_EULA"] = "Y",
                    ["MSSQL_SA_PASSWORD"] = "NodaTime!Passw0rd",
                })
        {
        }

        public override DbConnection OpenConnection()
            => new SqlConnection($"Server={Host},{MappedPort.ToString(CultureInfo.InvariantCulture)};User ID=sa;Password=NodaTime!Passw0rd;TrustServerCertificate=True;Encrypt=False");

        protected override void ConfigureHandlers() => SqlServerDapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb);
    }
}
