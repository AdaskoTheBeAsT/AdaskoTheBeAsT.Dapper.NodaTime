using System.Data;
using MySqlConnector;
using NodaTime;

namespace AdaskoTheBeAsT.Dapper.NodaTime.MySql
{
    /// <summary>MySqlConnector configuration. OffsetDateTime is normalized to UTC because MySQL has no offset column type.</summary>
    public sealed class MySqlNodaTimeConfiguration : NodaTimeTypeHandlerConfigurationBase
    {
        public override void SetInstant(IDbDataParameter parameter, Instant value)
        {
            base.SetInstant(parameter, value);
            TrySetNative<MySqlParameter>(parameter, p => p.MySqlDbType = MySqlDbType.DateTime);
        }

        public override void SetLocalDate(IDbDataParameter parameter, LocalDate value)
        {
            base.SetLocalDate(parameter, value);
            TrySetNative<MySqlParameter>(parameter, p => p.MySqlDbType = MySqlDbType.Date);
        }

        public override void SetLocalDateTime(IDbDataParameter parameter, LocalDateTime value)
        {
            base.SetLocalDateTime(parameter, value);
            TrySetNative<MySqlParameter>(parameter, p => p.MySqlDbType = MySqlDbType.DateTime);
        }

        public override void SetLocalTime(IDbDataParameter parameter, LocalTime value)
        {
            base.SetLocalTime(parameter, value);
            TrySetNative<MySqlParameter>(parameter, p => p.MySqlDbType = MySqlDbType.Time);
        }

        public override void SetOffsetDateTime(IDbDataParameter parameter, OffsetDateTime value)
        {
            parameter.Value = value.ToDateTimeOffset().UtcDateTime;
            parameter.DbType = DbType.DateTime2;
            TrySetNative<MySqlParameter>(parameter, p => p.MySqlDbType = MySqlDbType.DateTime);
        }

        public override void SetOffset(IDbDataParameter parameter, Offset value)
        {
            base.SetOffset(parameter, value);
            TrySetNative<MySqlParameter>(parameter, p => p.MySqlDbType = MySqlDbType.Int32);
        }

        public override void SetDuration(IDbDataParameter parameter, Duration value)
        {
            base.SetDuration(parameter, value);
            TrySetNative<MySqlParameter>(parameter, p => p.MySqlDbType = MySqlDbType.Int64);
        }

        public override void SetPeriod(IDbDataParameter parameter, Period? value)
        {
            base.SetPeriod(parameter, value);
            TrySetNative<MySqlParameter>(parameter, p => p.MySqlDbType = MySqlDbType.VarChar);
        }

        public override void SetCalendarSystem(IDbDataParameter parameter, CalendarSystem? value)
        {
            base.SetCalendarSystem(parameter, value);
            TrySetNative<MySqlParameter>(parameter, p => p.MySqlDbType = MySqlDbType.VarChar);
        }

        public override void SetDateTimeZone(IDbDataParameter parameter, DateTimeZone? value)
        {
            base.SetDateTimeZone(parameter, value);
            TrySetNative<MySqlParameter>(parameter, p => p.MySqlDbType = MySqlDbType.VarChar);
        }
    }
}
