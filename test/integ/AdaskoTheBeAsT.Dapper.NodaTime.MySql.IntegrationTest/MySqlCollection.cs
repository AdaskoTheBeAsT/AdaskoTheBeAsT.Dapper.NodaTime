using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.MySql.IntegrationTest
{
    [CollectionDefinition("mysql")]
    public sealed class MySqlCollection
        : ICollectionFixture<MySqlFixture>;
}
