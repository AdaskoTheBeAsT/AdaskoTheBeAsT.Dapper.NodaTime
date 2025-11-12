using System;
using System.Data;
using Dapper;
using NodaTime;
using NodaTime.Text;

namespace AdaskoTheBeAsT.Dapper.NodaTime
{
    public sealed class PeriodHandler
        : SqlMapper.TypeHandler<Period>
    {
        public static readonly PeriodHandler Default = new();

        private PeriodHandler()
        {
        }

        public override void SetValue(IDbDataParameter parameter, Period? value)
        {
            parameter.Value = value == null ? DBNull.Value : PeriodPattern.Roundtrip.Format(value);
            parameter.SetSqlDbType(SqlDbType.VarChar);
        }

        public override Period Parse(object value)
        {
            if (value is null || value is DBNull)
            {
                throw new DataException("Cannot convert null/DBNull to Period");
            }

            if (value is Period period)
            {
                return period;
            }

            if (value is string str)
            {
                return PeriodPattern.Roundtrip.Parse(str).GetValueOrThrow();
            }

            throw new DataException($"Cannot convert {value.GetType()} to NodaTime.Period");
        }
    }
}
