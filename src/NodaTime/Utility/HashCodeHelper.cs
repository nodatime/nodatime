// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Diagnostics.CodeAnalysis;

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
        private const int HashcodeMultiplier = 37;

        /// <summary>
        /// The initial hash value.
        /// </summary>
        private const int HashcodeInitializer = 17;

        public int Value { get; }

        internal HashCodeHelper(int value)
        {
            Value = value;
        }

        /// <summary>
        /// Returns the initial value for a hash code.
        /// </summary>
        /// <returns>The initial interger value.</returns>
        internal static HashCodeHelper Initialize() => new HashCodeHelper(HashcodeInitializer);

        /// <summary>
        /// Adds the hash value for the given value to the current hash and returns the new value.
        /// </summary>
        /// <typeparam name="T">The type of the value being hashed.</typeparam>
        /// <param name="value">The value to hash.</param>
        /// <returns>The new hash code.</returns>
        internal HashCodeHelper Hash<T>(T value)
        {
            int hash = 0;
            if (value != null)
            {
                hash = value.GetHashCode();
            }
            return MakeHash(hash);
        }

        /// <summary>
        /// Adds a new hash value to the current hash value and returns the new value.
        /// </summary>
        /// <param name="extraValue">The value to add to the hash code.</param>
        /// <returns>The new hash code.</returns>
        [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", Justification = "Deliberately overflowing.")]
        private HashCodeHelper MakeHash(int extraValue)
        {
            unchecked
            {
                return new HashCodeHelper(Value * HashcodeMultiplier + extraValue);
            }
        }
    }
}