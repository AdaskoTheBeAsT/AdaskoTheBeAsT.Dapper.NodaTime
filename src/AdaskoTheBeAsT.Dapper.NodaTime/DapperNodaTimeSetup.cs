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
        /// <param name="configuration">The selected database dialect configuration.</param>
        public static void Register(IDateTimeZoneProvider provider, INodaTimeTypeHandlerConfiguration configuration)
        {
            if (provider is null)
            {
                throw new System.ArgumentNullException(nameof(provider));
            }

            if (configuration is null)
            {
                throw new System.ArgumentNullException(nameof(configuration));
            }

            SqlMapper.AddTypeHandler(new InstantHandler(configuration));
            SqlMapper.AddTypeHandler(new LocalDateHandler(configuration));
            SqlMapper.AddTypeHandler(new LocalDateTimeHandler(configuration));
            SqlMapper.AddTypeHandler(new LocalTimeHandler(configuration));
            SqlMapper.AddTypeHandler(new OffsetDateTimeHandler(configuration));
            SqlMapper.AddTypeHandler(new DurationHandler(configuration));
            SqlMapper.AddTypeHandler(new OffsetHandler(configuration));
            SqlMapper.AddTypeHandler(new CalendarSystemHandler(configuration));
            SqlMapper.AddTypeHandler(new DateTimeZoneHandler(provider, configuration));
            SqlMapper.AddTypeHandler(new PeriodHandler(configuration));
        }
    }
}
