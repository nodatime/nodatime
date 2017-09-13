// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Text;

namespace NodaTime.Text.Patterns
{
    /// <summary>
    /// Extends <see cref="TextCursor"/> to simplify parsing patterns such as "uuuu-MM-dd".
    /// </summary>
    internal sealed class PatternCursor : TextCursor
    {
        /// <summary>
        /// The character signifying the start of an embedded pattern.
        /// </summary>
        internal const char EmbeddedPatternStart = '<';
        /// <summary>
        /// The character signifying the end of an embedded pattern.
        /// </summary>
        internal const char EmbeddedPatternEnd = '>';

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
                        throw new InvalidPatternException(TextErrorMessages.EscapeAtEndOfString);
                    }
                }
                builder.Append(Current);
            }
            if (!endQuoteFound)
            {
                throw new InvalidPatternException(TextErrorMessages.MissingEndQuote, closeQuote);
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
                throw new InvalidPatternException(TextErrorMessages.RepeatCountExceeded, patternCharacter, maximumCount);
            }
            return repeatLength;
        }

        /// <summary>
        /// Returns a string containing the embedded pattern within this one.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The cursor is expected to be positioned immediately before the <see cref="EmbeddedPatternStart"/> character (<c>&lt;</c>),
        /// and on success the cursor will be positioned on the <see cref="EmbeddedPatternEnd" /> character (<c>&gt;</c>).
        /// </para>
        /// <para>Quote characters (' and ") and escaped characters (escaped with a backslash) are handled
        /// but not unescaped: the resulting pattern should be ready for parsing as normal. It is assumed that the
        /// embedded pattern will itself handle embedded patterns, so if the input is on the first <c>&lt;</c>
        /// of <c>"before &lt;outer1 &lt;inner&gt; outer2&gt; after"</c>
        /// this method will return <c>"outer1 &lt;inner&gt; outer2"</c> and the cursor will be positioned
        /// on the final <c>&gt;</c> afterwards.
        /// </para>
        /// </remarks>
        /// <returns>The embedded pattern, not including the start/end pattern characters.</returns>
        internal string GetEmbeddedPattern()
        {
            if (!MoveNext() || Current != EmbeddedPatternStart)
            {
                throw new InvalidPatternException(string.Format(TextErrorMessages.MissingEmbeddedPatternStart, EmbeddedPatternStart));
            }
            int startIndex = Index + 1;
            int depth = 1; // For nesting
            while (MoveNext())
            {
                char current = Current;
                if (current == EmbeddedPatternEnd)
                {
                    depth--;
                    if (depth == 0)
                    {
                        return Value.Substring(startIndex, Index - startIndex);
                    }
                }
                else if (current == EmbeddedPatternStart)
                {
                    depth++;
                }
                else if (current == '\\')
                {
                    if (!MoveNext())
                    {
                        throw new InvalidPatternException(TextErrorMessages.EscapeAtEndOfString);
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
            throw new InvalidPatternException(string.Format(TextErrorMessages.MissingEmbeddedPatternEnd, EmbeddedPatternEnd));
        }
    }
}
