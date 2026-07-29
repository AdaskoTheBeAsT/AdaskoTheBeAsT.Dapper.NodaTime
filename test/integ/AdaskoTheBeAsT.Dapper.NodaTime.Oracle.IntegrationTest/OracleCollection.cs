using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.Oracle.IntegrationTest
{
    [CollectionDefinition("oracle")]
    public sealed class OracleCollection
        : ICollectionFixture<OracleFixture>;
}
