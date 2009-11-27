#region Copyright and license information
// Copyright 2009 Jon Skeet
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
using NodaTime.ZoneInfoCompiler;
using NUnit.Framework;

namespace ZoneInfoCompiler.Test
{    
    /// <summary>
    /// This is a test class for containing all of the ParserHelper unit tests.
    ///</summary>
    [TestFixture]
    public class TokensTest
    {
        private static string[] EmptyTokenList = new string[0];

        private static string OneInput = "One";
        private static string[] OneTokenList = { "One" };

        private static string MultipleInput = "One Two  \tThree\n\nFour   ";
        private static string[] MultipleTokenList = { "One", "Two", "Three", "Four" };

        private static string LeadingSpacesMultipleInput = "  One Two  \tThree\n\nFour   ";
        private static string[] LeadingSpacesMultipleTokenList = { "", "One", "Two", "Three", "Four" };

        [Test]
        public void Tokenize_nullArgument_Exception()
        {
            string line = null;
            Assert.Throws(typeof(ArgumentNullException), () => Tokens.Tokenize(line));
        }

        [Test]
        public void Tokenize_emptyString_noTokens()
        {
            string line = string.Empty;
            Tokens tokens = Tokens.Tokenize(line);
            ValidateTokens(tokens, EmptyTokenList);
        }

        [Test]
        public void Tokenize_oneToken()
        {
            string line = OneInput;
            Tokens tokens = Tokens.Tokenize(line);
            ValidateTokens(tokens, OneTokenList);
        }

        [Test]
        public void Tokenize_multipleToken()
        {
            string line = MultipleInput;
            Tokens tokens = Tokens.Tokenize(line);
            ValidateTokens(tokens, MultipleTokenList);
        }

        [Test]
        public void Tokenize_leadingSpacesMultipleToken()
        {
            string line = LeadingSpacesMultipleInput;
            Tokens tokens = Tokens.Tokenize(line);
            ValidateTokens(tokens, LeadingSpacesMultipleTokenList);
        }

        /// <summary>
        /// Validates the enumerator results.
        /// </summary>
        /// <param name="en">The enumerator to validate.</param>
        /// <param name="expected">The expected values.</param>
        private void ValidateTokens(Tokens tokens, string[] expected)
        {
            for (int i = 0; i < expected.Length; i++) {
                Assert.True(tokens.HasNextToken, "Not enough items in enumertion");
                string actual = tokens.NextToken(i.ToString());
                if (actual == null) {
                    Assert.Fail("The enumeration item at index [" + i + "] is null");
                }
                Assert.AreEqual(expected[i], actual, "The enumeration item at index [" + i + "] is not correct");
            }
            Assert.False(tokens.HasNextToken, "Too many items in enumeration");
        }
    }
}
