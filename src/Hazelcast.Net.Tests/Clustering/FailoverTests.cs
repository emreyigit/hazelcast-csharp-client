﻿// Copyright (c) 2008-2021, Hazelcast, Inc. All Rights Reserved.
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
using Hazelcast.Clustering;
using Hazelcast.Core;
using Hazelcast.DistributedObjects;
using Hazelcast.Partitioning;
using Hazelcast.Security;
using Hazelcast.Testing;
using Hazelcast.Testing.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Hazelcast.Exceptions;
using Hazelcast.Configuration;

namespace Hazelcast.Tests.Clustering
{
    [Category("enterprise")]
    [Timeout(120_000)]
    internal class FailoverTests : MultipleClusterRemoteTestBase
    {
        private IDisposable HConsoleForTest()

            => HConsole.Capture(options => options
                .ClearAll()
                .Configure().SetMinLevel()
                .Configure<HConsoleLoggerProvider>().SetMaxLevel()
                .Configure<Failover>().SetPrefix("FAILOVER").SetMaxLevel()
                .Configure<FailoverTests>().SetPrefix("TEST").SetMaxLevel()
            );

        private const string ClusterAName = "clusterA";
        private const string ClusterBName = "clusterB";

        private const string ClusterAKey = "keyForClusterA";
        private const string ClusterAData = "dataForClusterA";

        private const string ClusterBKey = "keyForClusterB";
        private const string ClusterBData = "dataForClusterB";

        private static ClusterState MockClusterState(HazelcastOptions options)
        {
            return new ClusterState(options, "clusterName", "clientName", Mock.Of<Partitioner>(), new NullLoggerFactory());
        }

        [Test]
        public async Task TestFailsWhenEnabledWithNoCluster()
        {
            var failoverOptions = new HazelcastFailoverOptionsBuilder().Build();

            await AssertEx.ThrowsAsync<ConfigurationException>(async () => await HazelcastClientFactory.StartNewFailoverClientAsync(failoverOptions));
        }


        [Test]
        public void TestClusterOptionsRotate()
        {
            const string clusterName1 = "first";
            const string address1 = "1.1.1.1";

            const string clusterName2 = "second";
            const string address2 = "2.2.2.2";
            const string username = "MARVIN";

            var clusterChangedCount = 0;

            var failoverOptions = new HazelcastFailoverOptionsBuilder()
                .With(fo =>
                {
                    fo.TryCount = 2;
                    fo.Clients.Add(new HazelcastOptionsBuilder()
                        .With(o =>
                        {
                            o.ClusterName = clusterName1;
                            o.Networking.Addresses.Add(address1);
                        })
                        .Build());
                    fo.Clients.Add(new HazelcastOptionsBuilder()
                        .With(o =>
                        {
                            o.ClusterName = clusterName2;
                            o.Networking.Addresses.Add(address2);
                            o.Authentication.ConfigureUsernamePasswordCredentials(username, "");
                        })
                        .Build());
                })
                .Build();

            failoverOptions.Enabled = true;

            var options = failoverOptions.Clients[0];
            options.FailoverOptions = failoverOptions;

            var clusterState = MockClusterState(options);

            var failover = new Failover(clusterState, options);

            failover.ClusterChanged += _ => 
            {
                clusterChangedCount++;
            };

            void AssertCluster1(Failover fo)
            {
                Assert.AreEqual(clusterName1, fo.CurrentClusterOptions.ClusterName);
                Assert.True(fo.CurrentClusterOptions.Networking.Addresses.Contains(address1));
            };

            void AssertCluster2(Failover fo)
            {
                Assert.AreEqual(clusterName2, fo.CurrentClusterOptions.ClusterName);
                Assert.True(fo.CurrentClusterOptions.Networking.Addresses.Contains(address2));
                Assert.False(fo.CurrentClusterOptions.Networking.Addresses.Contains(address1));
                Assert.AreEqual(fo.CurrentClusterOptions.Authentication.CredentialsFactory.Service.NewCredentials().Name, username);
            };

            // initial one must be cluster 1
            AssertCluster1(failover);
            clusterState.ChangeState(ClientState.Disconnected);

            var expectedCount = 1;

            // Loop 1, Try 1
            Assert.That(failover.TryNextCluster());
            AssertCluster2(failover);
            Assert.AreEqual(expectedCount, clusterChangedCount);
            Assert.AreEqual(expectedCount, failover.CurrentTryCount);
            expectedCount++;

            // Loop 1, Try 2
            Assert.That(failover.TryNextCluster());
            AssertCluster1(failover);
            Assert.AreEqual(expectedCount, clusterChangedCount);
            Assert.AreEqual(expectedCount, failover.CurrentTryCount);
            expectedCount++;

            // Loop 2, Try 1
            Assert.That(failover.TryNextCluster());
            AssertCluster2(failover);
            Assert.AreEqual(expectedCount, clusterChangedCount);
            Assert.AreEqual(expectedCount, failover.CurrentTryCount);
            expectedCount++;

            // Loop 2, Try 2
            Assert.That(failover.TryNextCluster());
            AssertCluster1(failover);
            Assert.AreEqual(expectedCount, clusterChangedCount);
            Assert.AreEqual(expectedCount, failover.CurrentTryCount);

            // Loop 3-> spent all tries
            Assert.False(failover.TryNextCluster());
        }

        [TestCase(true, 1)]
        [TestCase(false, 1)]
        public async Task TestClientCanFailover(bool useSmartConnection, int memberCount)
        {
            //var _ = HConsoleForTest();

            var numberOfStateChanged = 0;

            var failoverOptions = new HazelcastFailoverOptionsBuilder()
                 .With(fo =>
                 {
                     fo.TryCount = 2;

                     // first cluster is the primary, and able to config everything
                     fo.Clients.Add(new HazelcastOptionsBuilder()
                         .With(o =>
                         {
                             o.ClusterName = RcClusterPrimary.Id;
                             o.Networking.Addresses.Clear();
                             o.Networking.Addresses.Add("127.0.0.1:5701");
                             o.Networking.ReconnectMode = Hazelcast.Networking.ReconnectMode.ReconnectAsync;
                             o.Networking.ConnectionTimeoutMilliseconds = 10_000;
                             o.Networking.ConnectionRetry.ClusterConnectionTimeoutMilliseconds = 10_000;
                             o.Networking.SmartRouting = useSmartConnection;
                             o.AddSubscriber(events =>
                                 events.StateChanged((sender, arg) =>
                                 {
                                     HConsole.WriteLine(this, $"State Changed:{arg.State}");
                                     numberOfStateChanged++;
                                 }));
                         })
                         .Build());

                     // cannot override load balancer, retry, heartbeat etc. just network info
                     fo.Clients.Add(new HazelcastOptionsBuilder()
                         .With(o =>
                         {
                             o.ClusterName = RcClusterAlternative.Id;
                             o.Networking.Addresses.Add("127.0.0.1:5703");
                             o.Networking.SmartRouting = useSmartConnection;// that doesn't override to primary                     
                             o.Authentication.ConfigureUsernamePasswordCredentials("test", "1234");
                         })
                         .Build());
                 })
                 .WithHConsoleLogger()
                 .Build();

            //Actual testing

            HConsole.WriteLine(this, "Creating Members");
            var membersA = await StartMembersOn(RcClusterPrimary.Id, 1);
            var membersB = await StartMembersOn(RcClusterAlternative.Id, 1);

            var client = await HazelcastClientFactory.StartNewFailoverClientAsync(failoverOptions);
            var mapName = CreateUniqueName();
            var map = await client.GetMapAsync<string, string>(mapName);
            Assert.IsNotNull(map);

            // first cluster should be A
            await AssertClusterA(map, client.ClusterName);

            HConsole.WriteLine(this, $"SHUTDOWN: Members of Cluster A :{RcClusterPrimary.Id}");
            await KillMembersOnAsync(RcClusterPrimary.Id, membersA);

            //Now, we should failover to cluster B
            await AssertEx.SucceedsEventually(() => { Assert.AreEqual(ClientState.Connected, client.State); }, 60_000, 500);
            await AssertClusterB(map, client.ClusterName);

            // Start cluster A again
            HConsole.WriteLine(this, $"START: Members of Cluster A :{RcClusterPrimary.Id}");
            membersA = await StartMembersOn(RcClusterPrimary.Id, memberCount);

            // Kill B
            HConsole.WriteLine(this, $"SHUTDOWN: Members of Cluster B :{RcClusterAlternative.Id}");
            await KillMembersOnAsync(RcClusterAlternative.Id, membersB);

            await AssertEx.SucceedsEventually(() => { Assert.AreEqual(ClientState.Connected, client.State); }, 60_000, 500);
            await AssertClusterA(map, client.ClusterName);

            Assert.GreaterOrEqual(numberOfStateChanged, 8);
            /*
                Expected State Flow: Due to test environment, client can experience more state changes, it's ok.
                0 Starting
                1 Started
                2 Connected
                3 Disconnected                      
                4 Switched
                5 Connected
                6 Disconnected      
                7 Switched
                8 Connected
             */
            await KillMembersOnAsync(RcClusterPrimary.Id, membersA);
        }


        [TestCase(true, 1)]
        [TestCase(false, 1)]
        public async Task TestClientCanFailoverFirstClusterNotUp(bool useSmartConnection, int memberCount)
        {
            var _ = HConsoleForTest();

            var numberOfStateChanged = 0;

            var failoverOptions = new HazelcastFailoverOptionsBuilder()
                .With(fo =>
                {
                    fo.TryCount = 2;

                    // first cluster is the primary, and able to config everything
                    fo.Clients.Add(new HazelcastOptionsBuilder()
                        .With(o =>
                        {
                            o.ClusterName = RcClusterPrimary.Id;
                            o.Networking.Addresses.Clear();
                            o.Networking.Addresses.Add("127.0.0.1:5701");
                            o.Networking.ReconnectMode = Hazelcast.Networking.ReconnectMode.ReconnectAsync;
                            o.Networking.ConnectionTimeoutMilliseconds = 10_000;
                            o.Networking.ConnectionRetry.ClusterConnectionTimeoutMilliseconds = 10_000;
                            o.Networking.SmartRouting = useSmartConnection;
                            o.AddSubscriber(events =>
                                events.StateChanged((sender, arg) =>
                                {
                                    HConsole.WriteLine(this, $"State Changed:{arg.State}");
                                    numberOfStateChanged++;
                                }));
                        })
                        .Build());

                    // cannot override load balancer, retry, heartbeat etc. just network info
                    fo.Clients.Add(new HazelcastOptionsBuilder()
                        .With(o =>
                        {
                            o.ClusterName = RcClusterAlternative.Id;
                            o.Networking.Addresses.Add("127.0.0.1:5703");
                            o.Networking.SmartRouting = useSmartConnection;// that doesn't override to primary                    
                            o.Authentication.ConfigureUsernamePasswordCredentials("test", "1234");
                        })
                        .Build());
                })
                .WithHConsoleLogger()
                .Build();

            //Actual testing
            HConsole.WriteLine(this, "Creating Members");

            var membersB = await StartMembersOn(RcClusterAlternative.Id, 1);

            var client = await HazelcastClientFactory.StartNewFailoverClientAsync(failoverOptions);
            var mapName = CreateUniqueName();
            var map = await client.GetMapAsync<string, string>(mapName);
            Assert.IsNotNull(map);

            // first cluster should be B, A is not up
            await AssertEx.SucceedsEventually(() => { Assert.AreEqual(ClientState.Connected, client.State); }, 60_000, 500);
            await AssertClusterB(map, client.ClusterName);

            //Start A before switch
            HConsole.WriteLine(this, $"START: Members of Cluster A :{RcClusterPrimary.Id}");
            var membersA = await StartMembersOn(RcClusterPrimary.Id, 1);

            // Kill B
            HConsole.WriteLine(this, $"SHUTDOWN: Members of Cluster B :{RcClusterAlternative.Id}");
            await KillMembersOnAsync(RcClusterAlternative.Id, membersB);

            //We should be at A
            await AssertEx.SucceedsEventually(() => { Assert.AreEqual(ClientState.Connected, client.State); }, 60_000, 500);
            await AssertClusterA(map, client.ClusterName);

            // Start cluster B again
            HConsole.WriteLine(this, $"START: Members of Cluster B :{RcClusterAlternative.Id}");
            membersB = await StartMembersOn(RcClusterAlternative.Id, memberCount);

            // Kill A
            HConsole.WriteLine(this, $"SHUTDOWN: Members of Cluster A :{RcClusterPrimary.Id}");
            await KillMembersOnAsync(RcClusterPrimary.Id, membersA);

            //Now, we should failover to cluster B
            await AssertEx.SucceedsEventually(() => { Assert.AreEqual(ClientState.Connected, client.State); }, 60_000, 500);
            await AssertClusterB(map, client.ClusterName);

            Assert.GreaterOrEqual(numberOfStateChanged, 8);
            /*
                Expected State Flow: Due to test environment, client can experience more state changes, it's ok.
                0 Starting
                1 Started
                2 Connected
                3 Disconnected                      
                4 Switched
                5 Connected
                6 Disconnected      
                7 Switched
                8 Connected
             */

            await KillMembersOnAsync(RcClusterAlternative.Id, membersB);
            await KillMembersOnAsync(RcClusterPrimary.Id, membersA);
        }

        [Test]

        public async Task TestClientThrowExceptionOnFailover()
        {
            var _ = HConsoleForTest();

            var failoverOptions = new HazelcastFailoverOptionsBuilder()
                .With(fo =>
                {
                    fo.TryCount = 2;

                    // first cluster is the primary, and able to config everything
                    fo.Clients.Add(new HazelcastOptionsBuilder()
                        .With(o =>
                        {
                            o.ClusterName = RcClusterPrimary.Id;
                            o.Networking.Addresses.Clear();
                            o.Networking.Addresses.Add("127.0.0.1:5701");
                            o.Networking.ReconnectMode = Hazelcast.Networking.ReconnectMode.ReconnectAsync;
                            o.Networking.ConnectionTimeoutMilliseconds = 10_000;
                            o.Networking.ConnectionRetry.ClusterConnectionTimeoutMilliseconds = 10_000;
                        })
                        .Build());

                    // cannot override load balancer, retry, heartbeat etc. just network info
                    fo.Clients.Add(new HazelcastOptionsBuilder()
                        .With(o =>
                        {
                            o.ClusterName = RcClusterAlternative.Id;
                            o.Networking.Addresses.Add("127.0.0.1:5702");
                            o.Authentication.ConfigureUsernamePasswordCredentials("test", "1234");
                        })
                        .Build());
                })
                .WithHConsoleLogger()
                .Build();

            HConsole.WriteLine(this, "Creating Members");
            var membersA = await StartMembersOn(RcClusterPrimary.Id, 1);

            var client = await HazelcastClientFactory.StartNewFailoverClientAsync(failoverOptions);
            var mapName = CreateUniqueName();

            var map = await client.GetMapAsync<string, string>(mapName);
            await map.PutAsync(ClusterAKey, ClusterAData);

            HConsole.WriteLine(this, $"SHUTDOWN: Members of Cluster A :{RcClusterPrimary.Id}");

            // kill all members, client will try to fall over and eventually fail and shutdown
            await KillMembersOnAsync(RcClusterPrimary.Id, membersA);

            await AssertEx.SucceedsEventually(async () =>
            {
                try
                {
                    await client.GetMapAsync<string, string>(ClusterAKey);
                    throw new Exception("Expected GetMapAsync to fail.");
                }
                catch (ClientOfflineException)
                {
                    // expected
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw; // fail
                }
            }, 60_000, 500);
        }

        [Test]
        public async Task TestClientCannotFailoverToDifferentPartitionCount()
        {
            var _ = HConsoleForTest();

            var isLastStateConnected = false;

            var failoverOptions = new HazelcastFailoverOptionsBuilder()
                .With(fo =>
                {
                    fo.TryCount = 2;

                    // first cluster is the primary, and able to config everything
                    fo.Clients.Add(new HazelcastOptionsBuilder()
                        .With(o =>
                        {
                            o.ClusterName = RcClusterPrimary.Id;
                            o.Networking.Addresses.Clear();
                            o.Networking.Addresses.Add("127.0.0.1:5701");
                            o.Networking.ReconnectMode = Hazelcast.Networking.ReconnectMode.ReconnectAsync;
                            o.Networking.ConnectionTimeoutMilliseconds = 10_000;
                            o.Networking.ConnectionRetry.ClusterConnectionTimeoutMilliseconds = 10_000;
                            o.AddSubscriber(events =>
                                events.StateChanged((sender, arg) =>
                                {
                                    HConsole.WriteLine(this, $"State Changed:{arg.State}");
                                    isLastStateConnected = arg.State == ClientState.Connected;
                                }));
                        })
                        .Build());

                    // cannot override load balancer, retry, heartbeat etc. just network info
                    fo.Clients.Add(new HazelcastOptionsBuilder()
                        .With(o =>
                        {
                            o.ClusterName = RcClusterAlternative.Id;
                            o.Networking.Addresses.Add("127.0.0.1:5703");
                            o.Authentication.ConfigureUsernamePasswordCredentials("test", "1234");
                        })
                        .Build());
                })
                .WithHConsoleLogger()
                .Build();

            HConsole.WriteLine(this, "Creating Members");
            var membersA = await StartMembersOn(RcClusterPrimary.Id, 1);
            var membersB = await StartMembersOn(RcClusterPartition.Id, 1);

            // Since connections are managed at the backend,
            // cannot catch the exception with an simple assertion

            var client = await HazelcastClientFactory.StartNewFailoverClientAsync(failoverOptions);
            var mapName = CreateUniqueName();

            var map = await client.GetMapAsync<string, string>(mapName);
            await map.PutAsync(ClusterAKey, ClusterAData);
            await client.GetMapAsync<string, string>(ClusterAKey);

            HConsole.WriteLine(this, $"SHUTDOWN: Members of Cluster A :{RcClusterPrimary.Id}");
            await KillMembersOnAsync(RcClusterPrimary.Id, membersA);

            //Failover to B and fail due to different partition count            

            HConsole.WriteLine(this, $"START: Members of Cluster A :{RcClusterPrimary.Id}");
            membersA = await StartMembersOn(RcClusterPrimary.Id, 1);

            var val = await map.GetAsync(ClusterAKey);

            Assert.AreEqual(ClientState.Connected, client.State);
            Assert.True(isLastStateConnected);
        }

        private async Task AssertClusterA(IHMap<string, string> map, string currentClusterId)
        {
            HConsole.WriteLine(this, $"Asserting Cluster A - {RcClusterPrimary.Id}");

            Assert.AreEqual(RcClusterPrimary.Id, currentClusterId);

            await map.PutAsync(ClusterAKey, ClusterAData);
            var readData = await map.GetAsync(ClusterAKey);
            Assert.AreEqual(ClusterAData, readData);

            readData = await map.GetAsync(ClusterBData);
            Assert.AreNotEqual(ClusterBData, readData);
        }

        private async Task AssertClusterB(IHMap<string, string> map, string currentClusterId)
        {
            HConsole.WriteLine(this, $"Asserting Cluster B - {RcClusterAlternative.Id}");

            Assert.AreEqual(RcClusterAlternative.Id, currentClusterId);

            await map.PutAsync(ClusterBKey, ClusterBData);
            var readData = await map.GetAsync(ClusterBKey);
            Assert.AreEqual(ClusterBData, readData);

            readData = await map.GetAsync(ClusterAKey);
            Assert.AreNotEqual(ClusterAData, readData);
        }

    }
}
