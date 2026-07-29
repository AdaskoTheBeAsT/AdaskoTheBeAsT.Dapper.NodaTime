using System;
using System.Data;
using Dapper;
using NodaTime;

namespace AdaskoTheBeAsT.Dapper.NodaTime
{
    public sealed class DateTimeZoneHandler : SqlMapper.TypeHandler<DateTimeZone>
    {
        private readonly IDateTimeZoneProvider _provider;
        private readonly INodaTimeTypeHandlerConfiguration _configuration;

        public DateTimeZoneHandler(IDateTimeZoneProvider provider, INodaTimeTypeHandlerConfiguration configuration)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override void SetValue(IDbDataParameter parameter, DateTimeZone? value) => _configuration.SetDateTimeZone(parameter, value);

        public override DateTimeZone Parse(object value)
        {
            NodaTimeValueParser.ThrowIfNull(value, "DateTimeZone");
            if (value is DateTimeZone dateTimeZone)
            {
                return dateTimeZone;
            }

            if (value is string id)
            {
                return _provider[id];
            }

            throw new DataException($"Cannot convert {value.GetType()} to NodaTime.DateTimeZone");
        }
    }
}
