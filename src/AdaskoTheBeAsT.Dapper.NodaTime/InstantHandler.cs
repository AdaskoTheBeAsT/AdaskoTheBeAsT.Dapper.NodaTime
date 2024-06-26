using System;
using System.Data;
using Dapper;
using NodaTime;

namespace AdaskoTheBeAsT.Dapper.NodaTime
{
    public sealed class InstantHandler
        : SqlMapper.TypeHandler<Instant>
    {
        public static readonly InstantHandler Default = new();

        private InstantHandler()
        {
        }

        public override void SetValue(IDbDataParameter parameter, Instant value)
        {
            parameter.Value = value.ToDateTimeUtc();
            parameter.SetSqlDbType(SqlDbType.DateTime2);
        }

        public override Instant Parse(object value)
        {
            if (value is Instant instant)
            {
                return instant;
            }

            if (value is DateTime dateTime)
            {
                var dt = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                return Instant.FromDateTimeUtc(dt);
            }

            if (value is DateTimeOffset dateTimeOffset)
            {
                return Instant.FromDateTimeOffset(dateTimeOffset);
            }

            throw new DataException($"Cannot convert {value.GetType()} to NodaTime.Instant");
        }
    }
}
