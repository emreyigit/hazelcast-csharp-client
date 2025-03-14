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
using System.Linq;
using System.Threading.Tasks;
using Hazelcast.Query;
using Hazelcast.Testing;
using NUnit.Framework;

namespace Hazelcast.Tests.Remote
{
    [TestFixture]
    public class ClientTxMapTest : SingleMemberClientRemoteTestBase
    {
        [Test]
        public async Task TestGetForUpdate()
        {
            var dictionary = await Client.GetMapAsync<string, int>(CreateUniqueName());
            await using var _ = DestroyAndDispose(dictionary);

            const string key = "key";
            const int initialValue = 111;
            const int newValue = 123;

            await dictionary.SetAsync(key, initialValue);

            await using var context = await Client.BeginTransactionAsync();

            var txDictionary = await context.GetMapAsync<string, int>(dictionary.Name);

            var val = await txDictionary.GetForUpdateAsync(key);
            Assert.AreEqual(initialValue, val);
            Assert.IsTrue(await dictionary.IsLockedAsync(key));
            await txDictionary.SetAsync(key, newValue);
            await context.CommitAsync();
            Assert.IsFalse(await dictionary.IsLockedAsync(key));
            Assert.That(await dictionary.GetAsync(key), Is.EqualTo(newValue));
        }

        [Test]
        public async Task TestKeySetPredicate()
        {
            var dictionary = await Client.GetMapAsync<string, string>(CreateUniqueName());
            await using var _ = DestroyAndDispose(dictionary);

            await dictionary.SetAsync("key1", "value1");
            await dictionary.SetAsync("key2", "value2");
            await dictionary.SetAsync("key3", "value3");

            await using var context = await Client.BeginTransactionAsync();
            var txDictionary = await context.GetMapAsync<string, string>(dictionary.Name);

            var sqlPredicate = new SqlPredicate("this == value1");
            var keys = await txDictionary.GetKeysAsync(sqlPredicate);

            Assert.AreEqual(1, keys.Count);
            Assert.AreEqual("key1", keys.First());

            var values = await txDictionary.GetValuesAsync(sqlPredicate);

            Assert.AreEqual(1, values.Count);
            Assert.AreEqual("value1", values.First());

            await context.CommitAsync();
        }

        [Test]
        public async Task TestKeySetValues()
        {
            var dictionary = await Client.GetMapAsync<string, string>(CreateUniqueName());
            await using var _ = DestroyAndDispose(dictionary);

            //var map = client.GetMap<object, object>(name);
            await dictionary.SetAsync("key1", "value1");
            await dictionary.SetAsync("key2", "value2");
            await using var context = await Client.BeginTransactionAsync();
            var txDictionary = await context.GetMapAsync<string, string>(dictionary.Name);
            Assert.That(await txDictionary.PutAsync("key3", "value3"), Is.Null);
            Assert.AreEqual(3, await txDictionary.GetSizeAsync());
            Assert.AreEqual(3, (await txDictionary.GetKeysAsync()).Count);
            Assert.AreEqual(3, (await txDictionary.GetValuesAsync()).Count);
            await context.CommitAsync();
            Assert.AreEqual(3, await dictionary.GetSizeAsync());
            Assert.AreEqual(3, (await dictionary.GetKeysAsync()).Count);
            Assert.AreEqual(3, (await dictionary.GetValuesAsync()).Count);
        }

        [Test]
        public async Task TestPutAndRollBack()
        {
            var dictionary = await Client.GetMapAsync<string, string>(CreateUniqueName());
            await using var _ = DestroyAndDispose(dictionary);

            var key = "key";
            var value = "value";
            await using var context = await Client.BeginTransactionAsync();
            var mapTxn = await context.GetMapAsync<string, string>(dictionary.Name);
            await mapTxn.SetAsync(key, value);
            await context.RollbackAsync();
            Assert.That(await dictionary.GetAsync(key), Is.Null);
        }

        [Test]
        public async Task TestPutGet()
        {
            var dictionary = await Client.GetMapAsync<string, string>(CreateUniqueName());
            await using var _ = DestroyAndDispose(dictionary);

            await using var context = await Client.BeginTransactionAsync();
            var txDictionary = await context.GetMapAsync<string, string>(dictionary.Name);
            Assert.That(await txDictionary.PutAsync("key1", "value1"), Is.Null);
            Assert.That(await txDictionary.GetAsync("key1"), Is.EqualTo("value1"));
            Assert.That(await dictionary.GetAsync("key1"), Is.Null);
            await context.CommitAsync();
            Assert.That(await dictionary.GetAsync("key1"), Is.EqualTo("value1"));
        }

        [Test]
        public async Task TestPutWithTTL()
        {
            var dictionary = await Client.GetMapAsync<string, string>(CreateUniqueName());
            await using var _ = DestroyAndDispose(dictionary);

            await using var context = await Client.BeginTransactionAsync();
            var txDictionary = await context.GetMapAsync<string, string>(dictionary.Name);
            var ttlMillis = 1000;
            Assert.That(await txDictionary.PutAsync("key1", "value1", TimeSpan.FromMilliseconds(ttlMillis)), Is.Null);
            Assert.That(await txDictionary.GetAsync("key1"), Is.EqualTo("value1"));

            await context.CommitAsync();

            Assert.That(await dictionary.GetAsync("key1"), Is.EqualTo("value1"));

            await AssertEx.SucceedsEventually(async () =>
            {
                Assert.That(await dictionary.GetAsync("key1"), Is.Null);
            }, 4000, 500);
        }

        [Test]
        public async Task TestTnxMapContainsKey()
        {
            var dictionary = await Client.GetMapAsync<string, string>(CreateUniqueName());
            await using var _ = DestroyAndDispose(dictionary);

            await dictionary.SetAsync("key1", "value1");
            await using var context = await Client.BeginTransactionAsync();
            var txDictionary = await context.GetMapAsync<string, string>(dictionary.Name);
            await txDictionary.SetAsync("key2", "value2");
            Assert.IsTrue(await txDictionary.ContainsKeyAsync("key1"));
            Assert.IsTrue(await txDictionary.ContainsKeyAsync("key2"));
            Assert.IsFalse(await txDictionary.ContainsKeyAsync("key3"));
            await context.CommitAsync();
        }

        [Test]
        public async Task TestTnxMapDelete()
        {
            var dictionary = await Client.GetMapAsync<string, string>(CreateUniqueName());
            await using var _ = DestroyAndDispose(dictionary);

            var key = "key1";
            var value = "old1";
            await dictionary.SetAsync(key, value);
            await using var context = await Client.BeginTransactionAsync();
            var txDictionary = await context.GetMapAsync<string, string>(dictionary.Name);
            await txDictionary.DeleteAsync(key);
            await context.CommitAsync();
            Assert.That(await dictionary.GetAsync(key), Is.Null);
        }

        [Test]
        public async Task TestTnxMapIsEmpty()
        {
            var dictionary = await Client.GetMapAsync<string, int>(CreateUniqueName());
            await using var _ = DestroyAndDispose(dictionary);

            await using var context = await Client.BeginTransactionAsync();
            var txDictionary = await context.GetMapAsync<string, int>(dictionary.Name);
            Assert.IsTrue(await txDictionary.IsEmptyAsync());
            await context.CommitAsync();
        }

        [Test]
        public async Task TestTnxMapPutIfAbsent()
        {
            var dictionary = await Client.GetMapAsync<string, string>(CreateUniqueName());
            await using var _ = DestroyAndDispose(dictionary);

            var keyValue1 = "keyValue1";
            var keyValue2 = "keyValue2";
            await dictionary.SetAsync(keyValue1, keyValue1);
            await using var context = await Client.BeginTransactionAsync();
            var txDictionary = await context.GetMapAsync<string, string>(dictionary.Name);
            await txDictionary.PutIfAbsentAsync(keyValue1, "NOT_THIS");
            await txDictionary.PutIfAbsentAsync(keyValue2, keyValue2);
            await context.CommitAsync();
            Assert.That(await dictionary.GetAsync(keyValue1), Is.EqualTo(keyValue1));
            Assert.That(await dictionary.GetAsync(keyValue2), Is.EqualTo(keyValue2));
        }

        [Test]
        public async Task TestTnxMapRemove()
        {
            var dictionary = await Client.GetMapAsync<string, string>(CreateUniqueName());
            await using var _ = DestroyAndDispose(dictionary);

            var key = "key1";
            var value = "old1";
            await dictionary.SetAsync(key, value);
            await using var context = await Client.BeginTransactionAsync();
            var txDictionary = await context.GetMapAsync<string, string>(dictionary.Name);
            var removed = await txDictionary.RemoveAsync(key);
            await context.CommitAsync();
            Assert.That(await dictionary.GetAsync(key), Is.Null);
            Assert.AreEqual(value, removed);
        }

        [Test]
        public async Task TestTnxMapRemoveKeyValue()
        {
            var dictionary = await Client.GetMapAsync<string, string>(CreateUniqueName());
            await using var _ = DestroyAndDispose(dictionary);

            var key1 = "key1";
            var oldValue1 = "old1";
            var key2 = "key2";
            var oldValue2 = "old2";
            await dictionary.SetAsync(key1, oldValue1);
            await dictionary.SetAsync(key2, oldValue2);
            await using var context = await Client.BeginTransactionAsync();

            var txDictionary = await context.GetMapAsync<string, string>(dictionary.Name);
            await txDictionary.RemoveAsync(key1, oldValue1);
            await txDictionary.RemoveAsync(key2, "NO_REMOVE_AS_NOT_VALUE");
            await context.CommitAsync();
            Assert.That(await dictionary.GetAsync(key1), Is.Null);
            Assert.That(await dictionary.GetAsync(key2), Is.EqualTo(oldValue2));
        }

        [Test]
        public async Task TestTnxMapReplace()
        {
            var dictionary = await Client.GetMapAsync<string, string>(CreateUniqueName());
            await using var _ = DestroyAndDispose(dictionary);

            var key1 = "key1";
            var key2 = "key2";
            var replaceValue = "replaceValue";
            await dictionary.SetAsync(key1, "OLD_VALUE");
            await using var context = await Client.BeginTransactionAsync();
            var txDictionary = await context.GetMapAsync<string, string>(dictionary.Name);
            await txDictionary.ReplaceAsync(key1, replaceValue);
            await txDictionary.ReplaceAsync(key2, "NOT_POSSIBLE");
            await context.CommitAsync();
            Assert.That(await dictionary.GetAsync(key1), Is.EqualTo(replaceValue));
            Assert.That(await dictionary.GetAsync(key2), Is.Null);
        }

        [Test]
        public async Task TestTnxMapReplaceKeyValue()
        {
            var dictionary = await Client.GetMapAsync<string, string>(CreateUniqueName());
            await using var _ = DestroyAndDispose(dictionary);

            var key1 = "key1";
            var oldValue1 = "old1";
            var newValue1 = "new1";
            var key2 = "key2";
            var oldValue2 = "old2";
            await dictionary.SetAsync(key1, oldValue1);
            await dictionary.SetAsync(key2, oldValue2);
            await using var context = await Client.BeginTransactionAsync();
            var txDictionary = await context.GetMapAsync<string, string>(dictionary.Name);
            await txDictionary.ReplaceAsync(key1, oldValue1, newValue1);
            await txDictionary.ReplaceAsync(key2, "NOT_OLD_VALUE", "NEW_VALUE_CANT_BE_THIS");
            await context.CommitAsync();
            Assert.That(await dictionary.GetAsync(key1), Is.EqualTo(newValue1));
            Assert.That(await dictionary.GetAsync(key2), Is.EqualTo(oldValue2));
        }

        [Test]
        public async Task TesttxDictionaryGet_BeforeCommit()
        {
            var dictionary = await Client.GetMapAsync<string, string>(CreateUniqueName());
            await using var _ = DestroyAndDispose(dictionary);

            var key = "key";
            var value = "Value";
            await using var context = await Client.BeginTransactionAsync();
            var txDictionary = await context.GetMapAsync<string, string>(dictionary.Name);
            await txDictionary.SetAsync(key, value);
            Assert.That(await txDictionary.GetAsync(key), Is.EqualTo(value));
            Assert.That(await dictionary.GetAsync(key), Is.Null);
            await context.CommitAsync();
        }

        [Test]
        public async Task TesttxDictionaryPut()
        {
            var dictionary = await Client.GetMapAsync<string, string>(CreateUniqueName());
            await using var _ = DestroyAndDispose(dictionary);

            var key = "key";
            var value = "Value";
            await using var context = await Client.BeginTransactionAsync();
            var txDictionary = await context.GetMapAsync<string, string>(dictionary.Name);
            await txDictionary.SetAsync(key, value);
            await context.CommitAsync();
            Assert.That(await dictionary.GetAsync(key), Is.EqualTo(value));
        }

        /// <exception cref="System.Exception" />
        [Test]
        public async Task TesttxDictionaryPut_BeforeCommit()
        {
            var dictionary = await Client.GetMapAsync<string, string>(CreateUniqueName());
            await using var _ = DestroyAndDispose(dictionary);

            var key = "key";
            var value = "Value";
            await using var context = await Client.BeginTransactionAsync();
            var txDictionary = await context.GetMapAsync<string, string>(dictionary.Name);
            Assert.That(await txDictionary.PutAsync(key, value), Is.Null);
            await context.CommitAsync();
        }

        [Test]
        public async Task TesttxDictionarySet()
        {
            var dictionary = await Client.GetMapAsync<string, string>(CreateUniqueName());
            await using var _ = DestroyAndDispose(dictionary);

            var key = "key";
            var value = "Value";
            await using var context = await Client.BeginTransactionAsync();
            var txDictionary = await context.GetMapAsync<string, string>(dictionary.Name);
            await txDictionary.SetAsync(key, value);
            await context.CommitAsync();
            Assert.That(await dictionary.GetAsync(key), Is.EqualTo(value));
        }

        [Test]
        public async Task TestUnlockAfterRollback()
        {
            var dictionary = await Client.GetMapAsync<string, string>(CreateUniqueName());
            await using var _ = DestroyAndDispose(dictionary);

            var key = "key";
            await using var context = await Client.BeginTransactionAsync();
            var txDictionary = await context.GetMapAsync<string, string>(dictionary.Name);
            await txDictionary.SetAsync(key, "value");
            await context.RollbackAsync();
            Assert.IsFalse(await dictionary.IsLockedAsync(key));
        }
    }
}
