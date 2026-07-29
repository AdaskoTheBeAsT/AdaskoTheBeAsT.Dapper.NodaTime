using System.Data;
using NodaTime;
using Npgsql;
using NpgsqlTypes;

namespace AdaskoTheBeAsT.Dapper.NodaTime.PostgreSql
{
    public sealed class PostgreSqlNodaTimeConfiguration : NodaTimeTypeHandlerConfigurationBase
    {
        public override void SetInstant(IDbDataParameter parameter, Instant value)
        {
            base.SetInstant(parameter, value);
            TrySetNative<NpgsqlParameter>(parameter, p => p.NpgsqlDbType = NpgsqlDbType.TimestampTz);
        }

        public override void SetLocalDate(IDbDataParameter parameter, LocalDate value)
        {
            base.SetLocalDate(parameter, value);
            TrySetNative<NpgsqlParameter>(parameter, p => p.NpgsqlDbType = NpgsqlDbType.Date);
        }

        public override void SetLocalDateTime(IDbDataParameter parameter, LocalDateTime value)
        {
            base.SetLocalDateTime(parameter, value);
            TrySetNative<NpgsqlParameter>(parameter, p => p.NpgsqlDbType = NpgsqlDbType.Timestamp);
        }

        public override void SetLocalTime(IDbDataParameter parameter, LocalTime value)
        {
            base.SetLocalTime(parameter, value);
            TrySetNative<NpgsqlParameter>(parameter, p => p.NpgsqlDbType = NpgsqlDbType.Time);
        }

        public override void SetOffsetDateTime(IDbDataParameter parameter, OffsetDateTime value)
        {
            parameter.Value = value.ToDateTimeOffset().ToUniversalTime();
            parameter.DbType = DbType.DateTimeOffset;
            TrySetNative<NpgsqlParameter>(parameter, p => p.NpgsqlDbType = NpgsqlDbType.TimestampTz);
        }

        public override void SetOffset(IDbDataParameter parameter, Offset value)
        {
            base.SetOffset(parameter, value);
            TrySetNative<NpgsqlParameter>(parameter, p => p.NpgsqlDbType = NpgsqlDbType.Integer);
        }

        public override void SetDuration(IDbDataParameter parameter, Duration value)
        {
            base.SetDuration(parameter, value);
            TrySetNative<NpgsqlParameter>(parameter, p => p.NpgsqlDbType = NpgsqlDbType.Bigint);
        }

        public override void SetPeriod(IDbDataParameter parameter, Period? value)
        {
            base.SetPeriod(parameter, value);
            TrySetNative<NpgsqlParameter>(parameter, p => p.NpgsqlDbType = NpgsqlDbType.Varchar);
        }

        public override void SetCalendarSystem(IDbDataParameter parameter, CalendarSystem? value)
        {
            base.SetCalendarSystem(parameter, value);
            TrySetNative<NpgsqlParameter>(parameter, p => p.NpgsqlDbType = NpgsqlDbType.Varchar);
        }

        public override void SetDateTimeZone(IDbDataParameter parameter, DateTimeZone? value)
        {
            base.SetDateTimeZone(parameter, value);
            TrySetNative<NpgsqlParameter>(parameter, p => p.NpgsqlDbType = NpgsqlDbType.Varchar);
        }
    }
}
