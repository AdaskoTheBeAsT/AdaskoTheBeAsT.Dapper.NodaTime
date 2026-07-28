using System;
using System.Data;
using Dapper;
using NodaTime;

namespace AdaskoTheBeAsT.Dapper.NodaTime
{
    public sealed class OffsetDateTimeHandler : SqlMapper.TypeHandler<OffsetDateTime>
    {
        private readonly INodaTimeTypeHandlerConfiguration _configuration;

        public OffsetDateTimeHandler(INodaTimeTypeHandlerConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override void SetValue(IDbDataParameter parameter, OffsetDateTime value) => _configuration.SetOffsetDateTime(parameter, value);

        public override OffsetDateTime Parse(object value)
        {
            NodaTimeValueParser.ThrowIfNull(value, "OffsetDateTime");
            if (value is OffsetDateTime offsetDateTime)
            {
                return offsetDateTime;
            }

            if (value is DateTimeOffset dateTimeOffset)
            {
                return OffsetDateTime.FromDateTimeOffset(dateTimeOffset);
            }

            if (value is DateTime dateTime)
            {
                return OffsetDateTime.FromDateTimeOffset(new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)));
            }

            if (value is string text)
            {
                return OffsetDateTime.FromDateTimeOffset(NodaTimeValueParser.ParseDateTimeOffset(text, "OffsetDateTime"));
            }

            throw new DataException($"Cannot convert {value.GetType()} to NodaTime.OffsetDateTime");
        }
    }
}
