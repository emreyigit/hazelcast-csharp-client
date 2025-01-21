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
    /// Adds a new cache configuration to a running cluster.
    /// If a cache configuration with the given {@code name} already exists, then
    /// the new configuration is ignored and the existing one is preserved.
    ///</summary>
#if SERVER_CODEC
    internal static class DynamicConfigAddCacheConfigServerCodec
#else
    internal static class DynamicConfigAddCacheConfigCodec
#endif
    {
        public const int RequestMessageType = 1773056; // 0x1B0E00
        public const int ResponseMessageType = 1773057; // 0x1B0E01
        private const int RequestStatisticsEnabledFieldOffset = Messaging.FrameFields.Offset.PartitionId + BytesExtensions.SizeOfInt;
        private const int RequestManagementEnabledFieldOffset = RequestStatisticsEnabledFieldOffset + BytesExtensions.SizeOfBool;
        private const int RequestReadThroughFieldOffset = RequestManagementEnabledFieldOffset + BytesExtensions.SizeOfBool;
        private const int RequestWriteThroughFieldOffset = RequestReadThroughFieldOffset + BytesExtensions.SizeOfBool;
        private const int RequestBackupCountFieldOffset = RequestWriteThroughFieldOffset + BytesExtensions.SizeOfBool;
        private const int RequestAsyncBackupCountFieldOffset = RequestBackupCountFieldOffset + BytesExtensions.SizeOfInt;
        private const int RequestMergeBatchSizeFieldOffset = RequestAsyncBackupCountFieldOffset + BytesExtensions.SizeOfInt;
        private const int RequestDisablePerEntryInvalidationEventsFieldOffset = RequestMergeBatchSizeFieldOffset + BytesExtensions.SizeOfInt;
        private const int RequestInitialFrameSize = RequestDisablePerEntryInvalidationEventsFieldOffset + BytesExtensions.SizeOfBool;
        private const int ResponseInitialFrameSize = Messaging.FrameFields.Offset.ResponseBackupAcks + BytesExtensions.SizeOfByte;

#if SERVER_CODEC
        public sealed class RequestParameters
        {

            /// <summary>
            /// cache name
            ///</summary>
            public string Name { get; set; }

            /// <summary>
            /// class name of key type
            ///</summary>
            public string KeyType { get; set; }

            /// <summary>
            /// class name of value type
            ///</summary>
            public string ValueType { get; set; }

            /// <summary>
            /// {@code true} to enable gathering of statistics, otherwise {@code false}
            ///</summary>
            public bool StatisticsEnabled { get; set; }

            /// <summary>
            /// {@code true} to enable management interface on this cache or {@code false}
            ///</summary>
            public bool ManagementEnabled { get; set; }

            /// <summary>
            /// {@code true} to enable read through from a {@code CacheLoader}
            ///</summary>
            public bool ReadThrough { get; set; }

            /// <summary>
            /// {@code true} to enable write through to a {@code CacheWriter}
            ///</summary>
            public bool WriteThrough { get; set; }

            /// <summary>
            /// name of cache loader factory class, if one is configured
            ///</summary>
            public string CacheLoaderFactory { get; set; }

            /// <summary>
            /// name of cache writer factory class, if one is configured
            ///</summary>
            public string CacheWriterFactory { get; set; }

            /// <summary>
            /// Factory name of cache loader factory class, if one is configured
            ///</summary>
            public string CacheLoader { get; set; }

            /// <summary>
            /// Factory name of cache writer factory class, if one is configured
            ///</summary>
            public string CacheWriter { get; set; }

            /// <summary>
            /// number of synchronous backups
            ///</summary>
            public int BackupCount { get; set; }

            /// <summary>
            /// number of asynchronous backups
            ///</summary>
            public int AsyncBackupCount { get; set; }

            /// <summary>
            /// data type used to store entries. Valid values are {@code BINARY},
            /// {@code OBJECT} and {@code NATIVE}.
            ///</summary>
            public string InMemoryFormat { get; set; }

            /// <summary>
            /// name of an existing configured split brain protection to be used to determine the minimum
            /// number of members required in the cluster for the cache to remain functional.
            /// When {@code null}, split brain protection does not apply to this cache's operations.
            ///</summary>
            public string SplitBrainProtectionName { get; set; }

            /// <summary>
            /// name of a class implementing SplitBrainMergePolicy
            /// that handles merging of values for this cache while recovering from
            /// network partitioning
            ///</summary>
            public string MergePolicy { get; set; }

            /// <summary>
            /// number of entries to be sent in a merge operation
            ///</summary>
            public int MergeBatchSize { get; set; }

            /// <summary>
            /// when {@code true} disables invalidation events for per entry but
            /// full-flush invalidation events are still enabled.
            ///</summary>
            public bool DisablePerEntryInvalidationEvents { get; set; }

            /// <summary>
            /// partition lost listener configurations
            ///</summary>
            public ICollection<Hazelcast.Protocol.Models.ListenerConfigHolder> PartitionLostListenerConfigs { get; set; }

            /// <summary>
            /// expiry policy factory class name. When configuring an expiry policy,
            /// either this or {@ode timedExpiryPolicyFactoryConfig} should be configured.
            ///</summary>
            public string ExpiryPolicyFactoryClassName { get; set; }

            /// <summary>
            /// expiry policy factory with duration configuration
            ///</summary>
            public Hazelcast.Models.TimedExpiryPolicyFactoryOptions TimedExpiryPolicyFactoryConfig { get; set; }

            /// <summary>
            /// cache entry listeners configuration
            ///</summary>
            public ICollection<Hazelcast.Models.CacheSimpleEntryListenerOptions> CacheEntryListeners { get; set; }

            /// <summary>
            /// cache eviction configuration
            ///</summary>
            public Hazelcast.Protocol.Models.EvictionConfigHolder EvictionConfig { get; set; }

            /// <summary>
            /// reference to an existing WAN replication configuration
            ///</summary>
            public Hazelcast.Models.WanReplicationRef WanReplicationRef { get; set; }

            /// <summary>
            /// Event Journal configuration
            ///</summary>
            public Hazelcast.Models.EventJournalOptions EventJournalConfig { get; set; }

            /// <summary>
            /// hot restart configuration
            ///</summary>
            public Hazelcast.Models.HotRestartOptions HotRestartConfig { get; set; }

            /// <summary>
            /// merkle tree configuration
            ///</summary>
            public Hazelcast.Models.MerkleTreeOptions MerkleTreeConfig { get; set; }

            /// <summary>
            /// Data persistence configuration
            ///</summary>
            public Hazelcast.Models.DataPersistenceOptions DataPersistenceConfig { get; set; }

            /// <summary>
            /// Name of the User Code Namespace applied to this instance.
            ///</summary>
            public string UserCodeNamespace { get; set; }

            /// <summary>
            /// <c>true</c> if the merkleTreeConfig is received from the client, <c>false</c> otherwise.
            /// If this is false, merkleTreeConfig has the default value for its type.
            /// </summary>
            public bool IsMerkleTreeConfigExists { get; set; }

            /// <summary>
            /// <c>true</c> if the dataPersistenceConfig is received from the client, <c>false</c> otherwise.
            /// If this is false, dataPersistenceConfig has the default value for its type.
            /// </summary>
            public bool IsDataPersistenceConfigExists { get; set; }

            /// <summary>
            /// <c>true</c> if the userCodeNamespace is received from the client, <c>false</c> otherwise.
            /// If this is false, userCodeNamespace has the default value for its type.
            /// </summary>
            public bool IsUserCodeNamespaceExists { get; set; }
        }
#endif

        public static ClientMessage EncodeRequest(string name, string keyType, string valueType, bool statisticsEnabled, bool managementEnabled, bool readThrough, bool writeThrough, string cacheLoaderFactory, string cacheWriterFactory, string cacheLoader, string cacheWriter, int backupCount, int asyncBackupCount, string inMemoryFormat, string splitBrainProtectionName, string mergePolicy, int mergeBatchSize, bool disablePerEntryInvalidationEvents, ICollection<Hazelcast.Protocol.Models.ListenerConfigHolder> partitionLostListenerConfigs, string expiryPolicyFactoryClassName, Hazelcast.Models.TimedExpiryPolicyFactoryOptions timedExpiryPolicyFactoryConfig, ICollection<Hazelcast.Models.CacheSimpleEntryListenerOptions> cacheEntryListeners, Hazelcast.Protocol.Models.EvictionConfigHolder evictionConfig, Hazelcast.Models.WanReplicationRef wanReplicationRef, Hazelcast.Models.EventJournalOptions eventJournalConfig, Hazelcast.Models.HotRestartOptions hotRestartConfig, Hazelcast.Models.MerkleTreeOptions merkleTreeConfig, Hazelcast.Models.DataPersistenceOptions dataPersistenceConfig, string userCodeNamespace)
        {
            var clientMessage = new ClientMessage
            {
                IsRetryable = false,
                OperationName = "DynamicConfig.AddCacheConfig"
            };
            var initialFrame = new Frame(new byte[RequestInitialFrameSize], (FrameFlags) ClientMessageFlags.Unfragmented);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.MessageType, RequestMessageType);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.PartitionId, -1);
            initialFrame.Bytes.WriteBoolL(RequestStatisticsEnabledFieldOffset, statisticsEnabled);
            initialFrame.Bytes.WriteBoolL(RequestManagementEnabledFieldOffset, managementEnabled);
            initialFrame.Bytes.WriteBoolL(RequestReadThroughFieldOffset, readThrough);
            initialFrame.Bytes.WriteBoolL(RequestWriteThroughFieldOffset, writeThrough);
            initialFrame.Bytes.WriteIntL(RequestBackupCountFieldOffset, backupCount);
            initialFrame.Bytes.WriteIntL(RequestAsyncBackupCountFieldOffset, asyncBackupCount);
            initialFrame.Bytes.WriteIntL(RequestMergeBatchSizeFieldOffset, mergeBatchSize);
            initialFrame.Bytes.WriteBoolL(RequestDisablePerEntryInvalidationEventsFieldOffset, disablePerEntryInvalidationEvents);
            clientMessage.Append(initialFrame);
            StringCodec.Encode(clientMessage, name);
            CodecUtil.EncodeNullable(clientMessage, keyType, StringCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, valueType, StringCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, cacheLoaderFactory, StringCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, cacheWriterFactory, StringCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, cacheLoader, StringCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, cacheWriter, StringCodec.Encode);
            StringCodec.Encode(clientMessage, inMemoryFormat);
            CodecUtil.EncodeNullable(clientMessage, splitBrainProtectionName, StringCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, mergePolicy, StringCodec.Encode);
            ListMultiFrameCodec.EncodeNullable(clientMessage, partitionLostListenerConfigs, ListenerConfigHolderCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, expiryPolicyFactoryClassName, StringCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, timedExpiryPolicyFactoryConfig, TimedExpiryPolicyFactoryConfigCodec.Encode);
            ListMultiFrameCodec.EncodeNullable(clientMessage, cacheEntryListeners, CacheSimpleEntryListenerConfigCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, evictionConfig, EvictionConfigHolderCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, wanReplicationRef, WanReplicationRefCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, eventJournalConfig, EventJournalConfigCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, hotRestartConfig, HotRestartConfigCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, merkleTreeConfig, MerkleTreeConfigCodec.Encode);
            DataPersistenceConfigCodec.Encode(clientMessage, dataPersistenceConfig);
            CodecUtil.EncodeNullable(clientMessage, userCodeNamespace, StringCodec.Encode);
            return clientMessage;
        }

#if SERVER_CODEC
        public static RequestParameters DecodeRequest(ClientMessage clientMessage)
        {
            using var iterator = clientMessage.GetEnumerator();
            var request = new RequestParameters();
            var initialFrame = iterator.Take();
            request.StatisticsEnabled = initialFrame.Bytes.ReadBoolL(RequestStatisticsEnabledFieldOffset);
            request.ManagementEnabled = initialFrame.Bytes.ReadBoolL(RequestManagementEnabledFieldOffset);
            request.ReadThrough = initialFrame.Bytes.ReadBoolL(RequestReadThroughFieldOffset);
            request.WriteThrough = initialFrame.Bytes.ReadBoolL(RequestWriteThroughFieldOffset);
            request.BackupCount = initialFrame.Bytes.ReadIntL(RequestBackupCountFieldOffset);
            request.AsyncBackupCount = initialFrame.Bytes.ReadIntL(RequestAsyncBackupCountFieldOffset);
            request.MergeBatchSize = initialFrame.Bytes.ReadIntL(RequestMergeBatchSizeFieldOffset);
            request.DisablePerEntryInvalidationEvents = initialFrame.Bytes.ReadBoolL(RequestDisablePerEntryInvalidationEventsFieldOffset);
            request.Name = StringCodec.Decode(iterator);
            request.KeyType = CodecUtil.DecodeNullable(iterator, StringCodec.Decode);
            request.ValueType = CodecUtil.DecodeNullable(iterator, StringCodec.Decode);
            request.CacheLoaderFactory = CodecUtil.DecodeNullable(iterator, StringCodec.Decode);
            request.CacheWriterFactory = CodecUtil.DecodeNullable(iterator, StringCodec.Decode);
            request.CacheLoader = CodecUtil.DecodeNullable(iterator, StringCodec.Decode);
            request.CacheWriter = CodecUtil.DecodeNullable(iterator, StringCodec.Decode);
            request.InMemoryFormat = StringCodec.Decode(iterator);
            request.SplitBrainProtectionName = CodecUtil.DecodeNullable(iterator, StringCodec.Decode);
            request.MergePolicy = CodecUtil.DecodeNullable(iterator, StringCodec.Decode);
            request.PartitionLostListenerConfigs = ListMultiFrameCodec.DecodeNullable(iterator, ListenerConfigHolderCodec.Decode);
            request.ExpiryPolicyFactoryClassName = CodecUtil.DecodeNullable(iterator, StringCodec.Decode);
            request.TimedExpiryPolicyFactoryConfig = CodecUtil.DecodeNullable(iterator, TimedExpiryPolicyFactoryConfigCodec.Decode);
            request.CacheEntryListeners = ListMultiFrameCodec.DecodeNullable(iterator, CacheSimpleEntryListenerConfigCodec.Decode);
            request.EvictionConfig = CodecUtil.DecodeNullable(iterator, EvictionConfigHolderCodec.Decode);
            request.WanReplicationRef = CodecUtil.DecodeNullable(iterator, WanReplicationRefCodec.Decode);
            request.EventJournalConfig = CodecUtil.DecodeNullable(iterator, EventJournalConfigCodec.Decode);
            request.HotRestartConfig = CodecUtil.DecodeNullable(iterator, HotRestartConfigCodec.Decode);
            if (iterator.Current?.Next != null)
            {
                request.MerkleTreeConfig = CodecUtil.DecodeNullable(iterator, MerkleTreeConfigCodec.Decode);
                request.IsMerkleTreeConfigExists = true;
            }
            else request.IsMerkleTreeConfigExists = false;
            if (iterator.Current?.Next != null)
            {
                request.DataPersistenceConfig = DataPersistenceConfigCodec.Decode(iterator);
                request.IsDataPersistenceConfigExists = true;
            }
            else request.IsDataPersistenceConfigExists = false;
            if (iterator.Current?.Next != null)
            {
                request.UserCodeNamespace = CodecUtil.DecodeNullable(iterator, StringCodec.Decode);
                request.IsUserCodeNamespaceExists = true;
            }
            else request.IsUserCodeNamespaceExists = false;
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
