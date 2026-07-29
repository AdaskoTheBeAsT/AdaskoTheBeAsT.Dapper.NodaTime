using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.PostgreSql.IntegrationTest
{
    [CollectionDefinition("postgresql")]
    public sealed class PostgreSqlCollection
        : ICollectionFixture<PostgreSqlFixture>;
}
