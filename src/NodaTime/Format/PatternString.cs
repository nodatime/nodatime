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
using NodaTime.Properties;
using NodaTime.Utility;
using System.Text;

namespace NodaTime.Format
{
    /// <summary>
    ///   Provides a simple pattern string parser for format strings.
    /// </summary>
    internal class PatternString
    {
        private readonly string pattern;
        private int length;
        private int index;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "PatternString" /> class.
        /// </summary>
        /// <param name = "pattern">The format pattern string.</param>
        internal PatternString(string pattern)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }
            if (pattern == string.Empty)
            {
                throw new ArgumentException(@"Pattern is empty", "pattern");
            }
            this.pattern = pattern;
            length = pattern.Length;
            index = 0;
        }

        /// <summary>
        ///   Gets the next character.
        /// </summary>
        /// <returns>The next character from the string.</returns>
        /// <exception cref = "FormatException">if there are no more characters.</exception>
        internal char GetNextCharacter()
        {
            char ch;
            if (!TryGetNextCharacter(out ch))
            {
                throw new FormatException(Resources.Format_InvalidString);
            }
            return ch;
        }

        /// <summary>
        ///   Gets the quoted string.
        /// </summary>
        /// <param name = "closeQuote">The close quote character to match for the end of the quoted string.</param>
        /// <returns>The quoted string sans open and close quotes. This can be an empty string but will not be <c>null</c>.</returns>
        /// <exception cref = "FormatException">If the end quote is missing.</exception>
        internal string GetQuotedString(char closeQuote)
        {
            var builder = new StringBuilder(length - index);
            bool endQuoteFound = false;
            while (index < length)
            {
                char ch = pattern[index++];
                if (ch == closeQuote)
                {
                    endQuoteFound = true;
                    break;
                }
                if (ch == '\\')
                {
                    if (index >= length)
                    {
                        throw new FormatException(Resources.Format_InvalidString);
                    }
                    ch = pattern[index++];
                }
                builder.Append(ch);
            }
            if (!endQuoteFound)
            {
                throw new FormatException(ResourceHelper.GetMessage("Format_BadQuote", closeQuote));
            }
            return builder.ToString();
        }

        /// <summary>
        ///   Gets the pattern repeat count.
        /// </summary>
        /// <param name = "patternCharacter">The pattern character to count.</param>
        /// <param name = "maximumCount">The maximum number of repetitions allowed.</param>
        /// <returns>The repetition count which is alway at least <c>1</c>.</returns>
        /// <exception cref = "FormatException">if the count exceeds <paramref name = "maximumCount" />.</exception>
        internal int GetRepeatCount(char patternCharacter, int maximumCount)
        {
            int startPos = index;
            while (index < length && pattern[index] == patternCharacter)
            {
                index++;
            }
            int repeatLength = index - startPos + 1;
            if (repeatLength > maximumCount)
            {
                throw new FormatException(Resources.Format_InvalidString);
            }
            return repeatLength;
        }

        /// <summary>
        ///   Returns whether there are unread characters.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this pattern string has unread characters; otherwise, <c>false</c>.
        /// </returns>
        internal bool HasMoreCharacters
        {
            get
            {
                return index < length;
            }
        }

        /// <summary>
        ///   Tries to get the next character.
        /// </summary>
        /// <param name = "character">Where to store the next character. This will be <c>\u0000</c> if there are
        ///   no more characters.</param>
        /// <returns><c>true</c> if there is a next character, <c>false</c> otherwise.</returns>
        internal bool TryGetNextCharacter(out char character)
        {
            if (index < length)
            {
                character = pattern[index++];
                return true;
            }
            character = '\u0000';
            return false;
        }

        internal void TrimTail()
        {
            while (length > 0 && Char.IsWhiteSpace(value[length - 1]))
            {
                length--;
            }
        }

        internal void RemoveTrailingInQuoteSpaces()
        {
            throw new NotImplementedException();
        }
    }
}