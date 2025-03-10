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
    /// Adds a new map configuration to a running cluster.
    /// If a map configuration with the given {@code name} already exists, then
    /// the new configuration is ignored and the existing one is preserved.
    ///</summary>
#if SERVER_CODEC
    internal static class DynamicConfigAddMapConfigServerCodec
#else
    internal static class DynamicConfigAddMapConfigCodec
#endif
    {
        public const int RequestMessageType = 1772544; // 0x1B0C00
        public const int ResponseMessageType = 1772545; // 0x1B0C01
        private const int RequestBackupCountFieldOffset = Messaging.FrameFields.Offset.PartitionId + BytesExtensions.SizeOfInt;
        private const int RequestAsyncBackupCountFieldOffset = RequestBackupCountFieldOffset + BytesExtensions.SizeOfInt;
        private const int RequestTimeToLiveSecondsFieldOffset = RequestAsyncBackupCountFieldOffset + BytesExtensions.SizeOfInt;
        private const int RequestMaxIdleSecondsFieldOffset = RequestTimeToLiveSecondsFieldOffset + BytesExtensions.SizeOfInt;
        private const int RequestReadBackupDataFieldOffset = RequestMaxIdleSecondsFieldOffset + BytesExtensions.SizeOfInt;
        private const int RequestMergeBatchSizeFieldOffset = RequestReadBackupDataFieldOffset + BytesExtensions.SizeOfBool;
        private const int RequestStatisticsEnabledFieldOffset = RequestMergeBatchSizeFieldOffset + BytesExtensions.SizeOfInt;
        private const int RequestMetadataPolicyFieldOffset = RequestStatisticsEnabledFieldOffset + BytesExtensions.SizeOfBool;
        private const int RequestPerEntryStatsEnabledFieldOffset = RequestMetadataPolicyFieldOffset + BytesExtensions.SizeOfInt;
        private const int RequestInitialFrameSize = RequestPerEntryStatsEnabledFieldOffset + BytesExtensions.SizeOfBool;
        private const int ResponseInitialFrameSize = Messaging.FrameFields.Offset.ResponseBackupAcks + BytesExtensions.SizeOfByte;

#if SERVER_CODEC
        public sealed class RequestParameters
        {

            /// <summary>
            /// name of the map
            ///</summary>
            public string Name { get; set; }

            /// <summary>
            /// number of synchronous backups
            ///</summary>
            public int BackupCount { get; set; }

            /// <summary>
            /// number of asynchronous backups
            ///</summary>
            public int AsyncBackupCount { get; set; }

            /// <summary>
            /// maximum number of seconds for each entry to stay in the map.
            ///</summary>
            public int TimeToLiveSeconds { get; set; }

            /// <summary>
            /// maximum number of seconds for each entry to stay idle in the map
            ///</summary>
            public int MaxIdleSeconds { get; set; }

            /// <summary>
            /// map eviction configuration
            ///</summary>
            public Hazelcast.Protocol.Models.EvictionConfigHolder EvictionConfig { get; set; }

            /// <summary>
            /// {@code true} to enable reading local backup entries, {@code false} otherwise
            ///</summary>
            public bool ReadBackupData { get; set; }

            /// <summary>
            /// control caching of de-serialized values. Valid values are {@code NEVER}
            /// (Never cache de-serialized object), {@code INDEX_ONLY} (Cache values only
            /// when they are inserted into an index) and {@code ALWAYS} (Always cache
            /// de-serialized values
            ///</summary>
            public string CacheDeserializedValues { get; set; }

            /// <summary>
            /// Name of a class implementing SplitBrainMergePolicy that handles merging of values for this cache
            /// while recovering from network partitioning.
            ///</summary>
            public string MergePolicy { get; set; }

            /// <summary>
            /// Number of entries to be sent in a merge operation
            ///</summary>
            public int MergeBatchSize { get; set; }

            /// <summary>
            /// data type used to store entries. Valid values are {@code BINARY},
            /// {@code OBJECT} and {@code NATIVE}.
            ///</summary>
            public string InMemoryFormat { get; set; }

            /// <summary>
            /// entry listener configurations
            ///</summary>
            public ICollection<Hazelcast.Protocol.Models.ListenerConfigHolder> ListenerConfigs { get; set; }

            /// <summary>
            /// partition lost listener configurations
            ///</summary>
            public ICollection<Hazelcast.Protocol.Models.ListenerConfigHolder> PartitionLostListenerConfigs { get; set; }

            /// <summary>
            /// {@code true} to enable gathering of statistics, otherwise {@code false}
            ///</summary>
            public bool StatisticsEnabled { get; set; }

            /// <summary>
            /// name of an existing configured split brain protection to be used to determine the minimum
            /// number of members required in the cluster for the map to remain functional.
            /// When {@code null}, split brain protection does not apply to this map's operations.
            ///</summary>
            public string SplitBrainProtectionName { get; set; }

            /// <summary>
            /// configuration of backing map store or {@code null} for none
            ///</summary>
            public Hazelcast.Protocol.Models.MapStoreConfigHolder MapStoreConfig { get; set; }

            /// <summary>
            /// configuration of near cache or {@code null} for none
            ///</summary>
            public Hazelcast.Protocol.Models.NearCacheConfigHolder NearCacheConfig { get; set; }

            /// <summary>
            /// reference to an existing WAN replication configuration
            ///</summary>
            public Hazelcast.Models.WanReplicationRef WanReplicationRef { get; set; }

            /// <summary>
            /// index configurations
            ///</summary>
            public IList<Hazelcast.Models.IndexOptions> IndexConfigs { get; set; }

            /// <summary>
            /// map attributes
            ///</summary>
            public ICollection<Hazelcast.Models.AttributeOptions> AttributeConfigs { get; set; }

            /// <summary>
            /// configurations for query caches on this map
            ///</summary>
            public ICollection<Hazelcast.Protocol.Models.QueryCacheConfigHolder> QueryCacheConfigs { get; set; }

            /// <summary>
            /// name of class implementing {@code com.hazelcast.core.PartitioningStrategy}
            /// or {@code null}
            ///</summary>
            public string PartitioningStrategyClassName { get; set; }

            /// <summary>
            /// a serialized instance of a partitioning strategy
            ///</summary>
            public IData PartitioningStrategyImplementation { get; set; }

            /// <summary>
            /// hot restart configuration
            ///</summary>
            public Hazelcast.Models.HotRestartOptions HotRestartConfig { get; set; }

            /// <summary>
            /// event journal configuration
            ///</summary>
            public Hazelcast.Models.EventJournalOptions EventJournalConfig { get; set; }

            /// <summary>
            /// merkle tree configuration
            ///</summary>
            public Hazelcast.Models.MerkleTreeOptions MerkleTreeConfig { get; set; }

            /// <summary>
            /// metadata policy configuration for the supported data types. Valid values
            /// are {@code CREATE_ON_UPDATE} and {@code OFF}
            ///</summary>
            public int MetadataPolicy { get; set; }

            /// <summary>
            /// {@code true} to enable entry level statistics for the entries of this map.
            ///  otherwise {@code false}. Default value is {@code false}
            ///</summary>
            public bool PerEntryStatsEnabled { get; set; }

            /// <summary>
            /// Data persistence configuration
            ///</summary>
            public Hazelcast.Models.DataPersistenceOptions DataPersistenceConfig { get; set; }

            /// <summary>
            /// Tiered-Store configuration
            ///</summary>
            public Hazelcast.Models.TieredStoreOptions TieredStoreConfig { get; set; }

            /// <summary>
            /// List of attributes used for creating AttributePartitioningStrategy.
            ///</summary>
            public ICollection<Hazelcast.Models.PartitioningAttributeOptions> PartitioningAttributeConfigs { get; set; }

            /// <summary>
            /// Name of the User Code Namespace applied to this instance.
            ///</summary>
            public string UserCodeNamespace { get; set; }

            /// <summary>
            /// <c>true</c> if the perEntryStatsEnabled is received from the client, <c>false</c> otherwise.
            /// If this is false, perEntryStatsEnabled has the default value for its type.
            /// </summary>
            public bool IsPerEntryStatsEnabledExists { get; set; }

            /// <summary>
            /// <c>true</c> if the dataPersistenceConfig is received from the client, <c>false</c> otherwise.
            /// If this is false, dataPersistenceConfig has the default value for its type.
            /// </summary>
            public bool IsDataPersistenceConfigExists { get; set; }

            /// <summary>
            /// <c>true</c> if the tieredStoreConfig is received from the client, <c>false</c> otherwise.
            /// If this is false, tieredStoreConfig has the default value for its type.
            /// </summary>
            public bool IsTieredStoreConfigExists { get; set; }

            /// <summary>
            /// <c>true</c> if the partitioningAttributeConfigs is received from the client, <c>false</c> otherwise.
            /// If this is false, partitioningAttributeConfigs has the default value for its type.
            /// </summary>
            public bool IsPartitioningAttributeConfigsExists { get; set; }

            /// <summary>
            /// <c>true</c> if the userCodeNamespace is received from the client, <c>false</c> otherwise.
            /// If this is false, userCodeNamespace has the default value for its type.
            /// </summary>
            public bool IsUserCodeNamespaceExists { get; set; }
        }
#endif

        public static ClientMessage EncodeRequest(string name, int backupCount, int asyncBackupCount, int timeToLiveSeconds, int maxIdleSeconds, Hazelcast.Protocol.Models.EvictionConfigHolder evictionConfig, bool readBackupData, string cacheDeserializedValues, string mergePolicy, int mergeBatchSize, string inMemoryFormat, ICollection<Hazelcast.Protocol.Models.ListenerConfigHolder> listenerConfigs, ICollection<Hazelcast.Protocol.Models.ListenerConfigHolder> partitionLostListenerConfigs, bool statisticsEnabled, string splitBrainProtectionName, Hazelcast.Protocol.Models.MapStoreConfigHolder mapStoreConfig, Hazelcast.Protocol.Models.NearCacheConfigHolder nearCacheConfig, Hazelcast.Models.WanReplicationRef wanReplicationRef, ICollection<Hazelcast.Models.IndexOptions> indexConfigs, ICollection<Hazelcast.Models.AttributeOptions> attributeConfigs, ICollection<Hazelcast.Protocol.Models.QueryCacheConfigHolder> queryCacheConfigs, string partitioningStrategyClassName, IData partitioningStrategyImplementation, Hazelcast.Models.HotRestartOptions hotRestartConfig, Hazelcast.Models.EventJournalOptions eventJournalConfig, Hazelcast.Models.MerkleTreeOptions merkleTreeConfig, int metadataPolicy, bool perEntryStatsEnabled, Hazelcast.Models.DataPersistenceOptions dataPersistenceConfig, Hazelcast.Models.TieredStoreOptions tieredStoreConfig, ICollection<Hazelcast.Models.PartitioningAttributeOptions> partitioningAttributeConfigs, string userCodeNamespace)
        {
            var clientMessage = new ClientMessage
            {
                IsRetryable = false,
                OperationName = "DynamicConfig.AddMapConfig"
            };
            var initialFrame = new Frame(new byte[RequestInitialFrameSize], (FrameFlags) ClientMessageFlags.Unfragmented);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.MessageType, RequestMessageType);
            initialFrame.Bytes.WriteIntL(Messaging.FrameFields.Offset.PartitionId, -1);
            initialFrame.Bytes.WriteIntL(RequestBackupCountFieldOffset, backupCount);
            initialFrame.Bytes.WriteIntL(RequestAsyncBackupCountFieldOffset, asyncBackupCount);
            initialFrame.Bytes.WriteIntL(RequestTimeToLiveSecondsFieldOffset, timeToLiveSeconds);
            initialFrame.Bytes.WriteIntL(RequestMaxIdleSecondsFieldOffset, maxIdleSeconds);
            initialFrame.Bytes.WriteBoolL(RequestReadBackupDataFieldOffset, readBackupData);
            initialFrame.Bytes.WriteIntL(RequestMergeBatchSizeFieldOffset, mergeBatchSize);
            initialFrame.Bytes.WriteBoolL(RequestStatisticsEnabledFieldOffset, statisticsEnabled);
            initialFrame.Bytes.WriteIntL(RequestMetadataPolicyFieldOffset, metadataPolicy);
            initialFrame.Bytes.WriteBoolL(RequestPerEntryStatsEnabledFieldOffset, perEntryStatsEnabled);
            clientMessage.Append(initialFrame);
            StringCodec.Encode(clientMessage, name);
            CodecUtil.EncodeNullable(clientMessage, evictionConfig, EvictionConfigHolderCodec.Encode);
            StringCodec.Encode(clientMessage, cacheDeserializedValues);
            StringCodec.Encode(clientMessage, mergePolicy);
            StringCodec.Encode(clientMessage, inMemoryFormat);
            ListMultiFrameCodec.EncodeNullable(clientMessage, listenerConfigs, ListenerConfigHolderCodec.Encode);
            ListMultiFrameCodec.EncodeNullable(clientMessage, partitionLostListenerConfigs, ListenerConfigHolderCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, splitBrainProtectionName, StringCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, mapStoreConfig, MapStoreConfigHolderCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, nearCacheConfig, NearCacheConfigHolderCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, wanReplicationRef, WanReplicationRefCodec.Encode);
            ListMultiFrameCodec.EncodeNullable(clientMessage, indexConfigs, IndexConfigCodec.Encode);
            ListMultiFrameCodec.EncodeNullable(clientMessage, attributeConfigs, AttributeConfigCodec.Encode);
            ListMultiFrameCodec.EncodeNullable(clientMessage, queryCacheConfigs, QueryCacheConfigHolderCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, partitioningStrategyClassName, StringCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, partitioningStrategyImplementation, DataCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, hotRestartConfig, HotRestartConfigCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, eventJournalConfig, EventJournalConfigCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, merkleTreeConfig, MerkleTreeConfigCodec.Encode);
            DataPersistenceConfigCodec.Encode(clientMessage, dataPersistenceConfig);
            TieredStoreConfigCodec.Encode(clientMessage, tieredStoreConfig);
            ListMultiFrameCodec.EncodeNullable(clientMessage, partitioningAttributeConfigs, PartitioningAttributeConfigCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, userCodeNamespace, StringCodec.Encode);
            return clientMessage;
        }

#if SERVER_CODEC
        public static RequestParameters DecodeRequest(ClientMessage clientMessage)
        {
            using var iterator = clientMessage.GetEnumerator();
            var request = new RequestParameters();
            var initialFrame = iterator.Take();
            request.BackupCount = initialFrame.Bytes.ReadIntL(RequestBackupCountFieldOffset);
            request.AsyncBackupCount = initialFrame.Bytes.ReadIntL(RequestAsyncBackupCountFieldOffset);
            request.TimeToLiveSeconds = initialFrame.Bytes.ReadIntL(RequestTimeToLiveSecondsFieldOffset);
            request.MaxIdleSeconds = initialFrame.Bytes.ReadIntL(RequestMaxIdleSecondsFieldOffset);
            request.ReadBackupData = initialFrame.Bytes.ReadBoolL(RequestReadBackupDataFieldOffset);
            request.MergeBatchSize = initialFrame.Bytes.ReadIntL(RequestMergeBatchSizeFieldOffset);
            request.StatisticsEnabled = initialFrame.Bytes.ReadBoolL(RequestStatisticsEnabledFieldOffset);
            request.MetadataPolicy = initialFrame.Bytes.ReadIntL(RequestMetadataPolicyFieldOffset);
            if (initialFrame.Bytes.Length >= RequestPerEntryStatsEnabledFieldOffset + BytesExtensions.SizeOfBool)
            {
                request.PerEntryStatsEnabled = initialFrame.Bytes.ReadBoolL(RequestPerEntryStatsEnabledFieldOffset);
                request.IsPerEntryStatsEnabledExists = true;
            }
            else request.IsPerEntryStatsEnabledExists = false;
            request.Name = StringCodec.Decode(iterator);
            request.EvictionConfig = CodecUtil.DecodeNullable(iterator, EvictionConfigHolderCodec.Decode);
            request.CacheDeserializedValues = StringCodec.Decode(iterator);
            request.MergePolicy = StringCodec.Decode(iterator);
            request.InMemoryFormat = StringCodec.Decode(iterator);
            request.ListenerConfigs = ListMultiFrameCodec.DecodeNullable(iterator, ListenerConfigHolderCodec.Decode);
            request.PartitionLostListenerConfigs = ListMultiFrameCodec.DecodeNullable(iterator, ListenerConfigHolderCodec.Decode);
            request.SplitBrainProtectionName = CodecUtil.DecodeNullable(iterator, StringCodec.Decode);
            request.MapStoreConfig = CodecUtil.DecodeNullable(iterator, MapStoreConfigHolderCodec.Decode);
            request.NearCacheConfig = CodecUtil.DecodeNullable(iterator, NearCacheConfigHolderCodec.Decode);
            request.WanReplicationRef = CodecUtil.DecodeNullable(iterator, WanReplicationRefCodec.Decode);
            request.IndexConfigs = ListMultiFrameCodec.DecodeNullable(iterator, IndexConfigCodec.Decode);
            request.AttributeConfigs = ListMultiFrameCodec.DecodeNullable(iterator, AttributeConfigCodec.Decode);
            request.QueryCacheConfigs = ListMultiFrameCodec.DecodeNullable(iterator, QueryCacheConfigHolderCodec.Decode);
            request.PartitioningStrategyClassName = CodecUtil.DecodeNullable(iterator, StringCodec.Decode);
            request.PartitioningStrategyImplementation = CodecUtil.DecodeNullable(iterator, DataCodec.Decode);
            request.HotRestartConfig = CodecUtil.DecodeNullable(iterator, HotRestartConfigCodec.Decode);
            request.EventJournalConfig = CodecUtil.DecodeNullable(iterator, EventJournalConfigCodec.Decode);
            request.MerkleTreeConfig = CodecUtil.DecodeNullable(iterator, MerkleTreeConfigCodec.Decode);
            if (iterator.Current?.Next != null)
            {
                request.DataPersistenceConfig = DataPersistenceConfigCodec.Decode(iterator);
                request.IsDataPersistenceConfigExists = true;
            }
            else request.IsDataPersistenceConfigExists = false;
            if (iterator.Current?.Next != null)
            {
                request.TieredStoreConfig = TieredStoreConfigCodec.Decode(iterator);
                request.IsTieredStoreConfigExists = true;
            }
            else request.IsTieredStoreConfigExists = false;
            if (iterator.Current?.Next != null)
            {
                request.PartitioningAttributeConfigs = ListMultiFrameCodec.DecodeNullable(iterator, PartitioningAttributeConfigCodec.Decode);
                request.IsPartitioningAttributeConfigsExists = true;
            }
            else request.IsPartitioningAttributeConfigsExists = false;
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
