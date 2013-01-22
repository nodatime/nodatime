// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NodaTime.Utility;
using NodaTime.ZoneInfoCompiler.Tzdb;

namespace NodaTime.ZoneInfoCompiler
{
    /// <summary>
    ///   Provides a simple string tokenizer that breaks the string into words that are separated by
    ///   white space.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     Multiple white spaces in a row are treated as one separator. White space at the beginning of
    ///     the line cause an empty token to be returned as the first token. White space at the end of
    ///     the line are ignored.
    ///   </para>
    /// </remarks>
    public class Tokens
    {
        /// <summary>
        ///   Represents an empty token list.
        /// </summary>
        private static readonly string[] NoTokens = new string[0];

        /// <summary>
        ///   The list of words. This will never be null but may be empty.
        /// </summary>
        private readonly IList<string> words;

        /// <summary>
        ///   The current index into the words list.
        /// </summary>
        private int index;

        /// <summary>
        ///   Initializes a new instance of the <see cref="Tokens" /> class.
        /// </summary>
        /// <param name="words">The words list.</param>
        private Tokens(IList<string> words)
        {
            this.words = words;
        }

        /// <summary>
        ///   Gets a value indicating whether this instance has another token.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has another token; otherwise, <c>false</c>.
        /// </value>
        public bool HasNextToken
        {
            get { return index < words.Count; }
        }

        /// <summary>
        ///   Returns the next token.
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
        ///   Returns an object that contains the list of the whitespace separated words in the given
        ///   string. The string is assumed to be culture invariant.
        /// </summary>
        /// <param name="text">The text to break into words.</param>
        /// <returns>The tokenized text.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is null.</exception>
        public static Tokens Tokenize(string text)
        {
            Preconditions.CheckNotNull(text, "text");
            text = text.TrimEnd();
            var parts = Regex.Split(text, @"\s+", RegexOptions.Compiled | RegexOptions.CultureInvariant);
            if (parts.Length == 1 && string.IsNullOrEmpty(parts[0]))
            {
                parts = NoTokens;
            }
            var list = new List<string>(parts);
            return new Tokens(list);
        }

        /// <summary>
        ///   Tries to get the next token.
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
    }
}
