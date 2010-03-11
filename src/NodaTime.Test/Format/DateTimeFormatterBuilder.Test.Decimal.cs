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
using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test.Format
{
    public partial class DateTimeFormatterBuilderTest
    {
        private static DateTimeFormatter Build(Func<DateTimeFormatterBuilder, DateTimeFormatterBuilder> append)
        {
            var builder =  new DateTimeFormatterBuilder();
            var customBuilder = append(builder);
            return customBuilder.ToFormatter();
        }

        object[] PrintTestData =
        {
//TODO: is this a bug in CenturyOfYearField ? (formatter prints 20, but not 21)
            //new TestCaseData( new DateTimeFormatterBuilder()
            //                    .AppendCenturyOfEra(0, 4)
            //                    .ToFormatter(),
            //                    new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZones.Utc),
            //                    "21"),

            new TestCaseData( Build(b=>b.AppendYear(0,4)),
                                new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZones.Utc),
                                "2004"),

            new TestCaseData( Build(b=>b.AppendYearOfCentury(0,4)),
                                new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZones.Utc),
                                "4"),

            new TestCaseData( Build(b=>b.AppendMonthOfYear(0)),
                                new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZones.Utc),
                                "6"),

            new TestCaseData( Build(b=>b.AppendWeekOfWeekYear(0)),
                                new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZones.Utc),
                                "24"),

            new TestCaseData( Build(b=>b.AppendDayOfYear(0)),
                                new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZones.Utc),
                                "161"),

            new TestCaseData( Build(b=>b.AppendDayOfMonth(0)),
                                new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZones.Utc),
                                "9"),

            new TestCaseData( Build(b=>b.AppendDayOfWeek(0)),
                                new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZones.Utc),
                                "3"),

            new TestCaseData( Build(b=>b.AppendHourOfDay(0)),
                                new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZones.Utc),
                                "10"),

            new TestCaseData( Build(b=>b.AppendClockHourOfDay(0)),
                                new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZones.Utc),
                                "10"),

            new TestCaseData( Build(b=>b.AppendHourOfHalfDay(0)),
                                new ZonedDateTime(2004, 6, 9, 18, 20, 30, 40, DateTimeZones.Utc),
                                "6"),

            new TestCaseData( Build(b=>b.AppendClockHourOfHalfDay(0)),
                                new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZones.Utc),
                                "10"),

            new TestCaseData( Build(b=>b.AppendMinuteOfDay(0)),
                                new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZones.Utc),
                                "620"),

            new TestCaseData( Build(b=>b.AppendMinuteOfHour(0)),
                                new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZones.Utc),
                                "20"),

            new TestCaseData( Build(b=>b.AppendSecondOfDay(0)),
                                new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZones.Utc),
                                "37230"),

            new TestCaseData( Build(b=>b.AppendSecondOfMinute(0)),
                                new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZones.Utc),
                                "30"),
//TODO: Bug?
            //new TestCaseData( Build(b=>b.AppendMillisecondsOfDay(0)),
            //                    new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZones.Utc),
            //                    "37230040"),

            //new TestCaseData( Build(b=>b.AppendMillisecondsOfSecond(0)),
            //                    new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZones.Utc),
            //                    "40"),

        };

        [Test]
        [TestCaseSource("PrintTestData")]
        public void AppendDecimalField_PrintFieldValue(DateTimeFormatter formatter, ZonedDateTime dt, string expectedTezt)
        {
            Assert.That(formatter.Print(dt), Is.EqualTo(expectedTezt));
        }

        [Test]
        public void AppendYearOfEra_PrinterEstimateLengthAsMaxDigits()
        {
            var minDigits = 2;
            var maxDigits = 4;

            var printer = builder
                .AppendYearOfEra(minDigits, maxDigits)
                .ToPrinter();

            Assert.That(printer.EstimatedPrintedLength, Is.EqualTo(maxDigits));
        }

        [Test]
        public void AppendYearOfEra_PrintYearOfEraValue_PadToMinDigits()
        {
            var minDigits = 8;
            var maxDigits = 10;
            var dt = new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZones.Utc);

            var formatter = builder
                .AppendYearOfEra(minDigits, maxDigits)
                .ToFormatter();

            Assert.That(formatter.Print(dt), Is.EqualTo("00002004"));
        }
    }
}
