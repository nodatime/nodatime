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

using NUnit.Framework;
using NodaTime.Globalization;
using NodaTime.Test.Text;

namespace NodaTime.Test
{
	public partial class LocalTimeTest
	{
        [Test]
        [TestCaseSource(typeof(LocalTimePatternTestSupport), "AllParseData")]
        public void TestParseExact_multiple(LocalTimePatternTestSupport.LocalTimeData data)
        {
            FormattingTestSupport.RunParseMultipleTest(data, formats => LocalTime.ParseExact(data.S, formats, new NodaFormatInfo(data.C)));
        }

        [Test]
        [TestCaseSource(typeof(LocalTimePatternTestSupport), "AllParseData")]
        public void TestParseExact(LocalTimePatternTestSupport.LocalTimeData data)
        {
            FormattingTestSupport.RunParseSingleTest(data, format => LocalTime.ParseExact(data.S, format, new NodaFormatInfo(data.C)));
        }

        [Test]
        [TestCaseSource(typeof(LocalTimePatternTestSupport), "AllParseData")]
        public void TestTryParseExact_multiple(LocalTimePatternTestSupport.LocalTimeData data)
        {
            FormattingTestSupport.RunTryParseMultipleTest(data, (string[] formats, out LocalTime value) => LocalTime.TryParseExact(data.S, formats, new NodaFormatInfo(data.C), out value));
        }

        [Test]
        [TestCaseSource(typeof(LocalTimePatternTestSupport), "AllParseData")]
        public void TestTryParseExact_single(LocalTimePatternTestSupport.LocalTimeData data)
        {
            FormattingTestSupport.RunTryParseSingleTest(data, (string format, out LocalTime value) => LocalTime.TryParseExact(data.S, format, new NodaFormatInfo(data.C), out value));
        }
	}
}
