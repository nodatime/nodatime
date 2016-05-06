// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Utility;
using NUnit.Framework;

namespace NodaTime.Test.Utility
{
    public class CacheTest
    {
        private int factoryCallCount;

        /// <summary>
        /// Creates a cache of size 3, from string to int, case-insensitive, where the value
        /// associated with a key is its length. The factory call count is reset to 0, and incremented
        /// each time the value factory is called.
        /// </summary>
        private Cache<string, int> CreateCache()
        {
            factoryCallCount = 0;
            return new Cache<string, int>(3, text => { factoryCallCount++; return text.Length; }, 
                                          StringComparer.OrdinalIgnoreCase);
        }

        [Test]
        public void Clear()
        {
            var cache = CreateCache();
            cache.GetOrAdd("A");
            Assert.AreEqual(1, cache.Count);
            cache.Clear();
            Assert.AreEqual(0, cache.Count);
            CollectionAssert.IsEmpty(cache.Keys);
        }

        [Test]
        public void GetOrAdd_NewEntry()
        {
            var cache = CreateCache();
            Assert.AreEqual(0, cache.Count);
            Assert.AreEqual(1, cache.GetOrAdd("A"));
            Assert.AreEqual(1, cache.Count);
            Assert.AreEqual(1, factoryCallCount);
            CollectionAssert.AreEqual(new[] { "A" }, cache.Keys);
        }

        [Test]
        public void GetOrAdd_ExistingEntry()
        {
            var cache = CreateCache();
            Assert.AreEqual(1, cache.GetOrAdd("A"));
            Assert.AreEqual(1, cache.GetOrAdd("A"));
            Assert.AreEqual(1, cache.GetOrAdd("A"));
            Assert.AreEqual(1, factoryCallCount);
            CollectionAssert.AreEqual(new[] { "A" }, cache.Keys);
        }

        [Test]
        public void GetOrAdd_ReplacementEntry()
        {
            var cache = CreateCache();
            Assert.AreEqual(1, cache.GetOrAdd("A"));
            Assert.AreEqual(2, cache.GetOrAdd("BB"));
            Assert.AreEqual(3, cache.GetOrAdd("CCC"));
            CollectionAssert.AreEqual(new[] { "A", "BB", "CCC" }, cache.Keys);
            Assert.AreEqual(4, cache.GetOrAdd("DDDD"));
            CollectionAssert.AreEqual(new[] { "BB", "CCC", "DDDD" }, cache.Keys);
            Assert.AreEqual(3, cache.Count);
            Assert.AreEqual(4, factoryCallCount);
        }
    }
}
