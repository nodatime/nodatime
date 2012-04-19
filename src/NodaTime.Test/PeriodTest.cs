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

using System;
using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public class PeriodTest
    {
        // June 19th 2010, 2:30:15am
        private static readonly LocalDateTime TestDateTime1 = new LocalDateTime(2010, 6, 19, 2, 30, 15);
        // June 19th 2010, 4:45:10am
        private static readonly LocalDateTime TestDateTime2 = new LocalDateTime(2010, 6, 19, 4, 45, 10);
        // June 19th 2010
        private static readonly LocalDate TestDate1 = new LocalDate(2010, 6, 19);
        // March 1st 2011
        private static readonly LocalDate TestDate2 = new LocalDate(2011, 3, 1);
        // March 1st 2012
        private static readonly LocalDate TestDate3 = new LocalDate(2012, 3, 1);

        private const PeriodUnits HoursMinutesPeriodType = PeriodUnits.Hours | PeriodUnits.Minutes;

        [Test]
        public void BetweenLocalDateTimes_WithoutSpecifyingUnits_OmitsWeeks()
        {
            Period actual = Period.Between(new LocalDateTime(2012, 2, 21, 0, 0), new LocalDateTime(2012, 2, 28, 0, 0));
            Period expected = Period.FromDays(7);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDateTimes_MovingForwardWithAllFields_GivesExactResult()
        {
            Period actual = Period.Between(TestDateTime1, TestDateTime2);
            Period expected = Period.FromHours(2) + Period.FromMinutes(14) + Period.FromSeconds(55);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDateTimes_MovingBackwardWithAllFields_GivesExactResult()
        {
            Period actual = Period.Between(TestDateTime2, TestDateTime1);
            Period expected = Period.FromHours(-2) + Period.FromMinutes(-14) + Period.FromSeconds(-55);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDateTimes_MovingForwardWithHoursAndMinutes_RoundsTowardsStart()
        {
            Period actual = Period.Between(TestDateTime1, TestDateTime2, HoursMinutesPeriodType);
            Period expected = Period.FromHours(2) + Period.FromMinutes(14);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDateTimes_MovingBackwardWithHoursAndMinutes_RoundsTowardsStart()
        {
            Period actual = Period.Between(TestDateTime2, TestDateTime1, HoursMinutesPeriodType);
            Period expected = Period.FromHours(-2) + Period.FromMinutes(-14);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_InvalidUnits()
        {
            Assert.Throws<ArgumentException>(() => Period.Between(TestDate1, TestDate2, 0));
            Assert.Throws<ArgumentException>(() => Period.Between(TestDate1, TestDate2, (PeriodUnits) (-1)));
            Assert.Throws<ArgumentException>(() => Period.Between(TestDate1, TestDate2, PeriodUnits.AllTimeUnits));
            Assert.Throws<ArgumentException>(() => Period.Between(TestDate1, TestDate2, PeriodUnits.Years | PeriodUnits.Hours));
        }

        [Test]
        public void BetweenLocalDates_MovingForwardNoLeapYears_WithExactResults()
        {
            Period actual = Period.Between(TestDate1, TestDate2);
            Period expected = Period.FromMonths(8) + Period.FromDays(10);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_MovingForwardInLeapYear_WithExactResults()
        {
            Period actual = Period.Between(TestDate1, TestDate3);
            Period expected = Period.FromYears(1) + Period.FromMonths(8) + Period.FromDays(11);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_MovingBackwardNoLeapYears_WithExactResults()
        {
            Period actual = Period.Between(TestDate2, TestDate1);
            Period expected = Period.FromMonths(-8) + Period.FromDays(-12);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_MovingBackwardInLeapYear_WithExactResults()
        {
            // This is asymmetric with moving forward, because we first take off a whole year, which
            // takes us to March 1st 2011, then 8 months to take us to July 1st 2010, then 12 days
            // to take us back to June 19th. In this case, the fact that our start date is in a leap
            // year had no effect.
            Period actual = Period.Between(TestDate3, TestDate1);
            Period expected = Period.FromYears(-1) + Period.FromMonths(-8) + Period.FromDays(-12);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_MovingForward_WithJustMonths()
        {
            Period actual = Period.Between(TestDate1, TestDate3, PeriodUnits.Months);
            Period expected = Period.FromMonths(20);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_MovingBackward_WithJustMonths()
        {
            Period actual = Period.Between(TestDate3, TestDate1, PeriodUnits.Months);
            Period expected = Period.FromMonths(-20);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_AssymetricForwardAndBackward()
        {
            // February 10th 2010
            LocalDate d1 = new LocalDate(2010, 2, 10);
            // March 30th 2010
            LocalDate d2 = new LocalDate(2010, 3, 30);
            // Going forward, we go to March 10th (1 month) then March 30th (20 days)
            Assert.AreEqual(Period.FromMonths(1) + Period.FromDays(20), Period.Between(d1, d2));
            // Going backward, we go to February 28th (-1 month, day is rounded) then February 10th (-18 days)
            Assert.AreEqual(Period.FromMonths(-1) + Period.FromDays(-18), Period.Between(d2, d1));
        }

        [Test]
        public void BetweenLocalDateTimes_InvalidUnits()
        {
            Assert.Throws<ArgumentException>(() => Period.Between(TestDate1, TestDate2, 0));
            Assert.Throws<ArgumentException>(() => Period.Between(TestDate1, TestDate2, (PeriodUnits)(-1)));
        }

        [Test]
        public void BetweenLocalTimes_InvalidUnits()
        {
            LocalTime t1 = new LocalTime(10, 0);
            LocalTime t2 = new LocalTime(15, 30, 45, 20, 5);
            Assert.Throws<ArgumentException>(() => Period.Between(t1, t2, 0));
            Assert.Throws<ArgumentException>(() => Period.Between(t1, t2, (PeriodUnits)(-1)));
            Assert.Throws<ArgumentException>(() => Period.Between(t1, t2, PeriodUnits.YearMonthDay));
            Assert.Throws<ArgumentException>(() => Period.Between(t1, t2, PeriodUnits.Years | PeriodUnits.Hours));
        }

        [Test]
        public void BetweenLocalTimes_MovingForwards()
        {
            LocalTime t1 = new LocalTime(10, 0);
            LocalTime t2 = new LocalTime(15, 30, 45, 20, 5);
            Assert.AreEqual(Period.FromHours(5) + Period.FromMinutes(30) + Period.FromSeconds(45) +
                               Period.FromMillseconds(20) + Period.FromTicks(5),
                               Period.Between(t1, t2));
        }

        [Test]
        public void BetweenLocalTimes_MovingBackwards()
        {
            LocalTime t1 = new LocalTime(15, 30, 45, 20, 5);
            LocalTime t2 = new LocalTime(10, 0);
            Assert.AreEqual(Period.FromHours(-5) + Period.FromMinutes(-30) + Period.FromSeconds(-45) +
                               Period.FromMillseconds(-20) + Period.FromTicks(-5),
                               Period.Between(t1, t2));
        }

        [Test]
        public void BetweenLocalTimes_MovingForwards_WithJustHours()
        {
            LocalTime t1 = new LocalTime(11, 30);
            LocalTime t2 = new LocalTime(17, 15);
            Assert.AreEqual(Period.FromHours(5), Period.Between(t1, t2, PeriodUnits.Hours));
        }

        [Test]
        public void BetweenLocalTimes_MovingBackwards_WithJustHours()
        {
            LocalTime t1 = new LocalTime(17, 15);
            LocalTime t2 = new LocalTime(11, 30);
            Assert.AreEqual(Period.FromHours(-5), Period.Between(t1, t2, PeriodUnits.Hours));
        }

        [Test]
        public void Addition_WithDifferent_PeriodTypes()
        {
            Period p1 = Period.FromHours(3);
            Period p2 = Period.FromMinutes(20);
            Period sum = p1 + p2;
            Assert.AreEqual(3, sum.Hours);
            Assert.AreEqual(20, sum.Minutes);
        }

        [Test]
        public void Addition_With_IdenticalPeriodTypes()
        {
            Period p1 = Period.FromHours(3);
            Period p2 = Period.FromHours(2);
            Period sum = p1 + p2;
            Assert.AreEqual(5, sum.Hours);
        }

        [Test]
        public void Subtraction_WithDifferent_PeriodTypes()
        {
            Period p1 = Period.FromHours(3);
            Period p2 = Period.FromMinutes(20);
            Period sum = p1 - p2;
            Assert.AreEqual(3, sum.Hours);
            Assert.AreEqual(-20, sum.Minutes);
        }

        [Test]
        public void Subtraction_With_IdenticalPeriodTypes()
        {
            Period p1 = Period.FromHours(3);
            Period p2 = Period.FromHours(2);
            Period sum = p1 - p2;
            Assert.AreEqual(1, sum.Hours);
        }

        [Test]
        public void Addition_CombinesUnits()
        {
            Period p1 = Period.FromHours(3);
            Period p2 = Period.FromDays(1);
            Assert.AreEqual(PeriodUnits.Hours | PeriodUnits.Days, (p1 + p2).Units);
        }

        [Test]
        public void Subtraction_CombinesUnits()
        {
            Period p1 = Period.FromHours(3);
            Period p2 = Period.FromDays(1);
            Assert.AreEqual(PeriodUnits.Hours | PeriodUnits.Days, (p1 - p2).Units);
        }

        [Test]
        public void Equality_IgnoresUnits()
        {
            Period period1 = Period.FromHours(5);
            Period period2 = period1 + Period.FromMinutes(0);
            Assert.AreEqual(period1, period2);
            Assert.AreNotEqual(period1.Units, period2.Units);
        }

        [Test]
        public void Equality_WhenEqual()
        {
            Assert.AreEqual(Period.FromHours(10), Period.FromHours(10));
            Assert.AreEqual(Period.FromMinutes(15), Period.FromMinutes(15));
            Assert.AreEqual(Period.FromDays(5), Period.FromDays(5));
        }

        [Test]
        public void Equality_WithDifferentPeriodTypes_OnlyConsidersValues()
        {
            Period allFields = Period.FromMinutes(1) + Period.FromHours(1) - Period.FromMinutes(1);
            Assert.AreEqual(PeriodUnits.Minutes | PeriodUnits.Hours, allFields.Units);
            
            Period justHours = Period.FromHours(1);
            Assert.AreEqual(PeriodUnits.Hours, justHours.Units);

            Assert.AreEqual(allFields, justHours);
        }

        [Test]
        public void Equality_WhenUnequal()
        {
            Assert.IsFalse(Period.FromHours(10).Equals(Period.FromHours(20)));
            Assert.IsFalse(Period.FromMinutes(15).Equals(Period.FromSeconds(15)));
            Assert.IsFalse(Period.FromHours(1).Equals(Period.FromMinutes(60)));
            Assert.IsFalse(Period.FromHours(1).Equals(new object()));
            Assert.IsFalse(Period.FromHours(1).Equals(null));
            Assert.IsFalse(Period.FromHours(1).Equals((object) null));
        }

        [Test]
        public void HasTimeComponent_SingleValued()
        {
            Assert.IsTrue(Period.FromHours(1).HasTimeComponent);
            Assert.IsFalse(Period.FromDays(1).HasTimeComponent);
        }

        [Test]
        public void HasDateComponent_SingleValued()
        {
            Assert.IsFalse(Period.FromHours(1).HasDateComponent);
            Assert.IsTrue(Period.FromDays(1).HasDateComponent);
        }

        [Test]
        public void HasTimeComponent_Compound()
        {
            LocalDateTime dt1 = new LocalDateTime(2000, 1, 1, 10, 45, 00);
            LocalDateTime dt2 = new LocalDateTime(2000, 2, 4, 11, 50, 00);

            // Case 1: Entire period is date-based (no time units available)
            Assert.IsFalse(Period.Between(dt1.Date, dt2.Date).HasTimeComponent);

            // Case 2: Period contains date and time units, but time units are all zero
            Assert.IsFalse(Period.Between(dt1.Date + LocalTime.Midnight, dt2.Date + LocalTime.Midnight).HasTimeComponent);

            // Case 3: Entire period is time-based, but 0. (Same local time twice here.)
            Assert.IsFalse(Period.Between(dt1.TimeOfDay, dt1.TimeOfDay).HasTimeComponent);

            // Case 4: Period contains date and time units, and some time units are non-zero
            Assert.IsTrue(Period.Between(dt1, dt2).HasTimeComponent);
            
            // Case 5: Entire period is time-based, and some time units are non-zero
            Assert.IsTrue(Period.Between(dt1.TimeOfDay, dt2.TimeOfDay).HasTimeComponent);
        }

        [Test]
        public void HasDateComponent_Compound()
        {
            LocalDateTime dt1 = new LocalDateTime(2000, 1, 1, 10, 45, 00);
            LocalDateTime dt2 = new LocalDateTime(2000, 2, 4, 11, 50, 00);

            // Case 1: Entire period is time-based (no date units available)
            Assert.IsFalse(Period.Between(dt1.TimeOfDay, dt2.TimeOfDay).HasDateComponent);

            // Case 2: Period contains date and time units, but date units are all zero
            Assert.IsFalse(Period.Between(dt1, dt1.Date + dt2.TimeOfDay).HasDateComponent);

            // Case 3: Entire period is date-based, but 0. (Same local date twice here.)
            Assert.IsFalse(Period.Between(dt1.Date, dt1.Date).HasDateComponent);

            // Case 4: Period contains date and time units, and some date units are non-zero
            Assert.IsTrue(Period.Between(dt1, dt2).HasDateComponent);

            // Case 5: Entire period is date-based, and some time units are non-zero
            Assert.IsTrue(Period.Between(dt1.Date, dt2.Date).HasDateComponent);
        }

        [Test]
        public void ToString_Positive()
        {
            Period period = Period.FromDays(1) +  Period.FromHours(2);
            Assert.AreEqual("P1DT2H", period.ToString());
        }

        [Test]
        public void ToString_Negative()
        {
            Period period = Period.FromDays(-1) + Period.FromHours(-2);
            Assert.AreEqual("P-1DT-2H", period.ToString());
        }

        [Test]
        public void ToString_Mixed()
        {
            Period period = Period.FromDays(-1) + Period.FromHours(2);
            Assert.AreEqual("P-1DT2H", period.ToString());
        }

        [Test]
        public void ToString_CompoundZero()
        {
            Period period = Period.FromDays(0) + Period.FromHours(0);
            Assert.AreEqual("P0DT0H", period.ToString());
        }

        [Test]
        public void ToBuilder_SingleUnit()
        {
            var builder = Period.FromHours(5).ToBuilder();
            var expected = new PeriodBuilder { Hours = 5 };
            Assert.AreEqual(expected, builder);
        }

        [Test]
        public void ToBuilder_MultipleUnits()
        {
            var builder = (Period.FromHours(5) + Period.FromWeeks(2)).ToBuilder();
            var expected = new PeriodBuilder { Hours = 5, Weeks = 2 };
            Assert.AreEqual(expected, builder);
        }

        [Test]
        public void Normalize_ZeroUnitsRemoved()
        {
            var original = new PeriodBuilder { Hours = 0, Minutes = 5 }.Build();
            Assert.AreEqual(PeriodUnits.Minutes | PeriodUnits.Hours, original.Units);

            var normalized = original.Normalize();

            // Equals doesn't check units, so check explicitly.
            Assert.AreEqual(original, normalized);
            Assert.AreEqual(PeriodUnits.Minutes, normalized.Units);
        }

        [Test]
        public void Normalize_Weeks()
        {
            var original = new PeriodBuilder { Weeks = 2, Days = 5 }.Build();
            var normalized = original.Normalize();
            var expected = new PeriodBuilder { Days = 19 }.Build();
            Assert.AreEqual(expected, normalized);
        }

        [Test]
        public void Normalize_Hours()
        {
            var original = new PeriodBuilder { Hours = 25, Days = 1 }.Build();
            var normalized = original.Normalize();
            var expected = new PeriodBuilder { Hours = 1, Days = 2 }.Build();
            Assert.AreEqual(expected, normalized);
        }

        [Test]
        public void Normalize_Minutes()
        {
            var original = new PeriodBuilder { Hours = 1, Minutes = 150 }.Build();
            var normalized = original.Normalize();
            var expected = new PeriodBuilder { Hours = 3, Minutes = 30}.Build();
            Assert.AreEqual(expected, normalized);
        }


        [Test]
        public void Normalize_Seconds()
        {
            var original = new PeriodBuilder { Minutes = 1, Seconds = 150 }.Build();
            var normalized = original.Normalize();
            var expected = new PeriodBuilder { Minutes = 3, Seconds = 30 }.Build();
            Assert.AreEqual(expected, normalized);
        }

        [Test]
        public void Normalize_Milliseconds()
        {
            var original = new PeriodBuilder { Seconds = 1, Milliseconds = 1500 }.Build();
            var normalized = original.Normalize();
            var expected = new PeriodBuilder { Seconds = 2, Milliseconds= 500 }.Build();
            Assert.AreEqual(expected, normalized);
        }

        [Test]
        public void Normalize_Ticks()
        {
            var original = new PeriodBuilder { Milliseconds = 1, Ticks = 15000 }.Build();
            var normalized = original.Normalize();
            var expected = new PeriodBuilder { Milliseconds = 2, Ticks = 5000 }.Build();
            Assert.AreEqual(expected, normalized);
        }

        [Test]
        public void Normalize_MultipleFields()
        {
            var original = new PeriodBuilder { Hours = 1, Minutes = 119, Seconds = 150 }.Build();
            var normalized = original.Normalize();
            var expected = new PeriodBuilder { Hours = 3, Minutes = 1, Seconds = 30 }.Build();
            Assert.AreEqual(expected, normalized);
        }

        [Test]
        public void Normalize_AllNegative()
        {
            var original = new PeriodBuilder { Hours = -1, Minutes = -119, Seconds = -150 }.Build();
            var normalized = original.Normalize();
            var expected = new PeriodBuilder { Hours = -3, Minutes = -1, Seconds = -30 }.Build();
            Assert.AreEqual(expected, normalized);
        }

        [Test]
        public void Normalize_MixedSigns_PositiveResult()
        {
            var original = new PeriodBuilder { Hours = 3, Minutes = -1 }.Build();
            var normalized = original.Normalize();
            var expected = new PeriodBuilder { Hours = 2, Minutes = 59 }.Build();
            Assert.AreEqual(expected, normalized);
        }

        [Test]
        public void Normalize_MixedSigns_NegativeResult()
        {
            var original = new PeriodBuilder { Hours = 1, Minutes = -121 }.Build();
            var normalized = original.Normalize();
            var expected = new PeriodBuilder { Hours = -1, Minutes = -1 }.Build();
            Assert.AreEqual(expected, normalized);
        }

        [Test]
        public void Normalize_DoesntAffectMonthsAndYears()
        {
            var original = new PeriodBuilder { Years = 2, Months = 1, Days = 400 }.Build();
            Assert.AreEqual(original, original.Normalize());
        }

        [Test]
        public void Normalize_EmptyResult()
        {
            var original = new PeriodBuilder { Years = 0 }.Build();
            Assert.AreEqual(Period.Empty, original.Normalize());
        }

        [Test]
        public void ToString_SingleUnit()
        {
            var period = Period.FromHours(5);
            Assert.AreEqual("PT5H", period.ToString());
        }

        [Test]
        public void ToString_MultipleUnits()
        {
            var period = new PeriodBuilder { Hours = 5, Minutes = 30 }.Build();
            Assert.AreEqual("PT5H30M", period.ToString());
        }

        [Test]
        public void EmptyPeriodHasNoUnits()
        {
            Assert.AreEqual(PeriodUnits.None, Period.Empty.Units);
        }

        [Test]
        public void ToDuration_InvalidWithYears()
        {
            Period period = Period.FromYears(1);
            Assert.Throws<InvalidOperationException>(() => period.ToDuration());
        }

        [Test]
        public void ToDuration_InvalidWithMonths()
        {
            Period period = Period.FromMonths(1);
            Assert.Throws<InvalidOperationException>(() => period.ToDuration());
        }

        [Test]
        public void ToDuration_ValidAllAcceptableUnits()
        {
            Period period = new PeriodBuilder
            {
                Weeks = 1,
                Days = 2,
                Hours = 3,
                Minutes = 4,
                Seconds = 5,
                Milliseconds = 6,
                Ticks = 7
            }.Build();
            Assert.AreEqual(
                1 * NodaConstants.TicksPerStandardWeek +
                2 * NodaConstants.TicksPerStandardDay +
                3 * NodaConstants.TicksPerHour +
                4 * NodaConstants.TicksPerMinute +
                5 * NodaConstants.TicksPerSecond +
                6 * NodaConstants.TicksPerMillisecond + 7,
                period.ToDuration().Ticks);
        }

        [Test]
        public void ToDuration_ValidWithZeroValuesInMonthYearUnits()
        {
            Period period = Period.FromMonths(1) + Period.FromYears(1);
            period = period - period + Period.FromDays(1);
            Assert.AreEqual(PeriodUnits.YearMonthDay, period.Units);
            Assert.AreEqual(Duration.OneStandardDay, period.ToDuration());
        }

        [Test]
        public void ToDuration_Overflow()
        {
            Period period = Period.FromSeconds(long.MaxValue);
            Assert.Throws<OverflowException>(() => period.ToDuration());
        }

        [Test]
        public void ToDuration_Overflow_WhenPossiblyValid()
        {
            // These two should pretty much cancel each other out - and would, if we had a 128-bit integer
            // representation to use.
            Period period = Period.FromSeconds(long.MaxValue) + Period.FromMinutes(long.MinValue / 60);
            Assert.Throws<OverflowException>(() => period.ToDuration());
        }
    }
}
