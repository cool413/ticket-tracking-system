using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Services.DataCommon.Extensions
{
    public static class CollectionExtensions
    {
        public static TResult NoLocking<T, TResult>(this IQueryable<T> query,
         Func<IQueryable<T>, TResult> queryAction)
        {
            CustomDbCommandInterceptor.EnableNolock.Value = true;
            TResult result = queryAction(query);
            CustomDbCommandInterceptor.EnableNolock.Value = false;
            return result;
        }

        public static TResult Recompiling<T, TResult>(this IQueryable<T> query,
            Func<IQueryable<T>, TResult> queryAction)
        {
            CustomDbCommandInterceptor.EnableRecompile.Value = true;
            TResult result = queryAction(query);
            CustomDbCommandInterceptor.EnableRecompile.Value = false;
            return result;
        }

        public static IEnumerable<List<T>> Partition<T>(this IList<T> source, int batchSize)
        {
            for (var i = 0; i < Math.Ceiling(source.Count / (double)batchSize); i++)
            {
                yield return new List<T>(source.Skip(batchSize * i).Take(batchSize));
            }
        }

        private const string RelationCommandCache = "_relationalCommandCache";
        private const string SelectExpression = "_selectExpression";
        private const string QuerySqlGeneratorFactory = "_querySqlGeneratorFactory";
        private const string RelationalQueryContext = "_relationalQueryContext";

        public static string ToSql<T>(this IQueryable<T> query)
        {
            var enumerator = query.Provider.Execute<IEnumerable<T>>(query.Expression).GetEnumerator();
            var relationalCommandCache = enumerator.GetPrivateValue(RelationCommandCache);

            var selectExpression = relationalCommandCache.GetPrivateValue<SelectExpression>(SelectExpression);

            var factory = relationalCommandCache.GetPrivateValue<IQuerySqlGeneratorFactory>(QuerySqlGeneratorFactory);

            var sqlGenerator = factory.Create();
            var command = sqlGenerator.GetCommand(selectExpression);

            return command.CommandText;
        }

        public static (string, IReadOnlyDictionary<string, object>) ToSqlWithParams<T>(this IQueryable<T> query)
        {
            var enumerator = query.Provider.Execute<IEnumerable<T>>(query.Expression).GetEnumerator();

            var relationalCommandCache = enumerator.GetPrivateValue(RelationCommandCache);
            var selectExpression = relationalCommandCache.GetPrivateValue<SelectExpression>(SelectExpression);

            var factory = relationalCommandCache.GetPrivateValue<IQuerySqlGeneratorFactory>(QuerySqlGeneratorFactory);

            var queryContext = enumerator.GetPrivateValue<RelationalQueryContext>(RelationalQueryContext);

            var sqlGenerator = factory.Create();
            var command = sqlGenerator.GetCommand(selectExpression);

            var parametersDict = queryContext.ParameterValues;
            var sql = command.CommandText;
            return (sql, parametersDict);
        }

        private static object GetPrivateValue(this object obj, string privateField)
            => obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);

        private static T GetPrivateValue<T>(this object obj, string privateField)
            => (T)obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);

        public static IQueryable<TQuery> In<TKey, TQuery>(
            this IQueryable<TQuery> query,
            IEnumerable<TKey> values,
            Expression<Func<TQuery, TKey>> keySelector)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            if (!values.Any())
            {
                return query.Take(0);
            }

            var distinctValues = Bucketize(values);

            if (distinctValues.Length > 2048)
            {
                throw new ArgumentException("Too many parameters for SQL Server, reduce the number of parameters", nameof(keySelector));
            }

            var predicates = distinctValues
                .Select(v =>
                {
                    // Create an expression that captures the variable so EF can turn this into a parameterized SQL query
                    Expression<Func<TKey>> valueAsExpression = () => v;
                    return Expression.Equal(keySelector.Body, valueAsExpression.Body);
                })
                .ToList();

            while (predicates.Count > 1)
            {
                predicates = PairWise(predicates).Select(p => Expression.OrElse(p.Item1, p.Item2)).ToList();
            }

            var body = predicates.Single();

            var clause = Expression.Lambda<Func<TQuery, bool>>(body, keySelector.Parameters);

            return query.Where(clause);
        }

        private static IEnumerable<(T, T)> PairWise<T>(this IEnumerable<T> source)
        {
            var sourceEnumerator = source.GetEnumerator();
            while (sourceEnumerator.MoveNext())
            {
                var a = sourceEnumerator.Current;
                sourceEnumerator.MoveNext();
                var b = sourceEnumerator.Current;

                yield return (a, b);
            }
        }

        private static TKey[] Bucketize<TKey>(IEnumerable<TKey> values)
        {
            var distinctValueList = values.Distinct().ToList();

            // Calculate bucket size as 1,2,4,8,16,32,64,...
            var bucket = 1;
            while (distinctValueList.Count > bucket)
            {
                bucket *= 2;
            }

            // Fill all slots.
            var lastValue = distinctValueList.Last();
            for (var index = distinctValueList.Count; index < bucket; index++)
            {
                distinctValueList.Add(lastValue);
            }

            return distinctValueList.ToArray();
        }

        public static DataTable UDTToDataTable<T>(this IEnumerable<T> items, string[] hideColumns = null) where T : class
        {
            var dataTable = new DataTable();
            var propertyDescriptorCollection = System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            for (var i = 0; i < propertyDescriptorCollection.Count; i++)
            {
                var propertyDescriptor = propertyDescriptorCollection[i];
                var type = propertyDescriptor.PropertyType;

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    type = Nullable.GetUnderlyingType(type);
                }

                var col = propertyDescriptor.Name;
                if (hideColumns != null && hideColumns.Any(h => h.Equals(col, StringComparison.InvariantCultureIgnoreCase)))
                {
                    continue;
                }
                dataTable.Columns.Add(col, type);
            }
            var values = new object[dataTable.Columns.Count];
            foreach (var dataItem in items)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    values[i] = propertyDescriptorCollection[dataTable.Columns[i].ColumnName].GetValue(dataItem);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
    }
}
