using Dapper;
using NodaTime;

namespace AdaskoTheBeAsT.Dapper.NodaTime
{
    public static class DapperNodaTimeSetup
    {
        /// <summary>
        /// Convenience method to register all type handlers for Noda Time.
        /// </summary>
        /// <param name="provider">The date time zone provider to use for <see cref="DateTimeZone"/>.</param>
        public static void Register(IDateTimeZoneProvider provider)
        {
            if (provider is null)
            {
                throw new System.ArgumentNullException(nameof(provider));
            }

            SqlMapper.AddTypeHandler(InstantHandler.Default);
            SqlMapper.AddTypeHandler(LocalDateHandler.Default);
            SqlMapper.AddTypeHandler(LocalDateTimeHandler.Default);
            SqlMapper.AddTypeHandler(LocalTimeHandler.Default);
            SqlMapper.AddTypeHandler(OffsetDateTimeHandler.Default);
            SqlMapper.AddTypeHandler(DurationHandler.Default);
            SqlMapper.AddTypeHandler(OffsetHandler.Default);
            SqlMapper.AddTypeHandler(CalendarSystemHandler.Default);
            SqlMapper.AddTypeHandler(DateTimeZoneHandler.Default(provider));
            SqlMapper.AddTypeHandler(PeriodHandler.Default);
        }
    }
}
