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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hazelcast.Models;
using Hazelcast.Sql;
using Hazelcast.Linq;
using Microsoft.Extensions.Logging;

namespace Hazelcast.Examples.Sql
{
    // ReSharper disable once UnusedMember.Global
    public class SqlBasicQueryExample
    {
        public static async Task Main(params string[] args)
        {
            var options = new HazelcastOptionsBuilder()
                .With(args)
                .WithConsoleLogger()
                .With("Logging:LogLevel:Hazelcast.Examples", "Information")
                .Build();

            // create a Hazelcast client and connect to a server running on localhost
            await using var client = await HazelcastClientFactory.StartNewClientAsync(options);

            var logger = client.Options.LoggerFactory.CreateLogger<SqlBasicQueryExample>();

            // get the distributed map from the cluster and populate it
            await using var map = await client.GetMapAsync<int, string>(nameof(SqlBasicQueryExample));
            await map.SetAllAsync(Enumerable.Range(1, 10).ToDictionary(v => v, v => $"Value #{v}"));

            //Before you can query data in a map, you need to create a mapping to one, using the map connector.
            //see details: https://docs.hazelcast.com/hazelcast/latest/sql/create-mapping
            await client.Sql.ExecuteCommandAsync(
               $"CREATE MAPPING {map.Name} TYPE IMap OPTIONS ('keyFormat'='int', 'valueFormat'='varchar')");

            //// query and print all rows
            //{
            //    await using var result = await client.Sql.ExecuteQueryAsync($"SELECT __key, this FROM {map.Name}");

            //    var count = 1;
            //    await foreach (var row in result)
            //        logger.LogInformation("Row #{RowCount}: {RowKey}, {RowValue}", count++, row.GetKey<int>(), row.GetValue<string>());
            //}

            //// query and print all rows sorted by key descending
            //{
            //    // index must be added to be able to sort by attribute
            //    await map.AddIndexAsync(IndexType.Sorted, "__key");

            //    await using var result = await client.Sql.ExecuteQueryAsync($"SELECT __key, this FROM {map.Name} ORDER BY __key DESC");

            //    var count = 1;
            //    await foreach (var row in result)
            //        logger.LogInformation("Row (sorted) #{RowCount}: {RowKey}, {RowValue}", count++, row.GetKey<int>(), row.GetValue<string>());
            //}

            //// query and print rows filtered via parameters
            //{
            //    var (min, max) = (3, 7);
            //    await using var result = await client.Sql.ExecuteQueryAsync(
            //        $"SELECT __key, this FROM {map.Name} WHERE __key >= ? and __key <= ?",
            //        min, max
            //    );

            //    var count = 1;
            //    await foreach (var row in result)
            //        logger.LogInformation("Row (filtered) #{RowCount}: {RowKey}, {RowValue}", count++, row.GetKey<int>(), row.GetValue<string>());
            //}

            {

                //  var result = map.AsQueryable().Where(x => x.Key == 1).ToList(); //throws since we don't support sync operations. Should we? by blocking the thread??

                var min = 1;//local fields will be evaluated.     

                //We have to call AsQueryable() because of ambiguity between different enumerable interfaces, IAsyncEnumerable vs IQuerable LINQ extension.
                await using var sqlResult = await map.AsQueryable()
                                                        .Where(x => x.Key > min && x.Key < 10)
                                                        .GetAsync();

                //SQL: 

                //GetAsync is our extension to provide async call since regular linq operations does not support async operations.
                //There is an iterface but it is under Entity Framework package which means a dependency.
                //In future, if IAsyncQuerable will be available under .NET, we may also implement that.

                //Rest is same with Sql usage. It can be consumed as async.
                //If we use ISqlQueryResult, user cannot use custom projection on data. Should we go to that way?
                //Ex: this is not valid now -> map.AsQueryable().Select(p=> p.LastName+", "+p.Name).GetAsync();
                
                await foreach (var row in sqlResult)
                    logger.LogInformation(row.GetKey<int>() + "-" + row.GetValue<string>());


                //OUTPUT:

                /*
                    QUERY: SELECT * FROM (SELECT * FROM SqlBasicQueryExample) AS T WHERE ((__key > 1) AND (__key < 10))

                    info: Hazelcast.Examples.Sql.SqlBasicQueryExample[0]
                          2-Value #2
                    info: Hazelcast.Examples.Sql.SqlBasicQueryExample[0]
                          8-Value #8
                    info: Hazelcast.Examples.Sql.SqlBasicQueryExample[0]
                          5-Value #5
                    info: Hazelcast.Examples.Sql.SqlBasicQueryExample[0]
                          4-Value #4
                    info: Hazelcast.Examples.Sql.SqlBasicQueryExample[0]
                          9-Value #9
                    info: Hazelcast.Examples.Sql.SqlBasicQueryExample[0]
                          6-Value #6
                    info: Hazelcast.Examples.Sql.SqlBasicQueryExample[0]
                          7-Value #7
                    info: Hazelcast.Examples.Sql.SqlBasicQueryExample[0]
                          3-Value #3                 
                  */

            }
        }
    }
}
