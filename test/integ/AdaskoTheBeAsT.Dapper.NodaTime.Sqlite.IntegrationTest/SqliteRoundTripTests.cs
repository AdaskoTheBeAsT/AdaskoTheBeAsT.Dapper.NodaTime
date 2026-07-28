using System;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;
using AdaskoTheBeAsT.Dapper.NodaTime.IntegrationTests.Common;
using Dapper;
using Microsoft.Data.Sqlite;
using NodaTime;
using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.Sqlite.IntegrationTest
{
    public sealed class SqliteFixture : IAsyncLifetime
    {
        private readonly string _databasePath = Path.Combine(Path.GetTempPath(), $"dapper-nodatime-{Guid.NewGuid():N}.db");

        public DbConnection OpenConnection() => new SqliteConnection($"Data Source={_databasePath};Pooling=False");

#if NET8_0_OR_GREATER
        public async ValueTask InitializeAsync()
        {
            SqliteDapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb);
            var sql = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "db", "001_CreateNodaTimeRoundTrip.sql"));
            await using var connection = OpenConnection();
            await connection.OpenAsync();
            await connection.ExecuteAsync(sql);
        }

        public ValueTask DisposeAsync() { File.Delete(_databasePath); return ValueTask.CompletedTask; }
#endif
#if NET462_OR_GREATER
        public async Task InitializeAsync()
        {
            SqliteDapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb);
            var sql = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "db", "001_CreateNodaTimeRoundTrip.sql"));
            using var connection = OpenConnection();
            await connection.OpenAsync();
            await connection.ExecuteAsync(sql);
        }

        public Task DisposeAsync()
        {
            File.Delete(_databasePath);
            return Task.CompletedTask;
        }
#endif
    }

    [CollectionDefinition("sqlite")] public sealed class SqliteCollection : ICollectionFixture<SqliteFixture> { }

    [Collection("sqlite")] public sealed class SqliteRoundTripTests : NodaTimeRoundTripTests { private readonly SqliteFixture _fixture; public SqliteRoundTripTests(SqliteFixture fixture) => _fixture = fixture; protected override DbConnection OpenConnection() => _fixture.OpenConnection(); }
}
