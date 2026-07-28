using NodaTime;

namespace AdaskoTheBeAsT.Dapper.NodaTime.SqlServer
{
    public static class SqlServerDapperNodaTimeSetup
    {
        public static void Register(IDateTimeZoneProvider provider) => DapperNodaTimeSetup.Register(provider, new SqlServerNodaTimeConfiguration());
    }
}
