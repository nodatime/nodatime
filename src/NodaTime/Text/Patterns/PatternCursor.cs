// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Text;
using NodaTime.Properties;

namespace NodaTime.Text.Patterns
{
    /// <summary>
    /// Extends <see cref="TextCursor"/> to simplify parsing patterns such as "yyyy-MM-dd".
    /// </summary>
    internal sealed class PatternCursor : TextCursor
    {
        internal PatternCursor(string pattern)
            : base(pattern)
        {
        }

        /// <summary>
        /// Gets the quoted string.
        /// </summary>
        /// <remarks>The cursor is left positioned at the end of the quoted region.</remarks>
        /// <param name="closeQuote">The close quote character to match for the end of the quoted string.</param>
        /// <returns>The quoted string sans open and close quotes. This can be an empty string but will not be null.</returns>
        internal string GetQuotedString(char closeQuote)
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
                        throw new InvalidPatternException(Messages.Parse_EscapeAtEndOfString);
                    }
                }
                builder.Append(Current);
            }
            if (!endQuoteFound)
            {
                throw new InvalidPatternException(Messages.Parse_MissingEndQuote, closeQuote);
            }
            MovePrevious();
            return builder.ToString();
        }

        /// <summary>
        /// Gets the pattern repeat count.
        /// </summary>
        /// <param name="maximumCount">The maximum number of repetitions allowed.</param>
        /// <returns>The repetition count which is alway at least <c>1</c>.</returns>
        internal int GetRepeatCount(int maximumCount)
        {
            return GetRepeatCount(maximumCount, Current);
        }

        /// <summary>
        /// Gets the pattern repeat count.
        /// </summary>
        /// <param name="maximumCount">The maximum number of repetitions allowed.</param>
        /// <param name="patternCharacter">The pattern character to count.</param>
        /// <returns>The repetition count which is alway at least <c>1</c>.</returns>
        internal int GetRepeatCount(int maximumCount, char patternCharacter)
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
                throw new InvalidPatternException(Messages.Parse_RepeatCountExceeded, patternCharacter, maximumCount);
            }
            return repeatLength;
        }
    }
}
