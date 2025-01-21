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
    /// Removes the mapping for a key from this map if it is present. The map will not contain a mapping for the
    /// specified key once the call returns. The object to be removed will be accessible only in the current transaction
    /// context until the transaction is committed.
    ///</summary>
#if SERVER_CODEC
    internal static class TransactionalMapRemoveServerCodec
#else
    internal static class TransactionalMapRemoveCodec
#endif
    {
        public const int RequestMessageType = 920320; // 0x0E0B00
        public const int ResponseMessageType = 920321; // 0x0E0B01
        private const int RequestTxnIdFieldOffset = Messaging.FrameFields.Offset.PartitionId + BytesExtensions.SizeOfInt;
        private const int RequestThreadIdFieldOffset = RequestTxnIdFieldOffset + BytesExtensions.SizeOfCodecGuid;
        private const int RequestInitialFrameSize = RequestThreadIdFieldOffset + BytesExtensions.SizeOfLong;
        private const int ResponseInitialFrameSize = Messaging.FrameFields.Offset.ResponseBackupAcks + BytesExtensions.SizeOfByte;

#if SERVER_CODEC
        public sealed class RequestParameters
        {

            /// <summary>
            /// Name of the Transactional Map
            ///</summary>
            public string Name { get; set; }

            /// <summary>
            /// ID of the this transaction operation
            ///</summary>
            public Guid TxnId { get; set; }

            /// <summary>
            /// The id of the user thread performing the operation. It is used to guarantee that only the lock holder thread (if a lock exists on the entry) can perform the requested operation.
            ///</summary>
            public long ThreadId { get; set; }

            /// <summary>
            /// Remove the mapping for this key.
            ///</summary>
            public IData Key { get; set; }
        }
#endif

        public static ClientMessage EncodeRequest(string name, Guid txnId, long threadId, IData key)
        {
            var clientMessage = new ClientMessage
            {
                IsRetryable = false,
                OperationName = "TransactionalMap.Remove"
            };
            var initialFrame = new Frame(new byte[RequestInitialFrameSize], (FrameFlags) ClientMessageFlags.Unfragmented);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.MessageType, RequestMessageType);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.PartitionId, -1);
            initialFrame.Bytes.WriteGuidL(RequestTxnIdFieldOffset, txnId);
            initialFrame.Bytes.WriteLongL(RequestThreadIdFieldOffset, threadId);
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
            request.TxnId = initialFrame.Bytes.ReadGuidL(RequestTxnIdFieldOffset);
            request.ThreadId = initialFrame.Bytes.ReadLongL(RequestThreadIdFieldOffset);
            request.Name = StringCodec.Decode(iterator);
            request.Key = DataCodec.Decode(iterator);
            return request;
        }
#endif

        public sealed class ResponseParameters
        {

            /// <summary>
            /// The previous value associated with key, or null if there was no mapping for key
            ///</summary>
            public IData Response { get; set; }
        }

#if SERVER_CODEC
        public static ClientMessage EncodeResponse(IData response)
        {
            var clientMessage = new ClientMessage();
            var initialFrame = new Frame(new byte[ResponseInitialFrameSize], (FrameFlags) ClientMessageFlags.Unfragmented);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.MessageType, ResponseMessageType);
            clientMessage.Append(initialFrame);
            CodecUtil.EncodeNullable(clientMessage, response, DataCodec.Encode);
            return clientMessage;
        }
#endif

        public static ResponseParameters DecodeResponse(ClientMessage clientMessage)
        {
            using var iterator = clientMessage.GetEnumerator();
            var response = new ResponseParameters();
            iterator.Take(); // empty initial frame
            response.Response = CodecUtil.DecodeNullable(iterator, DataCodec.Decode);
            return response;
        }

    }
}
