using System;
using System.Data;
using Dapper;
using NodaTime;

namespace AdaskoTheBeAsT.Dapper.NodaTime
{
    public sealed class LocalDateHandler : SqlMapper.TypeHandler<LocalDate>
    {
        private readonly INodaTimeTypeHandlerConfiguration _configuration;

        public LocalDateHandler(INodaTimeTypeHandlerConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override void SetValue(IDbDataParameter parameter, LocalDate value) => _configuration.SetLocalDate(parameter, value);

        public override LocalDate Parse(object value)
        {
            NodaTimeValueParser.ThrowIfNull(value, "LocalDate");
            if (value is LocalDate localDate)
            {
                return localDate;
            }

            if (value is DateTime dateTime)
            {
                return LocalDateTime.FromDateTime(dateTime).Date;
            }

#if NET8_0_OR_GREATER
            if (value is DateOnly dateOnly)
            {
                return LocalDate.FromDateOnly(dateOnly);
            }
#endif

            if (value is string text)
            {
                return LocalDateTime.FromDateTime(NodaTimeValueParser.ParseDateTime(text, "LocalDate")).Date;
            }

            throw new DataException($"Cannot convert {value.GetType()} to NodaTime.LocalDate");
        }
    }
}
