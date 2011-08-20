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
using System.Text;

namespace NodaTime.Format
{
    /// <summary>
    /// Equivalent of Pattern but without throwing exceptions
    /// </summary>
    internal class NonThrowingPattern : NonThrowingParsable
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="Pattern" /> class.
        /// </summary>
        /// <param name="pattern">The format pattern string.</param>
        internal NonThrowingPattern(string pattern)
            : base(pattern)
        {
        }

        /// <summary>
        ///   Gets the quoted string using the current character as the close quote character.
        /// </summary>
        /// <param name="failure">A ref parameter to accept an early failure result of the current parsing operation.
        /// It is expected that this will be null before the call, and this method will set it to a non-null value
        /// if this method could not complete successfully.</param>
        /// <returns>The quoted string sans open and close quotes. This can be an empty string but will not be <c>null</c>.</returns>
        /// <exception cref="FormatException">If the end quote is missing.</exception>
        internal string GetQuotedString<T>(ref ParseResult<T> failure)
        {
            return GetQuotedString(Current, ref failure);
        }

        /// <summary>
        ///   Gets the quoted string.
        /// </summary>
        /// <param name="closeQuote">The close quote character to match for the end of the quoted string.</param>
        /// <param name="failure">A ref parameter to accept an early failure result of the current parsing operation.
        /// It is expected that this will be null before the call, and this method will set it to a non-null value
        /// if this method could not complete successfully.</param>
        /// <returns>The quoted string sans open and close quotes. This can be an empty string but will not be <c>null</c>.</returns>
        /// <exception cref="FormatException">If the end quote is missing.</exception>
        internal string GetQuotedString<T>(char closeQuote, ref ParseResult<T> failure)
        {
            var builder = new StringBuilder(Length - Index);
            bool endQuoteFound = false;
            while (MoveNext())
            {
                if (Current == closeQuote)
                {
                    MoveNext();
                    endQuoteFound = true;
                    break;
                }
                if (Current == '\\')
                {
                    if (!MoveNext())
                    {
                        failure = ParseResult<T>.EscapeAtEndOfString;
                        return null;
                    }
                }
                builder.Append(Current);
            }
            if (!endQuoteFound)
            {
                failure = ParseResult<T>.MissingEndQuote(closeQuote);
                return null;
            }
            return builder.ToString();
        }

        /// <summary>
        ///   Gets the pattern repeat count.
        /// </summary>
        /// <param name="maximumCount">The maximum number of repetitions allowed.</param>
        /// <param name="failure">A ref parameter to accept an early failure result of the current parsing operation.
        /// It is expected that this will be null before the call, and this method will set it to a non-null value
        /// if this method could not complete successfully.</param>
        /// <returns>The repetition count which is alway at least <c>1</c>.</returns>
        /// <exception cref="FormatException">if the count exceeds <paramref name = "maximumCount" />.</exception>
        internal int GetRepeatCount<T>(int maximumCount, ref ParseResult<T> failure)
        {
            return GetRepeatCount(maximumCount, Current, ref failure);
        }

        /// <summary>
        ///   Gets the pattern repeat count.
        /// </summary>
        /// <param name="maximumCount">The maximum number of repetitions allowed.</param>
        /// <param name="patternCharacter">The pattern character to count.</param>
        /// <param name="failure">A ref parameter to accept an early failure result of the current parsing operation.
        /// It is expected that this will be null before the call, and this method will set it to a non-null value
        /// if this method could not complete successfully.</param>
        /// <returns>The repetition count which is alway at least <c>1</c>.</returns>
        /// <exception cref="FormatException">if the count exceeds <paramref name = "maximumCount" />.</exception>
        internal int GetRepeatCount<T>(int maximumCount, char patternCharacter, ref ParseResult<T> failure)
        {
            int startPos = Index;
            while (MoveNext() && Current == patternCharacter)
            {
            }
            int repeatLength = Index - startPos;
            if (Index < Length)
            {
                MovePrevious();
            }
            if (repeatLength > maximumCount)
            {
                failure = ParseResult<T>.RepeatCountExceeded(patternCharacter, maximumCount);
                return 0;
            }
            return repeatLength;
        }
    }
}
