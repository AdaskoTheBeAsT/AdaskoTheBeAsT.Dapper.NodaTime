using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;
using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.SqlServer.Test
{
    public sealed class DatabaseFixture
        : IAsyncLifetime
    {
#if NET462
        private const string DatabaseName = "DapperNodaTimeTests_NET462";
#elif NET47
        private const string DatabaseName = "DapperNodaTimeTests_NET47";
#elif NET471
        private const string DatabaseName = "DapperNodaTimeTests_NET471";
#elif NET472
        private const string DatabaseName = "DapperNodaTimeTests_NET472";
#elif NET48
        private const string DatabaseName = "DapperNodaTimeTests_NET48";
#elif NET481
        private const string DatabaseName = "DapperNodaTimeTests_NET481";
#elif NET8_0
        private const string DatabaseName = "DapperNodaTimeTests_NET8";
#elif NET9_0
        private const string DatabaseName = "DapperNodaTimeTests_NET9";
#elif NET10_0
        private const string DatabaseName = "DapperNodaTimeTests_NET10";
#else
        private const string DatabaseName = "DapperNodaTimeTests_Unknown";
#endif

        private const string CreateDatabaseSql =
            $"IF DB_ID('{DatabaseName}') IS NULL CREATE DATABASE [{DatabaseName}];";

        private readonly MsSqlContainer _msSqlContainer;

        public DatabaseFixture()
        {
            _msSqlContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-latest")
                .WithEnvironment("ACCEPT_EULA", "Y")
                .WithExposedPort(1433)
                .WithWaitStrategy(
                    Wait.ForUnixContainer()
                        .UntilCommandIsCompleted(
                            "/opt/mssql-tools18/bin/sqlcmd",
                            "-C",
                            "-Q",
                            "SELECT 1;"))
                .WithPassword("NodaTime!Passw0rd")
                .Build();
        }

        public string ConnectionString { get; private set; } = string.Empty;

#if NET8_0_OR_GREATER
        public async ValueTask InitializeAsync()
#else
        public async Task InitializeAsync()
#endif
        {
#if NET8_0_OR_GREATER
            await _msSqlContainer.StartAsync(TestContext.Current.CancellationToken).ConfigureAwait(false);
#else
            await _msSqlContainer.StartAsync().ConfigureAwait(false);
#endif
            await CreateDatabaseAsync().ConfigureAwait(false);

            ConnectionString = BuildConnectionString(DatabaseName);
        }

#if NET8_0_OR_GREATER
        public ValueTask DisposeAsync() => _msSqlContainer.DisposeAsync();
#else
        public Task DisposeAsync() => _msSqlContainer.DisposeAsync().AsTask();
#endif

        private async Task CreateDatabaseAsync()
        {
#if NET8_0_OR_GREATER
            await using var connection = new SqlConnection(_msSqlContainer.GetConnectionString());
            await connection.OpenAsync(TestContext.Current.CancellationToken).ConfigureAwait(false);
            await using var command = connection.CreateCommand();
            command.CommandText = CreateDatabaseSql;
            await command.ExecuteNonQueryAsync(TestContext.Current.CancellationToken).ConfigureAwait(false);
#else
            using var connection = new SqlConnection(_msSqlContainer.GetConnectionString());
            await connection.OpenAsync().ConfigureAwait(false);
            using var command = connection.CreateCommand();
            command.CommandText = CreateDatabaseSql;
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
#endif
        }

        private string BuildConnectionString(string databaseName)
        {
            var builder = new SqlConnectionStringBuilder(_msSqlContainer.GetConnectionString())
            {
                InitialCatalog = databaseName,
            };

            return builder.ConnectionString;
        }
    }
}
