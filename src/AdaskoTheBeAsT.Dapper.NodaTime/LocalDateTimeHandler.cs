using System;
using System.Data;
using Dapper;
using NodaTime;

namespace AdaskoTheBeAsT.Dapper.NodaTime
{
    public sealed class LocalDateTimeHandler : SqlMapper.TypeHandler<LocalDateTime>
    {
        private readonly INodaTimeTypeHandlerConfiguration _configuration;

        public LocalDateTimeHandler(INodaTimeTypeHandlerConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override void SetValue(IDbDataParameter parameter, LocalDateTime value) => _configuration.SetLocalDateTime(parameter, value);

        public override LocalDateTime Parse(object value)
        {
            NodaTimeValueParser.ThrowIfNull(value, "LocalDateTime");
            if (value is LocalDateTime localDateTime)
            {
                return localDateTime;
            }

            if (value is DateTime dateTime)
            {
                return LocalDateTime.FromDateTime(dateTime);
            }

            if (value is string text)
            {
                return LocalDateTime.FromDateTime(NodaTimeValueParser.ParseDateTime(text, "LocalDateTime"));
            }

            throw new DataException($"Cannot convert {value.GetType()} to NodaTime.LocalDateTime");
        }
    }
}
