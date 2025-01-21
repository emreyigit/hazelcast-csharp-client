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

// <auto-generated>
//   This code was generated by a tool.
//   Hazelcast Client Protocol Code Generator @068f5eb
//   https://github.com/hazelcast/hazelcast-client-protocol
//   Change to this file will be lost if the code is regenerated.
// </auto-generated>

#pragma warning disable IDE0051 // Remove unused private members
// ReSharper disable UnusedMember.Local
// ReSharper disable RedundantUsingDirective
// ReSharper disable CheckNamespace

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Hazelcast.Protocol.BuiltInCodecs;
using Hazelcast.Protocol.CustomCodecs;
using Hazelcast.Core;
using Hazelcast.Messaging;
using Hazelcast.Clustering;
using Hazelcast.Serialization;
using Microsoft.Extensions.Logging;

namespace Hazelcast.Protocol.Codecs
{
    /// <summary>
    /// Adds a CP Group view listener to a connection.
    ///</summary>
#if SERVER_CODEC
    internal static class ClientAddCPGroupViewListenerServerCodec
#else
    internal static class ClientAddCPGroupViewListenerCodec
#endif
    {
        public const int RequestMessageType = 5888; // 0x001700
        public const int ResponseMessageType = 5889; // 0x001701
        private const int RequestInitialFrameSize = Messaging.FrameFields.Offset.PartitionId + BytesExtensions.SizeOfInt;
        private const int ResponseInitialFrameSize = Messaging.FrameFields.Offset.ResponseBackupAcks + BytesExtensions.SizeOfByte;
        private const int EventGroupsViewVersionFieldOffset = Messaging.FrameFields.Offset.PartitionId + BytesExtensions.SizeOfInt;
        private const int EventGroupsViewInitialFrameSize = EventGroupsViewVersionFieldOffset + BytesExtensions.SizeOfLong;
        private const int EventGroupsViewMessageType = 5890; // 0x001702

#if SERVER_CODEC
        public sealed class RequestParameters
        {
        }
#endif

        public static ClientMessage EncodeRequest()
        {
            var clientMessage = new ClientMessage
            {
                IsRetryable = false,
                OperationName = "Client.AddCPGroupViewListener"
            };
            var initialFrame = new Frame(new byte[RequestInitialFrameSize], (FrameFlags) ClientMessageFlags.Unfragmented);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.MessageType, RequestMessageType);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.PartitionId, -1);
            clientMessage.Append(initialFrame);
            return clientMessage;
        }

#if SERVER_CODEC
        public static RequestParameters DecodeRequest(ClientMessage clientMessage)
        {
            using var iterator = clientMessage.GetEnumerator();
            var request = new RequestParameters();
            iterator.Take(); // empty initial frame
            return request;
        }
#endif

        public sealed class ResponseParameters
        {
        }

#if SERVER_CODEC
        public static ClientMessage EncodeResponse()
        {
            var clientMessage = new ClientMessage();
            var initialFrame = new Frame(new byte[ResponseInitialFrameSize], (FrameFlags) ClientMessageFlags.Unfragmented);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.MessageType, ResponseMessageType);
            clientMessage.Append(initialFrame);
            return clientMessage;
        }
#endif

        public static ResponseParameters DecodeResponse(ClientMessage clientMessage)
        {
            using var iterator = clientMessage.GetEnumerator();
            var response = new ResponseParameters();
            iterator.Take(); // empty initial frame
            return response;
        }

#if SERVER_CODEC
        public static ClientMessage EncodeGroupsViewEvent(long version, ICollection<Hazelcast.CP.CPGroupInfo> groupsInfo, ICollection<KeyValuePair<Guid, Guid>> cpToApUuids)
        {
            var clientMessage = new ClientMessage();
            var initialFrame = new Frame(new byte[EventGroupsViewInitialFrameSize], (FrameFlags) ClientMessageFlags.Unfragmented);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.MessageType, EventGroupsViewMessageType);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.PartitionId, -1);
            initialFrame.Bytes.WriteLongL(EventGroupsViewVersionFieldOffset, version);
            clientMessage.Append(initialFrame);
            clientMessage.Flags |= ClientMessageFlags.Event;
            ListMultiFrameCodec.Encode(clientMessage, groupsInfo, RaftGroupInfoCodec.Encode);
            EntryListUUIDUUIDCodec.Encode(clientMessage, cpToApUuids);
            return clientMessage;
        }
#endif
        public static ValueTask HandleEventAsync(ClientMessage clientMessage, Func<long, ICollection<Hazelcast.CP.CPGroupInfo>, IList<KeyValuePair<Guid, Guid>>, object, ValueTask> handleGroupsViewEventAsync, object state, ILoggerFactory loggerFactory)
        {
            using var iterator = clientMessage.GetEnumerator();
            var messageType = clientMessage.MessageType;
            if (messageType == EventGroupsViewMessageType)
            {
                var initialFrame = iterator.Take();
                var version =  initialFrame.Bytes.ReadLongL(EventGroupsViewVersionFieldOffset);
                var groupsInfo = ListMultiFrameCodec.Decode(iterator, RaftGroupInfoCodec.Decode);
                var cpToApUuids = EntryListUUIDUUIDCodec.Decode(iterator);
                return handleGroupsViewEventAsync(version, groupsInfo, cpToApUuids, state);
            }
            loggerFactory.CreateLogger(typeof(EventHandler)).LogDebug("Unknown message type received on event handler :" + messageType);
            return default;
        }
    }
}
