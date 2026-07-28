using System;
using System.Data;
using NodaTime;
using NodaTime.Text;

namespace AdaskoTheBeAsT.Dapper.NodaTime
{
    /// <summary>Portable <see cref="DbType"/> based defaults for Noda Time handlers.</summary>
    public abstract class NodaTimeTypeHandlerConfigurationBase : INodaTimeTypeHandlerConfiguration
    {
        public virtual void SetInstant(IDbDataParameter parameter, Instant value)
        {
            parameter.Value = value.ToDateTimeUtc();
            parameter.DbType = DbType.DateTime2;
        }

        public virtual void SetLocalDate(IDbDataParameter parameter, LocalDate value)
        {
            parameter.Value = value.AtMidnight().ToDateTimeUnspecified();
            parameter.DbType = DbType.Date;
        }

        public virtual void SetLocalDateTime(IDbDataParameter parameter, LocalDateTime value)
        {
            parameter.Value = value.ToDateTimeUnspecified();
            parameter.DbType = DbType.DateTime2;
        }

        public virtual void SetLocalTime(IDbDataParameter parameter, LocalTime value)
        {
            parameter.Value = TimeSpan.FromTicks(value.TickOfDay);
            parameter.DbType = DbType.Time;
        }

        public virtual void SetOffsetDateTime(IDbDataParameter parameter, OffsetDateTime value)
        {
            parameter.Value = value.ToDateTimeOffset();
            parameter.DbType = DbType.DateTimeOffset;
        }

        public virtual void SetOffset(IDbDataParameter parameter, Offset value)
        {
            parameter.Value = value.Seconds;
            parameter.DbType = DbType.Int32;
        }

        public virtual void SetDuration(IDbDataParameter parameter, Duration value)
        {
            parameter.Value = value.ToInt64Nanoseconds();
            parameter.DbType = DbType.Int64;
        }

        public virtual void SetPeriod(IDbDataParameter parameter, Period? value)
        {
            parameter.Value = value is null ? DBNull.Value : PeriodPattern.Roundtrip.Format(value);
            parameter.DbType = DbType.AnsiString;
        }

        public virtual void SetCalendarSystem(IDbDataParameter parameter, CalendarSystem? value)
        {
            parameter.Value = value is null ? DBNull.Value : value.Id;
            parameter.DbType = DbType.AnsiString;
        }

        public virtual void SetDateTimeZone(IDbDataParameter parameter, DateTimeZone? value)
        {
            parameter.Value = value is null ? DBNull.Value : value.Id;
            parameter.DbType = DbType.AnsiString;
        }

        /// <summary>Applies a provider-specific type when the supplied parameter belongs to that provider.</summary>
        /// <param name="parameter">The parameter to configure.</param>
        /// <param name="set">The native configuration action.</param>
        protected static void TrySetNative<TParameter>(IDbDataParameter parameter, Action<TParameter> set)
            where TParameter : class
        {
            if (set is null)
            {
                throw new ArgumentNullException(nameof(set));
            }

            if (parameter is TParameter nativeParameter)
            {
#pragma warning disable CC0031 // Null checked above; analyzer does not track generic delegate parameters.
                set(nativeParameter);
#pragma warning restore CC0031
            }
        }
    }
}
