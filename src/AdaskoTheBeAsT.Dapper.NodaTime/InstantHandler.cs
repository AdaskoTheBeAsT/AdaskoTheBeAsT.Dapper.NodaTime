using System;
using System.Data;
using Dapper;
using NodaTime;

namespace AdaskoTheBeAsT.Dapper.NodaTime
{
    public sealed class InstantHandler : SqlMapper.TypeHandler<Instant>
    {
        private readonly INodaTimeTypeHandlerConfiguration _configuration;

        public InstantHandler(INodaTimeTypeHandlerConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override void SetValue(IDbDataParameter parameter, Instant value) => _configuration.SetInstant(parameter, value);

        public override Instant Parse(object value)
        {
            NodaTimeValueParser.ThrowIfNull(value, "Instant");
            if (value is Instant instant)
            {
                return instant;
            }

            if (value is DateTime dateTime)
            {
                return Instant.FromDateTimeUtc(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc));
            }

            if (value is DateTimeOffset dateTimeOffset)
            {
                return Instant.FromDateTimeOffset(dateTimeOffset);
            }

            if (value is string text)
            {
                if (text.EndsWith("Z", StringComparison.OrdinalIgnoreCase) || text.LastIndexOf('+') > 10 || text.LastIndexOf('-') > 10)
                {
                    return Instant.FromDateTimeOffset(NodaTimeValueParser.ParseDateTimeOffset(text, "Instant"));
                }

                return Instant.FromDateTimeUtc(DateTime.SpecifyKind(NodaTimeValueParser.ParseDateTime(text, "Instant"), DateTimeKind.Utc));
            }

            throw new DataException($"Cannot convert {value.GetType()} to NodaTime.Instant");
        }
    }
}
