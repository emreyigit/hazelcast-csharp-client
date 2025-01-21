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
    /// Subscribes to this topic. When someone publishes a message on this topic. onMessage() function of the given
    /// MessageListener is called. More than one message listener can be added on one instance.
    ///</summary>
#if SERVER_CODEC
    internal static class TopicAddMessageListenerServerCodec
#else
    internal static class TopicAddMessageListenerCodec
#endif
    {
        public const int RequestMessageType = 262656; // 0x040200
        public const int ResponseMessageType = 262657; // 0x040201
        private const int RequestLocalOnlyFieldOffset = Messaging.FrameFields.Offset.PartitionId + BytesExtensions.SizeOfInt;
        private const int RequestInitialFrameSize = RequestLocalOnlyFieldOffset + BytesExtensions.SizeOfBool;
        private const int ResponseResponseFieldOffset = Messaging.FrameFields.Offset.ResponseBackupAcks + BytesExtensions.SizeOfByte;
        private const int ResponseInitialFrameSize = ResponseResponseFieldOffset + BytesExtensions.SizeOfCodecGuid;
        private const int EventTopicPublishTimeFieldOffset = Messaging.FrameFields.Offset.PartitionId + BytesExtensions.SizeOfInt;
        private const int EventTopicUuidFieldOffset = EventTopicPublishTimeFieldOffset + BytesExtensions.SizeOfLong;
        private const int EventTopicInitialFrameSize = EventTopicUuidFieldOffset + BytesExtensions.SizeOfCodecGuid;
        private const int EventTopicMessageType = 262658; // 0x040202

#if SERVER_CODEC
        public sealed class RequestParameters
        {

            /// <summary>
            /// Name of the Topic
            ///</summary>
            public string Name { get; set; }

            /// <summary>
            /// if true listens only local events on registered member
            ///</summary>
            public bool LocalOnly { get; set; }
        }
#endif

        public static ClientMessage EncodeRequest(string name, bool localOnly)
        {
            var clientMessage = new ClientMessage
            {
                IsRetryable = false,
                OperationName = "Topic.AddMessageListener"
            };
            var initialFrame = new Frame(new byte[RequestInitialFrameSize], (FrameFlags) ClientMessageFlags.Unfragmented);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.MessageType, RequestMessageType);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.PartitionId, -1);
            initialFrame.Bytes.WriteBoolL(RequestLocalOnlyFieldOffset, localOnly);
            clientMessage.Append(initialFrame);
            StringCodec.Encode(clientMessage, name);
            return clientMessage;
        }

#if SERVER_CODEC
        public static RequestParameters DecodeRequest(ClientMessage clientMessage)
        {
            using var iterator = clientMessage.GetEnumerator();
            var request = new RequestParameters();
            var initialFrame = iterator.Take();
            request.LocalOnly = initialFrame.Bytes.ReadBoolL(RequestLocalOnlyFieldOffset);
            request.Name = StringCodec.Decode(iterator);
            return request;
        }
#endif

        public sealed class ResponseParameters
        {

            /// <summary>
            /// returns the registration id
            ///</summary>
            public Guid Response { get; set; }
        }

#if SERVER_CODEC
        public static ClientMessage EncodeResponse(Guid response)
        {
            var clientMessage = new ClientMessage();
            var initialFrame = new Frame(new byte[ResponseInitialFrameSize], (FrameFlags) ClientMessageFlags.Unfragmented);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.MessageType, ResponseMessageType);
            initialFrame.Bytes.WriteGuidL(ResponseResponseFieldOffset, response);
            clientMessage.Append(initialFrame);
            return clientMessage;
        }
#endif

        public static ResponseParameters DecodeResponse(ClientMessage clientMessage)
        {
            using var iterator = clientMessage.GetEnumerator();
            var response = new ResponseParameters();
            var initialFrame = iterator.Take();
            response.Response = initialFrame.Bytes.ReadGuidL(ResponseResponseFieldOffset);
            return response;
        }

#if SERVER_CODEC
        public static ClientMessage EncodeTopicEvent(IData item, long publishTime, Guid uuid)
        {
            var clientMessage = new ClientMessage();
            var initialFrame = new Frame(new byte[EventTopicInitialFrameSize], (FrameFlags) ClientMessageFlags.Unfragmented);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.MessageType, EventTopicMessageType);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.PartitionId, -1);
            initialFrame.Bytes.WriteLongL(EventTopicPublishTimeFieldOffset, publishTime);
            initialFrame.Bytes.WriteGuidL(EventTopicUuidFieldOffset, uuid);
            clientMessage.Append(initialFrame);
            clientMessage.Flags |= ClientMessageFlags.Event;
            DataCodec.Encode(clientMessage, item);
            return clientMessage;
        }
#endif
        public static ValueTask HandleEventAsync(ClientMessage clientMessage, Func<IData, long, Guid, object, ValueTask> handleTopicEventAsync, object state, ILoggerFactory loggerFactory)
        {
            using var iterator = clientMessage.GetEnumerator();
            var messageType = clientMessage.MessageType;
            if (messageType == EventTopicMessageType)
            {
                var initialFrame = iterator.Take();
                var publishTime =  initialFrame.Bytes.ReadLongL(EventTopicPublishTimeFieldOffset);
                var uuid =  initialFrame.Bytes.ReadGuidL(EventTopicUuidFieldOffset);
                var item = DataCodec.Decode(iterator);
                return handleTopicEventAsync(item, publishTime, uuid, state);
            }
            loggerFactory.CreateLogger(typeof(EventHandler)).LogDebug("Unknown message type received on event handler :" + messageType);
            return default;
        }
    }
}
