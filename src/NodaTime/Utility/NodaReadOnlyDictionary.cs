// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections;
using System.Collections.Generic;

namespace NodaTime.Utility
{
    /// <summary>
    /// Implementation of IDictionary{,} which delegates to an underlying dictionary for read, but 
    /// doesn't support any mutation operations.
    /// </summary>
    /// <remarks>The "Noda" prefix is to avoid any confusion with the BCL ReadOnlyDictionary type
    /// introduced in .NET 4.5.</remarks>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    internal sealed class NodaReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> original;

        internal NodaReadOnlyDictionary(IDictionary<TKey, TValue> original)
        {
            this.original = Preconditions.CheckNotNull(original, nameof(original));
        }

        public bool ContainsKey(TKey key) => original.ContainsKey(key);

        public ICollection<TKey> Keys => original.Keys;

        public bool TryGetValue(TKey key, out TValue value) => original.TryGetValue(key, out value);

        public ICollection<TValue> Values => original.Values;

        public TValue this[TKey key]
        {
            get { return original[key]; } 
            set
            {
                throw new NotSupportedException("Cannot set a value in a read-only dictionary");
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) => original.Contains(item);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => original.CopyTo(array, arrayIndex);

        public int Count => original.Count;

        public bool IsReadOnly => true;

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => original.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            throw new NotSupportedException("Cannot add to a read-only dictionary");
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException("Cannot add to a read-only dictionary");
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            throw new NotSupportedException("Cannot remove from a read-only dictionary");
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException("Cannot remove from a read-only dictionary");
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            throw new NotSupportedException("Cannot clear a read-only dictionary");
        }
    }
}
