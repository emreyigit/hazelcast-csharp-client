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
//   Hazelcast Client Protocol Code Generator @c89bc95
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
    /// Releases the lock for the specified key. It never blocks and returns immediately.
    /// If the current thread is the holder of this lock, then the hold count is decremented.If the hold count is zero,
    /// then the lock is released.  If the current thread is not the holder of this lock,
    /// then ILLEGAL_MONITOR_STATE is thrown.
    ///</summary>
#if SERVER_CODEC
    internal static class MapUnlockServerCodec
#else
    internal static class MapUnlockCodec
#endif
    {
        public const int RequestMessageType = 70400; // 0x011300
        public const int ResponseMessageType = 70401; // 0x011301
        private const int RequestThreadIdFieldOffset = Messaging.FrameFields.Offset.PartitionId + BytesExtensions.SizeOfInt;
        private const int RequestReferenceIdFieldOffset = RequestThreadIdFieldOffset + BytesExtensions.SizeOfLong;
        private const int RequestInitialFrameSize = RequestReferenceIdFieldOffset + BytesExtensions.SizeOfLong;
        private const int ResponseInitialFrameSize = Messaging.FrameFields.Offset.ResponseBackupAcks + BytesExtensions.SizeOfByte;

#if SERVER_CODEC
        public sealed class RequestParameters
        {

            /// <summary>
            /// name of map
            ///</summary>
            public string Name { get; set; }

            /// <summary>
            /// Key for the map entry to unlock
            ///</summary>
            public IData Key { get; set; }

            /// <summary>
            /// The id of the user thread performing the operation. It is used to guarantee that only the lock holder thread (if a lock exists on the entry) can perform the requested operation.
            ///</summary>
            public long ThreadId { get; set; }

            /// <summary>
            /// The client-wide unique id for this request. It is used to make the request idempotent by sending the same reference id during retries.
            ///</summary>
            public long ReferenceId { get; set; }
        }
#endif

        public static ClientMessage EncodeRequest(string name, IData key, long threadId, long referenceId)
        {
            var clientMessage = new ClientMessage
            {
                IsRetryable = true,
                OperationName = "Map.Unlock"
            };
            var initialFrame = new Frame(new byte[RequestInitialFrameSize], (FrameFlags) ClientMessageFlags.Unfragmented);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.MessageType, RequestMessageType);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.PartitionId, -1);
            initialFrame.Bytes.WriteLongL(RequestThreadIdFieldOffset, threadId);
            initialFrame.Bytes.WriteLongL(RequestReferenceIdFieldOffset, referenceId);
            clientMessage.Append(initialFrame);
            StringCodec.Encode(clientMessage, name);
            DataCodec.Encode(clientMessage, key);
            return clientMessage;
        }

#if SERVER_CODEC
        public static RequestParameters DecodeRequest(ClientMessage clientMessage)
        {
            using var iterator = clientMessage.GetEnumerator();
            var request = new RequestParameters();
            var initialFrame = iterator.Take();
            request.ThreadId = initialFrame.Bytes.ReadLongL(RequestThreadIdFieldOffset);
            request.ReferenceId = initialFrame.Bytes.ReadLongL(RequestReferenceIdFieldOffset);
            request.Name = StringCodec.Decode(iterator);
            request.Key = DataCodec.Decode(iterator);
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

    }
}
