// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;

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
        private readonly object mutex = new object();
        private readonly Func<TKey, TValue> valueFactory;
        private readonly LinkedList<TKey> keyList; 
        private readonly Dictionary<TKey, TValue> dictionary; 

        internal Cache(int size, Func<TKey, TValue> valueFactory, IEqualityComparer<TKey> keyComparer)
        {
            this.size = size;
            this.valueFactory = valueFactory;
            this.dictionary = new Dictionary<TKey,TValue>(keyComparer);
            this.keyList = new LinkedList<TKey>();
        }

        /// <summary>
        /// Fetches a value from the cache, populating it if necessary.
        /// </summary>
        /// <param name="key">Key to fetch</param>
        /// <returns>The value associated with the key.</returns>
        internal TValue GetOrAdd(TKey key)
        {
            lock (mutex)
            {
                // First check the cache...
                if (dictionary.TryGetValue(key, out TValue value))
                {
                    return value;
                }

                // Make space if necessary...
                if (dictionary.Count == size)
                {
                    TKey firstKey = keyList.First.Value;
                    keyList.RemoveFirst();
                    dictionary.Remove(firstKey);
                }

                // Create and cache the new value
                value = valueFactory(key);
                keyList.AddLast(key);
                dictionary[key] = value;
                return value;
            }
        }

        /// <summary>
        /// Returns the number of entries currently in the cache, primarily for diagnostic purposes.
        /// </summary>
        internal int Count
        { 
            get 
            {
                lock (mutex)
                {
                    return dictionary.Count;
                }
            }
        }

        /// <summary>
        /// Returns a copy of the keys in the cache as a list, for diagnostic purposes.
        /// </summary>
        internal List<TKey> Keys
        {
            get
            {
                lock (mutex)
                {
                    return new List<TKey>(keyList);
                }
            }
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        internal void Clear()
        {
            lock (mutex)
            {
                keyList.Clear();
                dictionary.Clear();
            }
        }
    }
}
