#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2012 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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
        private IDictionary<TKey, TValue> original;

        internal NodaReadOnlyDictionary(IDictionary<TKey, TValue> original)
        {
            this.original = Preconditions.CheckNotNull(original, "original");
        }

        public bool ContainsKey(TKey key)
        {
            return original.ContainsKey(key);
        }

        public ICollection<TKey> Keys { get { return original.Keys; } }
    
        public bool TryGetValue(TKey key, out TValue value)
        {
            return original.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values { get { return original.Values; } }

        public TValue this[TKey key]
        {
            get { return original[key]; } 
            set
            {
                throw new NotSupportedException("Cannot set a value in a read-only dictionary");
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return original.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            original.CopyTo(array, arrayIndex);
        }

        public int Count { get { return original.Count; } }

        public bool IsReadOnly { get { return true; } }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return original.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

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
