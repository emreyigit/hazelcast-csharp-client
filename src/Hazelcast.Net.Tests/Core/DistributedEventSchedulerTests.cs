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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hazelcast.Clustering;
using Hazelcast.Core;
using Hazelcast.Messaging;
using Hazelcast.Testing;
using Hazelcast.Testing.Logging;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Hazelcast.Tests.Core
{
    [TestFixture]
    public class DistributedEventSchedulerTests
    {
        [Test]
        public void TaskSchedulers()
        {
            // ThreadPoolTaskScheduler (by default)
            Console.WriteLine(TaskScheduler.Current);
            Console.WriteLine(TaskScheduler.Default);

            // nope
            Assert.Throws<InvalidOperationException>(() => Console.WriteLine(TaskScheduler.FromCurrentSynchronizationContext()));
        }

        [Test]
        public void ThreadPoolQueueOrder()
        {
            // this test produces output such as "1 7 8 9 2 4 5 3 6 0" clearly showing
            // that the default thread pool queue is not ordered

            for (var i = 0; i < 10; i++)
                ThreadPool.QueueUserWorkItem(Console.Write, i + " ");

            Thread.Sleep(1000);
        }

        [Test]
        public async Task ThreadPoolTaskSchedulerOrder()
        {
            // this test produces output such as "3 6 7 8 9 4 1 5 0 2" clearly showing
            // that the default task scheduler, which is ThreadPoolTaskScheduler, and
            // relies on the thread pool queue, does not order tasks either

            var tasks = new List<Task>();
            for (var i = 0; i < 10; i++)
            {
                var capture = i;
                tasks.Add(Task.Run(() => Console.Write(capture + " ")));
            }
            await Task.WhenAll(tasks);
        }

        [Test]
        public async Task SchedulerTest()
        {
            // this test verifies that the scheduler works as expected

            HConsole.Configure(options => options
                .ConfigureDefaults(this)
                .Configure<DistributedEventScheduler>().SetMaxLevel());

            using var loggerFactory = LoggerFactory.Create(builder => builder.AddHConsole());
            var logger = loggerFactory.CreateLogger("TEST");
            var scheduler = new DistributedEventScheduler(loggerFactory);

            var pe = new ConcurrentDictionary<int, long>();

            var exceptionCount = 0;

            var concurrentCount = 0;
            var maxConcurentCount = 0;
            var concurrentLock = new object();

            scheduler.HandlerError += (sender, args) =>
            {
                Interlocked.Increment(ref exceptionCount);
                var message = "An event handler has thrown: " + args.Exception.Message;
                if (args.Message.CorrelationId < 25)
                {
                    args.Handled = true;
                    message += " (handling)";
                }
                HConsole.WriteLine(this, "WARN: " + message);
            };

            var s = new ClusterSubscription(async (clientMessage, state) =>
            {
                HConsole.WriteLine(this, $"Handling event for partition: {clientMessage.PartitionId}, sequence: {clientMessage.CorrelationId}");

                lock (pe)
                {
                    // for each partition, events trigger in the right order
                    if (pe.TryGetValue(clientMessage.PartitionId, out var pv))
                        Assert.That(clientMessage.CorrelationId, Is.GreaterThan(pv));
                    pe[clientMessage.PartitionId] = clientMessage.CorrelationId;
                }

                //await Task.Yield();
                lock (concurrentLock) concurrentCount++;
                await Task.Delay(200);
                lock (concurrentLock)
                {
                    maxConcurentCount = Math.Max(maxConcurentCount, concurrentCount);
                    concurrentCount--;
                }

                if (clientMessage.CorrelationId % 10 == 0) throw new Exception($"Throwing for partition: {clientMessage.PartitionId} sequence: {clientMessage.CorrelationId}");
                HConsole.WriteLine(this, $"Handled event for partition: {clientMessage.PartitionId} sequence: {clientMessage.CorrelationId}");
            });

            for (var i = 0; i < 50; i++)
            {
                var capture = i;
                var m = new ClientMessage(new Frame(new byte[64]))
                {
                    PartitionId = RandomProvider.Random.Next(4),
                    CorrelationId = capture,
                };

                // can add the events
                Assert.That(scheduler.Add(s, m), Is.True);
            }

            // make sure everything is completed
            // (disposing aborts all queues)
            await AssertEx.SucceedsEventually(() =>
            {
                Assert.That(scheduler.Count, Is.EqualTo(0));
            }, 8000, 200);

            // aborts the scheduler & all current queues!
            await scheduler.DisposeAsync();

            // the exceptions have triggered the OnError handler
            Assert.That(exceptionCount, Is.EqualTo(5));

            // the scheduler counts exceptions
            Assert.That(scheduler.ExceptionCount, Is.EqualTo(5));
            Assert.That(scheduler.UnhandledExceptionCount, Is.EqualTo(2));

            // cannot add events to disposed event scheduler
            Assert.That(scheduler.Add(s, new ClientMessage(new Frame(new byte[64]))), Is.False);

            // some events ran concurrently
            Console.WriteLine(maxConcurentCount);
            Assert.That(maxConcurentCount, Is.GreaterThan(1));

            // all tasks are gone
            Assert.That(scheduler.Count, Is.EqualTo(0));
        }
    }
}
