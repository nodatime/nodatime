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
using System.Collections.Generic;
using NUnit.Framework;
using NodaTime;
using NodaTime.ZoneInfoCompiler;
using NodaTime.ZoneInfoCompiler.Tzdb;

namespace ZoneInfoCompiler.Test
{
    /// <summary>
    /// This is a test class for containing all of the ParserHelper unit tests.
    ///</summary>
    [TestFixture]
    public class TokensTest
    {
        private static readonly string[] EmptyTokenList = new string[0];

        private const string OneInput = "One";
        private static readonly string[] OneTokenList = { "One" };

        private const string MultipleInput = "One Two  \tThree\n\nFour   ";
        private static readonly string[] MultipleTokenList = { "One", "Two", "Three", "Four" };

        private const string LeadingSpacesMultipleInput = "  One Two  \tThree\n\nFour   ";
        private static readonly string[] LeadingSpacesMultipleTokenList = { "", "One", "Two", "Three", "Four" };

        [Test]
        public void Tokenize_nullArgument_Exception()
        {
            string line = null;
            Assert.Throws(typeof(ArgumentNullException), () => Tokens.Tokenize(line));
        }

        [Test]
        public void Tokenize_emptyString_noTokens()
        {
            var line = string.Empty;
            var tokens = Tokens.Tokenize(line);
            ValidateTokens(tokens, EmptyTokenList);
        }

        [Test]
        public void Tokenize_oneToken()
        {
            var tokens = Tokens.Tokenize(OneInput);
            ValidateTokens(tokens, OneTokenList);
        }

        [Test]
        public void Tokenize_multipleToken()
        {
            var tokens = Tokens.Tokenize(MultipleInput);
            ValidateTokens(tokens, MultipleTokenList);
        }

        [Test]
        public void Tokenize_leadingSpacesMultipleToken()
        {
            var tokens = Tokens.Tokenize(LeadingSpacesMultipleInput);
            ValidateTokens(tokens, LeadingSpacesMultipleTokenList);
        }

        [Test]
        public void ParseOffset_ZeroHoursWithMinutesAndSeconds()
        {
            // Initial offset for Paris
            const string text = "0:09:21";
            var offset = ParserHelper.ParseOffset(text);
            Assert.AreEqual(Offset.Create(0, 9, 21, 0), offset);
        }

        [Test]
        public void ParseOffset_NegativeZeroHoursWithMinutesAndSeconds()
        {
            // Initial offset for Ouagadougou
            const string text = "-0:06:04";
            var offset = ParserHelper.ParseOffset(text);
            Assert.AreEqual(Offset.Create(0, 6, 4, 0, true), offset);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="expected"></param>
        private static void ValidateTokens(Tokens tokens, IList<string> expected)
        {
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.True(tokens.HasNextToken, "Not enough items in enumertion");
                var actual = tokens.NextToken(i.ToString());
                if (actual == null)
                {
                    Assert.Fail("The enumeration item at index [" + i + "] is null");
                }
                Assert.AreEqual(expected[i], actual, "The enumeration item at index [" + i + "] is not correct");
            }
            Assert.False(tokens.HasNextToken, "Too many items in enumeration");
        }
    }
}