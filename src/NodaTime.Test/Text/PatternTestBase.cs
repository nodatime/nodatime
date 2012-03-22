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

using NUnit.Framework;

namespace NodaTime.Test.Text
{
    /// <summary>
    /// Base class for all the pattern tests (when we've migrated OffsetPattern off FormattingTestSupport).
    /// Derived classes should have internal static fields with the names listed in the TestCaseSource
    /// attributes here: InvalidPatternData, ParseFailureData, ParseData, FormatData. Any field
    /// which is missing cause that test to be ignored for that concrete subclass.
    /// </summary>
    public abstract class PatternTestBase<T>
    {
        [Test]
        [TestCaseSource("InvalidPatternData")]
        public void InvalidPatterns(PatternTestData<T> data)
        {
            data.TestInvalidPattern();
        }

        [Test]
        [TestCaseSource("ParseFailureData")]
        public void ParseFailures(PatternTestData<T> data)
        {
            data.TestParseFailure();
        }

        [Test]
        [TestCaseSource("ParseData")]
        public void Parse(PatternTestData<T> data)
        {
            data.TestParse();
        }

        [Test]
        [TestCaseSource("FormatData")]
        public void Format(PatternTestData<T> data)
        {
            data.TestFormat();
        }

    }
}
