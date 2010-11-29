using NodaTime.Fields;
using NodaTime.Periods;
using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public class Period2Test
    {
        // June 19th 2010, 2:30:15am
        private static readonly LocalDateTime TestTime1 = new LocalDateTime(2010, 6, 19, 2, 30, 15);
        // June 19th 2010, 4:45:10am
        private static readonly LocalDateTime TestTime2 = new LocalDateTime(2010, 6, 19, 4, 45, 10);

        private static readonly PeriodType HoursMinutesPeriodType = PeriodType.Time
            .WithSecondsRemoved()
            .WithMillisecondsRemoved()
            .WithTicksRemoved();

        [Test]
        public void BetweenLocalDateTimes_MovingForwardWithAllFields_GivesExactResult()
        {
            Period2 actual = Period2.Between(TestTime1, TestTime2);
            Period2 expected = Period2.FromHours(2) + Period2.FromMinutes(14) + Period2.FromSeconds(55);
            SpecialAssertEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDateTimes_MovingBackwardWithAllFields_GivesExactResult()
        {
            Period2 actual = Period2.Between(TestTime2, TestTime1);
            Period2 expected = Period2.FromHours(-2) + Period2.FromMinutes(-14) + Period2.FromSeconds(-55);
            SpecialAssertEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDateTimes_MovingForwardWithHoursAndMinutes_RoundsTowardsStart()
        {
            Period2 actual = Period2.Between(TestTime1, TestTime2, HoursMinutesPeriodType);
            Period2 expected = Period2.FromHours(2) + Period2.FromMinutes(14);
            SpecialAssertEqual(expected, actual);
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
        public void BetweenLocalDateTimes_MovingBackwardWithHoursAndMinutes_RoundsTowardsStart()
        {
            Period2 actual = Period2.Between(TestTime2, TestTime1, HoursMinutesPeriodType);
            Period2 expected = Period2.FromHours(-2) + Period2.FromMinutes(-14);
            SpecialAssertEqual(expected, actual);
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
