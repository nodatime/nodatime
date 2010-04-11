#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NodaTime.ZoneInfoCompiler.Tzdb;

namespace NodaTime.ZoneInfoCompiler
{
    /// <summary>
    /// Provides a simple string tokenizer that breaks the string into words that are separated by
    /// white space.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Multiple white spaces in a row are treated as one separator. White space at the beginning of
    /// the line cause an empty token to be returned as the first token. White space at the end of
    /// the line are ignored.
    /// </para>
    /// </remarks>
    public class Tokens
    {
        /// <summary>
        /// Represents an empty token list.
        /// </summary>
        private static readonly string[] NoTokens = new string[0];

        /// <summary>
        /// The list of words. This will never be null but may be empty.
        /// </summary>
        private readonly IList<string> words;

        /// <summary>
        /// The current index into the words list.
        /// </summary>
        private int index;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tokens"/> class.
        /// </summary>
        /// <param name="words">The words list.</param>
        private Tokens(IList<string> words)
        {
            this.words = words;
        }

        /// <summary>
        /// Returns an object that contains the list of the whitespace separated words in the given
        /// string. The string is assumed to be culture invariant.
        /// </summary>
        /// <param name="text">The text to break into words.</param>
        /// <returns>The tokenized text.</returns>
        /// <exception cref="ArgumentNullException">If the text is null.</exception>
        public static Tokens Tokenize(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            text = text.TrimEnd();
            string[] parts = Regex.Split(text, @"\s+", RegexOptions.Compiled | RegexOptions.CultureInvariant);
            if (parts.Length == 1 && string.IsNullOrEmpty(parts[0]))
            {
                parts = NoTokens;
            }
            var list = new List<string>(parts);
            return new Tokens(list);
        }

        /// <summary>
        /// Returns the next token.
        /// </summary>
        /// <param name="name">The name of the token. Used in the exception to identify the missing token.</param>
        /// <returns>The next token.</returns>
        /// <exception cref="MissingTokenException">Thrown if there is no next token.</exception>
        public string NextToken(string name)
        {
            if (HasNextToken)
            {
                return words[index++];
            }
            throw new MissingTokenException(name);
        }

        /// <summary>
        /// Tries to get the next token.
        /// </summary>
        /// <param name="name">The name of the token.</param>
        /// <param name="result">Where to place the next token.</param>
        /// <returns>True if there was a next token, false otherwise.</returns>
        public bool TryNextToken(string name, out string result)
        {
            if (HasNextToken)
            {
                result = NextToken(name);
                return true;
            }
            result = string.Empty;
            return false;
        }

        /// <summary>
        /// Gets a value indicating whether this instance has another token.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has another token; otherwise, <c>false</c>.
        /// </value>
        public bool HasNextToken { get { return index < words.Count; } }
    }
}