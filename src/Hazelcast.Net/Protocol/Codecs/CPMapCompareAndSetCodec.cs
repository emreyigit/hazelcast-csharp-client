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
    /// Tests if the value associated with the key is expectedValue and if so associates key with
    /// newValue.
    ///</summary>
#if SERVER_CODEC
    internal static class CPMapCompareAndSetServerCodec
#else
    internal static class CPMapCompareAndSetCodec
#endif
    {
        public const int RequestMessageType = 2295296; // 0x230600
        public const int ResponseMessageType = 2295297; // 0x230601
        private const int RequestInitialFrameSize = Messaging.FrameFields.Offset.PartitionId + BytesExtensions.SizeOfInt;
        private const int ResponseResponseFieldOffset = Messaging.FrameFields.Offset.ResponseBackupAcks + BytesExtensions.SizeOfByte;
        private const int ResponseInitialFrameSize = ResponseResponseFieldOffset + BytesExtensions.SizeOfBool;

#if SERVER_CODEC
        public sealed class RequestParameters
        {

            /// <summary>
            /// CP group ID of this CPMap instance.
            ///</summary>
            public Hazelcast.CP.CPGroupId GroupId { get; set; }

            /// <summary>
            /// Name of this CPMap instance.
            ///</summary>
            public string Name { get; set; }

            /// <summary>
            /// Key of the data that is subject of the compare and set.
            ///</summary>
            public IData Key { get; set; }

            /// <summary>
            /// The expected value associated with key.
            ///</summary>
            public IData ExpectedValue { get; set; }

            /// <summary>
            /// The new value to associate with key.
            ///</summary>
            public IData NewValue { get; set; }
        }
#endif

        public static ClientMessage EncodeRequest(Hazelcast.CP.CPGroupId groupId, string name, IData key, IData expectedValue, IData newValue)
        {
            var clientMessage = new ClientMessage
            {
                IsRetryable = false,
                OperationName = "CPMap.CompareAndSet"
            };
            var initialFrame = new Frame(new byte[RequestInitialFrameSize], (FrameFlags) ClientMessageFlags.Unfragmented);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.MessageType, RequestMessageType);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.PartitionId, -1);
            clientMessage.Append(initialFrame);
            RaftGroupIdCodec.Encode(clientMessage, groupId);
            StringCodec.Encode(clientMessage, name);
            DataCodec.Encode(clientMessage, key);
            DataCodec.Encode(clientMessage, expectedValue);
            DataCodec.Encode(clientMessage, newValue);
            return clientMessage;
        }

#if SERVER_CODEC
        public static RequestParameters DecodeRequest(ClientMessage clientMessage)
        {
            using var iterator = clientMessage.GetEnumerator();
            var request = new RequestParameters();
            iterator.Take(); // empty initial frame
            request.GroupId = RaftGroupIdCodec.Decode(iterator);
            request.Name = StringCodec.Decode(iterator);
            request.Key = DataCodec.Decode(iterator);
            request.ExpectedValue = DataCodec.Decode(iterator);
            request.NewValue = DataCodec.Decode(iterator);
            return request;
        }
#endif

        public sealed class ResponseParameters
        {

            /// <summary>
            /// True if key was associated with newValue, otherwise false.
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
