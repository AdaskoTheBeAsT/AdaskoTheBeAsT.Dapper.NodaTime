using NodaTime;

namespace AdaskoTheBeAsT.Dapper.NodaTime.PostgreSql
{
    public static class PostgreSqlDapperNodaTimeSetup
    {
        public static void Register(IDateTimeZoneProvider provider) => DapperNodaTimeSetup.Register(provider, new PostgreSqlNodaTimeConfiguration());
    }
}
