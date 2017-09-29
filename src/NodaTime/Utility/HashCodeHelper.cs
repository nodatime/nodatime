// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Utility
{
    /// <summary>
    /// Provides method to help with generating hash codes for structures and classes. This handles
    /// value types, nullable type, and objects.
    /// </summary>
    /// <remarks>
    /// The basic usage pattern is:
    /// <example>
    /// <code>
    ///  public override int GetHashCode() => HashCodeHelper.Initialize().Hash(Field1).Hash(Field2).Hash(Field3).Value;
    /// </code>
    /// </example>
    /// </remarks>
    internal struct HashCodeHelper
    {
        /// <summary>
        /// The multiplier for each value.
        /// </summary>
        private const int HashCodeMultiplier = 37;

        /// <summary>
        /// The initial hash value.
        /// </summary>
        private const int HashCodeInitializer = 17;

        public int Value { get; }

        internal HashCodeHelper(int value)
        {
            Value = value;
        }

        /// <summary>
        /// Convenience method to hash two values.
        /// </summary>
        internal static int Hash<T1, T2>(T1 t1, T2 t2)
        {
            unchecked
            {
                int hash = HashCodeInitializer;
                hash = hash * HashCodeMultiplier + (t1?.GetHashCode() ?? 0);
                hash = hash * HashCodeMultiplier + (t2?.GetHashCode() ?? 0);
                return hash;
            }
        }

        /// <summary>
        /// Convenience method to hash three values.
        /// </summary>
        internal static int Hash<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        {
            unchecked
            {
                int hash = HashCodeInitializer;
                hash = hash * HashCodeMultiplier + (t1?.GetHashCode() ?? 0);
                hash = hash * HashCodeMultiplier + (t2?.GetHashCode() ?? 0);
                hash = hash * HashCodeMultiplier + (t3?.GetHashCode() ?? 0);
                return hash;
            }
        }

        /// <summary>
        /// Returns the initial value for a hash code.
        /// </summary>
        /// <returns>The initial integer wrapped in a <see cref="HashCodeHelper"/> value.</returns>
        internal static HashCodeHelper Initialize() => new HashCodeHelper(HashCodeInitializer);

        /// <summary>
        /// Adds the hash value for the given value to the current hash and returns the new value.
        /// </summary>
        /// <typeparam name="T">The type of the value being hashed.</typeparam>
        /// <param name="value">The value to hash.</param>
        /// <returns>The new hash code.</returns>
        internal HashCodeHelper Hash<T>(T value)
        {
            unchecked
            {
                return new HashCodeHelper(Value * HashCodeMultiplier + (value?.GetHashCode() ?? 0));
            }
        }
    }
}