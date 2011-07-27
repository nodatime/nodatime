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
using NodaTime.Format;
using NodaTime.Globalization;
using NUnit.Framework;
using System.Resources;
using NodaTime.Properties;
using System.Globalization;
using System;
#endregion

namespace NodaTime.Test.Format
{
    [TestFixture]
    [Category("Formating")]
    [Category("Format")]
    public class OffsetFormatTest
    {
        [Test]
        public void TestFormatJlk()
        {
            // OffsetFormat.Format(Offset.Zero, "hh", new NodaFormatInfo(FormattingTestSupport.EnUs));
            
            TestFormat(new OffsetFormattingTestSupport.OffsetData(5, 12, 34, 567) { C = FormattingTestSupport.FrFr, S = "+5:12:34,567", F = "g" });
            
        }
        

        [Test]
        [TestCaseSource(typeof(OffsetFormattingTestSupport), "OffsetFormattingCommonData")]
        [TestCaseSource(typeof(OffsetFormattingTestSupport), "OffsetFormatData")]
        public void TestFormat(OffsetFormattingTestSupport.OffsetData data)
        {
            FormattingTestSupport.RunFormatTest(data, () => OffsetFormat.Format(data.V, data.F, new NodaFormatInfo(data.C)));
        }
    }
}