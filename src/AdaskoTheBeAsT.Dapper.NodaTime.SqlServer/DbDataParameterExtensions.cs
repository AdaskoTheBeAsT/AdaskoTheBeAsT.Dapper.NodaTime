using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
#if NET9_0_OR_GREATER
using System.Threading;
#endif

namespace AdaskoTheBeAsT.Dapper.NodaTime.SqlServer
{
    internal static class DbDataParameterExtensions
    {
#if NET9_0_OR_GREATER
        private static readonly Lock SyncRoot = new();
#else
        private static readonly object SyncRoot = new();
#endif
        private static readonly Dictionary<Type, Action<IDbDataParameter, SqlDbType>?> Setters = new();

        internal static void TrySetSqlDbType(this IDbDataParameter parameter, SqlDbType sqlDbType)
        {
            Action<IDbDataParameter, SqlDbType>? setter;
            lock (SyncRoot)
            {
                if (!Setters.TryGetValue(parameter.GetType(), out setter))
                {
                    setter = CompileSetter(parameter.GetType());
                    Setters.Add(parameter.GetType(), setter);
                }
            }

            setter?.Invoke(parameter, sqlDbType);
        }

        private static Action<IDbDataParameter, SqlDbType>? CompileSetter(Type parameterType)
        {
            var property = parameterType.GetProperty("SqlDbType", BindingFlags.Instance | BindingFlags.Public);
            if (property?.CanWrite != true || property.PropertyType != typeof(SqlDbType))
            {
                return null;
            }

            var parameter = Expression.Parameter(typeof(IDbDataParameter), "parameter");
            var dbType = Expression.Parameter(typeof(SqlDbType), "dbType");
            var assign = Expression.Assign(Expression.Property(Expression.Convert(parameter, parameterType), property), dbType);
            return Expression.Lambda<Action<IDbDataParameter, SqlDbType>>(assign, parameter, dbType).Compile();
        }
    }
}
