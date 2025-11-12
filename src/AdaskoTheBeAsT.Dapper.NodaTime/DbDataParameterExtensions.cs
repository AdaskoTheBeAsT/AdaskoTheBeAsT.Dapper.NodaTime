using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace AdaskoTheBeAsT.Dapper.NodaTime
{
    public static class DbDataParameterExtensions
    {
        private static readonly object SyncRoot = new();
        private static readonly Dictionary<Type, Action<IDbDataParameter, SqlDbType>> CompiledSetters = new();

        public static void SetSqlDbType(this IDbDataParameter parameter, SqlDbType sqlDbType)
        {
            var parameterType = parameter.GetType();

            Action<IDbDataParameter, SqlDbType> setter;
            lock (SyncRoot)
            {
                if (!CompiledSetters.TryGetValue(parameterType, out setter))
                {
                    setter = CompileSetterAction(parameterType);
                    CompiledSetters[parameterType] = setter;
                }
            }

            setter(parameter, sqlDbType);
        }

        private static Action<IDbDataParameter, SqlDbType> CompileSetterAction(Type parameterType)
        {
            var property = parameterType.GetProperty("SqlDbType", BindingFlags.Instance | BindingFlags.Public);
            if (property == null)
            {
                throw new InvalidOperationException($"Property 'SqlDbType' not found on type '{parameterType}'.");
            }

            if (property.PropertyType != typeof(SqlDbType))
            {
                throw new InvalidOperationException($"Property 'SqlDbType' on type '{parameterType}' is not of type '{typeof(SqlDbType)}'.");
            }

            if (!property.CanWrite)
            {
                throw new InvalidOperationException($"Property 'SqlDbType' on type '{parameterType}' is not writable.");
            }

            var dataParam = Expression.Parameter(typeof(IDbDataParameter), "dataParameter");
            var sqlDbTypeParam = Expression.Parameter(typeof(SqlDbType), "sqlDbType");
            var convertedVar = Expression.Variable(parameterType, "converted");
            var assignConvertedExpr = Expression.Assign(convertedVar, Expression.Convert(dataParam, parameterType));
            var getPropertyExpr = Expression.Property(convertedVar, property);
            var assignSqlDbTypeExpr = Expression.Assign(getPropertyExpr, sqlDbTypeParam);
#pragma warning disable S3878
            var blockExpr = Expression.Block(
                new[] { convertedVar },
                new Expression[] { assignConvertedExpr, assignSqlDbTypeExpr });
#pragma warning restore S3878
            return Expression.Lambda<Action<IDbDataParameter, SqlDbType>>(
                blockExpr,
                dataParam,
                sqlDbTypeParam)
                .Compile();
        }
    }
}
