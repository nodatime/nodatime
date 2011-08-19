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
#region usings
using NUnit.Framework;
using NodaTime.Format;
using NodaTime.Globalization;

#endregion

namespace NodaTime.Test.Format
{
    [TestFixture, Category("Formatting"), Category("Parse")]
    public class OffsetParseTest
    {
        [Test]
        [TestCaseSource(typeof(OffsetFormattingTestSupport), "OffsetFormattingCommonData")]
        [TestCaseSource(typeof(OffsetFormattingTestSupport), "OffsetParseData")]
        public void TestParseExact_multiple(OffsetFormattingTestSupport.OffsetData data)
        {
            FormattingTestSupport.RunParseMultipleTest(data, formats => OffsetParse.ParseExact(data.S, formats, new NodaFormatInfo(data.C), data.Styles));
        }

        [Test]
        [TestCaseSource(typeof(OffsetFormattingTestSupport), "OffsetFormattingCommonData")]
        [TestCaseSource(typeof(OffsetFormattingTestSupport), "OffsetParseData")]
        public void TestParseExact_single(OffsetFormattingTestSupport.OffsetData data)
        {
            FormattingTestSupport.RunParseSingleTest(data, format => OffsetParse.ParseExact(data.S, format, new NodaFormatInfo(data.C), data.Styles));
        }

        [Test]
        [TestCaseSource(typeof(OffsetFormattingTestSupport), "OffsetFormattingCommonData")]
        [TestCaseSource(typeof(OffsetFormattingTestSupport), "OffsetParseData")]
        public void TestTryParseExact(OffsetFormattingTestSupport.OffsetData data)
        {
            FormattingTestSupport.RunTryParseSingleTest(data, (string format, out Offset value) => OffsetParse.TryParseExact(data.S, format, new NodaFormatInfo(data.C), data.Styles, out value));
        }

        [Test]
        [TestCaseSource(typeof(OffsetFormattingTestSupport), "OffsetFormattingCommonData")]
        [TestCaseSource(typeof(OffsetFormattingTestSupport), "OffsetParseData")]
        public void TestTryParseExactMultiple(OffsetFormattingTestSupport.OffsetData data)
        {
            FormattingTestSupport.RunTryParseMultipleTest(data, (string[] formats, out Offset value) => OffsetParse.TryParseExactMultiple(data.S, formats, new NodaFormatInfo(data.C), data.Styles, out value));
        }
    }
}
