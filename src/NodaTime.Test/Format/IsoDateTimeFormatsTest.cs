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

namespace NodaTime.Test.Format
{
    [TestFixture]
    public partial class IsoDateTimeFormatsTest
    {
        #region Zones
        private static readonly DateTimeZone UTC = DateTimeZone.Utc;
        private static readonly DateTimeZone London = DateTimeZone.ForId("Europe/London");
        private static readonly DateTimeZone Paris = DateTimeZone.ForId("Europe/Paris");
        #endregion

        private object[] DateFormatterTestData = { new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "2004-06-09"), };

        [Test]
        [TestCaseSource("DateFormatterTestData")]
        public void DateFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.Date.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] TimeFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "10:20:30.040Z"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, London), "10:20:30.040+01:00"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, Paris), "10:20:30.040+02:00"),
        };

        [Test]
        [TestCaseSource("TimeFormatterTestData")]
        public void TimeFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.Time.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] TimeWithNoMillisecondsFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "10:20:30Z"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, London), "10:20:30+01:00"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, Paris), "10:20:30+02:00"),
        };

        [Test]
        [TestCaseSource("TimeWithNoMillisecondsFormatterTestData")]
        public void TimeWithNoMillisecondsFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.TimeNoMilliseconds.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] TTimeFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "T10:20:30.040Z"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, London), "T10:20:30.040+01:00"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, Paris), "T10:20:30.040+02:00"),
        };

        [Test]
        [TestCaseSource("TTimeFormatterTestData")]
        public void TTimeFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.TTime.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] TTimeNoMillisecondsFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "T10:20:30Z"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, London), "T10:20:30+01:00"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, Paris), "T10:20:30+02:00"),
        };

        [Test]
        [TestCaseSource("TTimeNoMillisecondsFormatterTestData")]
        public void TTimeNoMillisecondsFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.TTimeNoMilliseconds.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] DateTimeFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "2004-06-09T10:20:30.040Z"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, London), "2004-06-09T10:20:30.040+01:00"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, Paris), "2004-06-09T10:20:30.040+02:00"),
        };

        [Test]
        [TestCaseSource("DateTimeFormatterTestData")]
        public void DateTimeFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.DateTime.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] DateTimeNoMillisecondsFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "2004-06-09T10:20:30Z"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, London), "2004-06-09T10:20:30+01:00"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, Paris), "2004-06-09T10:20:30+02:00"),
        };

        [Test]
        [TestCaseSource("DateTimeNoMillisecondsFormatterTestData")]
        public void DateTimeNoMillisecondsFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.DateTimeNoMilliseconds.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] OrdinalDateFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "2004-161"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, London), "2004-161"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, Paris), "2004-161"),
        };

        [Test]
        [TestCaseSource("OrdinalDateFormatterTestData")]
        public void OrdinalDateFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.OrdinalDate.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] OrdinalDateTimeFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "2004-161T10:20:30.040Z"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "2004-161T11:20:30.040+01:00"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "2004-161T12:20:30.040+02:00"),
        };

        [Test]
        [TestCaseSource("OrdinalDateTimeFormatterTestData")]
        public void OrdinalDateTimeFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.OrdinalDateTime.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] OrdinalDateTimeNoMillisecondsFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "2004-161T10:20:30Z"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "2004-161T11:20:30+01:00"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "2004-161T12:20:30+02:00"),
        };

        [Test]
        [TestCaseSource("OrdinalDateTimeNoMillisecondsFormatterTestData")]
        public void OrdinalDateTimeNoMillisecondsFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.OrdinalDateTimeNoMilliseconds.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] WeekDateFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "2004-W24-3"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "2004-W24-3"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "2004-W24-3"),
        };

        [Test]
        [TestCaseSource("WeekDateFormatterTestData")]
        public void WeekDateFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.WeekDate.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] WeekDateTimeFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "2004-W24-3T10:20:30.040Z"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "2004-W24-3T11:20:30.040+01:00"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "2004-W24-3T12:20:30.040+02:00"),
        };

        [Test]
        [TestCaseSource("WeekDateTimeFormatterTestData")]
        public void WeekDateTimeFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.WeekDateTime.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] WeekDateTimeNoMillisecondsFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "2004-W24-3T10:20:30Z"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "2004-W24-3T11:20:30+01:00"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "2004-W24-3T12:20:30+02:00"),
        };

        [Test]
        [TestCaseSource("WeekDateTimeNoMillisecondsFormatterTestData")]
        public void WeekDateTimeNoMilisecondsFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.WeekDateTimeNoMilliseconds.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] BasicDateFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "20040609"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "20040609"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "20040609"),
        };

        [Test]
        [TestCaseSource("BasicDateFormatterTestData")]
        public void BasicDateFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.BasicDate.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] BasicTimeFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "102030.040Z"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "112030.040+0100"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "122030.040+0200"),
        };

        [Test]
        [TestCaseSource("BasicTimeFormatterTestData")]
        public void BasicTimeFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.BasicTime.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] BasicTimeNoMillisecondsFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "102030Z"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "112030+0100"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "122030+0200"),
        };

        [Test]
        [TestCaseSource("BasicTimeNoMillisecondsFormatterTestData")]
        public void BasicTimeNoMillisecondsFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.BasicTimeNoMilliseconds.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] BasicTTimeFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "T102030.040Z"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "T112030.040+0100"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "T122030.040+0200"),
        };

        [Test]
        [TestCaseSource("BasicTTimeFormatterTestData")]
        public void BasicTTimeFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.BasicTTime.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] BasicTTimeNoMillisecondsFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "T102030Z"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "T112030+0100"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "T122030+0200"),
        };

        [Test]
        [TestCaseSource("BasicTTimeNoMillisecondsFormatterTestData")]
        public void BasicTTimeNoMillisecondsFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.BasicTTimeNoMilliseconds.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] BasicDateTimeFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "20040609T102030.040Z"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "20040609T112030.040+0100"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "20040609T122030.040+0200"),
        };

        [Test]
        [TestCaseSource("BasicDateTimeFormatterTestData")]
        public void BasicDateTimeFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.BasicDateTime.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] BasicDateTimeNoMillisecondsFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "20040609T102030Z"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "20040609T112030+0100"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "20040609T122030+0200"),
        };

        [Test]
        [TestCaseSource("BasicDateTimeNoMillisecondsFormatterTestData")]
        public void BasicDateTimeNoMillisecondsFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.BasicDateTimeNoMilliseconds.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] BasicOrdinalDateFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "2004161"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "2004161"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "2004161"),
        };

        [Test]
        [TestCaseSource("BasicOrdinalDateFormatterTestData")]
        public void BasicOrdinalDateFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.BasicOrdinalDate.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] BasicOrdinalDateTimeFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "2004161T102030.040Z"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "2004161T112030.040+0100"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "2004161T122030.040+0200"),
        };

        [Test]
        [TestCaseSource("BasicOrdinalDateTimeFormatterTestData")]
        public void BasicOrdinalDateTimeFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.BasicOrdinalDateTime.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] BasicOrdinalDateTimeNoMillisecondsFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "2004161T102030Z"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "2004161T112030+0100"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "2004161T122030+0200"),
        };

        [Test]
        [TestCaseSource("BasicOrdinalDateTimeNoMillisecondsFormatterTestData")]
        public void BasicOrdinalDateTimeNoMillisecondsFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.BasicOrdinalDateTimeNoMilliseconds.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] BasicWeekDateFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "2004W243"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "2004W243"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "2004W243"),
        };

        [Test]
        [TestCaseSource("BasicWeekDateFormatterTestData")]
        public void BasicWeekDateFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.BasicWeekDate.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] BasicWeekDateTimeFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "2004W243T102030.040Z"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "2004W243T112030.040+0100"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "2004W243T122030.040+0200"),
        };

        [Test]
        [TestCaseSource("BasicWeekDateTimeFormatterTestData")]
        public void BasicWeekDateTimeFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.BasicWeekDateTime.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] BasicWeekDateTimeNoMillisecondsFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "2004W243T102030Z"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "2004W243T112030+0100"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "2004W243T122030+0200"),
        };

        [Test]
        [TestCaseSource("BasicWeekDateTimeNoMillisecondsFormatterTestData")]
        public void BasicWeekDateTimeNoMillisecondsFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.BasicWeekDateTimeNoMilliseconds.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] YearFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "2004"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "2004"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "2004"),
        };

        [Test]
        [TestCaseSource("YearFormatterTestData")]
        public void YearFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.Year.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] YearMonthFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "2004-06"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "2004-06"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "2004-06"),
        };

        [Test]
        [TestCaseSource("YearMonthFormatterTestData")]
        public void YearMonthFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.YearMonth.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] YearMonthDayFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "2004-06-09"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "2004-06-09"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "2004-06-09"),
        };

        [Test]
        [TestCaseSource("YearMonthDayFormatterTestData")]
        public void YearMonthDayFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.YearMonthDay.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] WeekYearFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "2004"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "2004"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "2004"),
        };

        [Test]
        [TestCaseSource("WeekYearFormatterTestData")]
        public void WeekYearFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.WeekYear.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] WeekYearWeekFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "2004-W24"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "2004-W24"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "2004-W24"),
        };

        [Test]
        [TestCaseSource("WeekYearWeekFormatterTestData")]
        public void WeekYearWeekFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.WeekYearWeek.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] WeekYearWeekDayFormatterTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "2004-W24-3"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "2004-W24-3"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "2004-W24-3"),
        };

        [Test]
        [TestCaseSource("WeekYearWeekDayFormatterTestData")]
        public void WeekYearWeekDayFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.WeekYearWeekDay.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] HourTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "10"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "11"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "12"),
        };

        [Test]
        [TestCaseSource("HourTestData")]
        public void HourFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.Hour.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] HourMinuteTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "10:20"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "11:20"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "12:20"),
        };

        [Test]
        [TestCaseSource("HourMinuteTestData")]
        public void HourMinuteFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.HourMinute.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] HourMinuteSecondTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "10:20:30"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "11:20:30"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "12:20:30"),
        };

        [Test]
        [TestCaseSource("HourMinuteSecondTestData")]
        public void HourMinuteSecondFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.HourMinuteSecond.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] HourMinuteSecondMillisecondsTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "10:20:30.040"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "11:20:30.040"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "12:20:30.040"),
        };

        [Test]
        [TestCaseSource("HourMinuteSecondMillisecondsTestData")]
        public void HourMinuteSecondMillisecondsFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.HourMinuteSecondMilliseconds.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] HourMinuteSecondFractionTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "10:20:30.040"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "11:20:30.040"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "12:20:30.040"),
        };

        [Test]
        [TestCaseSource("HourMinuteSecondFractionTestData")]
        public void HourMinuteSecondFractionFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.HourMinuteSecondFraction.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] DateHourTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "2004-06-09T10"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "2004-06-09T11"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "2004-06-09T12"),
        };

        [Test]
        [TestCaseSource("DateHourTestData")]
        public void DateHourFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.DateHour.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] DateHourMinuteTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "2004-06-09T10:20"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "2004-06-09T11:20"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "2004-06-09T12:20"),
        };

        [Test]
        [TestCaseSource("DateHourMinuteTestData")]
        public void DateHourMinuteFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.DateHourMinute.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] DateHourMinuteSecondTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "2004-06-09T10:20:30"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "2004-06-09T11:20:30"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "2004-06-09T12:20:30"),
        };

        [Test]
        [TestCaseSource("DateHourMinuteSecondTestData")]
        public void DateHourMinuteSecondFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.DateHourMinuteSecond.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] DateHourMinuteSecondMillisecondsTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "2004-06-09T10:20:30.040"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "2004-06-09T11:20:30.040"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "2004-06-09T12:20:30.040"),
        };

        [Test]
        [TestCaseSource("DateHourMinuteSecondMillisecondsTestData")]
        public void DateHourMinuteSecondMillisecondsFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.DateHourMinuteSecondMilliseconds.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        private object[] DateHourMinuteSecondFractionTestData = {
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), "2004-06-09T10:20:30.040"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 11, 20, 30, 40, London), "2004-06-09T11:20:30.040"),
            new TestCaseData(CreateZonedDateTime(2004, 6, 9, 12, 20, 30, 40, Paris), "2004-06-09T12:20:30.040"),
        };

        [Test]
        [TestCaseSource("DateHourMinuteSecondFractionTestData")]
        public void DateHourMinuteSecondFractionFormatter_Prints(ZonedDateTime dateTime, string dateTimeText)
        {
            Assert.That(IsoDateTimeFormats.DateHourMinuteSecondFraction.Print(dateTime), Is.EqualTo(dateTimeText));
        }

        /// <summary>
        /// Single method to handle creating a ZonedDateTime so that while we mess around with
        /// organization, we don't need to change multiple calls.
        /// </summary>
        private static ZonedDateTime CreateZonedDateTime(int year, int month, int day, int hour, int minute, int second, int millis, DateTimeZone zone)
        {
            return zone.AtExactly(new LocalDateTime(year, month, day, hour, minute, second, millis));
        }
    }
}