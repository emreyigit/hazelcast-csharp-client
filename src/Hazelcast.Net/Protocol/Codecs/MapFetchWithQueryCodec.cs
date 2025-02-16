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
    /// Fetches the specified number of entries from the specified partition starting from specified table index
    /// that match the predicate and applies the projection logic on them.
    ///</summary>
#if SERVER_CODEC
    internal static class MapFetchWithQueryServerCodec
#else
    internal static class MapFetchWithQueryCodec
#endif
    {
        public const int RequestMessageType = 81920; // 0x014000
        public const int ResponseMessageType = 81921; // 0x014001
        private const int RequestBatchFieldOffset = Messaging.FrameFields.Offset.PartitionId + BytesExtensions.SizeOfInt;
        private const int RequestInitialFrameSize = RequestBatchFieldOffset + BytesExtensions.SizeOfInt;
        private const int ResponseInitialFrameSize = Messaging.FrameFields.Offset.ResponseBackupAcks + BytesExtensions.SizeOfByte;

#if SERVER_CODEC
        public sealed class RequestParameters
        {

            /// <summary>
            /// Name of the map
            ///</summary>
            public string Name { get; set; }

            /// <summary>
            /// The index-size pairs that define the state of iteration
            ///</summary>
            public IList<KeyValuePair<int, int>> IterationPointers { get; set; }

            /// <summary>
            /// The number of items to be batched
            ///</summary>
            public int Batch { get; set; }

            /// <summary>
            /// projection to transform the entries with
            ///</summary>
            public IData Projection { get; set; }

            /// <summary>
            /// predicate to filter the entries with
            ///</summary>
            public IData Predicate { get; set; }
        }
#endif

        public static ClientMessage EncodeRequest(string name, ICollection<KeyValuePair<int, int>> iterationPointers, int batch, IData projection, IData predicate)
        {
            var clientMessage = new ClientMessage
            {
                IsRetryable = true,
                OperationName = "Map.FetchWithQuery"
            };
            var initialFrame = new Frame(new byte[RequestInitialFrameSize], (FrameFlags) ClientMessageFlags.Unfragmented);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.MessageType, RequestMessageType);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.PartitionId, -1);
            initialFrame.Bytes.WriteIntL(RequestBatchFieldOffset, batch);
            clientMessage.Append(initialFrame);
            StringCodec.Encode(clientMessage, name);
            EntryListIntegerIntegerCodec.Encode(clientMessage, iterationPointers);
            DataCodec.Encode(clientMessage, projection);
            DataCodec.Encode(clientMessage, predicate);
            return clientMessage;
        }

#if SERVER_CODEC
        public static RequestParameters DecodeRequest(ClientMessage clientMessage)
        {
            using var iterator = clientMessage.GetEnumerator();
            var request = new RequestParameters();
            var initialFrame = iterator.Take();
            request.Batch = initialFrame.Bytes.ReadIntL(RequestBatchFieldOffset);
            request.Name = StringCodec.Decode(iterator);
            request.IterationPointers = EntryListIntegerIntegerCodec.Decode(iterator);
            request.Projection = DataCodec.Decode(iterator);
            request.Predicate = DataCodec.Decode(iterator);
            return request;
        }
#endif

        public sealed class ResponseParameters
        {

            /// <summary>
            /// List of fetched entries.
            ///</summary>
            public IList<IData> Results { get; set; }

            /// <summary>
            /// The index-size pairs that define the state of iteration
            ///</summary>
            public IList<KeyValuePair<int, int>> IterationPointers { get; set; }
        }

#if SERVER_CODEC
        public static ClientMessage EncodeResponse(ICollection<IData> results, ICollection<KeyValuePair<int, int>> iterationPointers)
        {
            var clientMessage = new ClientMessage();
            var initialFrame = new Frame(new byte[ResponseInitialFrameSize], (FrameFlags) ClientMessageFlags.Unfragmented);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.MessageType, ResponseMessageType);
            clientMessage.Append(initialFrame);
            ListMultiFrameCodec.EncodeContainsNullable(clientMessage, results, DataCodec.Encode);
            EntryListIntegerIntegerCodec.Encode(clientMessage, iterationPointers);
            return clientMessage;
        }
#endif

        public static ResponseParameters DecodeResponse(ClientMessage clientMessage)
        {
            using var iterator = clientMessage.GetEnumerator();
            var response = new ResponseParameters();
            iterator.Take(); // empty initial frame
            response.Results = ListMultiFrameCodec.DecodeContainsNullable(iterator, DataCodec.Decode);
            response.IterationPointers = EntryListIntegerIntegerCodec.Decode(iterator);
            return response;
        }

    }
}
