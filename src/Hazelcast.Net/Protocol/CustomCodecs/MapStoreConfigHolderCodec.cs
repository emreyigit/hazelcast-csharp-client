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
    internal static class MapStoreConfigHolderCodec
    {
        private const int EnabledFieldOffset = 0;
        private const int WriteCoalescingFieldOffset = EnabledFieldOffset + BytesExtensions.SizeOfBool;
        private const int WriteDelaySecondsFieldOffset = WriteCoalescingFieldOffset + BytesExtensions.SizeOfBool;
        private const int WriteBatchSizeFieldOffset = WriteDelaySecondsFieldOffset + BytesExtensions.SizeOfInt;
        private const int OffloadFieldOffset = WriteBatchSizeFieldOffset + BytesExtensions.SizeOfInt;
        private const int InitialFrameSize = OffloadFieldOffset + BytesExtensions.SizeOfBool;

        public static void Encode(ClientMessage clientMessage, Hazelcast.Protocol.Models.MapStoreConfigHolder mapStoreConfigHolder)
        {
            clientMessage.Append(Frame.CreateBeginStruct());

            var initialFrame = new Frame(new byte[InitialFrameSize]);
            initialFrame.Bytes.WriteBoolL(EnabledFieldOffset, mapStoreConfigHolder.IsEnabled);
            initialFrame.Bytes.WriteBoolL(WriteCoalescingFieldOffset, mapStoreConfigHolder.IsWriteCoalescing);
            initialFrame.Bytes.WriteIntL(WriteDelaySecondsFieldOffset, mapStoreConfigHolder.WriteDelaySeconds);
            initialFrame.Bytes.WriteIntL(WriteBatchSizeFieldOffset, mapStoreConfigHolder.WriteBatchSize);
            initialFrame.Bytes.WriteBoolL(OffloadFieldOffset, mapStoreConfigHolder.IsOffload);
            clientMessage.Append(initialFrame);

            CodecUtil.EncodeNullable(clientMessage, mapStoreConfigHolder.ClassName, StringCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, mapStoreConfigHolder.Implementation, DataCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, mapStoreConfigHolder.FactoryClassName, StringCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, mapStoreConfigHolder.FactoryImplementation, DataCodec.Encode);
            MapCodec.EncodeNullable(clientMessage, mapStoreConfigHolder.Properties, StringCodec.Encode, StringCodec.Encode);
            StringCodec.Encode(clientMessage, mapStoreConfigHolder.InitialLoadMode);

            clientMessage.Append(Frame.CreateEndStruct());
        }

        public static Hazelcast.Protocol.Models.MapStoreConfigHolder Decode(IEnumerator<Frame> iterator)
        {
            // begin frame
            iterator.Take();

            var initialFrame = iterator.Take();
            var enabled = initialFrame.Bytes.ReadBoolL(EnabledFieldOffset);

            var writeCoalescing = initialFrame.Bytes.ReadBoolL(WriteCoalescingFieldOffset);
            var writeDelaySeconds = initialFrame.Bytes.ReadIntL(WriteDelaySecondsFieldOffset);
            var writeBatchSize = initialFrame.Bytes.ReadIntL(WriteBatchSizeFieldOffset);
            var isOffloadExists = false;
            bool offload = default;
            if (initialFrame.Bytes.Length >= OffloadFieldOffset + BytesExtensions.SizeOfBool)
            {
                offload = initialFrame.Bytes.ReadBoolL(OffloadFieldOffset);
                isOffloadExists = true;
            }
            var className = CodecUtil.DecodeNullable(iterator, StringCodec.Decode);
            var implementation = CodecUtil.DecodeNullable(iterator, DataCodec.Decode);
            var factoryClassName = CodecUtil.DecodeNullable(iterator, StringCodec.Decode);
            var factoryImplementation = CodecUtil.DecodeNullable(iterator, DataCodec.Decode);
            var properties = MapCodec.DecodeNullable(iterator, StringCodec.Decode, StringCodec.Decode);
            var initialLoadMode = StringCodec.Decode(iterator);

            iterator.SkipToStructEnd();
            return new Hazelcast.Protocol.Models.MapStoreConfigHolder(enabled, writeCoalescing, writeDelaySeconds, writeBatchSize, className, implementation, factoryClassName, factoryImplementation, properties, initialLoadMode, isOffloadExists, offload);
        }
    }
}

#pragma warning restore IDE0051 // Remove unused private members
