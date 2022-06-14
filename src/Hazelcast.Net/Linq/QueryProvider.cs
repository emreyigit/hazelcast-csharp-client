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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Hazelcast.Sql;

namespace Hazelcast.Linq
{
    internal class QueryProvider : IQueryProvider
    {
        internal readonly ISqlService SqlService;
        private readonly string _mapName;
        private readonly QueryTranslator _translator;

        public QueryProvider(ISqlService sqlService, string mapName)
        {
            SqlService = sqlService;
            _translator = new QueryTranslator(mapName);
            _mapName = mapName;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(MapQuery<>).MakeGenericType(expression.Type), new object[] { this, expression });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new MapQuery<TElement>(this, expression);
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException("Synchron operations on map are not supported. Please, invoke GetAsync() extension.");
        }

        public string GetQueryText(Expression expression)
        {
            var sql = _translator.Translate(ExpressionEvaluator.EvaluatePartially(expression));
            Console.WriteLine("QUERY: " + sql);//todo:refactor.            
            return sql;
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)Execute(expression);
        }
    }
}
