using System;
using System.Data;
using System.Globalization;
using NodaTime;

namespace AdaskoTheBeAsT.Dapper.NodaTime
{
    internal static class NodaTimeValueParser
    {
        internal static void ThrowIfNull(object value, string targetType)
        {
            if (value is null || value is DBNull)
            {
                throw new DataException($"Cannot convert null/DBNull to {targetType}");
            }
        }

        internal static DateTime ParseDateTime(string value, string targetType)
        {
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dateTime))
            {
                return dateTime;
            }

            throw new DataException($"Cannot convert '{value}' to {targetType}");
        }

        internal static DateTimeOffset ParseDateTimeOffset(string value, string targetType)
        {
            if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dateTimeOffset))
            {
                return dateTimeOffset;
            }

            throw new DataException($"Cannot convert '{value}' to {targetType}");
        }

        internal static TimeSpan ParseTimeSpan(string value, string targetType)
        {
            if (TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out var timeSpan))
            {
                return timeSpan;
            }

            return ParseDateTime(value, targetType).TimeOfDay;
        }
    }
}
