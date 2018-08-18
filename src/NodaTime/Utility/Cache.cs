// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NodaTime.Utility
{
    /// <summary>
    /// Implements a thread-safe cache of a fixed size, with a single computation function.
    /// (That happens to be all we need at the time of writing.)
    /// </summary>
    /// <remarks>
    /// For simplicity's sake, eviction is currently on a least-recently-added basis (not LRU). This
    /// may change in the future.
    /// </remarks>
    /// <typeparam name="TKey">Type of key</typeparam>
    /// <typeparam name="TValue">Type of value</typeparam>
    internal sealed class Cache<TKey, TValue>
    {
        private readonly int size;
        private readonly Func<TKey, TValue> valueFactory;
        // List of keys in (rough) insertion order. Used for eviction ordering.
        // The same key may appear in the list multiple times.
        private readonly ConcurrentQueue<TKey> keyList; 
        private readonly ConcurrentDictionary<TKey, TValue> dictionary; 

        internal Cache(int size, Func<TKey, TValue> valueFactory, IEqualityComparer<TKey> keyComparer)
        {
            this.size = size;
            this.valueFactory = valueFactory;
            this.dictionary = new ConcurrentDictionary<TKey,TValue>(keyComparer);
            this.keyList = new ConcurrentQueue<TKey>();
        }

        /// <summary>
        /// Fetches a value from the cache, populating it if necessary.
        /// </summary>
        /// <param name="key">Key to fetch</param>
        /// <returns>The value associated with the key.</returns>
        internal TValue GetOrAdd(TKey key)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                return value;
            }
            // Add the key to the eviction queue is we're *probably* going to be adding it now.
            // There's no easy way of telling whether a particular call to GetOrAdd has added the key.
            keyList.Enqueue(key);
            value = dictionary.GetOrAdd(key, valueFactory);

            // Trim to size if necessary, bearing in mind that other threads may be doing
            // the same thing at the same time, and that there may be multiple entries for a particular key.
            while (dictionary.Count > size && keyList.TryDequeue(out var keyToRemove))
            {
                dictionary.TryRemove(keyToRemove, out _);
            }
            return value;
        }

        /// <summary>
        /// Returns the number of entries currently in the cache, primarily for diagnostic purposes.
        /// </summary>
        internal int Count => dictionary.Count;

        /// <summary>
        /// Returns a copy of the keys in the cache as a list, for diagnostic purposes.
        /// </summary>
        internal List<TKey> Keys => dictionary.ToArray().Select(pair => pair.Key).ToList();

        /// <summary>
        /// Clears the cache. This is never surfaced publicly (directly or indirectly) - it's just
        /// for testing.
        /// </summary>
        internal void Clear()
        {
            // There's no Clear method on ConcurrentQueue, so we need to iterate over
            // it. 
            while (keyList.TryDequeue(out _))
            {
            }
            dictionary.Clear();
        }
    }
}
