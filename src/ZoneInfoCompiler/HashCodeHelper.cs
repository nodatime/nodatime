#region Copyright and license information
// Copyright 2009 James Keesey
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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NodaTime.ZoneInfoCompiler
{

    /// <summary>
    /// Provides methods to help generate good hash codes.
    /// </summary>
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
        /// The number of bits in an Int32.
        /// </summary>
        private const int BitsInAnInteger = 32;

        /// <summary>
        /// Adds the hash value for a bool to the current hash value and returns the new value.
        /// </summary>
        /// <param name="code">the previous hash code.</param>
        /// <param name="value">the value to add to the hash code.</param>
        /// <returns>the new hash code.</returns>
        public static int Hash(int code, bool value)
        {
            return Hash(code, value ? 0 : 1);
        }

        /// <summary>
        /// Adds the hash value for a byte to the current hash value and returns the new value.
        /// </summary>
        /// <param name="code">the previous hash code.</param>
        /// <param name="value">the value to add to the hash code.</param>
        /// <returns>the new hash code.</returns>
        public static int Hash(int code, byte value)
        {
            return Hash(code, (int)value);
        }

        /// <summary>
        /// Adds the hash value for a double to the current hash value and returns the new value.
        /// </summary>
        /// <param name="code">the previous hash code.</param>
        /// <param name="value">the value to add to the hash code.</param>
        /// <returns>the new hash code.</returns>
        public static int Hash(int code, double value)
        {
            return Hash(code, value.GetHashCode());
        }

        /// <summary>
        /// Adds the hash value for a float to the current hash value and returns the new value.
        /// </summary>
        /// <param name="code">the previous hash code.</param>
        /// <param name="value">the value to add to the hash code.</param>
        /// <returns>the new hash code.</returns>
        public static int Hash(int code, float value)
        {
            return Hash(code, value.GetHashCode());
        }

        /// <summary>
        /// Adds the hash value for a int to the current hash value and returns the new value.
        /// </summary>
        /// <param name="code">the previous hash code.</param>
        /// <param name="value">the value to add to the hash code.</param>
        /// <returns>the new hash code.</returns>
        [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", Justification = "Deliberately overflowing.")]
        public static int Hash(int code, int value)
        {
            code = (code * HashcodeMultiplier) + value;
            return code;
        }

        /// <summary>
        /// Adds the hash value for a long to the current hash value and returns the new value.
        /// </summary>
        /// <param name="code">the previous hash code.</param>
        /// <param name="value">the value to add to the hash code.</param>
        /// <returns>the new hash code.</returns>
        public static int Hash(int code, long value)
        {
            return Hash(code, value.GetHashCode());
        }

        /// <summary>
        /// Adds the hash value for a Object to the current hash value and returns the new value.
        /// </summary>
        /// <param name="code">the previous hash code.</param>
        /// <param name="value">the value to add to the hash code.</param>
        /// <returns>the new hash code.</returns>
        [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", Justification = "Deliberately overflowing.")]
        public static int Hash(int code, object value)
        {
            int hashValue = 0;
            if (value != null) {
                hashValue = value.GetHashCode();
            }
            return Hash(code, hashValue);
        }

        /// <summary>
        /// Adds the hash value for a short to the current hash value and returns the new value.
        /// </summary>
        /// <param name="code">the previous hash code.</param>
        /// <param name="value">the value to add to the hash code.</param>
        /// <returns>the new hash code.</returns>
        public static int Hash(int code, short value)
        {
            return Hash(code, (int)value);
        }

        /// <summary>
        /// Adds the hash code value of hashing all of the objects returned by the given iterator to
        /// the current hash value and returns the new value. If the iterator is <code>null</code>
        /// or returns no objects then it is treated as a <code>null</code> object reference and
        /// that is added to the hash code.
        /// </summary>
        /// <typeparam name="T">The type of objects being hashed.</typeparam>
        /// <param name="code">the previous hash code.</param>
        /// <param name="enumerable">The IEnumerable over all of the items to hash.</param>
        /// <returns>the new hash code.</returns>
        [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", Justification = "Deliberately overflowing.")]
        public static int HashEnumerable<T>(int code, IEnumerable<T> enumerable)
        {
            int value = 0;
            if (enumerable != null) {
                value = Initialize();
                foreach (var item in enumerable) {
                    value = Hash(value, item);
                }
            }
            code = (code * HashcodeMultiplier) + value;
            return code;
        }

        /// <summary>
        /// Returns an initialized hash code. This should be called first and the value returned
        /// should be passed into the succeeding <code>hash()</code> calls.
        /// </summary>
        /// <returns>the initialized hash code value.</returns>
        public static int Initialize()
        {
            return HashcodeInitializer;
        }
    }
}
