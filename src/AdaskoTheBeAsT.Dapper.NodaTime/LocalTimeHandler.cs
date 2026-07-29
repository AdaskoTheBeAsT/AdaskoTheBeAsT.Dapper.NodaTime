using System;
using System.Data;
using Dapper;
using NodaTime;

namespace AdaskoTheBeAsT.Dapper.NodaTime
{
    public sealed class LocalTimeHandler : SqlMapper.TypeHandler<LocalTime>
    {
        private readonly INodaTimeTypeHandlerConfiguration _configuration;

        public LocalTimeHandler(INodaTimeTypeHandlerConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override void SetValue(IDbDataParameter parameter, LocalTime value) => _configuration.SetLocalTime(parameter, value);

        public override LocalTime Parse(object value)
        {
            NodaTimeValueParser.ThrowIfNull(value, "LocalTime");
            if (value is LocalTime localTime)
            {
                return localTime;
            }

            if (value is TimeSpan timeSpan)
            {
                return LocalTime.FromTicksSinceMidnight(timeSpan.Ticks);
            }

            if (value is DateTime dateTime)
            {
                return LocalTime.FromTicksSinceMidnight(dateTime.TimeOfDay.Ticks);
            }

#if NET8_0_OR_GREATER
            if (value is TimeOnly timeOnly)
            {
                return LocalTime.FromTimeOnly(timeOnly);
            }
#endif

            if (value is string text)
            {
                return LocalTime.FromTicksSinceMidnight(NodaTimeValueParser.ParseTimeSpan(text, "LocalTime").Ticks);
            }

            throw new DataException($"Cannot convert {value.GetType()} to NodaTime.LocalTime");
        }
    }
}
