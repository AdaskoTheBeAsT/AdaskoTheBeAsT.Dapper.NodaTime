using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.Sqlite.IntegrationTest
{
    [CollectionDefinition("sqlite")]
    public sealed class SqliteCollection
        : ICollectionFixture<SqliteFixture>;
}
