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
using NodaTime.Format;
using NUnit.Framework;

namespace NodaTime.Test.Format
{
    public abstract class ParsableTest
    {
        [Test]
        public void TestConstructor()
        {
            const string testString = "test";
            Parsable parsable = MakeParsable(testString);
            ValidateParsable(parsable, testString);
            ValidateBeginningOfString(parsable);
        }

        [Test]
        public void TestConstructor_empty()
        {
            Assert.Throws<ArgumentException>(() => MakeParsable(""));
        }

        [Test]
        public void TestConstructor_null()
        {
            Assert.Throws<ArgumentNullException>(() => MakeParsable(null));
        }

        [Test]
        public void TestGetNextCharacter()
        {
            Parsable parsable = MakeParsable("test");
            ValidateBeginningOfString(parsable);
            Assert.AreEqual('t', parsable.GetNextCharacter());
            ValidateCharacter(parsable, 0, 't');
            Assert.AreEqual('e', parsable.GetNextCharacter());
            ValidateCharacter(parsable, 1, 'e');
            Assert.AreEqual('s', parsable.GetNextCharacter());
            ValidateCharacter(parsable, 2, 's');
            Assert.AreEqual('t', parsable.GetNextCharacter());
            ValidateCharacter(parsable, 3, 't');
            Assert.Throws<FormatException>(() => parsable.GetNextCharacter());
            ValidateEndOfString(parsable);
        }

        [Test]
        public void TestMove()
        {
            Parsable parsable = MakeParsable("test");
            ValidateBeginningOfString(parsable);
            Assert.True(parsable.Move(0));
            ValidateCharacter(parsable, 0, 't');
            Assert.True(parsable.Move(1));
            ValidateCharacter(parsable, 1, 'e');
            Assert.True(parsable.Move(2));
            ValidateCharacter(parsable, 2, 's');
            Assert.True(parsable.Move(3));
            ValidateCharacter(parsable, 3, 't');
            Assert.False(parsable.Move(4));
            ValidateEndOfString(parsable);
        }

        [Test]
        public void TestMove_NextPrevious()
        {
            Parsable parsable = MakeParsable("test");
            ValidateBeginningOfString(parsable);
            Assert.True(parsable.Move(2), "Move(2)");
            ValidateCharacter(parsable, 2, 's');
            Assert.True(parsable.MovePrevious(), "MovePrevious()");
            ValidateCharacter(parsable, 1, 'e');
            Assert.True(parsable.MoveCurrent(), "MoveCurrent()");
            ValidateCharacter(parsable, 1, 'e');
            Assert.True(parsable.MoveNext(), "MoveNext()");
            ValidateCharacter(parsable, 2, 's');
        }

        [Test]
        public void TestMove_invalid()
        {
            Parsable parsable = MakeParsable("test");
            ValidateBeginningOfString(parsable);
            Assert.False(parsable.Move(-1000));
            ValidateBeginningOfString(parsable);
            Assert.False(parsable.Move(1000));
            ValidateEndOfString(parsable);
            Assert.False(parsable.Move(-1000));
            ValidateBeginningOfString(parsable);
        }

        [Test]
        public void TestSkipWhiteSpaces()
        {
            const string testString = " \n  test \n  ";
            Parsable parsable = MakeParsable(testString);
            ValidateParsable(parsable, testString);
            Assert.True(parsable.MoveNext(), "MoveNext()");
            ValidateCharacter(parsable, 0, ' ');
            Assert.True(parsable.SkipWhiteSpaces());
            ValidateParsable(parsable, testString);
            ValidateCharacter(parsable, 4, 't');
        }

        [Test]
        public void TestSkipWhiteSpaces_end()
        {
            const string testString = "t   ";
            Parsable parsable = MakeParsable(testString);
            ValidateParsable(parsable, testString);
            Assert.True(parsable.Move(1), "Move(1)");
            ValidateCharacter(parsable, 1, ' ');
            Assert.False(parsable.SkipWhiteSpaces());
            ValidateParsable(parsable, testString);
            ValidateEndOfString(parsable);
        }

        [Test]
        public void TestSkipWhiteSpaces_inside()
        {
            const string testString = "t   est";
            Parsable parsable = MakeParsable(testString);
            ValidateParsable(parsable, testString);
            Assert.True(parsable.Move(1), "Move(1)");
            ValidateCharacter(parsable, 1, ' ');
            Assert.True(parsable.SkipWhiteSpaces());
            ValidateParsable(parsable, testString);
            ValidateCharacter(parsable, 4, 'e');
        }

        [Test]
        public void TestTrimLeadingInQuoteSpaces()
        {
            const string testString = "' \n abc  'test";
            const string trimmedString = "'abc  'test";
            Parsable parsable = MakeParsable(testString);
            ValidateParsable(parsable, testString);
            parsable.TrimLeadingInQuoteSpaces();
            ValidateParsable(parsable, trimmedString);
            ValidateBeginningOfString(parsable);
        }

        [Test]
        public void TestTrimLeadingInQuoteSpaces_noQuote()
        {
            const string testString = " test";
            Parsable parsable = MakeParsable(testString);
            ValidateParsable(parsable, testString);
            parsable.TrimLeadingInQuoteSpaces();
            ValidateParsable(parsable, testString);
            ValidateBeginningOfString(parsable);
        }

        [Test]
        public void TestTrimLeadingWhiteSpaces()
        {
            const string testString = " \n  test \n  ";
            const string trimmedString = "test \n  ";
            Parsable parsable = MakeParsable(testString);
            ValidateParsable(parsable, testString);
            parsable.TrimLeadingWhiteSpaces();
            ValidateParsable(parsable, trimmedString);
            ValidateBeginningOfString(parsable);
        }

        [Test]
        public void TestTrimTrailingInQuoteSpaces()
        {
            const string testString = " \n  test'  abc\n  '";
            const string trimmedString = " \n  test'  abc'";
            Parsable parsable = MakeParsable(testString);
            ValidateParsable(parsable, testString);
            parsable.TrimTrailingInQuoteSpaces();
            ValidateParsable(parsable, trimmedString);
            ValidateBeginningOfString(parsable);
        }

        [Test]
        public void TestTrimTrailingInQuoteSpaces_noQuote()
        {
            const string testString = "test ";
            Parsable parsable = MakeParsable(testString);
            ValidateParsable(parsable, testString);
            parsable.TrimTrailingInQuoteSpaces();
            ValidateParsable(parsable, testString);
            ValidateBeginningOfString(parsable);
        }

        [Test]
        public void TestTrimTrailingWhiteSpaces()
        {
            const string testString = " \n  test \n  ";
            const string trimmedString = " \n  test";
            Parsable parsable = MakeParsable(testString);
            ValidateParsable(parsable, testString);
            Assert.True(parsable.MoveNext(), "MoveNext()");
            ValidateCharacter(parsable, 0, ' ');
            parsable.TrimTrailingWhiteSpaces();
            ValidateParsable(parsable, trimmedString);
            ValidateBeginningOfString(parsable);
        }

        internal static void ValidateBeginningOfString(Parsable parsable)
        {
            ValidateCharacter(parsable, -1, Parsable.Nul);
        }

        internal static void ValidateCharacter(Parsable parsable, int index, char current)
        {
            TestHelper.AssertCharEqual(current, parsable.Current, "Parsable Current mismatch");
            Assert.AreEqual(index, parsable.Index, "Parsable Index mismatch");
        }

        internal static void ValidateEndOfString(Parsable parsable)
        {
            ValidateCharacter(parsable, parsable.Length, Parsable.Nul);
        }

        internal static void ValidateParsable(Parsable parsable, string value, int length = -1)
        {
            if (length < 0)
            {
                length = value.Length;
            }
            Assert.AreEqual(value, parsable.Value, "Parsable Value mismatch");
            Assert.AreEqual(length, parsable.Length, "Parsable Length mismatch");
        }

        internal abstract Parsable MakeParsable(string value);
    }
}