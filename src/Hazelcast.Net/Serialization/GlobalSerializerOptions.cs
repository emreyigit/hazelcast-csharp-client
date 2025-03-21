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
using Hazelcast.Core;

namespace Hazelcast.Serialization
{
    /// <summary>
    /// Configures the global serializer.
    /// </summary>
    public class GlobalSerializerOptions : SingletonServiceFactory<ISerializer>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalSerializerOptions"/> class.
        /// </summary>
        public GlobalSerializerOptions()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalSerializerOptions"/> class.
        /// </summary>
        private GlobalSerializerOptions(GlobalSerializerOptions other, bool shallow)
            : base(other, shallow)
        {
            OverrideClrSerialization = other.OverrideClrSerialization;
        }

        /// <summary>
        /// Whether to fully override (and ignore) the native CLR serialization.
        /// </summary>
        public bool OverrideClrSerialization { get; set; }

        /// <summary>
        /// Clones the options.
        /// </summary>
        internal new GlobalSerializerOptions Clone(bool shallow = true) => new GlobalSerializerOptions(this, shallow);
    }
}
