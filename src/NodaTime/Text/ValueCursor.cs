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

using System;
using System.Globalization;

namespace NodaTime.Text
{
    internal sealed class ValueCursor : TextCursor
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="ValueCursor" /> class.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        internal ValueCursor(string value)
            : base(value)
        {
        }

        /// <summary>
        ///   Attempts to match the specified character with the current character of the string. If the
        ///   character matches then the index is moved passed the character.
        /// </summary>
        /// <param name="character">The character to match.</param>
        /// <returns><c>true</c> if the character matches.</returns>
        internal bool Match(char character)
        {
            if (Current == character)
            {
                MoveNext();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempts to match the specified string with the current point in the string. If the
        /// character matches then the index is moved past the string.
        /// </summary>
        /// <param name="match">The string to match.</param>
        /// <returns><c>true</c> if the string matches.</returns>
        internal bool Match(string match)
        {
            unchecked
            {
                if (string.CompareOrdinal(Value, Index, match, 0, match.Length) == 0)
                {
                    Move(Index + match.Length);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Attempts to match the specified string with the current point in the string in a case-insensitive
        /// manner, according to the given comparison info.
        /// </summary>
        internal bool MatchCaseInsensitive(string match, CompareInfo compareInfo)
        {
            unchecked
            {
                if (match.Length > Value.Length - Index)
                {
                    return false;
                }
                // TODO(Post-V1): This will fail if the length in the input string is different to the length in the
                // match string for culture-specific reasons. It's not clear how to handle that...
                if (compareInfo.Compare(Value, Index, match.Length, match, 0, match.Length, CompareOptions.IgnoreCase) == 0)
                {
                    Move(Index + match.Length);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Parses digits at the current point in the string as a signed 64-bit integer value.
        /// Currently this method only supports cultures whose negative sign is "-" (and
        /// using ASCII digits).
        /// </summary>
        /// <param name="result">The result integer value. The value of this is not guaranteed
        /// to be anything specific if the return value is non-null.</param>
        /// <returns>null if the digits were parsed, or the appropriate parse failure</returns>
        internal ParseResult<T> ParseInt64<T>(out long result)
        {
            unchecked
            {
                result = 0L;
                int startIndex = Index;
                bool negative = Current == '-';
                if (negative)
                {
                    if (!MoveNext())
                    {
                        return ParseResult<T>.EndOfString;
                    }
                }
                int count = 0;
                int digit;
                while (result < 922337203685477580 && (digit = GetDigit()) != -1)
                {
                    result = result * 10 + digit;
                    count++;
                    if (!MoveNext())
                    {
                        break;
                    }
                }

                if (count == 0)
                {
                    return ParseResult<T>.MissingNumber;
                }

                if (result >= 922337203685477580 && (digit = GetDigit()) != -1)
                {
                    if (result > 922337203685477580)
                    {
                        return BuildNumberOutOfRangeResult<T>(startIndex);
                    }
                    if (negative && digit == 8)
                    {
                        MoveNext();
                        result = long.MinValue;
                        return null;
                    }
                    if (digit > 7)
                    {
                        return BuildNumberOutOfRangeResult<T>(startIndex);
                    }
                    // We know we can cope with this digit...
                    result = result * 10 + digit;
                    MoveNext();
                    if (GetDigit() != -1)
                    {
                        // Too many digits. Die.
                        return BuildNumberOutOfRangeResult<T>(startIndex);
                    }
                }
                if (negative)
                {
                    result = -result;
                }
                return null;
            }
        }

        private ParseResult<T> BuildNumberOutOfRangeResult<T>(int startIndex)
        {
            Move(startIndex);
            if (Current == '-')
            {
                MoveNext();
            }
            // End of string works like not finding a digit.
            while (GetDigit() != -1)
            {
                MoveNext();
            }
            string badValue = Value.Substring(startIndex, Index - startIndex);
            Move(startIndex);
            return ParseResult<T>.ValueOutOfRange(badValue);
        }

        /// <summary>
        /// Parses digits at the current point in the string. If the minimum required
        /// digits are not present then the index is unchanged. If there are more digits than
        /// the maximum allowed they are ignored.
        /// </summary>
        /// <param name="minimumDigits">The minimum allowed digits.</param>
        /// <param name="maximumDigits">The maximum allowed digits.</param>
        /// <param name="result">The result integer value. The value of this is not guaranteed
        /// to be anything specific if the return value is false.</param>
        /// <returns><c>true</c> if the digits were parsed.</returns>
        internal bool ParseDigits(int minimumDigits, int maximumDigits, out int result)
        {
            unchecked
            {
                result = 0;
                int startIndex = Index;
                int count = 0;
                int digit;
                while (count < maximumDigits && (digit = GetDigit()) != -1)
                {
                    result = result * 10 + digit;
                    count++;
                    if (!MoveNext())
                    {
                        break;
                    }
                }
                if (count < minimumDigits)
                {
                    Move(startIndex);
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Parses digits at the current point in the string as a fractional value.
        /// At least one digit must be present, if allRequired is false there's no requirement for *all*
        /// the digits to be present.
        /// </summary>
        /// <param name="maximumDigits">The maximum allowed digits.</param>
        /// <param name="scale">The scale of the fractional value.</param>
        /// <param name="result">The result value scaled by scale. The value of this is not guaranteed
        /// to be anything specific if the return value is false.</param>
        /// <param name="allRequired">If true, <paramref name="maximumDigits"/> digits must be present in the
        /// input sequence. If false, there must be just at least one digit.</param>
        /// <returns><c>true</c> if the digits were parsed.</returns>
        internal bool ParseFraction(int maximumDigits, int scale, out int result, bool allRequired)
        {
            unchecked
            {
                if (scale < maximumDigits)
                {
                    scale = maximumDigits;
                }
                result = GetDigit();
                if (result == -1)
                {
                    return false;
                }
                int count = 1;
                int digit;
                while (MoveNext() && count < maximumDigits && (digit = GetDigit()) != -1)
                {
                    result = (result * 10) + digit;
                    count++;
                }
                result = (int)(result * Math.Pow(10.0, scale - count));
                return !allRequired || (count == maximumDigits);
            }
        }

        /// <summary>
        /// Gets the integer value of the current digit character, or -1 for "not a digit".
        /// </summary>
        /// <remarks>
        /// This currently only handles ASCII digits, which is all we have to parse to stay in line with the BCL.
        /// </remarks>
        private int GetDigit()
        {
            unchecked
            {
                int c = Current;
                return c < '0' || c > '9' ? -1 : c - '0';                
            }
        }
    }
}
