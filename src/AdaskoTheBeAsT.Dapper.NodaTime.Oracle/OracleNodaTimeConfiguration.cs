using System;
using System.Data;
using NodaTime;
using Oracle.ManagedDataAccess.Client;

namespace AdaskoTheBeAsT.Dapper.NodaTime.Oracle
{
    public sealed class OracleNodaTimeConfiguration : NodaTimeTypeHandlerConfigurationBase
    {
        public override void SetInstant(IDbDataParameter parameter, Instant value)
        {
            TrySetNative<OracleParameter>(parameter, p => p.OracleDbType = OracleDbType.TimeStamp);
            parameter.Value = value.ToDateTimeUtc();
        }

        public override void SetLocalDate(IDbDataParameter parameter, LocalDate value)
        {
            TrySetNative<OracleParameter>(parameter, p => p.OracleDbType = OracleDbType.Date);
            parameter.Value = value.AtMidnight().ToDateTimeUnspecified();
        }

        public override void SetLocalDateTime(IDbDataParameter parameter, LocalDateTime value)
        {
            TrySetNative<OracleParameter>(parameter, p => p.OracleDbType = OracleDbType.TimeStamp);
            parameter.Value = value.ToDateTimeUnspecified();
        }

        public override void SetLocalTime(IDbDataParameter parameter, LocalTime value)
        {
            TrySetNative<OracleParameter>(parameter, p => p.OracleDbType = OracleDbType.IntervalDS);
            parameter.Value = TimeSpan.FromTicks(value.TickOfDay);
        }

        public override void SetOffsetDateTime(IDbDataParameter parameter, OffsetDateTime value)
        {
            TrySetNative<OracleParameter>(parameter, p => p.OracleDbType = OracleDbType.TimeStampTZ);
            parameter.Value = value.ToDateTimeOffset();
        }

        public override void SetOffset(IDbDataParameter parameter, Offset value)
        {
            base.SetOffset(parameter, value);
            TrySetNative<OracleParameter>(parameter, p => p.OracleDbType = OracleDbType.Int32);
        }

        public override void SetDuration(IDbDataParameter parameter, Duration value)
        {
            base.SetDuration(parameter, value);
            TrySetNative<OracleParameter>(parameter, p => p.OracleDbType = OracleDbType.Int64);
        }

        public override void SetPeriod(IDbDataParameter parameter, Period? value)
        {
            base.SetPeriod(parameter, value);
            TrySetNative<OracleParameter>(parameter, p => p.OracleDbType = OracleDbType.Varchar2);
        }

        public override void SetCalendarSystem(IDbDataParameter parameter, CalendarSystem? value)
        {
            base.SetCalendarSystem(parameter, value);
            TrySetNative<OracleParameter>(parameter, p => p.OracleDbType = OracleDbType.Varchar2);
        }

        public override void SetDateTimeZone(IDbDataParameter parameter, DateTimeZone? value)
        {
            base.SetDateTimeZone(parameter, value);
            TrySetNative<OracleParameter>(parameter, p => p.OracleDbType = OracleDbType.Varchar2);
        }
    }
}
