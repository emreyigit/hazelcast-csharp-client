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
    /// Makes an authentication request to the cluster using custom credentials.
    ///</summary>
#if SERVER_CODEC
    internal static class ClientAuthenticationCustomServerCodec
#else
    internal static class ClientAuthenticationCustomCodec
#endif
    {
        public const int RequestMessageType = 512; // 0x000200
        public const int ResponseMessageType = 513; // 0x000201
        private const int RequestUuidFieldOffset = Messaging.FrameFields.Offset.PartitionId + BytesExtensions.SizeOfInt;
        private const int RequestSerializationVersionFieldOffset = RequestUuidFieldOffset + BytesExtensions.SizeOfCodecGuid;
        private const int RequestRoutingModeFieldOffset = RequestSerializationVersionFieldOffset + BytesExtensions.SizeOfByte;
        private const int RequestCpDirectToLeaderRoutingFieldOffset = RequestRoutingModeFieldOffset + BytesExtensions.SizeOfByte;
        private const int RequestInitialFrameSize = RequestCpDirectToLeaderRoutingFieldOffset + BytesExtensions.SizeOfBool;
        private const int ResponseStatusFieldOffset = Messaging.FrameFields.Offset.ResponseBackupAcks + BytesExtensions.SizeOfByte;
        private const int ResponseMemberUuidFieldOffset = ResponseStatusFieldOffset + BytesExtensions.SizeOfByte;
        private const int ResponseSerializationVersionFieldOffset = ResponseMemberUuidFieldOffset + BytesExtensions.SizeOfCodecGuid;
        private const int ResponsePartitionCountFieldOffset = ResponseSerializationVersionFieldOffset + BytesExtensions.SizeOfByte;
        private const int ResponseClusterIdFieldOffset = ResponsePartitionCountFieldOffset + BytesExtensions.SizeOfInt;
        private const int ResponseFailoverSupportedFieldOffset = ResponseClusterIdFieldOffset + BytesExtensions.SizeOfCodecGuid;
        private const int ResponseMemberListVersionFieldOffset = ResponseFailoverSupportedFieldOffset + BytesExtensions.SizeOfBool;
        private const int ResponsePartitionListVersionFieldOffset = ResponseMemberListVersionFieldOffset + BytesExtensions.SizeOfInt;
        private const int ResponseInitialFrameSize = ResponsePartitionListVersionFieldOffset + BytesExtensions.SizeOfInt;

#if SERVER_CODEC
        public sealed class RequestParameters
        {

            /// <summary>
            /// Cluster name that client will connect to.
            ///</summary>
            public string ClusterName { get; set; }

            /// <summary>
            /// Secret byte array for authentication.
            ///</summary>
            public byte[] Credentials { get; set; }

            /// <summary>
            /// Unique string identifying the connected client uniquely.
            ///</summary>
            public Guid Uuid { get; set; }

            /// <summary>
            /// The type of the client. E.g. JAVA, CPP, CSHARP, etc.
            ///</summary>
            public string ClientType { get; set; }

            /// <summary>
            /// client side supported version to inform server side
            ///</summary>
            public byte SerializationVersion { get; set; }

            /// <summary>
            /// The Hazelcast version of the client. (e.g. 3.7.2)
            ///</summary>
            public string ClientHazelcastVersion { get; set; }

            /// <summary>
            /// the name of the client instance
            ///</summary>
            public string ClientName { get; set; }

            /// <summary>
            /// User defined labels of the client instance
            ///</summary>
            public IList<string> Labels { get; set; }

            /// <summary>
            /// Identifies the routing mode of the client. It can be UNISCOKET(0), SMART(1) or SUBSET(2).
            ///</summary>
            public byte RoutingMode { get; set; }

            /// <summary>
            /// The client's CP direct-to-leader routing setting (enabled or disabled)
            ///</summary>
            public bool CpDirectToLeaderRouting { get; set; }

            /// <summary>
            /// <c>true</c> if the routingMode is received from the client, <c>false</c> otherwise.
            /// If this is false, routingMode has the default value for its type.
            /// </summary>
            public bool IsRoutingModeExists { get; set; }

            /// <summary>
            /// <c>true</c> if the cpDirectToLeaderRouting is received from the client, <c>false</c> otherwise.
            /// If this is false, cpDirectToLeaderRouting has the default value for its type.
            /// </summary>
            public bool IsCpDirectToLeaderRoutingExists { get; set; }
        }
#endif

        public static ClientMessage EncodeRequest(string clusterName, byte[] credentials, Guid uuid, string clientType, byte serializationVersion, string clientHazelcastVersion, string clientName, ICollection<string> labels, byte routingMode, bool cpDirectToLeaderRouting)
        {
            var clientMessage = new ClientMessage
            {
                IsRetryable = true,
                OperationName = "Client.AuthenticationCustom"
            };
            var initialFrame = new Frame(new byte[RequestInitialFrameSize], (FrameFlags) ClientMessageFlags.Unfragmented);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.MessageType, RequestMessageType);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.PartitionId, -1);
            initialFrame.Bytes.WriteGuidL(RequestUuidFieldOffset, uuid);
            initialFrame.Bytes.WriteByteL(RequestSerializationVersionFieldOffset, serializationVersion);
            initialFrame.Bytes.WriteByteL(RequestRoutingModeFieldOffset, routingMode);
            initialFrame.Bytes.WriteBoolL(RequestCpDirectToLeaderRoutingFieldOffset, cpDirectToLeaderRouting);
            clientMessage.Append(initialFrame);
            StringCodec.Encode(clientMessage, clusterName);
            ByteArrayCodec.Encode(clientMessage, credentials);
            StringCodec.Encode(clientMessage, clientType);
            StringCodec.Encode(clientMessage, clientHazelcastVersion);
            StringCodec.Encode(clientMessage, clientName);
            ListMultiFrameCodec.Encode(clientMessage, labels, StringCodec.Encode);
            return clientMessage;
        }

#if SERVER_CODEC
        public static RequestParameters DecodeRequest(ClientMessage clientMessage)
        {
            using var iterator = clientMessage.GetEnumerator();
            var request = new RequestParameters();
            var initialFrame = iterator.Take();
            request.Uuid = initialFrame.Bytes.ReadGuidL(RequestUuidFieldOffset);
            request.SerializationVersion = initialFrame.Bytes.ReadByteL(RequestSerializationVersionFieldOffset);
            if (initialFrame.Bytes.Length >= RequestRoutingModeFieldOffset + BytesExtensions.SizeOfByte)
            {
                request.RoutingMode = initialFrame.Bytes.ReadByteL(RequestRoutingModeFieldOffset);
                request.IsRoutingModeExists = true;
            }
            else request.IsRoutingModeExists = false;
            if (initialFrame.Bytes.Length >= RequestCpDirectToLeaderRoutingFieldOffset + BytesExtensions.SizeOfBool)
            {
                request.CpDirectToLeaderRouting = initialFrame.Bytes.ReadBoolL(RequestCpDirectToLeaderRoutingFieldOffset);
                request.IsCpDirectToLeaderRoutingExists = true;
            }
            else request.IsCpDirectToLeaderRoutingExists = false;
            request.ClusterName = StringCodec.Decode(iterator);
            request.Credentials = ByteArrayCodec.Decode(iterator);
            request.ClientType = StringCodec.Decode(iterator);
            request.ClientHazelcastVersion = StringCodec.Decode(iterator);
            request.ClientName = StringCodec.Decode(iterator);
            request.Labels = ListMultiFrameCodec.Decode(iterator, StringCodec.Decode);
            return request;
        }
#endif

        public sealed class ResponseParameters
        {

            /// <summary>
            /// A byte that represents the authentication status. It can be AUTHENTICATED(0), CREDENTIALS_FAILED(1),
            /// SERIALIZATION_VERSION_MISMATCH(2) or NOT_ALLOWED_IN_CLUSTER(3).
            ///</summary>
            public byte Status { get; set; }

            /// <summary>
            /// Address of the Hazelcast member which sends the authentication response.
            ///</summary>
            public Hazelcast.Networking.NetworkAddress Address { get; set; }

            /// <summary>
            /// UUID of the Hazelcast member which sends the authentication response.
            ///</summary>
            public Guid MemberUuid { get; set; }

            /// <summary>
            /// client side supported version to inform server side
            ///</summary>
            public byte SerializationVersion { get; set; }

            /// <summary>
            /// Version of the Hazelcast member which sends the authentication response.
            ///</summary>
            public string ServerHazelcastVersion { get; set; }

            /// <summary>
            /// Partition count of the cluster.
            ///</summary>
            public int PartitionCount { get; set; }

            /// <summary>
            /// The cluster id of the cluster.
            ///</summary>
            public Guid ClusterId { get; set; }

            /// <summary>
            /// Returns true if server supports clients with failover feature.
            ///</summary>
            public bool FailoverSupported { get; set; }

            /// <summary>
            /// Returns the list of TPC ports or null if TPC is disabled.
            ///</summary>
            public IList<int> TpcPorts { get; set; }

            /// <summary>
            /// Returns the token to use while authenticating TPC channels
            /// or null if TPC is disabled.
            ///</summary>
            public byte[] TpcToken { get; set; }

            /// <summary>
            /// Incremental member list version
            ///</summary>
            public int MemberListVersion { get; set; }

            /// <summary>
            /// List of member infos  at the cluster associated with the given version
            ///</summary>
            public IList<Hazelcast.Models.MemberInfo> MemberInfos { get; set; }

            /// <summary>
            /// Incremental state version of the partition table
            ///</summary>
            public int PartitionListVersion { get; set; }

            /// <summary>
            /// The partition table. In each entry, it has uuid of the member and list of partitions belonging to that member
            ///</summary>
            public IList<KeyValuePair<Guid, IList<int>>> Partitions { get; set; }

            /// <summary>
            /// Server/Member metadata represented as in key value pairs
            ///</summary>
            public IDictionary<string, string> KeyValuePairs { get; set; }

            /// <summary>
            /// <c>true</c> if the tpcPorts is received from the member, <c>false</c> otherwise.
            /// If this is false, tpcPorts has the default value for its type.
            /// </summary>
            public bool IsTpcPortsExists { get; set; }

            /// <summary>
            /// <c>true</c> if the tpcToken is received from the member, <c>false</c> otherwise.
            /// If this is false, tpcToken has the default value for its type.
            /// </summary>
            public bool IsTpcTokenExists { get; set; }

            /// <summary>
            /// <c>true</c> if the memberListVersion is received from the member, <c>false</c> otherwise.
            /// If this is false, memberListVersion has the default value for its type.
            /// </summary>
            public bool IsMemberListVersionExists { get; set; }

            /// <summary>
            /// <c>true</c> if the memberInfos is received from the member, <c>false</c> otherwise.
            /// If this is false, memberInfos has the default value for its type.
            /// </summary>
            public bool IsMemberInfosExists { get; set; }

            /// <summary>
            /// <c>true</c> if the partitionListVersion is received from the member, <c>false</c> otherwise.
            /// If this is false, partitionListVersion has the default value for its type.
            /// </summary>
            public bool IsPartitionListVersionExists { get; set; }

            /// <summary>
            /// <c>true</c> if the partitions is received from the member, <c>false</c> otherwise.
            /// If this is false, partitions has the default value for its type.
            /// </summary>
            public bool IsPartitionsExists { get; set; }

            /// <summary>
            /// <c>true</c> if the keyValuePairs is received from the member, <c>false</c> otherwise.
            /// If this is false, keyValuePairs has the default value for its type.
            /// </summary>
            public bool IsKeyValuePairsExists { get; set; }
        }

#if SERVER_CODEC
        public static ClientMessage EncodeResponse(byte status, Hazelcast.Networking.NetworkAddress address, Guid memberUuid, byte serializationVersion, string serverHazelcastVersion, int partitionCount, Guid clusterId, bool failoverSupported, ICollection<int> tpcPorts, byte[] tpcToken, int memberListVersion, ICollection<Hazelcast.Models.MemberInfo> memberInfos, int partitionListVersion, ICollection<KeyValuePair<Guid, IList<int>>> partitions, IDictionary<string, string> keyValuePairs)
        {
            var clientMessage = new ClientMessage();
            var initialFrame = new Frame(new byte[ResponseInitialFrameSize], (FrameFlags) ClientMessageFlags.Unfragmented);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.MessageType, ResponseMessageType);
            initialFrame.Bytes.WriteByteL(ResponseStatusFieldOffset, status);
            initialFrame.Bytes.WriteGuidL(ResponseMemberUuidFieldOffset, memberUuid);
            initialFrame.Bytes.WriteByteL(ResponseSerializationVersionFieldOffset, serializationVersion);
            initialFrame.Bytes.WriteIntL(ResponsePartitionCountFieldOffset, partitionCount);
            initialFrame.Bytes.WriteGuidL(ResponseClusterIdFieldOffset, clusterId);
            initialFrame.Bytes.WriteBoolL(ResponseFailoverSupportedFieldOffset, failoverSupported);
            initialFrame.Bytes.WriteIntL(ResponseMemberListVersionFieldOffset, memberListVersion);
            initialFrame.Bytes.WriteIntL(ResponsePartitionListVersionFieldOffset, partitionListVersion);
            clientMessage.Append(initialFrame);
            CodecUtil.EncodeNullable(clientMessage, address, AddressCodec.Encode);
            StringCodec.Encode(clientMessage, serverHazelcastVersion);
            CodecUtil.EncodeNullable(clientMessage, tpcPorts, ListIntegerCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, tpcToken, ByteArrayCodec.Encode);
            ListMultiFrameCodec.EncodeNullable(clientMessage, memberInfos, MemberInfoCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, partitions, EntryListUUIDListIntegerCodec.Encode);
            MapCodec.Encode(clientMessage, keyValuePairs, StringCodec.Encode, StringCodec.Encode);
            return clientMessage;
        }
#endif

        public static ResponseParameters DecodeResponse(ClientMessage clientMessage)
        {
            using var iterator = clientMessage.GetEnumerator();
            var response = new ResponseParameters();
            var initialFrame = iterator.Take();
            response.Status = initialFrame.Bytes.ReadByteL(ResponseStatusFieldOffset);
            response.MemberUuid = initialFrame.Bytes.ReadGuidL(ResponseMemberUuidFieldOffset);
            response.SerializationVersion = initialFrame.Bytes.ReadByteL(ResponseSerializationVersionFieldOffset);
            response.PartitionCount = initialFrame.Bytes.ReadIntL(ResponsePartitionCountFieldOffset);
            response.ClusterId = initialFrame.Bytes.ReadGuidL(ResponseClusterIdFieldOffset);
            response.FailoverSupported = initialFrame.Bytes.ReadBoolL(ResponseFailoverSupportedFieldOffset);
            if (initialFrame.Bytes.Length >= ResponseMemberListVersionFieldOffset + BytesExtensions.SizeOfInt)
            {
                response.MemberListVersion = initialFrame.Bytes.ReadIntL(ResponseMemberListVersionFieldOffset);
                response.IsMemberListVersionExists = true;
            }
            else response.IsMemberListVersionExists = false;
            if (initialFrame.Bytes.Length >= ResponsePartitionListVersionFieldOffset + BytesExtensions.SizeOfInt)
            {
                response.PartitionListVersion = initialFrame.Bytes.ReadIntL(ResponsePartitionListVersionFieldOffset);
                response.IsPartitionListVersionExists = true;
            }
            else response.IsPartitionListVersionExists = false;
            response.Address = CodecUtil.DecodeNullable(iterator, AddressCodec.Decode);
            response.ServerHazelcastVersion = StringCodec.Decode(iterator);
            if (iterator.Current?.Next != null)
            {
                response.TpcPorts = CodecUtil.DecodeNullable(iterator, ListIntegerCodec.Decode);
                response.IsTpcPortsExists = true;
            }
            else response.IsTpcPortsExists = false;
            if (iterator.Current?.Next != null)
            {
                response.TpcToken = CodecUtil.DecodeNullable(iterator, ByteArrayCodec.Decode);
                response.IsTpcTokenExists = true;
            }
            else response.IsTpcTokenExists = false;
            if (iterator.Current?.Next != null)
            {
                response.MemberInfos = ListMultiFrameCodec.DecodeNullable(iterator, MemberInfoCodec.Decode);
                response.IsMemberInfosExists = true;
            }
            else response.IsMemberInfosExists = false;
            if (iterator.Current?.Next != null)
            {
                response.Partitions = CodecUtil.DecodeNullable(iterator, EntryListUUIDListIntegerCodec.Decode);
                response.IsPartitionsExists = true;
            }
            else response.IsPartitionsExists = false;
            if (iterator.Current?.Next != null)
            {
                response.KeyValuePairs = MapCodec.Decode(iterator, StringCodec.Decode, StringCodec.Decode);
                response.IsKeyValuePairsExists = true;
            }
            else response.IsKeyValuePairsExists = false;
            return response;
        }

    }
}
