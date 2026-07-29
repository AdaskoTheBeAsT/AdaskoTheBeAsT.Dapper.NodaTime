using System;
using System.Data.Common;
using System.Threading.Tasks;
using AdaskoTheBeAsT.Dapper.NodaTime.IntegrationTests.Common;
using Dapper;
using Microsoft.Data.Sqlite;
using NodaTime;
using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.Sqlite.IntegrationTest
{
    public sealed class SqliteFixture
        : IAsyncLifetime, IDisposable
    {
        private readonly string _connectionString;

        private readonly DbConnection _schemaOwner;

        public SqliteFixture()
        {
            _connectionString = $"Data Source=dapper-nodatime-{Guid.NewGuid():N};Mode=Memory;Cache=Shared";
            _schemaOwner = new SqliteConnection(_connectionString);
        }

        public DbConnection OpenConnection() => new SqliteConnection(_connectionString);

        public void Dispose() => _schemaOwner.Dispose();

#if NET8_0_OR_GREATER
        public async ValueTask InitializeAsync()
        {
            SqliteDapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb);
            var sql = await SchemaResource.ReadAsync(typeof(SqliteFixture).Assembly, TestCancellation.Token);
            await _schemaOwner.OpenAsync(TestCancellation.Token);
            await _schemaOwner.ExecuteAsync(sql);
        }

        public ValueTask DisposeAsync() => _schemaOwner.DisposeAsync();
#endif

#if NET462_OR_GREATER
        public async Task InitializeAsync()
        {
            SqliteDapperNodaTimeSetup.Register(DateTimeZoneProviders.Tzdb);
            var sql = await SchemaResource.ReadAsync(typeof(SqliteFixture).Assembly, TestCancellation.Token);
            await _schemaOwner.OpenAsync(TestCancellation.Token);
            await _schemaOwner.ExecuteAsync(sql);
        }

        public Task DisposeAsync()
        {
            _schemaOwner.Dispose();
            return Task.CompletedTask;
        }
#endif
    }
}
