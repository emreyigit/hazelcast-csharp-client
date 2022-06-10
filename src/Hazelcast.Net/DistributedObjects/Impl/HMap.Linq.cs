// Copyright (c) 2008-2021, Hazelcast, Inc. All Rights Reserved.
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Hazelcast.Linq;

namespace Hazelcast.DistributedObjects.Impl
{
    internal partial class HMap<TKey, TValue> // LINQ
    {
        private MapQuery<KeyValuePair<TKey, TValue>> _mapQuery;

        public Type ElementType => typeof(KeyValuePair<TKey, TValue>);

        public Expression Expression => _mapQuery.Expression;

        public IQueryProvider Provider => _mapQuery.Provider;

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _mapQuery.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
