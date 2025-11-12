using System;
using System.Data;
using Dapper;
using NodaTime;

namespace AdaskoTheBeAsT.Dapper.NodaTime
{
    public sealed class DurationHandler
        : SqlMapper.TypeHandler<Duration>
    {
        public static readonly DurationHandler Default = new();

        private DurationHandler()
        {
        }

        public override void SetValue(IDbDataParameter parameter, Duration value)
        {
            parameter.Value = value.ToInt64Nanoseconds();
            parameter.SetSqlDbType(SqlDbType.BigInt);
        }

        public override Duration Parse(object value)
        {
            if (value is null || value is DBNull)
            {
                throw new DataException("Cannot convert null/DBNull to Duration");
            }

            if (value is Duration duration)
            {
                return duration;
            }

            if (value is long l)
            {
                return Duration.FromNanoseconds(l);
            }

            throw new DataException($"Cannot convert {value.GetType()} to NodaTime.Duration");
        }
    }
}
