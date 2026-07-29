using NodaTime;

namespace AdaskoTheBeAsT.Dapper.NodaTime.Oracle
{
    public static class OracleDapperNodaTimeSetup
    {
        public static void Register(IDateTimeZoneProvider provider)
        {
            DapperNodaTimeSetup.Register(provider, new OracleNodaTimeConfiguration());
        }
    }
}
