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
    /// Inserts all of the elements in the specified collection into this list at the specified position (optional operation).
    /// Shifts the element currently at that position (if any) and any subsequent elements to the right (increases their indices).
    /// The new elements will appear in this list in the order that they are returned by the specified collection's iterator.
    /// The behavior of this operation is undefined if the specified collection is modified while the operation is in progress.
    /// (Note that this will occur if the specified collection is this list, and it's nonempty.)
    ///</summary>
#if SERVER_CODEC
    internal static class ListAddAllWithIndexServerCodec
#else
    internal static class ListAddAllWithIndexCodec
#endif
    {
        public const int RequestMessageType = 331264; // 0x050E00
        public const int ResponseMessageType = 331265; // 0x050E01
        private const int RequestIndexFieldOffset = Messaging.FrameFields.Offset.PartitionId + BytesExtensions.SizeOfInt;
        private const int RequestInitialFrameSize = RequestIndexFieldOffset + BytesExtensions.SizeOfInt;
        private const int ResponseResponseFieldOffset = Messaging.FrameFields.Offset.ResponseBackupAcks + BytesExtensions.SizeOfByte;
        private const int ResponseInitialFrameSize = ResponseResponseFieldOffset + BytesExtensions.SizeOfBool;

#if SERVER_CODEC
        public sealed class RequestParameters
        {

            /// <summary>
            /// Name of the List
            ///</summary>
            public string Name { get; set; }

            /// <summary>
            /// index at which to insert the first element from the specified collection.
            ///</summary>
            public int Index { get; set; }

            /// <summary>
            /// The list of value to insert into the list.
            ///</summary>
            public IList<IData> ValueList { get; set; }
        }
#endif

        public static ClientMessage EncodeRequest(string name, int index, ICollection<IData> valueList)
        {
            var clientMessage = new ClientMessage
            {
                IsRetryable = false,
                OperationName = "List.AddAllWithIndex"
            };
            var initialFrame = new Frame(new byte[RequestInitialFrameSize], (FrameFlags) ClientMessageFlags.Unfragmented);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.MessageType, RequestMessageType);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.PartitionId, -1);
            initialFrame.Bytes.WriteIntL(RequestIndexFieldOffset, index);
            clientMessage.Append(initialFrame);
            StringCodec.Encode(clientMessage, name);
            ListMultiFrameCodec.Encode(clientMessage, valueList, DataCodec.Encode);
            return clientMessage;
        }

#if SERVER_CODEC
        public static RequestParameters DecodeRequest(ClientMessage clientMessage)
        {
            using var iterator = clientMessage.GetEnumerator();
            var request = new RequestParameters();
            var initialFrame = iterator.Take();
            request.Index = initialFrame.Bytes.ReadIntL(RequestIndexFieldOffset);
            request.Name = StringCodec.Decode(iterator);
            request.ValueList = ListMultiFrameCodec.Decode(iterator, DataCodec.Decode);
            return request;
        }
#endif

        public sealed class ResponseParameters
        {

            /// <summary>
            /// True if this list changed as a result of the call, false otherwise.
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
