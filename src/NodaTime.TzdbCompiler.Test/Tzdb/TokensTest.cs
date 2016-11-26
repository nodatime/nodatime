// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
using System;
using System.Collections.Generic;
using NodaTime.TzdbCompiler.Tzdb;

namespace NodaTime.TzdbCompiler.Test.Tzdb
{
    /// <summary>
    /// This is a test class for containing all of the ParserHelper unit tests.
    ///</summary>
    public class TokensTest
    {
        [Test]
        public void Tokenize_nullArgument_Exception()
        {
            string line = null;
            Assert.Throws(typeof(ArgumentNullException), () => Tokens.Tokenize(line));
        }

        [Test]
        [TestCase("", new string[0])]
        [TestCase("One", new[] { "One" })]
        [TestCase("One Two  \tThree\n\nFour   ", new[] { "One", "Two", "Three", "Four" })]
        [TestCase("  One Two  \tThree\n\nFour   ", new[] { "", "One", "Two", "Three", "Four" })]
        [TestCase("One \"TwoA TwoB\" Three", new[] { "One", "TwoA TwoB", "Three" })]
        [TestCase("One X\"TwoA TwoB\"Y Three", new[] { "One", "XTwoA TwoBY", "Three" })]
        [TestCase("One \" Spaced \" Three", new[] { "One", " Spaced ", "Three" })]
        public void Tokenize(string line, string[] expectedTokens)
        {
            var actualTokens = Tokens.Tokenize(line);
            AssertTokensEqual(expectedTokens, actualTokens);
        }

        [Test]
        public void ParseOffset_ZeroHoursWithMinutesAndSeconds()
        {
            // Initial offset for Paris
            const string text = "0:09:21";
            var offset = ParserHelper.ParseOffset(text);
            Duration duration = Duration.FromMinutes(9) + Duration.FromSeconds(21);
            Assert.AreEqual(Offset.FromTicks(duration.BclCompatibleTicks), offset);
        }

        [Test]
        public void ParseOffset_NegativeZeroHoursWithMinutesAndSeconds()
        {
            // Initial offset for Ouagadougou
            const string text = "-0:06:04";
            var offset = ParserHelper.ParseOffset(text);
            Duration duration = Duration.FromMinutes(6) + Duration.FromSeconds(4);
            Assert.AreEqual(Offset.FromTicks(-duration.BclCompatibleTicks), offset);
        }

        private static void AssertTokensEqual(IList<string> expectedTokens, Tokens tokens)
        {
            for (int i = 0; i < expectedTokens.Count; i++)
            {
                Assert.True(tokens.HasNextToken, "Not enough items in enumeration");
                var actual = tokens.NextToken(i.ToString());
                if (actual == null)
                {
                    Assert.Fail("The enumeration item at index [" + i + "] is null");
                }
                Assert.AreEqual(expectedTokens[i], actual, "The enumeration item at index [" + i + "] is not correct");
            }
            Assert.False(tokens.HasNextToken, "Too many items in enumeration");
        }
    }
}