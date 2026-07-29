using NodaTime;

namespace AdaskoTheBeAsT.Dapper.NodaTime.MySql
{
    public static class MySqlDapperNodaTimeSetup
    {
        public static void Register(IDateTimeZoneProvider provider)
        {
            DapperNodaTimeSetup.Register(provider, new MySqlNodaTimeConfiguration());
        }
    }
}
