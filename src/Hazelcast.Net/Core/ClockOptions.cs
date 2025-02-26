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
namespace Hazelcast.Core
{
    /// <summary>
    /// Represents the clock options.
    /// </summary>
    internal sealed class ClockOptions
    {
        /// <summary>
        /// Gets or sets the clock offset.
        /// </summary>
        public long OffsetMilliseconds { get; set; }

        /// <summary>
        /// Clones the options.
        /// </summary>
        internal ClockOptions Clone()
        {
            return new ClockOptions
            {
                OffsetMilliseconds = OffsetMilliseconds
            };
        }
    }
}
