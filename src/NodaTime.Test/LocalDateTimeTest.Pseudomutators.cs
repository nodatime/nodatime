// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;

namespace NodaTime.Test
{
    public partial class LocalDateTimeTest
    {
        [Test]
        public void PlusYear_Simple()
        {
            LocalDateTime start = new LocalDateTime(2011, 6, 26, 12, 15, 8);
            LocalDateTime expected = new LocalDateTime(2016, 6, 26, 12, 15, 8);
            Assert.AreEqual(expected, start.PlusYears(5));

            expected = new LocalDateTime(2006, 6, 26, 12, 15, 8);
            Assert.AreEqual(expected, start.PlusYears(-5));
        }

        [Test]
        public void PlusYear_LeapToNonLeap()
        {
            LocalDateTime start = new LocalDateTime(2012, 2, 29, 12, 15, 8);
            LocalDateTime expected = new LocalDateTime(2013, 2, 28, 12, 15, 8);
            Assert.AreEqual(expected, start.PlusYears(1));

            expected = new LocalDateTime(2011, 2, 28, 12, 15, 8);
            Assert.AreEqual(expected, start.PlusYears(-1));
        }

        [Test]
        public void PlusYear_LeapToLeap()
        {
            LocalDateTime start = new LocalDateTime(2012, 2, 29, 12, 15, 8);
            LocalDateTime expected = new LocalDateTime(2016, 2, 29, 12, 15, 8);
            Assert.AreEqual(expected, start.PlusYears(4));
        }

        [Test]
        public void PlusMonth_Simple()
        {
            LocalDateTime start = new LocalDateTime(2012, 4, 15, 12, 15, 8);
            LocalDateTime expected = new LocalDateTime(2012, 8, 15, 12, 15, 8);
            Assert.AreEqual(expected, start.PlusMonths(4));
        }

        [Test]
        public void PlusMonth_ChangingYear()
        {
            LocalDateTime start = new LocalDateTime(2012, 10, 15, 12, 15, 8);
            LocalDateTime expected = new LocalDateTime(2013, 2, 15, 12, 15, 8);
            Assert.AreEqual(expected, start.PlusMonths(4));
        }

        [Test]
        public void PlusMonth_WithTruncation()
        {
            LocalDateTime start = new LocalDateTime(2011, 1, 30, 12, 15, 8);
            LocalDateTime expected = new LocalDateTime(2011, 2, 28, 12, 15, 8);
            Assert.AreEqual(expected, start.PlusMonths(1));
        }

        [Test]
        public void PlusDays_Simple()
        {
            LocalDateTime start = new LocalDateTime(2011, 1, 15, 12, 15, 8);
            LocalDateTime expected = new LocalDateTime(2011, 1, 23, 12, 15, 8);
            Assert.AreEqual(expected, start.PlusDays(8));

            expected = new LocalDateTime(2011, 1, 7, 12, 15, 8);
            Assert.AreEqual(expected, start.PlusDays(-8));
        }

        [Test]
        public void PlusDays_MonthBoundary()
        {
            LocalDateTime start = new LocalDateTime(2011, 1, 26, 12, 15, 8);
            LocalDateTime expected = new LocalDateTime(2011, 2, 3, 12, 15, 8);
            Assert.AreEqual(expected, start.PlusDays(8));

            // Round-trip back across the boundary
            Assert.AreEqual(start, start.PlusDays(8).PlusDays(-8));
        }

        [Test]
        public void PlusDays_YearBoundary()
        {
            LocalDateTime start = new LocalDateTime(2011, 12, 26, 12, 15, 8);
            LocalDateTime expected = new LocalDateTime(2012, 1, 3, 12, 15, 8);
            Assert.AreEqual(expected, start.PlusDays(8));

            // Round-trip back across the boundary
            Assert.AreEqual(start, start.PlusDays(8).PlusDays(-8));
        }

        [Test]
        public void PlusDays_EndOfFebruary_InLeapYear()
        {
            LocalDateTime start = new LocalDateTime(2012, 2, 26, 12, 15, 8);
            LocalDateTime expected = new LocalDateTime(2012, 3, 5, 12, 15, 8);
            Assert.AreEqual(expected, start.PlusDays(8));
            // Round-trip back across the boundary
            Assert.AreEqual(start, start.PlusDays(8).PlusDays(-8));
        }

        [Test]
        public void PlusDays_EndOfFebruary_NotInLeapYear()
        {
            LocalDateTime start = new LocalDateTime(2011, 2, 26, 12, 15, 8);
            LocalDateTime expected = new LocalDateTime(2011, 3, 6, 12, 15, 8);
            Assert.AreEqual(expected, start.PlusDays(8));

            // Round-trip back across the boundary
            Assert.AreEqual(start, start.PlusDays(8).PlusDays(-8));
        }

        [Test]
        public void PlusWeeks_Simple()
        {
            LocalDateTime start = new LocalDateTime(2011, 4, 2, 12, 15, 8);
            LocalDateTime expectedForward = new LocalDateTime(2011, 4, 23, 12, 15, 8);
            LocalDateTime expectedBackward = new LocalDateTime(2011, 3, 12, 12, 15, 8);
            Assert.AreEqual(expectedForward, start.PlusWeeks(3));
            Assert.AreEqual(expectedBackward, start.PlusWeeks(-3));
        }

        [Test]
        public void PlusHours_Simple()
        {
            LocalDateTime start = new LocalDateTime(2011, 4, 2, 12, 15, 8);
            LocalDateTime expectedForward = new LocalDateTime(2011, 4, 2, 14, 15, 8);
            LocalDateTime expectedBackward = new LocalDateTime(2011, 4, 2, 10, 15, 8);
            Assert.AreEqual(expectedForward, start.PlusHours(2));
            Assert.AreEqual(expectedBackward, start.PlusHours(-2));
        }

        [Test]
        public void PlusHours_CrossingDayBoundary()
        {
            LocalDateTime start = new LocalDateTime(2011, 4, 2, 12, 15, 8);
            LocalDateTime expected = new LocalDateTime(2011, 4, 3, 8, 15, 8);
            Assert.AreEqual(expected, start.PlusHours(20));
            Assert.AreEqual(start, start.PlusHours(20).PlusHours(-20));
        }

        [Test]
        public void PlusHours_CrossingYearBoundary()
        {
            // Christmas day + 10 days and 1 hour
            LocalDateTime start = new LocalDateTime(2011, 12, 25, 12, 15, 8);
            LocalDateTime expected = new LocalDateTime(2012, 1, 4, 13, 15, 8);
            Assert.AreEqual(expected, start.PlusHours(241));
            Assert.AreEqual(start, start.PlusHours(241).PlusHours(-241));
        }

        // Having tested that hours cross boundaries correctly, the other time unit
        // tests are straightforward
        [Test]
        public void PlusMinutes_Simple()
        {
            LocalDateTime start = new LocalDateTime(2011, 4, 2, 12, 15, 8);
            LocalDateTime expectedForward = new LocalDateTime(2011, 4, 2, 12, 17, 8);
            LocalDateTime expectedBackward = new LocalDateTime(2011, 4, 2, 12, 13, 8);
            Assert.AreEqual(expectedForward, start.PlusMinutes(2));
            Assert.AreEqual(expectedBackward, start.PlusMinutes(-2));
        }

        [Test]
        public void PlusSeconds_Simple()
        {
            LocalDateTime start = new LocalDateTime(2011, 4, 2, 12, 15, 8);
            LocalDateTime expectedForward = new LocalDateTime(2011, 4, 2, 12, 15, 18);
            LocalDateTime expectedBackward = new LocalDateTime(2011, 4, 2, 12, 14, 58);
            Assert.AreEqual(expectedForward, start.PlusSeconds(10));
            Assert.AreEqual(expectedBackward, start.PlusSeconds(-10));
        }

        [Test]
        public void PlusMilliseconds_Simple()
        {
            LocalDateTime start = new LocalDateTime(2011, 4, 2, 12, 15, 8, 300);
            LocalDateTime expectedForward = new LocalDateTime(2011, 4, 2, 12, 15, 8, 700);
            LocalDateTime expectedBackward = new LocalDateTime(2011, 4, 2, 12, 15, 7, 900);
            Assert.AreEqual(expectedForward, start.PlusMilliseconds(400));
            Assert.AreEqual(expectedBackward, start.PlusMilliseconds(-400));
        }

        [Test]
        public void PlusTicks_Simple()
        {
            LocalDate date = new LocalDate(2011, 4, 2);
            LocalTime startTime = LocalTime.FromHourMinuteSecondMillisecondTick(12, 15, 8, 300, 7500);
            LocalTime expectedForwardTime = LocalTime.FromHourMinuteSecondMillisecondTick(12, 15, 8, 301, 1500);
            LocalTime expectedBackwardTime = LocalTime.FromHourMinuteSecondMillisecondTick(12, 15, 8, 300, 3500);
            Assert.AreEqual(date + expectedForwardTime, (date + startTime).PlusTicks(4000));
            Assert.AreEqual(date + expectedBackwardTime, (date + startTime).PlusTicks(-4000));
        }

        [Test]
        public void PlusTicks_Long()
        {
            Assert.IsTrue(NodaConstants.TicksPerDay > int.MaxValue);
            LocalDateTime start = new LocalDateTime(2011, 4, 2, 12, 15, 8);
            LocalDateTime expectedForward = new LocalDateTime(2011, 4, 3, 12, 15, 8);
            LocalDateTime expectedBackward = new LocalDateTime(2011, 4, 1, 12, 15, 8);
            Assert.AreEqual(expectedForward, start.PlusTicks(NodaConstants.TicksPerDay));
            Assert.AreEqual(expectedBackward, start.PlusTicks(-NodaConstants.TicksPerDay));
        }

        [Test]
        public void PlusNanoseconds_Simple()
        {
            // Just use the ticks values
            LocalDate date = new LocalDate(2011, 4, 2);
            LocalTime startTime = LocalTime.FromHourMinuteSecondMillisecondTick(12, 15, 8, 300, 7500);
            LocalTime expectedForwardTime = LocalTime.FromHourMinuteSecondMillisecondTick(12, 15, 8, 300, 7540);
            LocalTime expectedBackwardTime = LocalTime.FromHourMinuteSecondMillisecondTick(12, 15, 8, 300, 7460);
            Assert.AreEqual(date + expectedForwardTime, (date + startTime).PlusNanoseconds(4000));
            Assert.AreEqual(date + expectedBackwardTime, (date + startTime).PlusNanoseconds(-4000));
        }

        [Test]
        public void PlusTicks_CrossingDay()
        {
            LocalDateTime start = new LocalDateTime(2011, 4, 2, 12, 15, 8);
            LocalDateTime expectedForward = new LocalDateTime(2011, 4, 3, 12, 15, 8);
            LocalDateTime expectedBackward = new LocalDateTime(2011, 4, 1, 12, 15, 8);
            Assert.AreEqual(expectedForward, start.PlusNanoseconds(NodaConstants.NanosecondsPerDay));
            Assert.AreEqual(expectedBackward, start.PlusNanoseconds(-NodaConstants.NanosecondsPerDay));
        }

        [Test]
        public void Plus_FullPeriod()
        {
            LocalDateTime start = new LocalDateTime(2011, 4, 2, 12, 15, 8);
            var period = new PeriodBuilder { Years = 1, Months = 2, Weeks = 3, Days = 4, Hours = 5, Minutes = 6,
                                             Seconds = 7, Milliseconds = 8, Ticks = 9, Nanoseconds = 11 }.Build();
            var actual = start.Plus(period);
            var expected = new LocalDateTime(2012, 6, 27, 17, 21, 15).PlusNanoseconds(8000911);
            Assert.AreEqual(expected, actual, $"{expected:yyyy-MM-dd HH:mm:ss.fffffffff} != {actual:yyyy-MM-dd HH:mm:ss.fffffffff}");
        }

        // Each test case gives a day-of-month in November 2011 and a target "next day of week";
        // the result is the next day-of-month in November 2011 with that target day.
        // The tests are picked somewhat arbitrarily...
        [TestCase(10, IsoDayOfWeek.Wednesday, ExpectedResult = 16)]
        [TestCase(10, IsoDayOfWeek.Friday, ExpectedResult = 11)]
        [TestCase(10, IsoDayOfWeek.Thursday, ExpectedResult = 17)]
        [TestCase(11, IsoDayOfWeek.Wednesday, ExpectedResult = 16)]
        [TestCase(11, IsoDayOfWeek.Thursday, ExpectedResult = 17)]
        [TestCase(11, IsoDayOfWeek.Friday, ExpectedResult = 18)]
        [TestCase(11, IsoDayOfWeek.Saturday, ExpectedResult = 12)]
        [TestCase(11, IsoDayOfWeek.Sunday, ExpectedResult = 13)]
        [TestCase(12, IsoDayOfWeek.Friday, ExpectedResult = 18)]
        [TestCase(13, IsoDayOfWeek.Friday, ExpectedResult = 18)]
        public int Next(int dayOfMonth, IsoDayOfWeek targetDayOfWeek)
        {
            LocalDateTime start = new LocalDateTime(2011, 11, dayOfMonth, 15, 25, 30).PlusNanoseconds(123456789);
            LocalDateTime target = start.Next(targetDayOfWeek);
            Assert.AreEqual(2011, target.Year);
            Assert.AreEqual(11, target.Month);
            Assert.AreEqual(start.TimeOfDay, target.TimeOfDay);
            return target.Day;
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(8)]
        public void Next_InvalidArgument(IsoDayOfWeek targetDayOfWeek)
        {
            LocalDateTime start = new LocalDateTime(2011, 1, 1, 15, 25, 30).PlusNanoseconds(123456789);
            Assert.Throws<ArgumentOutOfRangeException>(() => start.Next(targetDayOfWeek));
        }

        // Each test case gives a day-of-month in November 2011 and a target "next day of week";
        // the result is the next day-of-month in November 2011 with that target day.
        [TestCase(10, IsoDayOfWeek.Wednesday, ExpectedResult = 9)]
        [TestCase(10, IsoDayOfWeek.Friday, ExpectedResult = 4)]
        [TestCase(10, IsoDayOfWeek.Thursday, ExpectedResult = 3)]
        [TestCase(11, IsoDayOfWeek.Wednesday, ExpectedResult = 9)]
        [TestCase(11, IsoDayOfWeek.Thursday, ExpectedResult = 10)]
        [TestCase(11, IsoDayOfWeek.Friday, ExpectedResult = 4)]
        [TestCase(11, IsoDayOfWeek.Saturday, ExpectedResult = 5)]
        [TestCase(11, IsoDayOfWeek.Sunday, ExpectedResult = 6)]
        [TestCase(12, IsoDayOfWeek.Friday, ExpectedResult = 11)]
        [TestCase(13, IsoDayOfWeek.Friday, ExpectedResult = 11)]
        public int Previous(int dayOfMonth, IsoDayOfWeek targetDayOfWeek)
        {
            LocalDateTime start = new LocalDateTime(2011, 11, dayOfMonth, 15, 25, 30).PlusNanoseconds(123456789);
            LocalDateTime target = start.Previous(targetDayOfWeek);
            Assert.AreEqual(2011, target.Year);
            Assert.AreEqual(11, target.Month);
            return target.Day;
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(8)]
        public void Previous_InvalidArgument(IsoDayOfWeek targetDayOfWeek)
        {
            LocalDateTime start = new LocalDateTime(2011, 1, 1, 15, 25, 30).PlusNanoseconds(123456789);
            Assert.Throws<ArgumentOutOfRangeException>(() => start.Previous(targetDayOfWeek));
        }

        // No tests for non-ISO-day-of-week calendars as we don't have any yet.

        [Test]
        public void Operator_MethodEquivalents()
        {
            LocalDateTime start = new LocalDateTime(2011, 1, 1, 15, 25, 30).PlusNanoseconds(123456789);
            Period period = Period.FromHours(1) + Period.FromDays(1);
            LocalDateTime end = start + period;
            Assert.AreEqual(start + period, LocalDateTime.Add(start, period));
            Assert.AreEqual(start + period, start.Plus(period));
            Assert.AreEqual(start - period, LocalDateTime.Subtract(start, period));
            Assert.AreEqual(start - period, start.Minus(period));
            Assert.AreEqual(period, end - start);
            Assert.AreEqual(period, LocalDateTime.Subtract(end, start));
            Assert.AreEqual(period, end.Minus(start));
        }

        [Test]
        public void With_TimeAdjuster()
        {
            LocalDateTime start = new LocalDateTime(2014, 6, 27, 12, 15, 8).PlusNanoseconds(123456789);
            LocalDateTime expected = new LocalDateTime(2014, 6, 27, 12, 15, 8);
            Assert.AreEqual(expected, start.With(TimeAdjusters.TruncateToSecond));
        }

        [Test]
        public void With_DateAdjuster()
        {
            LocalDateTime start = new LocalDateTime(2014, 6, 27, 12, 5, 8).PlusNanoseconds(123456789);
            LocalDateTime expected = new LocalDateTime(2014, 6, 30, 12, 5, 8).PlusNanoseconds(123456789);
            Assert.AreEqual(expected, start.With(DateAdjusters.EndOfMonth));
        }

        [Test]
        [TestCase(-9998, 1, 1, -1)]
        [TestCase(9999, 12, 31, 24)]
        [TestCase(1970, 1, 1, long.MaxValue)]
        [TestCase(1970, 1, 1, long.MinValue)]
        public void PlusHours_Overflow(int year, int month, int day, long hours)
        {
            TestHelper.AssertOverflow(new LocalDateTime(year, month, day, 0, 0).PlusHours, hours);
        }
    }
}
