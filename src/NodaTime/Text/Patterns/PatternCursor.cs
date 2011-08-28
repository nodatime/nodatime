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

using System.Text;

namespace NodaTime.Text.Patterns
{
    /// <summary>
    /// Extends <see cref="TextCursor"/> to simplify parsing patterns such as "yyyy-MM-dd".
    /// </summary>
    internal class PatternCursor : TextCursor
    {
        internal PatternCursor(string pattern)
            : base(pattern)
        {
        }

        /// <summary>
        /// Gets the quoted string using the current character as the close quote character.
        /// </summary>
        /// <param name="failure">A ref parameter to accept an early failure result of the current parsing operation.
        /// It is expected that this will be null before the call, and this method will set it to a non-null value
        /// if this method could not complete successfully.</param>
        /// <returns>The quoted string sans open and close quotes. This can be an empty string but will not be <c>null</c>.</returns>
        internal string GetQuotedString<T>(ref PatternParseResult<T> failure)
        {
            return GetQuotedString(Current, ref failure);
        }

        /// <summary>
        /// Gets the quoted string.
        /// </summary>
        /// <param name="closeQuote">The close quote character to match for the end of the quoted string.</param>
        /// <param name="failure">A ref parameter to accept an early failure result of the current parsing operation.
        /// It is expected that this will be null before the call, and this method will set it to a non-null value
        /// if this method could not complete successfully.</param>
        /// <returns>The quoted string sans open and close quotes. This can be an empty string but will not be <c>null</c>.</returns>
        internal string GetQuotedString<T>(char closeQuote, ref PatternParseResult<T> failure)
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
                        failure = PatternParseResult<T>.EscapeAtEndOfString;
                        return null;
                    }
                }
                builder.Append(Current);
            }
            if (!endQuoteFound)
            {
                failure = PatternParseResult<T>.MissingEndQuote(closeQuote);
                return null;
            }
            return builder.ToString();
        }

        /// <summary>
        /// Gets the pattern repeat count.
        /// </summary>
        /// <param name="maximumCount">The maximum number of repetitions allowed.</param>
        /// <param name="failure">A ref parameter to accept an early failure result of the current parsing operation.
        /// It is expected that this will be null before the call, and this method will set it to a non-null value
        /// if this method could not complete successfully.</param>
        /// <returns>The repetition count which is alway at least <c>1</c>.</returns>
        internal int GetRepeatCount<T>(int maximumCount, ref PatternParseResult<T> failure)
        {
            return GetRepeatCount(maximumCount, Current, ref failure);
        }

        /// <summary>
        /// Gets the pattern repeat count.
        /// </summary>
        /// <param name="maximumCount">The maximum number of repetitions allowed.</param>
        /// <param name="patternCharacter">The pattern character to count.</param>
        /// <param name="failure">A ref parameter to accept an early failure result of the current parsing operation.
        /// It is expected that this will be null before the call, and this method will set it to a non-null value
        /// if this method could not complete successfully.</param>
        /// <returns>The repetition count which is alway at least <c>1</c>.</returns>
        internal int GetRepeatCount<T>(int maximumCount, char patternCharacter, ref PatternParseResult<T> failure)
        {
            int startPos = Index;
            while (MoveNext() && Current == patternCharacter)
            {
            }
            int repeatLength = Index - startPos;
            // Move the cursor back to the last character of the repeated pattern
            MovePrevious();
            if (repeatLength > maximumCount)
            {
                failure = PatternParseResult<T>.RepeatCountExceeded(patternCharacter, maximumCount);
                return 0;
            }
            return repeatLength;
        }
    }
}
