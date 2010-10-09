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

using System;
using NodaTime.Format;
using NUnit.Framework;

namespace NodaTime.Test.Format
{
    public partial class DateTimeFormatterBuilderTest
    {
        private static DateTimeFormatter Build(Func<DateTimeFormatterBuilder, DateTimeFormatterBuilder> append)
        {
            var builder = new DateTimeFormatterBuilder();
            var customBuilder = append(builder);
            return customBuilder.ToFormatter();
        }

        private object[] PrintTestData = {
            new TestCaseData(Build(b => b.AppendCenturyOfEra(0, 4)), new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZone.Utc), "20").SetName(
                "CenturyOfEra"),
            new TestCaseData(Build(b => b.AppendYearOfCentury(0, 4)), new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZone.Utc), "4").SetName(
                "YearOfCentury"),
            new TestCaseData(Build(b => b.AppendYear(0, 4)), new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZone.Utc), "2004").SetName("Year"),
            new TestCaseData(Build(b => b.AppendMonthOfYear(0)), new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZone.Utc), "6").SetName("MonthOfYear"),
            new TestCaseData(Build(b => b.AppendWeekOfWeekYear(0)), new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZone.Utc), "24").SetName(
                "WeekOfWeekYear"),
            new TestCaseData(Build(b => b.AppendDayOfYear(0)), new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZone.Utc), "161").SetName("DayOfYear"),
            new TestCaseData(Build(b => b.AppendDayOfMonth(0)), new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZone.Utc), "9").SetName("DayOfMonth"),
            new TestCaseData(Build(b => b.AppendDayOfWeek(0)), new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZone.Utc), "3").SetName("DayOfWeek"),
            new TestCaseData(Build(b => b.AppendHourOfDay(0)), new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZone.Utc), "10").SetName("HourOfDay"),
            new TestCaseData(Build(b => b.AppendClockHourOfDay(0)), new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZone.Utc), "10").SetName(
                "ClockHourofDay"),
            new TestCaseData(Build(b => b.AppendHourOfHalfDay(0)), new ZonedDateTime(2004, 6, 9, 18, 20, 30, 40, DateTimeZone.Utc), "6").SetName("HourOfHalfDay")
            ,
            new TestCaseData(Build(b => b.AppendClockHourOfHalfDay(0)), new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZone.Utc), "10").SetName(
                "ClockHourOfHalfDay"),
            new TestCaseData(Build(b => b.AppendMinuteOfDay(0)), new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZone.Utc), "620").SetName("MinuteOfDay"),
            new TestCaseData(Build(b => b.AppendMinuteOfHour(0)), new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZone.Utc), "20").SetName("MinuteOfHour")
            ,
            new TestCaseData(Build(b => b.AppendSecondOfDay(0)), new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZone.Utc), "37230").SetName("SecondOfDay")
            ,
            new TestCaseData(Build(b => b.AppendSecondOfMinute(0)), new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZone.Utc), "30").SetName(
                "SecondOfMinute"), //TODO: Bug? (results are interchanged)
            new TestCaseData(Build(b => b.AppendMillisecondsOfDay(0)), new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZone.Utc), "37230040").SetName(
                "MillisecondsOfDay").Ignore(),
            new TestCaseData(Build(b => b.AppendMillisecondsOfSecond(0)), new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZone.Utc), "40").SetName(
                "MillisecondsOfSecond").Ignore(),
        };

        [Test]
        [TestCaseSource("PrintTestData")]
        public void AppendDecimalField_PrintFieldValue(DateTimeFormatter formatter, ZonedDateTime dt, string expectedText)
        {
            Assert.That(formatter.Print(dt), Is.EqualTo(expectedText));
        }

        private object[] ParseTestData = {
            new TestCaseData(Build(b => b.AppendYearOfEra(0, 4)), "2008", new ZonedDateTime(2008, 1, 1, 0, 0, 0, Chronology.IsoUtc)).SetName("YearOfEra"),
            new TestCaseData(Build(b => b.AppendYearOfCentury(0, 4)), "8", new ZonedDateTime(1908, 1, 1, 0, 0, 0, Chronology.IsoUtc)).SetName("YearOfCentury"),
            new TestCaseData(Build(b => b.AppendYear(0, 4)), "2008", new ZonedDateTime(2008, 1, 1, 0, 0, 0, Chronology.IsoUtc)).SetName("Year"),
            new TestCaseData(Build(b => b.AppendMonthOfYear(0)), "5", new ZonedDateTime(1970, 5, 1, 0, 0, 0, Chronology.IsoUtc)).SetName("MonthOfYear"),
            new TestCaseData(Build(b => b.AppendWeekOfWeekYear(0)), "2", new ZonedDateTime(1970, 1, 5, 0, 0, 0, Chronology.IsoUtc)).SetName("WeekOfWeekYear"),
            new TestCaseData(Build(b => b.AppendDayOfYear(0)), "32", new ZonedDateTime(1970, 2, 1, 0, 0, 0, Chronology.IsoUtc)).SetName("DayOfYear"),
            new TestCaseData(Build(b => b.AppendDayOfMonth(0)), "28", new ZonedDateTime(1970, 1, 28, 0, 0, 0, Chronology.IsoUtc)).SetName("DayOfMonth"),
            new TestCaseData(Build(b => b.AppendDayOfWeek(0)), "5", new ZonedDateTime(1970, 1, 2, 0, 0, 0, Chronology.IsoUtc)).SetName("DayOfWeek"),
            new TestCaseData(Build(b => b.AppendHourOfDay(0)), "19", new ZonedDateTime(1970, 1, 1, 19, 0, 0, Chronology.IsoUtc)).SetName("HourOfDay"),
            new TestCaseData(Build(b => b.AppendClockHourOfDay(0)), "10", new ZonedDateTime(1970, 1, 1, 10, 0, 0, Chronology.IsoUtc)).SetName("ClockHourOfDay"),
            new TestCaseData(Build(b => b.AppendHourOfHalfDay(0)), "6", new ZonedDateTime(1970, 1, 1, 6, 0, 0, Chronology.IsoUtc)).SetName("HourOfHalfDay"),
            new TestCaseData(Build(b => b.AppendClockHourOfHalfDay(0)), "5", new ZonedDateTime(1970, 1, 1, 5, 0, 0, Chronology.IsoUtc)).SetName(
                "ClockHourOfHalfDay"),
            new TestCaseData(Build(b => b.AppendMinuteOfDay(0)), "620", new ZonedDateTime(1970, 1, 1, 10, 20, 0, Chronology.IsoUtc)).SetName("MinuteOfDay"),
            new TestCaseData(Build(b => b.AppendMinuteOfHour(0)), "30", new ZonedDateTime(1970, 1, 1, 0, 30, 0, Chronology.IsoUtc)).SetName("MinuteOfHour"),
            new TestCaseData(Build(b => b.AppendSecondOfDay(0)), "37230", new ZonedDateTime(1970, 1, 1, 10, 20, 30, Chronology.IsoUtc)).SetName("SecondOfDay"),
            new TestCaseData(Build(b => b.AppendSecondOfMinute(0)), "30", new ZonedDateTime(1970, 1, 1, 0, 0, 30, Chronology.IsoUtc)).SetName("SecondOfMinute"),
            new TestCaseData(Build(b => b.AppendMillisecondsOfDay(0)), "900", new ZonedDateTime(1970, 1, 1, 0, 0, 0, 900, Chronology.IsoUtc)).SetName(
                "MillisecondsOfDay"),
            new TestCaseData(Build(b => b.AppendMillisecondsOfSecond(0)), "50", new ZonedDateTime(1970, 1, 1, 0, 0, 0, 50, Chronology.IsoUtc)).SetName(
                "MillisecondsOfSecond"),
        };

        [Test]
        [TestCaseSource("ParseTestData")]
        public void AppendDecimalField_ParseFieldValue(DateTimeFormatter formatter, string text, ZonedDateTime expectedDateTime)
        {
            var dt = formatter.Parse(text);
            Assert.That(dt, Is.EqualTo(expectedDateTime));
        }

        [Test]
        public void AppendYearOfEra_PrinterEstimateLengthAsMaxDigits()
        {
            var minDigits = 2;
            var maxDigits = 4;

            var printer = builder.AppendYearOfEra(minDigits, maxDigits).ToPrinter();

            Assert.That(printer.EstimatedPrintedLength, Is.EqualTo(maxDigits));
        }

        [Test]
        public void AppendYearOfEra_PrintYearOfEraValue_PadToMinDigits()
        {
            var minDigits = 8;
            var maxDigits = 10;
            var dt = new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZone.Utc);

            var formatter = builder.AppendYearOfEra(minDigits, maxDigits).ToFormatter();

            Assert.That(formatter.Print(dt), Is.EqualTo("00002004"));
        }
    }
}