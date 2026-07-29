using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.IntegrationTests.Common
{
    public abstract class DockerDatabaseFixture
        : IAsyncLifetime
    {
        private readonly IContainer _container;

        protected DockerDatabaseFixture(string image, int port, IReadOnlyDictionary<string, string> environment)
        {
            var builder = new ContainerBuilder(image).WithPortBinding(port, assignRandomHostPort: true);
            foreach (var item in environment)
            {
                builder = builder.WithEnvironment(item.Key, item.Value);
            }

            _container = builder.Build();
            Port = port;
        }

        protected int Port { get; }

        protected string Host => _container.Hostname;

        protected int MappedPort => _container.GetMappedPublicPort(Port);

#if NET8_0_OR_GREATER
        public async ValueTask InitializeAsync()
        {
            await _container.StartAsync(TestCancellation.Token);
            ConfigureHandlers();
            await WaitForDatabaseAsync();
            await ApplySchemaAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();
            GC.SuppressFinalize(this);
        }
#endif

#if NET462_OR_GREATER
        public async Task InitializeAsync()
        {
            await _container.StartAsync(TestCancellation.Token);
            ConfigureHandlers();
            await WaitForDatabaseAsync();
            await ApplySchemaAsync();
        }

        public async Task DisposeAsync()
        {
            await DisposeAsyncCore();
        }
#endif

        public abstract DbConnection OpenConnection();

        protected abstract void ConfigureHandlers();

        protected virtual ValueTask DisposeAsyncCore() => _container.DisposeAsync();

        private async Task WaitForDatabaseAsync()
        {
            Exception? lastException = null;
            for (var attempt = 0; attempt < 60; attempt++)
            {
                try
                {
#if NET8_0_OR_GREATER
                    await using var connection = OpenConnection();
#endif
#if NET462_OR_GREATER
                    using var connection = OpenConnection();
#endif
                    await connection.OpenAsync(TestCancellation.Token);
                    return;
                }
                catch (Exception exception)
                {
                    lastException = exception;
                    await Task.Delay(TimeSpan.FromSeconds(2), TestCancellation.Token);
                }
            }

            throw new InvalidOperationException("The database container did not become available.", lastException);
        }

        private async Task ApplySchemaAsync()
        {
            var sql = await SchemaResource.ReadAsync(GetType().Assembly, TestCancellation.Token);
#if NET8_0_OR_GREATER
            await using var connection = OpenConnection();
#endif
#if NET462_OR_GREATER
            using var connection = OpenConnection();
#endif
            await connection.OpenAsync(TestCancellation.Token);
            await connection.ExecuteAsync(sql);
        }
    }
}
