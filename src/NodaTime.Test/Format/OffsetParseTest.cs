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
using System;
using NodaTime.Format;
using NodaTime.Globalization;
using NUnit.Framework;
#endregion

namespace NodaTime.Test.Format
{
    [TestFixture]
    [Category("Formating")]
    [Category("Parse")]
    public class OffsetParseTest
    {
        internal static void RunTryParseInternal<T>(AbstractFormattingData<T> data, OffsetParseInfo parseInfo, Func<bool> test)
        {
            using (CultureSaver.SetCultures(data.ThreadCulture, data.ThreadUiCulture))
            {
                bool isSuccess = data.Kind == ParseFailureKind.None;
                Assert.IsTrue(isSuccess == test());
                data.Validate(parseInfo);
                if (isSuccess)
                {
                    Assert.AreEqual(data.V, parseInfo.Value);
                }
            }
        }

        [Test]
        [TestCaseSource(typeof(OffsetFormattingTestSupport), "OffsetFormattingCommonData")]
        //[TestCaseSource(typeof(OffsetFormattingTestSupport), "OffsetParseData")]
        public void TestParseExact_multiple(OffsetFormattingTestSupport.OffsetData data)
        {
            FormattingTestSupport.RunParseMultipleTest(data, formats => OffsetParse.ParseExact(data.S, formats, new NodaFormatInfo(data.C), data.Styles));
        }

        [Test]
        public void TestJlk()
        {
            /*
            new OffsetData(-3, 0, 0, 0) { C = EnUs, S = "-", F = "%-", PV = Offset.Zero  },
            new OffsetData(3, 0, 0, 0) { C = EnUs, S = "+", F = "%-", PV = Offset.Zero  },
            new OffsetData(-3, 0, 0, 0) { C = EnUs, S = "-", F = "%+", PV = Offset.Zero  },
            new OffsetData(3, 0, 0, 0) { C = EnUs, S = "", F = "%+", PV = Offset.Zero  },
            */
            var data = new OffsetFormattingTestSupport.OffsetData(5, 0, 0, 0)
                       {
                           C = FormattingTestSupport.EnUs,
                           S = "+5",
                           F = "g",
                       };
            TestParseExact_single(data);
        }

        [Test]
        //[TestCaseSource(typeof(OffsetFormattingTestSupport), "ParseExactCommon")]
        //[TestCaseSource(typeof(OffsetFormattingTestSupport), "ParseExactSingle")]
        //[TestCaseSource(typeof(OffsetFormattingTestSupport), "ParseExactStyle")]
        [TestCaseSource(typeof(OffsetFormattingTestSupport), "OffsetFormattingCommonData")]
        public void TestParseExact_single(OffsetFormattingTestSupport.OffsetData data)
        {
            FormattingTestSupport.RunParseSingleTest(data, format => OffsetParse.ParseExact(data.S, format, new NodaFormatInfo(data.C), data.Styles));
        }

        [Test]
        //[TestCaseSource(typeof(OffsetFormattingTestSupport), "ParseExactCommon")]
        //[TestCaseSource(typeof(OffsetFormattingTestSupport), "ParseExactSingle")]
        //[TestCaseSource(typeof(OffsetFormattingTestSupport), "ParseExactStyle")]
        [TestCaseSource(typeof(OffsetFormattingTestSupport), "OffsetFormattingCommonData")]
        public void TestTryParseExact(OffsetFormattingTestSupport.OffsetData data)
        {
            FormattingTestSupport.RunTryParseSingleTest(data, (string format, out Offset value) => OffsetParse.TryParseExact(data.S, format, new NodaFormatInfo(data.C), data.Styles, out value));
        }

        [Test]
        //[TestCaseSource(typeof(OffsetFormattingTestSupport), "ParseExactCommon")]
        //[TestCaseSource(typeof(OffsetFormattingTestSupport), "ParseExactMultiple")]
        //[TestCaseSource(typeof(OffsetFormattingTestSupport), "ParseExactStyle")]
        [TestCaseSource(typeof(OffsetFormattingTestSupport), "OffsetFormattingCommonData")]
        public void TestTryParseExactMultiple(OffsetFormattingTestSupport.OffsetData data)
        {
            FormattingTestSupport.RunTryParseMultipleTest(data, (string[] formats, out Offset value) => OffsetParse.TryParseExactMultiple(data.S, formats, new NodaFormatInfo(data.C), data.Styles, out value));
        }

        [Test]
        [TestCaseSource(typeof(OffsetFormattingTestSupport), "ParseExactCommon")]
        [TestCaseSource(typeof(OffsetFormattingTestSupport), "ParseExactMultiple")]
        [TestCaseSource(typeof(OffsetFormattingTestSupport), "ParseExactStyle")]
        public void TestTryParseExactMultiple_internal(OffsetFormattingTestSupport.OffsetData data)
        {
            string[] formats = null;
            if (data.F != null)
            {
                formats = data.F.Split('\0');
            }
            var parseInfo = new OffsetParseInfo(new NodaFormatInfo(data.C), false, data.Styles);
            RunTryParseInternal(data, parseInfo, () => OffsetParse.TryParseExactMultiple(data.S, formats, parseInfo));
        }

        [Test]
        [TestCaseSource(typeof(OffsetFormattingTestSupport), "ParseExactCommon")]
        [TestCaseSource(typeof(OffsetFormattingTestSupport), "ParseExactSingle")]
        [TestCaseSource(typeof(OffsetFormattingTestSupport), "ParseExactStyle")]
        public void TestTryParseExact_internal(OffsetFormattingTestSupport.OffsetData data)
        {
            var parseInfo = new OffsetParseInfo(new NodaFormatInfo(data.C), data.Kind == ParseFailureKind.None, data.Styles);
            RunTryParseInternal(data, parseInfo, () => OffsetParse.TryParseExact(data.S, data.F, parseInfo));
        }
    }
}