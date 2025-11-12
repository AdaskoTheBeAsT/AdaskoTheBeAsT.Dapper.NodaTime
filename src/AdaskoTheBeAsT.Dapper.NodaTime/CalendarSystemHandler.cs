using System;
using System.Data;
using Dapper;
using NodaTime;

namespace AdaskoTheBeAsT.Dapper.NodaTime
{
    public sealed class CalendarSystemHandler
        : SqlMapper.TypeHandler<CalendarSystem>
    {
        public static readonly CalendarSystemHandler Default = new();

        private CalendarSystemHandler()
        {
        }

        public override void SetValue(IDbDataParameter parameter, CalendarSystem? value)
        {
            parameter.Value = value == null ? DBNull.Value : value.Id;
            parameter.SetSqlDbType(SqlDbType.VarChar);
        }

        public override CalendarSystem Parse(object value)
        {
            if (value is null || value is DBNull)
            {
                throw new DataException("Cannot convert null/DBNull to CalendarSystem");
            }

            if (value is CalendarSystem calendarSystem)
            {
                return calendarSystem;
            }

            if (value is string id)
            {
                return CalendarSystem.ForId(id);
            }

            throw new DataException($"Cannot convert {value.GetType()} to NodaTime.CalendarSystem");
        }
    }
}
