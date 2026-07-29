using System.Data;
using NodaTime;

namespace AdaskoTheBeAsT.Dapper.NodaTime.SqlServer
{
    /// <summary>SQL Server parameter configuration supporting both SqlClient implementations.</summary>
    public sealed class SqlServerNodaTimeConfiguration : NodaTimeTypeHandlerConfigurationBase
    {
        public override void SetInstant(IDbDataParameter parameter, Instant value)
        {
            base.SetInstant(parameter, value);
            parameter.TrySetSqlDbType(SqlDbType.DateTime2);
        }

        public override void SetLocalDate(IDbDataParameter parameter, LocalDate value)
        {
            base.SetLocalDate(parameter, value);
            parameter.TrySetSqlDbType(SqlDbType.Date);
        }

        public override void SetLocalDateTime(IDbDataParameter parameter, LocalDateTime value)
        {
            base.SetLocalDateTime(parameter, value);
            parameter.TrySetSqlDbType(SqlDbType.DateTime2);
        }

        public override void SetLocalTime(IDbDataParameter parameter, LocalTime value)
        {
            base.SetLocalTime(parameter, value);
            parameter.TrySetSqlDbType(SqlDbType.Time);
        }

        public override void SetOffsetDateTime(IDbDataParameter parameter, OffsetDateTime value)
        {
            base.SetOffsetDateTime(parameter, value);
            parameter.TrySetSqlDbType(SqlDbType.DateTimeOffset);
        }

        public override void SetOffset(IDbDataParameter parameter, Offset value)
        {
            base.SetOffset(parameter, value);
            parameter.TrySetSqlDbType(SqlDbType.Int);
        }

        public override void SetDuration(IDbDataParameter parameter, Duration value)
        {
            base.SetDuration(parameter, value);
            parameter.TrySetSqlDbType(SqlDbType.BigInt);
        }

        public override void SetPeriod(IDbDataParameter parameter, Period? value)
        {
            base.SetPeriod(parameter, value);
            parameter.TrySetSqlDbType(SqlDbType.VarChar);
        }

        public override void SetCalendarSystem(IDbDataParameter parameter, CalendarSystem? value)
        {
            base.SetCalendarSystem(parameter, value);
            parameter.TrySetSqlDbType(SqlDbType.VarChar);
        }

        public override void SetDateTimeZone(IDbDataParameter parameter, DateTimeZone? value)
        {
            base.SetDateTimeZone(parameter, value);
            parameter.TrySetSqlDbType(SqlDbType.VarChar);
        }
    }
}
