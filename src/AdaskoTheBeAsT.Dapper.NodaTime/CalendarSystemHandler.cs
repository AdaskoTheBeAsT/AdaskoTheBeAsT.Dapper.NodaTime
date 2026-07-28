using System;
using System.Data;
using Dapper;
using NodaTime;

namespace AdaskoTheBeAsT.Dapper.NodaTime
{
    public sealed class CalendarSystemHandler : SqlMapper.TypeHandler<CalendarSystem>
    {
        private readonly INodaTimeTypeHandlerConfiguration _configuration;

        public CalendarSystemHandler(INodaTimeTypeHandlerConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override void SetValue(IDbDataParameter parameter, CalendarSystem? value) => _configuration.SetCalendarSystem(parameter, value);

        public override CalendarSystem Parse(object value)
        {
            NodaTimeValueParser.ThrowIfNull(value, "CalendarSystem");
            if (value is CalendarSystem calendarSystem)
            {
                return calendarSystem;
            }

            if (value is string id)
            {
                return CalendarSystem.ForId(id);
            }

            throw new DataException($"Cannot convert {value.GetType()} to NodaTime.CalendarSystem");
        }
    }
}
