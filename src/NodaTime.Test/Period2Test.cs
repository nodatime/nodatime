using NodaTime.Partials;
using NodaTime.Periods;
using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public class Period2Test
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

        private static readonly PeriodType HoursMinutesPeriodType = PeriodType.Time
            .WithSecondsRemoved()
            .WithMillisecondsRemoved()
            .WithTicksRemoved();

        [Test]
        public void BetweenLocalDateTimes_MovingForwardWithAllFields_GivesExactResult()
        {
            Period2 actual = Period2.Between(TestDateTime1, TestDateTime2);
            Period2 expected = Period2.FromHours(2) + Period2.FromMinutes(14) + Period2.FromSeconds(55);
            SpecialAssertEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDateTimes_MovingBackwardWithAllFields_GivesExactResult()
        {
            Period2 actual = Period2.Between(TestDateTime2, TestDateTime1);
            Period2 expected = Period2.FromHours(-2) + Period2.FromMinutes(-14) + Period2.FromSeconds(-55);
            SpecialAssertEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDateTimes_MovingForwardWithHoursAndMinutes_RoundsTowardsStart()
        {
            Period2 actual = Period2.Between(TestDateTime1, TestDateTime2, HoursMinutesPeriodType);
            Period2 expected = Period2.FromHours(2) + Period2.FromMinutes(14);
            SpecialAssertEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDateTimes_MovingBackwardWithHoursAndMinutes_RoundsTowardsStart()
        {
            Period2 actual = Period2.Between(TestDateTime2, TestDateTime1, HoursMinutesPeriodType);
            Period2 expected = Period2.FromHours(-2) + Period2.FromMinutes(-14);
            SpecialAssertEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_MovingForwardNoLeapYears_WithExactResults()
        {
            Period2 actual = Period2.Between(TestDate1, TestDate2);
            Period2 expected = Period2.FromMonths(8) + Period2.FromDays(10);
            SpecialAssertEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_MovingForwardInLeapYear_WithExactResults()
        {
            Period2 actual = Period2.Between(TestDate1, TestDate3);
            Period2 expected = Period2.FromYears(1) + Period2.FromMonths(8) + Period2.FromDays(11);
            SpecialAssertEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_MovingBackwardNoLeapYears_WithExactResults()
        {
            Period2 actual = Period2.Between(TestDate2, TestDate1);
            Period2 expected = Period2.FromMonths(-8) + Period2.FromDays(-12);
            SpecialAssertEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_MovingBackwardInLeapYear_WithExactResults()
        {
            // This is asymmetric with moving forward, because we first take off a whole year, which
            // takes us to March 1st 2011, then 8 months to take us to July 1st 2010, then 12 days
            // to take us back to June 19th. In this case, the fact that our start date is in a leap
            // year had no effect.
            Period2 actual = Period2.Between(TestDate3, TestDate1);
            Period2 expected = Period2.FromYears(-1) + Period2.FromMonths(-8) + Period2.FromDays(-12);
            SpecialAssertEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_MovingForward_WithJustMonths()
        {
            Period2 actual = Period2.Between(TestDate1, TestDate3, PeriodType.Months);
            Period2 expected = Period2.FromMonths(20);
            SpecialAssertEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_MovingBackward_WithJustMonths()
        {
            Period2 actual = Period2.Between(TestDate3, TestDate1, PeriodType.Months);
            Period2 expected = Period2.FromMonths(-20);
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
            SpecialAssertEqual(Period2.FromMonths(1) + Period2.FromDays(20), Period2.Between(d1, d2));
            // Going backward, we go to February 28th (-1 month, day is rounded) then February 10th (-18 days)
            SpecialAssertEqual(Period2.FromMonths(-1) + Period2.FromDays(-18), Period2.Between(d2, d1));
        }

        [Test]
        public void Addition_WithDifferent_PeriodTypes()
        {
            Period2 p1 = Period2.FromHours(3);
            Period2 p2 = Period2.FromMinutes(20);
            Period2 sum = p1 + p2;
            Assert.AreEqual(3, sum.Hours);
            Assert.AreEqual(20, sum.Minutes);
        }

        [Test]
        public void Addition_With_IdenticalPeriodTypes()
        {
            Period2 p1 = Period2.FromHours(3);
            Period2 p2 = Period2.FromHours(2);
            Period2 sum = p1 + p2;
            Assert.AreEqual(5, sum.Hours);
        }

        [Test]
        public void Subtraction_WithDifferent_PeriodTypes()
        {
            Period2 p1 = Period2.FromHours(3);
            Period2 p2 = Period2.FromMinutes(20);
            Period2 sum = p1 - p2;
            Assert.AreEqual(3, sum.Hours);
            Assert.AreEqual(-20, sum.Minutes);
        }

        [Test]
        public void Subtraction_With_IdenticalPeriodTypes()
        {
            Period2 p1 = Period2.FromHours(3);
            Period2 p2 = Period2.FromHours(2);
            Period2 sum = p1 - p2;
            Assert.AreEqual(1, sum.Hours);
        }

        [Test]
        public void Equality_WhenEqual()
        {
            SpecialAssertEqual(Period2.FromHours(10), Period2.FromHours(10));
            SpecialAssertEqual(Period2.FromMinutes(15), Period2.FromMinutes(15));
            SpecialAssertEqual(Period2.FromDays(5), Period2.FromDays(5));
        }

        [Test]
        public void Equality_WithDifferentPeriodTypes_OnlyConsidersValues()
        {
            Period2 allFields = Period2.FromMinutes(1) + Period2.FromHours(1) - Period2.FromMinutes(1);
            Assert.AreEqual(PeriodType.AllFields, allFields.PeriodType);
            
            Period2 justHours = Period2.FromHours(1);
            Assert.AreEqual(PeriodType.Hours, justHours.PeriodType);

            SpecialAssertEqual(allFields, justHours);
        }

        [Test]
        public void Equality_WhenUnequal()
        {
            Assert.IsFalse(Period2.FromHours(10).Equals(Period2.FromHours(20)));
            Assert.IsFalse(Period2.FromMinutes(15).Equals(Period2.FromSeconds(15)));
            Assert.IsFalse(Period2.FromHours(1).Equals(Period2.FromMinutes(60)));
            Assert.IsFalse(Period2.FromHours(1).Equals(new object()));
            Assert.IsFalse(Period2.FromHours(1).Equals(null));
            Assert.IsFalse(Period2.FromHours(1).Equals((object) null));
        }

        private void SpecialAssertEqual(Period2 period1, Period2 period2)
        {
            Assert.AreEqual(period1.GetHashCode(), period2.GetHashCode());
            // Don't use Assert.Equals, which will iterate over the period
            Assert.IsTrue(period1.Equals(period2));
        }
    }
}
