using System.Data;
using NodaTime;

namespace AdaskoTheBeAsT.Dapper.NodaTime
{
    /// <summary>Configures the database representation of Noda Time values.</summary>
    public interface INodaTimeTypeHandlerConfiguration
    {
        void SetInstant(IDbDataParameter parameter, Instant value);

        void SetLocalDate(IDbDataParameter parameter, LocalDate value);

        void SetLocalDateTime(IDbDataParameter parameter, LocalDateTime value);

        void SetLocalTime(IDbDataParameter parameter, LocalTime value);

        void SetOffsetDateTime(IDbDataParameter parameter, OffsetDateTime value);

        void SetOffset(IDbDataParameter parameter, Offset value);

        void SetDuration(IDbDataParameter parameter, Duration value);

        void SetPeriod(IDbDataParameter parameter, Period? value);

        void SetCalendarSystem(IDbDataParameter parameter, CalendarSystem? value);

        void SetDateTimeZone(IDbDataParameter parameter, DateTimeZone? value);
    }
}
