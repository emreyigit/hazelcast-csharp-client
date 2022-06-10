using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hazelcast.Linq
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Executes LINQ as SQL async.
        /// </summary>
        /// <typeparam name="T">Type of the object queried.</typeparam>
        /// <param name="query">IQuerable object.</param>
        /// <returns><see cref="Sql.ISqlQueryResult"/></returns>
        /// <exception cref="ArgumentNullException">In case of IQueryable<T> object is null.</exception>
        /// <exception cref="InvalidOperationException">In case of IQueryable<T> object is not a Hazelcast implementation.</exception>
        public static Task<Sql.ISqlQueryResult> GetAsync<T>(this IQueryable<T> query)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));

            if (query.Provider is QueryProvider provider)
            {
                return provider.SqlService.ExecuteQueryAsync(provider.GetQueryText());
            }
            else
                throw new InvalidOperationException("GetAsync() is only for Hazelcast IHMap.");
        }
    }
}
