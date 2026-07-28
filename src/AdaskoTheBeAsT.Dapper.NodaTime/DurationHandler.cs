using System;
using System.Data;
using Dapper;
using NodaTime;

namespace AdaskoTheBeAsT.Dapper.NodaTime
{
    public sealed class DurationHandler : SqlMapper.TypeHandler<Duration>
    {
        private readonly INodaTimeTypeHandlerConfiguration _configuration;

        public DurationHandler(INodaTimeTypeHandlerConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override void SetValue(IDbDataParameter parameter, Duration value) => _configuration.SetDuration(parameter, value);

        public override Duration Parse(object value)
        {
            NodaTimeValueParser.ThrowIfNull(value, "Duration");
            if (value is Duration duration)
            {
                return duration;
            }

            if (value is long nanoseconds)
            {
                return Duration.FromNanoseconds(nanoseconds);
            }

            if (value is int intNanoseconds)
            {
                return Duration.FromNanoseconds(intNanoseconds);
            }

            if (value is decimal decimalNanoseconds)
            {
                return Duration.FromNanoseconds(decimal.ToInt64(decimalNanoseconds));
            }

            throw new DataException($"Cannot convert {value.GetType()} to NodaTime.Duration");
        }
    }
}
