using System;
using System.Data;
using Dapper;
using NodaTime;

namespace AdaskoTheBeAsT.Dapper.NodaTime
{
    public sealed class DateTimeZoneHandler
        : SqlMapper.TypeHandler<DateTimeZone>
    {
        private readonly IDateTimeZoneProvider _provider;

        private DateTimeZoneHandler(IDateTimeZoneProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public static DateTimeZoneHandler Default(IDateTimeZoneProvider provider) => new(provider);

        public override void SetValue(IDbDataParameter parameter, DateTimeZone? value)
        {
            parameter.Value = value == null ? DBNull.Value : value.Id;
            parameter.SetSqlDbType(SqlDbType.VarChar);
        }

        public override DateTimeZone Parse(object value)
        {
            if (value is null || value is DBNull)
            {
                throw new DataException("Cannot convert null/DBNull to DateTimeZone");
            }

            if (value is DateTimeZone dateTimeZone)
            {
                return dateTimeZone;
            }

            if (value is string id)
            {
                return _provider[id];
            }

            throw new DataException($"Cannot convert {value.GetType()} to NodaTime.DateTimeZone");
        }
    }
}
