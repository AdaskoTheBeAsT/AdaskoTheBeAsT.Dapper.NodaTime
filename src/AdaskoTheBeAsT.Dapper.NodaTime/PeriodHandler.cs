using System;
using System.Data;
using Dapper;
using NodaTime;
using NodaTime.Text;

namespace AdaskoTheBeAsT.Dapper.NodaTime
{
    public sealed class PeriodHandler : SqlMapper.TypeHandler<Period>
    {
        private readonly INodaTimeTypeHandlerConfiguration _configuration;

        public PeriodHandler(INodaTimeTypeHandlerConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override void SetValue(IDbDataParameter parameter, Period? value) => _configuration.SetPeriod(parameter, value);

        public override Period Parse(object value)
        {
            NodaTimeValueParser.ThrowIfNull(value, "Period");
            if (value is Period period)
            {
                return period;
            }

            if (value is string text)
            {
                return PeriodPattern.Roundtrip.Parse(text).GetValueOrThrow();
            }

            throw new DataException($"Cannot convert {value.GetType()} to NodaTime.Period");
        }
    }
}
