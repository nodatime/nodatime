#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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
    ///    public override int GetHashCode()
    ///    {
    ///        int hash = HashCodeHelper.Initialize();
    ///        hash = HashCodeHelper.Hash(hash, Value1);
    ///        hash = HashCodeHelper.Hash(hash, Value2);
    ///        hash = HashCodeHelper.Hash(hash, Value3);
    ///        ...
    ///        return hash;
    ///    }
    /// </code>
    /// </example>
    /// </remarks>
    public static class HashCodeHelper
    {
        /// <summary>
        /// The multiplier for each value.
        /// </summary>
        private const int HashcodeMultiplier = 37;

        /// <summary>
        /// The initial hash value.
        /// </summary>
        private const int HashcodeInitializer = 17;

        /// <summary>
        /// Returns the initial value for a hash code.
        /// </summary>
        /// <returns>The initial interger value.</returns>
        public static int Initialize()
        {
            return HashcodeInitializer;
        }

        /// <summary>
        /// Adds the hash value for the given value to the current hash and returns the new value.
        /// </summary>
        /// <typeparam name="T">The type of the value being hashed.</typeparam>
        /// <param name="code">The previous hash code.</param>
        /// <param name="value">The value to hash.</param>
        /// <returns>The new hash code.</returns>
        public static int Hash<T>(int code, T value)
        {
            int hash = 0;
            if (value != null) {
                hash = value.GetHashCode();
            }
            return MakeHash(code, hash);
        }

        /// <summary>
        /// Adds the hash value for a int to the current hash value and returns the new value.
        /// </summary>
        /// <param name="code">The previous hash code.</param>
        /// <param name="value">The value to add to the hash code.</param>
        /// <returns>The new hash code.</returns>
        [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", Justification = "Deliberately overflowing.")]
        private static int MakeHash(int code, int value)
        {
            code = (code * HashcodeMultiplier) + value;
            return code;
        }
    }
}
