using System.Threading;
#if NET8_0_OR_GREATER
using Xunit;
#endif

namespace AdaskoTheBeAsT.Dapper.NodaTime.IntegrationTests.Common
{
    internal static class TestCancellation
    {
#if NET8_0_OR_GREATER
        internal static CancellationToken Token => TestContext.Current.CancellationToken;
#else
        internal static CancellationToken Token => CancellationToken.None;
#endif
    }
}
