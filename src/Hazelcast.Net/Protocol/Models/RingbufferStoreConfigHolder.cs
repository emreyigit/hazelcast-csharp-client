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
using System;
using System.Collections.Generic;
using Hazelcast.Models;
using Hazelcast.Serialization;

namespace Hazelcast.Protocol.Models;

internal class RingbufferStoreConfigHolder
{
    public RingbufferStoreConfigHolder(string className, string factoryClassName, IData implementation,
                                       IData factoryImplementation, Dictionary<string, string> properties,
                                       bool enabled)
    {
        ClassName = className;
        FactoryClassName = factoryClassName;
        Implementation = implementation;
        FactoryImplementation = factoryImplementation;
        Properties = properties;
        IsEnabled = enabled;
    }

    public string ClassName { get; }

    public string FactoryClassName { get; }

    public IData Implementation { get; }

    public IData FactoryImplementation { get; }

    public Dictionary<string, string> Properties { get; }

    public bool IsEnabled { get; }

    public static RingbufferStoreConfigHolder Of(RingbufferStoreOptions options)
    {
        if (options.ClassName == null && 
            options.FactoryClassName == null && 
            options.Enabled)
        {
            throw new ArgumentException("Either ClassName or FactoryClassName has to be non-null.", nameof(options));
        }

        return new RingbufferStoreConfigHolder(
            options.ClassName,
            options.FactoryClassName,
            null, null, // no implementations
            options.Properties, 
            options.Enabled);
    }
}
