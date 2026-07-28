using NodaTime;

namespace AdaskoTheBeAsT.Dapper.NodaTime.Sqlite
{
    public static class SqliteDapperNodaTimeSetup
    {
        public static void Register(IDateTimeZoneProvider provider)
        {
            DapperNodaTimeSetup.Register(provider, new SqliteNodaTimeConfiguration());
        }
    }
}
