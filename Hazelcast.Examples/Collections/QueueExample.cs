﻿// Copyright (c) 2008-2020, Hazelcast, Inc. All Rights Reserved.
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
using System.Threading.Tasks;
using Hazelcast.Client;
using Hazelcast.Config;

namespace Hazelcast.Examples.Collections
{
    internal class QueueExample
    {
        private static void Run(string[] args)
        {
            Environment.SetEnvironmentVariable("hazelcast.logging.level", "info");
            Environment.SetEnvironmentVariable("hazelcast.logging.type", "console");

            var config = new ClientConfig();
            config.GetNetworkConfig().AddAddress("127.0.0.1");
            var client = HazelcastClient.NewHazelcastClient(config);

            var queue = client.GetQueue<string>("queue-example");

            var producer = Task.Factory.StartNew(() =>
            {
                for (var i = 0; i < 100; i++)
                {
                    queue.Offer("value " + i);
                }
            });

            var consumer = Task.Factory.StartNew(() =>
            {
                var nConsumed = 0;
                string e;
                while (nConsumed++ < 100 && (e = queue.Take()) != null)
                {
                    Console.WriteLine("Consuming " + e);
                }
            });

            Task.WaitAll(producer, consumer);
            queue.Destroy();
            client.Shutdown();
        }
    }
}