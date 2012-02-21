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
            SpecialAssertEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDateTimes_MovingForwardWithAllFields_GivesExactResult()
        {
            Period actual = Period.Between(TestDateTime1, TestDateTime2);
            Period expected = Period.FromHours(2) + Period.FromMinutes(14) + Period.FromSeconds(55);
            SpecialAssertEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDateTimes_MovingBackwardWithAllFields_GivesExactResult()
        {
            Period actual = Period.Between(TestDateTime2, TestDateTime1);
            Period expected = Period.FromHours(-2) + Period.FromMinutes(-14) + Period.FromSeconds(-55);
            SpecialAssertEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDateTimes_MovingForwardWithHoursAndMinutes_RoundsTowardsStart()
        {
            Period actual = Period.Between(TestDateTime1, TestDateTime2, HoursMinutesPeriodType);
            Period expected = Period.FromHours(2) + Period.FromMinutes(14);
            SpecialAssertEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDateTimes_MovingBackwardWithHoursAndMinutes_RoundsTowardsStart()
        {
            Period actual = Period.Between(TestDateTime2, TestDateTime1, HoursMinutesPeriodType);
            Period expected = Period.FromHours(-2) + Period.FromMinutes(-14);
            SpecialAssertEqual(expected, actual);
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
            SpecialAssertEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_MovingForwardInLeapYear_WithExactResults()
        {
            Period actual = Period.Between(TestDate1, TestDate3);
            Period expected = Period.FromYears(1) + Period.FromMonths(8) + Period.FromDays(11);
            SpecialAssertEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_MovingBackwardNoLeapYears_WithExactResults()
        {
            Period actual = Period.Between(TestDate2, TestDate1);
            Period expected = Period.FromMonths(-8) + Period.FromDays(-12);
            SpecialAssertEqual(expected, actual);
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
            SpecialAssertEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_MovingForward_WithJustMonths()
        {
            Period actual = Period.Between(TestDate1, TestDate3, PeriodUnits.Months);
            Period expected = Period.FromMonths(20);
            SpecialAssertEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_MovingBackward_WithJustMonths()
        {
            Period actual = Period.Between(TestDate3, TestDate1, PeriodUnits.Months);
            Period expected = Period.FromMonths(-20);
            SpecialAssertEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_AssymetricForwardAndBackward()
        {
            // February 10th 2010
            LocalDate d1 = new LocalDate(2010, 2, 10);
            // March 30th 2010
            LocalDate d2 = new LocalDate(2010, 3, 30);
            // Going forward, we go to March 10th (1 month) then March 30th (20 days)
            SpecialAssertEqual(Period.FromMonths(1) + Period.FromDays(20), Period.Between(d1, d2));
            // Going backward, we go to February 28th (-1 month, day is rounded) then February 10th (-18 days)
            SpecialAssertEqual(Period.FromMonths(-1) + Period.FromDays(-18), Period.Between(d2, d1));
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
            LocalTime t1 = new LocalTime(10, 0, 0);
            LocalTime t2 = new LocalTime(15, 30, 45, 20, 5);
            Assert.Throws<ArgumentException>(() => Period.Between(t1, t2, 0));
            Assert.Throws<ArgumentException>(() => Period.Between(t1, t2, (PeriodUnits)(-1)));
            Assert.Throws<ArgumentException>(() => Period.Between(t1, t2, PeriodUnits.YearMonthDay));
            Assert.Throws<ArgumentException>(() => Period.Between(t1, t2, PeriodUnits.Years | PeriodUnits.Hours));
        }

        [Test]
        public void BetweenLocalTimes_MovingForwards()
        {
            LocalTime t1 = new LocalTime(10, 0, 0);
            LocalTime t2 = new LocalTime(15, 30, 45, 20, 5);
            SpecialAssertEqual(Period.FromHours(5) + Period.FromMinutes(30) + Period.FromSeconds(45) +
                               Period.FromMillseconds(20) + Period.FromTicks(5),
                               Period.Between(t1, t2));
        }

        [Test]
        public void BetweenLocalTimes_MovingBackwards()
        {
            LocalTime t1 = new LocalTime(15, 30, 45, 20, 5);
            LocalTime t2 = new LocalTime(10, 0, 0);
            SpecialAssertEqual(Period.FromHours(-5) + Period.FromMinutes(-30) + Period.FromSeconds(-45) +
                               Period.FromMillseconds(-20) + Period.FromTicks(-5),
                               Period.Between(t1, t2));
        }

        [Test]
        public void BetweenLocalTimes_MovingForwards_WithJustHours()
        {
            LocalTime t1 = new LocalTime(11, 30, 0);
            LocalTime t2 = new LocalTime(17, 15, 0);
            SpecialAssertEqual(Period.FromHours(5), Period.Between(t1, t2, PeriodUnits.Hours));
        }

        [Test]
        public void BetweenLocalTimes_MovingBackwards_WithJustHours()
        {
            LocalTime t1 = new LocalTime(17, 15, 0);
            LocalTime t2 = new LocalTime(11, 30, 0);
            SpecialAssertEqual(Period.FromHours(-5), Period.Between(t1, t2, PeriodUnits.Hours));
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
        public void Equality_WhenEqual()
        {
            SpecialAssertEqual(Period.FromHours(10), Period.FromHours(10));
            SpecialAssertEqual(Period.FromMinutes(15), Period.FromMinutes(15));
            SpecialAssertEqual(Period.FromDays(5), Period.FromDays(5));
        }

        [Test]
        public void Equality_WithDifferentPeriodTypes_OnlyConsidersValues()
        {
            Period allFields = Period.FromMinutes(1) + Period.FromHours(1) - Period.FromMinutes(1);
            Assert.AreEqual(PeriodUnits.Minutes | PeriodUnits.Hours, allFields.Units);
            
            Period justHours = Period.FromHours(1);
            Assert.AreEqual(PeriodUnits.Hours, justHours.Units);

            SpecialAssertEqual(allFields, justHours);
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

        private void SpecialAssertEqual(Period period1, Period period2)
        {
            Assert.AreEqual(period1.GetHashCode(), period2.GetHashCode());
            // Don't use Assert.Equals, which will iterate over the period
            Assert.IsTrue(period1.Equals(period2));
        }
    }
}
