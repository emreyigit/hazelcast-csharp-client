﻿// Copyright (c) 2008-2025, Hazelcast, Inc. All Rights Reserved.
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
using System.Collections.Generic;
using System.Threading.Tasks;
using Hazelcast.Core;
using Hazelcast.Protocol.Codecs;

namespace Hazelcast.DistributedObjects.Impl
{
    internal partial class HQueue<T> // Removing
    {
        /// <inheritdoc />
        public override async Task<bool> RemoveAsync(T item)
        {
            var itemData = ToSafeData(item);
            var requestMessage = QueueRemoveCodec.EncodeRequest(Name, itemData);
            var responseMessage = await Cluster.Messaging.SendToPartitionOwnerAsync(requestMessage, PartitionId).CfAwait();
            return QueueRemoveCodec.DecodeResponse(responseMessage).Response;
        }

        /// <inheritdoc />
        public override async Task<bool> RemoveAllAsync<TItem>(ICollection<TItem> items)
        {
            var itemsData = ToSafeData(items);
            var requestMessage = QueueCompareAndRemoveAllCodec.EncodeRequest(Name, itemsData);
            var responseMessage = await Cluster.Messaging.SendToPartitionOwnerAsync(requestMessage, PartitionId).CfAwait();
            return QueueCompareAndRemoveAllCodec.DecodeResponse(responseMessage).Response;
        }

        /// <inheritdoc />
        public override async Task<bool> RetainAllAsync<TItem>(ICollection<TItem> items)
        {
            var itemsData = ToSafeData(items);
            var requestMessage = QueueCompareAndRetainAllCodec.EncodeRequest(Name, itemsData);
            var responseMessage = await Cluster.Messaging.SendToPartitionOwnerAsync(requestMessage, PartitionId).CfAwait();
            return QueueCompareAndRetainAllCodec.DecodeResponse(responseMessage).Response;
        }

        /// <inheritdoc />
        public override
#if !HZ_OPTIMIZE_ASYNC
            async
#endif
            Task ClearAsync()
        {
            var requestMessage = QueueClearCodec.EncodeRequest(Name);
            var task = Cluster.Messaging.SendToPartitionOwnerAsync(requestMessage, PartitionId);

#if HZ_OPTIMIZE_ASYNC
            return task;
#else
            await task.CfAwait();
#endif
        }
    }
}
