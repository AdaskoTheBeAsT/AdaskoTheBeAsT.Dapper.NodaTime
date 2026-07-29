using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using AdaskoTheBeAsT.Dapper.NodaTime.IntegrationTests.Common;
using NodaTime;
using Oracle.ManagedDataAccess.Client;

namespace AdaskoTheBeAsT.Dapper.NodaTime.Oracle.IntegrationTest
{
    public sealed class OracleFixture
        : DockerDatabaseFixture
    {
        public OracleFixture()
            : base(
                "gvenzl/oracle-free:23-slim",
                1521,
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["ORACLE_PASSWORD"] = "NodaTime!Passw0rd",
                })
        {
        }

        public override DbConnection OpenConnection()
            => new OracleConnection($"User Id=system;Password=NodaTime!Passw0rd;Data Source={Host}:{MappedPort.ToString(CultureInfo.InvariantCulture)}/FREEPDB1");

        protected override void ConfigureHandlers() => OracleDapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb);
    }
}
