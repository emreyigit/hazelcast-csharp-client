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
    internal static class RingbufferStoreConfigHolderCodec
    {
        private const int EnabledFieldOffset = 0;
        private const int InitialFrameSize = EnabledFieldOffset + BytesExtensions.SizeOfBool;

        public static void Encode(ClientMessage clientMessage, Hazelcast.Protocol.Models.RingbufferStoreConfigHolder ringbufferStoreConfigHolder)
        {
            clientMessage.Append(Frame.CreateBeginStruct());

            var initialFrame = new Frame(new byte[InitialFrameSize]);
            initialFrame.Bytes.WriteBoolL(EnabledFieldOffset, ringbufferStoreConfigHolder.IsEnabled);
            clientMessage.Append(initialFrame);

            CodecUtil.EncodeNullable(clientMessage, ringbufferStoreConfigHolder.ClassName, StringCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, ringbufferStoreConfigHolder.FactoryClassName, StringCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, ringbufferStoreConfigHolder.Implementation, DataCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, ringbufferStoreConfigHolder.FactoryImplementation, DataCodec.Encode);
            MapCodec.EncodeNullable(clientMessage, ringbufferStoreConfigHolder.Properties, StringCodec.Encode, StringCodec.Encode);

            clientMessage.Append(Frame.CreateEndStruct());
        }

        public static Hazelcast.Protocol.Models.RingbufferStoreConfigHolder Decode(IEnumerator<Frame> iterator)
        {
            // begin frame
            iterator.Take();

            var initialFrame = iterator.Take();
            var enabled = initialFrame.Bytes.ReadBoolL(EnabledFieldOffset);

            var className = CodecUtil.DecodeNullable(iterator, StringCodec.Decode);
            var factoryClassName = CodecUtil.DecodeNullable(iterator, StringCodec.Decode);
            var implementation = CodecUtil.DecodeNullable(iterator, DataCodec.Decode);
            var factoryImplementation = CodecUtil.DecodeNullable(iterator, DataCodec.Decode);
            var properties = MapCodec.DecodeNullable(iterator, StringCodec.Decode, StringCodec.Decode);

            iterator.SkipToStructEnd();
            return new Hazelcast.Protocol.Models.RingbufferStoreConfigHolder(className, factoryClassName, implementation, factoryImplementation, properties, enabled);
        }
    }
}

#pragma warning restore IDE0051 // Remove unused private members
