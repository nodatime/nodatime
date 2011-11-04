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
using NUnit.Framework;

namespace NodaTime.Test
{
    public partial class LocalDateTest
    {
        [Test]
        public void PlusYear_Simple()
        {
            LocalDate start = new LocalDate(2011, 6, 26);
            LocalDate expected = new LocalDate(2016, 6, 26);
            Assert.AreEqual(expected, start.PlusYears(5));

            expected = new LocalDate(2006, 6, 26);
            Assert.AreEqual(expected, start.PlusYears(-5));
        }

        [Test]
        public void PlusYear_LeapToNonLeap()
        {
            LocalDate start = new LocalDate(2012, 2, 29);
            LocalDate expected = new LocalDate(2013, 2, 28);
            Assert.AreEqual(expected, start.PlusYears(1));

            expected = new LocalDate(2011, 2, 28);
            Assert.AreEqual(expected, start.PlusYears(-1));
        }

        [Test]
        public void PlusYear_LeapToLeap()
        {
            LocalDate start = new LocalDate(2012, 2, 29);
            LocalDate expected = new LocalDate(2016, 2, 29);
            Assert.AreEqual(expected, start.PlusYears(4));
        }

        [Test]
        public void PlusMonth_Simple()
        {
            LocalDate start = new LocalDate(2012, 4, 15);
            LocalDate expected = new LocalDate(2012, 8, 15);
            Assert.AreEqual(expected, start.PlusMonths(4));
        }

        [Test]
        public void PlusMonth_ChangingYear()
        {
            LocalDate start = new LocalDate(2012, 10, 15);
            LocalDate expected = new LocalDate(2013, 2, 15);
            Assert.AreEqual(expected, start.PlusMonths(4));
        }

        [Test]
        public void PlusMonth_WithTruncation()
        {
            LocalDate start = new LocalDate(2011, 1, 30);
            LocalDate expected = new LocalDate(2011, 2, 28);
            Assert.AreEqual(expected, start.PlusMonths(1));
        }

        [Test]
        public void PlusDays_Simple()
        {
            LocalDate start = new LocalDate(2011, 1, 15);
            LocalDate expected = new LocalDate(2011, 1, 23);
            Assert.AreEqual(expected, start.PlusDays(8));

            expected = new LocalDate(2011, 1, 7);
            Assert.AreEqual(expected, start.PlusDays(-8));
        }

        [Test]
        public void PlusDays_MonthBoundary()
        {
            LocalDate start = new LocalDate(2011, 1, 26);
            LocalDate expected = new LocalDate(2011, 2, 3);
            Assert.AreEqual(expected, start.PlusDays(8));

            // Round-trip back across the boundary
            Assert.AreEqual(start, start.PlusDays(8).PlusDays(-8));
        }

        [Test]
        public void PlusDays_YearBoundary()
        {
            LocalDate start = new LocalDate(2011, 12, 26);
            LocalDate expected = new LocalDate(2012, 1, 3);
            Assert.AreEqual(expected, start.PlusDays(8));

            // Round-trip back across the boundary
            Assert.AreEqual(start, start.PlusDays(8).PlusDays(-8));
        }

        [Test]
        public void PlusDays_EndOfFebruary_InLeapYear()
        {
            LocalDate start = new LocalDate(2012, 2, 26);
            LocalDate expected = new LocalDate(2012, 3, 5);
            Assert.AreEqual(expected, start.PlusDays(8));
            // Round-trip back across the boundary
            Assert.AreEqual(start, start.PlusDays(8).PlusDays(-8));
        }

        [Test]
        public void PlusDays_EndOfFebruary_NotInLeapYear()
        {
            LocalDate start = new LocalDate(2011, 2, 26);
            LocalDate expected = new LocalDate(2011, 3, 6);
            Assert.AreEqual(expected, start.PlusDays(8));

            // Round-trip back across the boundary
            Assert.AreEqual(start, start.PlusDays(8).PlusDays(-8));
        }

        [Test]
        public void PlusWeeks_Simple()
        {
            LocalDate start = new LocalDate(2011, 4, 2);
            LocalDate expectedForward = new LocalDate(2011, 4, 23);
            LocalDate expectedBackward = new LocalDate(2011, 3, 12);
            Assert.AreEqual(expectedForward, start.PlusWeeks(3));
            Assert.AreEqual(expectedBackward, start.PlusWeeks(-3));
        }

        // Each test case gives a day-of-month in November 2011 and a target "next day of week";
        // the result is the next day-of-month in November 2011 with that target day.
        // The tests are picked somewhat arbitrarily...
        [TestCase(10, IsoDayOfWeek.Wednesday, Result = 16)]
        [TestCase(10, IsoDayOfWeek.Friday, Result = 11)]
        [TestCase(10, IsoDayOfWeek.Thursday, Result = 17)]
        [TestCase(11, IsoDayOfWeek.Wednesday, Result = 16)]
        [TestCase(11, IsoDayOfWeek.Thursday, Result = 17)]
        [TestCase(11, IsoDayOfWeek.Friday, Result = 18)]
        [TestCase(11, IsoDayOfWeek.Saturday, Result = 12)]
        [TestCase(11, IsoDayOfWeek.Sunday, Result = 13)]
        [TestCase(12, IsoDayOfWeek.Friday, Result = 18)]
        [TestCase(13, IsoDayOfWeek.Friday, Result = 18)]
        public int Next(int dayOfMonth, IsoDayOfWeek targetDayOfWeek)
        {
            LocalDate start = new LocalDate(2011, 11, dayOfMonth);
            LocalDate target = start.Next(targetDayOfWeek);
            Assert.AreEqual(2011, target.Year);
            Assert.AreEqual(11, target.MonthOfYear);
            return target.DayOfMonth;
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(8)]
        public void Next_InvalidArgument(IsoDayOfWeek targetDayOfWeek)
        {
            LocalDate start = new LocalDate(2011, 1, 1);
            Assert.Throws<ArgumentOutOfRangeException>(() => start.Next(targetDayOfWeek));
        }

        // Each test case gives a day-of-month in November 2011 and a target "next day of week";
        // the result is the next day-of-month in November 2011 with that target day.
        [TestCase(10, IsoDayOfWeek.Wednesday, Result = 9)]
        [TestCase(10, IsoDayOfWeek.Friday, Result = 4)]
        [TestCase(10, IsoDayOfWeek.Thursday, Result = 3)]
        [TestCase(11, IsoDayOfWeek.Wednesday, Result = 9)]
        [TestCase(11, IsoDayOfWeek.Thursday, Result = 10)]
        [TestCase(11, IsoDayOfWeek.Friday, Result = 4)]
        [TestCase(11, IsoDayOfWeek.Saturday, Result = 5)]
        [TestCase(11, IsoDayOfWeek.Sunday, Result = 6)]
        [TestCase(12, IsoDayOfWeek.Friday, Result = 11)]
        [TestCase(13, IsoDayOfWeek.Friday, Result = 11)]
        public int Previous(int dayOfMonth, IsoDayOfWeek targetDayOfWeek)
        {
            LocalDate start = new LocalDate(2011, 11, dayOfMonth);
            LocalDate target = start.Previous(targetDayOfWeek);
            Assert.AreEqual(2011, target.Year);
            Assert.AreEqual(11, target.MonthOfYear);
            return target.DayOfMonth;
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(8)]
        public void Previous_InvalidArgument(IsoDayOfWeek targetDayOfWeek)
        {
            LocalDate start = new LocalDate(2011, 1, 1);
            Assert.Throws<ArgumentOutOfRangeException>(() => start.Previous(targetDayOfWeek));
        }

        // No tests for non-ISO-day-of-week calendars as we don't have any yet.
    }
}
