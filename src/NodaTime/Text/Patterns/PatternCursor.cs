// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Text;
using NodaTime.Properties;

namespace NodaTime.Text.Patterns
{
    /// <summary>
    /// Extends <see cref="TextCursor"/> to simplify parsing patterns such as "rrrr-MM-dd".
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
        /// Gets the pattern repeat count. The cursor is left on the final character of the
        /// repeated sequence.
        /// </summary>
        /// <param name="maximumCount">The maximum number of repetitions allowed.</param>
        /// <returns>The repetition count which is alway at least <c>1</c>.</returns>
        internal int GetRepeatCount(int maximumCount)
        {
            char patternCharacter = Current;
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

        /// <summary>
        /// Returns a string containing the embedded pattern within this one.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The cursor is expected to be positioned before the <paramref name="startPattern"/> character,
        /// and onsuccess the cursor will be positioned on the <paramref name="endPattern"/> character.
        /// </para>
        /// <para>Quote characters (' and ") and escaped characters (escaped with a backslash) are handled
        /// but not unescaped: the resulting pattern should be ready for parsing as normal.</para>
        /// </remarks>
        /// <param name="startPattern">The character expected to start the pattern.</param>
        /// <param name="endPattern">The character expected to end the pattern.</param>
        /// <returns>The embedded pattern, not including the start/end pattern characters.</returns>
        internal string GetEmbeddedPattern(char startPattern, char endPattern)
        {
            if (!MoveNext() || Current != startPattern)
            {
                throw new InvalidPatternException(string.Format(Messages.Parse_MissingEmbeddedPatternStart, startPattern));
            }
            int startIndex = Index + 1;
            while (MoveNext())
            {
                char current = Current;
                if (current == endPattern)
                {
                    return Value.Substring(startIndex, Index - startIndex);
                }
                if (current == '\\')
                {
                    if (!MoveNext())
                    {
                        throw new InvalidPatternException(Messages.Parse_EscapeAtEndOfString);
                    }
                }
                else if (current == '\'' || current == '\"')
                {
                    // We really don't care about the value here. It's slightly inefficient to
                    // create the substring and then ignore it, but it's unlikely to be significant.
                    GetQuotedString(current);
                }
            }
            // We've reached the end of the enclosing pattern without reaching the end of the embedded pattern. Oops.
            throw new InvalidPatternException(string.Format(Messages.Parse_MissingEmbeddedPatternEnd, endPattern));
        }
    }
}
