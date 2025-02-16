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
using System.Collections.Generic;
using Hazelcast.Protocol.BuiltInCodecs;
using Hazelcast.Protocol.CustomCodecs;
using Hazelcast.Core;
using Hazelcast.Messaging;
using Hazelcast.Clustering;
using Hazelcast.Serialization;
using Microsoft.Extensions.Logging;

namespace Hazelcast.Protocol.CustomCodecs
{
    internal static class NearCacheConfigHolderCodec
    {
        private const int SerializeKeysFieldOffset = 0;
        private const int InvalidateOnChangeFieldOffset = SerializeKeysFieldOffset + BytesExtensions.SizeOfBool;
        private const int TimeToLiveSecondsFieldOffset = InvalidateOnChangeFieldOffset + BytesExtensions.SizeOfBool;
        private const int MaxIdleSecondsFieldOffset = TimeToLiveSecondsFieldOffset + BytesExtensions.SizeOfInt;
        private const int CacheLocalEntriesFieldOffset = MaxIdleSecondsFieldOffset + BytesExtensions.SizeOfInt;
        private const int InitialFrameSize = CacheLocalEntriesFieldOffset + BytesExtensions.SizeOfBool;

        public static void Encode(ClientMessage clientMessage, Hazelcast.Protocol.Models.NearCacheConfigHolder nearCacheConfigHolder)
        {
            clientMessage.Append(Frame.CreateBeginStruct());

            var initialFrame = new Frame(new byte[InitialFrameSize]);
            initialFrame.Bytes.WriteBoolL(SerializeKeysFieldOffset, nearCacheConfigHolder.IsSerializeKeys);
            initialFrame.Bytes.WriteBoolL(InvalidateOnChangeFieldOffset, nearCacheConfigHolder.IsInvalidateOnChange);
            initialFrame.Bytes.WriteIntL(TimeToLiveSecondsFieldOffset, nearCacheConfigHolder.TimeToLiveSeconds);
            initialFrame.Bytes.WriteIntL(MaxIdleSecondsFieldOffset, nearCacheConfigHolder.MaxIdleSeconds);
            initialFrame.Bytes.WriteBoolL(CacheLocalEntriesFieldOffset, nearCacheConfigHolder.IsCacheLocalEntries);
            clientMessage.Append(initialFrame);

            StringCodec.Encode(clientMessage, nearCacheConfigHolder.Name);
            StringCodec.Encode(clientMessage, nearCacheConfigHolder.InMemoryFormat);
            EvictionConfigHolderCodec.Encode(clientMessage, nearCacheConfigHolder.EvictionConfigHolder);
            StringCodec.Encode(clientMessage, nearCacheConfigHolder.LocalUpdatePolicy);
            CodecUtil.EncodeNullable(clientMessage, nearCacheConfigHolder.PreloaderConfig, NearCachePreloaderConfigCodec.Encode);

            clientMessage.Append(Frame.CreateEndStruct());
        }

        public static Hazelcast.Protocol.Models.NearCacheConfigHolder Decode(IEnumerator<Frame> iterator)
        {
            // begin frame
            iterator.Take();

            var initialFrame = iterator.Take();
            var serializeKeys = initialFrame.Bytes.ReadBoolL(SerializeKeysFieldOffset);

            var invalidateOnChange = initialFrame.Bytes.ReadBoolL(InvalidateOnChangeFieldOffset);
            var timeToLiveSeconds = initialFrame.Bytes.ReadIntL(TimeToLiveSecondsFieldOffset);
            var maxIdleSeconds = initialFrame.Bytes.ReadIntL(MaxIdleSecondsFieldOffset);
            var cacheLocalEntries = initialFrame.Bytes.ReadBoolL(CacheLocalEntriesFieldOffset);
            var name = StringCodec.Decode(iterator);
            var inMemoryFormat = StringCodec.Decode(iterator);
            var evictionConfigHolder = EvictionConfigHolderCodec.Decode(iterator);
            var localUpdatePolicy = StringCodec.Decode(iterator);
            var preloaderConfig = CodecUtil.DecodeNullable(iterator, NearCachePreloaderConfigCodec.Decode);

            iterator.SkipToStructEnd();
            return new Hazelcast.Protocol.Models.NearCacheConfigHolder(name, inMemoryFormat, serializeKeys, invalidateOnChange, timeToLiveSeconds, maxIdleSeconds, evictionConfigHolder, cacheLocalEntries, localUpdatePolicy, preloaderConfig);
        }
    }
}

#pragma warning restore IDE0051 // Remove unused private members
