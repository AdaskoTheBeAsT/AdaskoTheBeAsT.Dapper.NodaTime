using System;
using System.Threading.Tasks;
using LocalDb;
using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.SqlServer.Test
{
    public sealed class DatabaseFixture
        : IAsyncLifetime,
            IDisposable
    {
#pragma warning disable IDISP006 // Implement IDisposable
        private SqlInstance? _sqlInstance;
        private SqlDatabase? _database;
#pragma warning restore IDISP006 // Implement IDisposable

        public string ConnectionString => _database?.ConnectionString ?? string.Empty;

#if NET8_0_OR_GREATER
        public async ValueTask InitializeAsync()
#else
        public async Task InitializeAsync()
#endif
        {
#if !NET462 && !NET47 && !NET471 && !NET472
            _sqlInstance?.Dispose();
#endif
            _sqlInstance = new SqlInstance(name: GetDatabaseName(), buildTemplate: _ => Task.CompletedTask);

#if NET8_0_OR_GREATER
            if (_database is not null)
            {
                await _database.DisposeAsync().ConfigureAwait(false);
            }
#else
            _database?.Dispose();
#endif
            _database = await _sqlInstance.Build("DapperNodaTimeTests").ConfigureAwait(false);
        }

#if NET8_0_OR_GREATER
        public async ValueTask DisposeAsync()
#else
        public Task DisposeAsync()
#endif
        {
#if NET8_0_OR_GREATER
            if (_database is not null)
            {
                await _database.DisposeAsync().ConfigureAwait(false);
            }
#else
            _database?.Dispose();
#endif
            _database = null;
            _sqlInstance?.Cleanup();
#if !NET462 && !NET47 && !NET471 && !NET472
            _sqlInstance?.Dispose();
#endif
            _sqlInstance = null;
#if NET8_0_OR_GREATER

#else
            return Task.CompletedTask;
#endif
        }

        public void Dispose()
        {
            _database?.Dispose();
#if !NET462 && !NET47 && !NET471 && !NET472
            _sqlInstance?.Dispose();
#endif
        }

        private static string GetDatabaseName()
        {
#if NET462
            return "DapperNodaTimeTests_NET462";
#elif NET47
            return "DapperNodaTimeTests_NET47";
#elif NET471
            return "DapperNodaTimeTests_NET471";
#elif NET472
            return "DapperNodaTimeTests_NET472";
#elif NET48
            return "DapperNodaTimeTests_NET48";
#elif NET481
            return "DapperNodaTimeTests_NET481";
#elif NET8_0
            return "DapperNodaTimeTests_NET8";
#elif NET9_0
            return "DapperNodaTimeTests_NET9";
#elif NET10_0
            return "DapperNodaTimeTests_NET10";
#else
            // Fallback or unknown TFM
            return "DapperNodaTimeTests_Unknown";
#endif
        }
    }
}
