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

using NodaTime.Format;
using NodaTime.TimeZones;

using NUnit.Framework;
using System;

namespace NodaTime.Test.Format
{
    public partial class IsoDateTimeFormatsTest
    {
        object[] DateElementParserTestData =
        {
            new TestCaseData("2006-06-09", new ZonedDateTime(2006, 6, 9, 0, 0, 0, 0, UTC) ),
            new TestCaseData("2006-06-9", new ZonedDateTime(2006, 6, 9, 0, 0, 0, 0, UTC) ),
            new TestCaseData("2006-6-09", new ZonedDateTime(2006, 6, 9, 0, 0, 0, 0, UTC) ),
            new TestCaseData("2006-6-9", new ZonedDateTime(2006, 6, 9, 0, 0, 0, 0, UTC) ),
            //Bug with weeks calculation
            new TestCaseData("2006-W27-3", new ZonedDateTime(2006, 7, 5, 0, 0, 0, 0, UTC) ),
            new TestCaseData("2006-123", new ZonedDateTime(2006, 5, 3, 0, 0, 0, 0, UTC) ),

        };

        [Test]
        [TestCaseSource("DateElementParserTestData")]
        public void DateElementParser_Parses(string dateTimeText, ZonedDateTime dateTime)
        {
            var result = IsoDateTimeFormats.DateElementParser.Parse(dateTimeText);
            Assert.That(result.ToInstant(), Is.EqualTo(dateTime.ToInstant()));
        }

        object[] DateParserTestData =
        {
            new TestCaseData("2006-06-09", new ZonedDateTime(2006, 6, 9, 0, 0, 0, 0, UTC) ),
            new TestCaseData("2006-06-9", new ZonedDateTime(2006, 6, 9, 0, 0, 0, 0, UTC) ),
            new TestCaseData("2006-6-09", new ZonedDateTime(2006, 6, 9, 0, 0, 0, 0, UTC) ),
            new TestCaseData("2006-6-9", new ZonedDateTime(2006, 6, 9, 0, 0, 0, 0, UTC) ),
            //Bug with weeks calculation
            new TestCaseData("2006-W27-3", new ZonedDateTime(2006, 7, 5, 0, 0, 0, 0, UTC) ),
            new TestCaseData("2006-123", new ZonedDateTime(2006, 5, 3, 0, 0, 0, 0, UTC) ),

        };

        [Test]
        [TestCaseSource("DateParserTestData")]
        public void DateParser_Parses(string dateTimeText, ZonedDateTime dateTime)
        {
            var result = IsoDateTimeFormats.DateParser.Parse(dateTimeText);
            Assert.That(result.ToInstant(), Is.EqualTo(dateTime.ToInstant()));
        }

    }
}
