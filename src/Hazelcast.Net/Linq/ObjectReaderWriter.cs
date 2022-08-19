using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Hazelcast.Core;
using Hazelcast.Sql;

namespace Hazelcast.Linq
{
    internal class ObjectReaderWriter<TKey, TValue> : IAsyncEnumerator<KeyValuePair<TKey, TValue>>, IAsyncDisposable
    {

        private readonly QueryProvider _queryProvider;
        private IAsyncEnumerator<SqlRow> _enumerator;
        private ISqlQueryResult _queryResult;
        private Expression _expression;

        public ObjectReaderWriter(QueryProvider queryProvider, Expression expression)
        {
            _queryProvider = queryProvider;
            _expression = expression;
        }

        public KeyValuePair<TKey, TValue> Current { get; private set; }



        public ValueTask DisposeAsync()
        {
            if (_queryResult != null)
                return _queryResult.DisposeAsync();

            return default;
        }

        public async ValueTask<bool> MoveNextAsync()
        {
            if (_enumerator == null)
            {
                // execute sql
                _queryResult = await _queryProvider.ExecuteQuery(_expression).CfAwait();
                _enumerator = _queryResult.GetAsyncEnumerator();
            }

            if (await _enumerator.MoveNextAsync().CfAwait())
            {
                var row = _enumerator.Current;
                var key = row.GetKey<TKey>();
                var val = row.GetValue<TValue>();
                Current = new KeyValuePair<TKey, TValue>(key, val);
                return true;
            }

            return false;
        }

    }
}
