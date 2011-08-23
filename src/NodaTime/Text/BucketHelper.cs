#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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

namespace NodaTime.Text
{
    /// <summary>
    /// Helper methods used to manipulate parse buckets.
    /// </summary>
    static internal class BucketHelper
    {
        /// <summary>
        /// Assigns the new value if the current value is not set.
        /// </summary>
        /// <remarks>
        /// When parsing an object by pattern a particular value (e.g. hours) can only be set once, or if set
        /// more than once it should be set to the same value. This method checks that the value has not been
        /// previously set or if it has that the new value is the same as the old one. If the new value is
        /// different than the old one then a failure is set.
        /// </remarks>
        /// <typeparam name="TValue">The type of value being set.</typeparam>
        /// <param name="currentValue">The current value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="patternCharacter">The pattern character for the error message if any.</param>
        /// <returns><c>true</c> if the current value was not previously set or if the current value equals the new value, <c>false</c> otherwise.</returns>
        internal static bool TryAssignNewValue<TValue>(ref TValue? currentValue, TValue newValue, char patternCharacter) where TValue : struct
        {
            // TODO: for some fields it makes sense to allow duplication with equal values; in others it doesn't.
            if (currentValue == null || currentValue.Value.Equals(newValue))
            {
                currentValue = newValue;
                return true;
            }
            // TODO: Would it make sense for this to return a ParseResult?
            return false;
        }

    }
}
