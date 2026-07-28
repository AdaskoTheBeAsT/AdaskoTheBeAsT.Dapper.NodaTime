using System.Data;
using Microsoft.Data.Sqlite;
using NodaTime;

namespace AdaskoTheBeAsT.Dapper.NodaTime.Sqlite
{
    public sealed class SqliteNodaTimeConfiguration : NodaTimeTypeHandlerConfigurationBase
    {
        public override void SetInstant(IDbDataParameter parameter, Instant value)
        {
            base.SetInstant(parameter, value);
            TrySetNative<SqliteParameter>(parameter, p => p.SqliteType = SqliteType.Text);
        }

        public override void SetLocalDate(IDbDataParameter parameter, LocalDate value)
        {
            base.SetLocalDate(parameter, value);
            TrySetNative<SqliteParameter>(parameter, p => p.SqliteType = SqliteType.Text);
        }

        public override void SetLocalDateTime(IDbDataParameter parameter, LocalDateTime value)
        {
            base.SetLocalDateTime(parameter, value);
            TrySetNative<SqliteParameter>(parameter, p => p.SqliteType = SqliteType.Text);
        }

        public override void SetLocalTime(IDbDataParameter parameter, LocalTime value)
        {
            base.SetLocalTime(parameter, value);
            TrySetNative<SqliteParameter>(parameter, p => p.SqliteType = SqliteType.Text);
        }

        public override void SetOffsetDateTime(IDbDataParameter parameter, OffsetDateTime value)
        {
            base.SetOffsetDateTime(parameter, value);
            TrySetNative<SqliteParameter>(parameter, p => p.SqliteType = SqliteType.Text);
        }

        public override void SetOffset(IDbDataParameter parameter, Offset value)
        {
            base.SetOffset(parameter, value);
            TrySetNative<SqliteParameter>(parameter, p => p.SqliteType = SqliteType.Integer);
        }

        public override void SetDuration(IDbDataParameter parameter, Duration value)
        {
            base.SetDuration(parameter, value);
            TrySetNative<SqliteParameter>(parameter, p => p.SqliteType = SqliteType.Integer);
        }

        public override void SetPeriod(IDbDataParameter parameter, Period? value)
        {
            base.SetPeriod(parameter, value);
            TrySetNative<SqliteParameter>(parameter, p => p.SqliteType = SqliteType.Text);
        }

        public override void SetCalendarSystem(IDbDataParameter parameter, CalendarSystem? value)
        {
            base.SetCalendarSystem(parameter, value);
            TrySetNative<SqliteParameter>(parameter, p => p.SqliteType = SqliteType.Text);
        }

        public override void SetDateTimeZone(IDbDataParameter parameter, DateTimeZone? value)
        {
            base.SetDateTimeZone(parameter, value);
            TrySetNative<SqliteParameter>(parameter, p => p.SqliteType = SqliteType.Text);
        }
    }
}
