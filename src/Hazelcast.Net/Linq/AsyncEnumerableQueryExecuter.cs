using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hazelcast.Linq
{
    internal class AsyncEnumerableQueryExecuter<T>
    {
        private readonly Expression _expression;
        private Func<CancellationToken, ValueTask<T>>? _func;

        /// <summary>
        /// Creates a new execution helper instance for the specified expression tree representing a computation over asynchronous enumerable sequences.
        /// </summary>
        /// <param name="expression">Expression tree representing a computation over asynchronous enumerable sequences.</param>
        public AsyncEnumerableQueryExecuter(Expression expression)
        {
            _expression = expression;
        }

        /// <summary>
        /// Evaluated the expression tree.
        /// </summary>
        /// <param name="token">Token to cancel the evaluation.</param>
        /// <returns>Task representing the evaluation of the expression tree.</returns>
        internal ValueTask<T> ExecuteAsync<TKey,TValue>(CancellationToken token)
        {
            if (_func == null)
            {
                var expression = Expression.Lambda<Func<CancellationToken, ValueTask<T>>>(new ObjectReaderWriter<TKey,TValue>()., Expression.Parameter(typeof(CancellationToken)));
                _func = expression.Compile();
            }

            return _func(token);
        }
    }
}
