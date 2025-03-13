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
//   Hazelcast Client Protocol Code Generator @2d41b6a
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
    /// The effect of this call is equivalent to set(k, v) on this VectorCollection once for each mapping from key k to value v.
    /// The behavior of this operation is undefined if the specified collection is modified while the operation is in progress.
    /// Note that all keys in the request should belong to the partition ID to which this request is being sent.
    /// Any key that matches to a different partition ID shall be ignored.
    /// The API implementation using this request may need to send multiple of these request messages for different partitions.
    ///</summary>
#if SERVER_CODEC
    internal static class VectorCollectionPutAllServerCodec
#else
    internal static class VectorCollectionPutAllCodec
#endif
    {
        public const int RequestMessageType = 2360064; // 0x240300
        public const int ResponseMessageType = 2360065; // 0x240301
        private const int RequestInitialFrameSize = Messaging.FrameFields.Offset.PartitionId + BytesExtensions.SizeOfInt;
        private const int ResponseInitialFrameSize = Messaging.FrameFields.Offset.ResponseBackupAcks + BytesExtensions.SizeOfByte;

#if SERVER_CODEC
        public sealed class RequestParameters
        {

            /// <summary>
            /// Name of the Vector Collection.
            ///</summary>
            public string Name { get; set; }

            /// <summary>
            /// Key/VectorDocument entries to be stored in this VectorCollection.
            ///</summary>
            public ICollection<KeyValuePair<IData, Hazelcast.Models.IVectorDocument<IData>>> Entries { get; set; }
        }
#endif

        public static ClientMessage EncodeRequest(string name, ICollection<KeyValuePair<IData, Hazelcast.Models.IVectorDocument<IData>>> entries)
        {
            var clientMessage = new ClientMessage
            {
                IsRetryable = false,
                OperationName = "VectorCollection.PutAll"
            };
            var initialFrame = new Frame(new byte[RequestInitialFrameSize], (FrameFlags) ClientMessageFlags.Unfragmented);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.MessageType, RequestMessageType);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.PartitionId, -1);
            clientMessage.Append(initialFrame);
            StringCodec.Encode(clientMessage, name);
            EntryListCodec.Encode(clientMessage, entries, DataCodec.Encode, VectorDocumentCodec.Encode);
            return clientMessage;
        }

#if SERVER_CODEC
        public static RequestParameters DecodeRequest(ClientMessage clientMessage)
        {
            using var iterator = clientMessage.GetEnumerator();
            var request = new RequestParameters();
            iterator.Take(); // empty initial frame
            request.Name = StringCodec.Decode(iterator);
            request.Entries = EntryListCodec.Decode(iterator, DataCodec.Decode, VectorDocumentCodec.Decode);
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
