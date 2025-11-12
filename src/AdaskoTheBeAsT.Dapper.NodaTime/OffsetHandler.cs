using System;
using System.Data;
using Dapper;
using NodaTime;

namespace AdaskoTheBeAsT.Dapper.NodaTime
{
    public sealed class OffsetHandler
        : SqlMapper.TypeHandler<Offset>
    {
        public static readonly OffsetHandler Default = new();

        private OffsetHandler()
        {
        }

        public override void SetValue(IDbDataParameter parameter, Offset value)
        {
            parameter.Value = value.Seconds;
            parameter.SetSqlDbType(SqlDbType.Int);
        }

        public override Offset Parse(object value)
        {
            if (value is null || value is DBNull)
            {
                throw new DataException("Cannot convert null/DBNull to Offset");
            }

            if (value is Offset offset)
            {
                return offset;
            }

            if (value is int seconds)
            {
                return Offset.FromSeconds(seconds);
            }

            throw new DataException($"Cannot convert {value.GetType()} to NodaTime.Offset");
        }
    }
}
