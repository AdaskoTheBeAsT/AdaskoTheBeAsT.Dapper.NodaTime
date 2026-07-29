using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.SqlServer.IntegrationTest
{
    [CollectionDefinition("sqlserver")]
    public sealed class SqlServerCollection
        : ICollectionFixture<SqlServerFixture>;
}
