// Copyright (c) 2008-2022, Hazelcast, Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
        /// <exception cref="InvalidOperationException">In case of IQueryable<T> object is not a Hazelcast.Net implementation.</exception>
        public static Task<Sql.ISqlQueryResult> GetAsync<T>(this IQueryable<T> query)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));

            if (query.Provider is QueryProvider provider)
            {
                return provider.SqlService.ExecuteQueryAsync(provider.GetQueryText(query.Expression));
            }
            else
                throw new InvalidOperationException("GetAsync() can only be used with Hazelcast IHMap.");
        }
    }
}
