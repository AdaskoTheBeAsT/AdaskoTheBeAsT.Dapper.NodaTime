using System;
using System.Data;
using Dapper;
using NodaTime;

namespace AdaskoTheBeAsT.Dapper.NodaTime
{
    public sealed class OffsetHandler : SqlMapper.TypeHandler<Offset>
    {
        private readonly INodaTimeTypeHandlerConfiguration _configuration;

        public OffsetHandler(INodaTimeTypeHandlerConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override void SetValue(IDbDataParameter parameter, Offset value) => _configuration.SetOffset(parameter, value);

        public override Offset Parse(object value)
        {
            NodaTimeValueParser.ThrowIfNull(value, "Offset");
            if (value is Offset offset)
            {
                return offset;
            }

            if (value is int seconds)
            {
                return Offset.FromSeconds(seconds);
            }

            if (value is long longSeconds)
            {
                return Offset.FromSeconds(checked((int)longSeconds));
            }

            if (value is decimal decimalSeconds)
            {
                return Offset.FromSeconds(decimal.ToInt32(decimalSeconds));
            }

            throw new DataException($"Cannot convert {value.GetType()} to NodaTime.Offset");
        }
    }
}
