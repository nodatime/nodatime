// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Annotations;
using NodaTime.Utility;
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
        /// manner, according to the given comparison info. The cursor is optionally updated to the end of the match.
        /// </summary>
        internal bool MatchCaseInsensitive(string match, CompareInfo compareInfo, bool moveOnSuccess)
        {
            unchecked
            {
                if (match.Length > Value.Length - Index)
                {
                    return false;
                }
                // Note: This will fail if the length in the input string is different to the length in the
                // match string for culture-specific reasons. It's not clear how to handle that...
                // See issue 210 for details - we're not intending to fix this, but it's annoying.
                if (compareInfo.Compare(Value, Index, match.Length, match, 0, match.Length, CompareOptions.IgnoreCase) == 0)
                {
                    if (moveOnSuccess)
                    {
                        Move(Index + match.Length);
                    }
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Compares the value from the current cursor position with the given match. If the
        /// given match string is longer than the remaining length, the comparison still goes
        /// ahead but the result is never 0: if the result of comparing to the end of the
        /// value returns 0, the result is -1 to indicate that the value is earlier than the given match.
        /// Conversely, if the remaining value is longer than the match string, the comparison only
        /// goes as far as the end of the match. So "xabcd" with the cursor at "a" will return 0 when
        /// matched with "abc".
        /// </summary>
        /// <returns>A negative number if the value (from the current cursor position) is lexicographically
        /// earlier than the given match string; 0 if they are equal (as far as the end of the match) and
        /// a positive number if the value is lexicographically later than the given match string.</returns>
        internal int CompareOrdinal(string match)
        {
            int remaining = Value.Length - Index;
            if (match.Length > remaining)
            {
                int ret = string.CompareOrdinal(Value, Index, match, 0, remaining);
                return ret == 0 ? -1 : ret;
            }
            return string.CompareOrdinal(Value, Index, match, 0, match.Length);
        }

        /// <summary>
        /// Parses digits at the current point in the string as a signed 64-bit integer value.
        /// Currently this method only supports cultures whose negative sign is "-" (and
        /// using ASCII digits).
        /// </summary>
        /// <param name="result">The result integer value. The value of this is not guaranteed
        /// to be anything specific if the return value is non-null.</param>
        /// <returns>null if the digits were parsed, or the appropriate parse failure</returns>
        internal ParseResult<T>? ParseInt64<T>(out long result)
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
                        Move(startIndex);
                        return ParseResult<T>.EndOfString(this);
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
                    Move(startIndex);
                    return ParseResult<T>.MissingNumber(this);
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
            return ParseResult<T>.ValueOutOfRange(this, badValue);
        }

        /// <summary>
        /// Parses digits at the current point in the string, as an <see cref="Int64"/> value.
        /// If the minimum required
        /// digits are not present then the index is unchanged. If there are more digits than
        /// the maximum allowed they are ignored.
        /// </summary>
        /// <param name="minimumDigits">The minimum allowed digits.</param>
        /// <param name="maximumDigits">The maximum allowed digits.</param>
        /// <param name="result">The result integer value. The value of this is not guaranteed
        /// to be anything specific if the return value is false.</param>
        /// <returns><c>true</c> if the digits were parsed.</returns>
        internal bool ParseInt64Digits(int minimumDigits, int maximumDigits, out long result)
        {
            unchecked
            {
                result = 0;
                int localIndex = Index;
                int maxIndex = localIndex + maximumDigits;
                if (maxIndex >= Length)
                {
                    maxIndex = Length;
                }
                for (; localIndex < maxIndex; localIndex++)
                {
                    // Optimized digit handling: rather than checking for the range, returning -1
                    // and then checking whether the result is -1, we can do both checks at once.
                    int digit = Value[localIndex] - '0';
                    if (digit < 0 || digit > 9)
                    {
                        break;
                    }
                    result = result * 10 + digit;
                }
                int count = localIndex - Index;
                if (count < minimumDigits)
                {
                    return false;
                }
                Move(localIndex);
                return true;
            }
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
                int localIndex = Index;
                int maxIndex = localIndex + maximumDigits;
                if (maxIndex >= Length)
                {
                    maxIndex = Length;
                }
                for (;  localIndex < maxIndex; localIndex++)
                {
                    // Optimized digit handling: rather than checking for the range, returning -1
                    // and then checking whether the result is -1, we can do both checks at once.
                    int digit = Value[localIndex] - '0';
                    if (digit < 0 || digit > 9)
                    {
                        break;
                    }
                    result = result * 10 + digit;
                }
                int count = localIndex - Index;
                if (count < minimumDigits)
                {
                    return false;
                }
                Move(localIndex);
                return true;
            }
        }

        /// <summary>
        /// Parses digits at the current point in the string as a fractional value.
        /// </summary>
        /// <param name="maximumDigits">The maximum allowed digits. Trusted to be less than or equal to scale.</param>
        /// <param name="scale">The scale of the fractional value.</param>
        /// <param name="result">The result value scaled by scale. The value of this is not guaranteed
        /// to be anything specific if the return value is false.</param>
        /// <param name="minimumDigits">The minimum number of digits that must be specified in the value.</param>
        /// <returns><c>true</c> if the digits were parsed.</returns>
        internal bool ParseFraction([Trusted] int maximumDigits, int scale, out int result, int minimumDigits)
        {
            unchecked
            {
                Preconditions.DebugCheckArgument(maximumDigits <= scale, nameof(maximumDigits),
                    "Must not allow more maximum digits than scale");

                result = 0;
                int localIndex = Index;
                int minIndex = localIndex + minimumDigits;
                if (minIndex > Length)
                {
                    // If we don't have all the digits we're meant to have, we can't possibly succeed.
                    return false;
                }
                int maxIndex = Math.Min(localIndex + maximumDigits, Length);
                for (; localIndex < maxIndex; localIndex++)
                {
                    // Optimized digit handling: rather than checking for the range, returning -1
                    // and then checking whether the result is -1, we can do both checks at once.
                    int digit = Value[localIndex] - '0';
                    if (digit < 0 || digit > 9)
                    {
                        break;
                    }
                    result = result * 10 + digit;
                }
                int count = localIndex - Index;
                // Couldn't parse the minimum number of digits required?
                if (count  < minimumDigits)
                {
                    return false;
                }
                result = (int) (result * Math.Pow(10.0, scale - count));
                Move(localIndex);
                return true;
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
