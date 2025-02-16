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
//   Hazelcast Client Protocol Code Generator @99380dc
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
    /// Causes the current thread to wait until the latch has counted down
    /// to zero, or an exception is thrown, or the specified waiting time
    /// elapses. If the current count is zero then this method returns
    /// immediately with the value true. If the current count is greater than
    /// zero, then the current thread becomes disabled for thread scheduling
    /// purposes and lies dormant until one of five things happen: the count
    /// reaches zero due to invocations of the {@code countDown} method, this
    /// ICountDownLatch instance is destroyed, the countdown owner becomes
    /// disconnected, some other thread Thread#interrupt interrupts the current
    /// thread, or the specified waiting time elapses. If the count reaches zero
    /// then the method returns with the value true. If the current thread has
    /// its interrupted status set on entry to this method, or is interrupted
    /// while waiting, then {@code InterruptedException} is thrown
    /// and the current thread's interrupted status is cleared. If the specified
    /// waiting time elapses then the value false is returned.  If the time is
    /// less than or equal to zero, the method will not wait at all.
    ///</summary>
#if SERVER_CODEC
    internal static class CountDownLatchAwaitServerCodec
#else
    internal static class CountDownLatchAwaitCodec
#endif
    {
        public const int RequestMessageType = 721408; // 0x0B0200
        public const int ResponseMessageType = 721409; // 0x0B0201
        private const int RequestInvocationUidFieldOffset = Messaging.FrameFields.Offset.PartitionId + BytesExtensions.SizeOfInt;
        private const int RequestTimeoutMsFieldOffset = RequestInvocationUidFieldOffset + BytesExtensions.SizeOfCodecGuid;
        private const int RequestInitialFrameSize = RequestTimeoutMsFieldOffset + BytesExtensions.SizeOfLong;
        private const int ResponseResponseFieldOffset = Messaging.FrameFields.Offset.ResponseBackupAcks + BytesExtensions.SizeOfByte;
        private const int ResponseInitialFrameSize = ResponseResponseFieldOffset + BytesExtensions.SizeOfBool;

#if SERVER_CODEC
        public sealed class RequestParameters
        {

            /// <summary>
            /// CP group id of this CountDownLatch instance
            ///</summary>
            public Hazelcast.CP.CPGroupId GroupId { get; set; }

            /// <summary>
            /// Name of this CountDownLatch instance
            ///</summary>
            public string Name { get; set; }

            /// <summary>
            /// UID of this invocation
            ///</summary>
            public Guid InvocationUid { get; set; }

            /// <summary>
            /// The maximum time in milliseconds to wait
            ///</summary>
            public long TimeoutMs { get; set; }
        }
#endif

        public static ClientMessage EncodeRequest(Hazelcast.CP.CPGroupId groupId, string name, Guid invocationUid, long timeoutMs)
        {
            var clientMessage = new ClientMessage
            {
                IsRetryable = true,
                OperationName = "CountDownLatch.Await"
            };
            var initialFrame = new Frame(new byte[RequestInitialFrameSize], (FrameFlags) ClientMessageFlags.Unfragmented);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.MessageType, RequestMessageType);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.PartitionId, -1);
            initialFrame.Bytes.WriteGuidL(RequestInvocationUidFieldOffset, invocationUid);
            initialFrame.Bytes.WriteLongL(RequestTimeoutMsFieldOffset, timeoutMs);
            clientMessage.Append(initialFrame);
            RaftGroupIdCodec.Encode(clientMessage, groupId);
            StringCodec.Encode(clientMessage, name);
            return clientMessage;
        }

#if SERVER_CODEC
        public static RequestParameters DecodeRequest(ClientMessage clientMessage)
        {
            using var iterator = clientMessage.GetEnumerator();
            var request = new RequestParameters();
            var initialFrame = iterator.Take();
            request.InvocationUid = initialFrame.Bytes.ReadGuidL(RequestInvocationUidFieldOffset);
            request.TimeoutMs = initialFrame.Bytes.ReadLongL(RequestTimeoutMsFieldOffset);
            request.GroupId = RaftGroupIdCodec.Decode(iterator);
            request.Name = StringCodec.Decode(iterator);
            return request;
        }
#endif

        public sealed class ResponseParameters
        {

            /// <summary>
            /// true if the count reached zero, false if
            /// the waiting time elapsed before the count reached 0
            ///</summary>
            public bool Response { get; set; }
        }

#if SERVER_CODEC
        public static ClientMessage EncodeResponse(bool response)
        {
            var clientMessage = new ClientMessage();
            var initialFrame = new Frame(new byte[ResponseInitialFrameSize], (FrameFlags) ClientMessageFlags.Unfragmented);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.MessageType, ResponseMessageType);
            initialFrame.Bytes.WriteBoolL(ResponseResponseFieldOffset, response);
            clientMessage.Append(initialFrame);
            return clientMessage;
        }
#endif

        public static ResponseParameters DecodeResponse(ClientMessage clientMessage)
        {
            using var iterator = clientMessage.GetEnumerator();
            var response = new ResponseParameters();
            var initialFrame = iterator.Take();
            response.Response = initialFrame.Bytes.ReadBoolL(ResponseResponseFieldOffset);
            return response;
        }

    }
}
