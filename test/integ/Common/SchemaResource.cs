using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AdaskoTheBeAsT.Dapper.NodaTime.IntegrationTests.Common
{
    internal static class SchemaResource
    {
        private const string ResourceName = "db.001_CreateNodaTimeRoundTrip.sql";

        internal static async Task<string> ReadAsync(Assembly assembly, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var stream = assembly.GetManifestResourceStream(ResourceName)
                ?? throw new InvalidOperationException($"Embedded schema resource '{ResourceName}' was not found in '{assembly.FullName}'.");
            using var reader = new StreamReader(stream);
#if NET8_0_OR_GREATER
            return await reader.ReadToEndAsync(cancellationToken);
#else
            return await reader.ReadToEndAsync();
#endif
        }
    }
}
