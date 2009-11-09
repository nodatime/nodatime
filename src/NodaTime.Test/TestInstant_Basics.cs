using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using NodaTime.Base;
using NodaTime.Chronologies;

using NUnit.Framework;

namespace NodaTime
{
    [TestFixture]
    public class TestInstant_Basics
    {
        // test in 2002/03 as time zones are more well known
        // (before the late 90's they were all over the place)
        private static readonly DateTimeZone Paris = DateTimeZone.ForID("Europe/Paris");
        private static readonly DateTimeZone London = DateTimeZone.ForID("Europe/London");

        private const long Y2002Days = 365 + 365 + 366 + 365 + 365 + 365 + 366 + 365 + 365 + 365 +
                                       366 + 365 + 365 + 365 + 366 + 365 + 365 + 365 + 366 + 365 +
                                       365 + 365 + 366 + 365 + 365 + 365 + 366 + 365 + 365 + 365 +
                                       366 + 365;
        private const long Y2003Days = 365 + 365 + 366 + 365 + 365 + 365 + 366 + 365 + 365 + 365 +
                                       366 + 365 + 365 + 365 + 366 + 365 + 365 + 365 + 366 + 365 +
                                       365 + 365 + 366 + 365 + 365 + 365 + 366 + 365 + 365 + 365 +
                                       366 + 365 + 365;

        // 2002-06-09
        private const long TestTimeNow =
            (Y2002Days + 31L + 28L + 31L + 30L + 31L + 9L - 1L) * DateTimeConstants.MillisecondsPerDay;

        // 2002-04-05
        private const long TestTime1 =
            (Y2002Days + 31L + 28L + 31L + 5L - 1L) * DateTimeConstants.MillisecondsPerDay
            + 12L * DateTimeConstants.MillisecondsPerHour
            + 24L * DateTimeConstants.MillisecondsPerMinute;

        // 2003-05-06
        private const long TestTime2 =
            (Y2003Days + 31L + 28L + 31L + 30L + 6L - 1L) * DateTimeConstants.MillisecondsPerDay
            + 14L * DateTimeConstants.MillisecondsPerHour
            + 28L * DateTimeConstants.MillisecondsPerMinute;

        private DateTimeZone originalDateTimeZone;
        private TimeZone originalTimeZone;
        private CultureInfo originalLocale;

        [SetUp]
        public void SetUp()
        {
            DateTimeUtils.SetCurrentMillisFixed(TestTimeNow);
            originalDateTimeZone = DateTimeZone.Default;
            // TODO: originalTimeZone = TimeZone.getDefault();
            // TODO: originalLocale = Locale.getDefault();
            DateTimeZone.Default = London;
            // TODO:TimeZone.setDefault(TimeZone.getTimeZone("Europe/London"));
            // TODO:Locale.setDefault(Locale.UK);
        }

        [TearDown]
        public void TearDown()
        {
            DateTimeUtils.SetCurrentMillisSystem();
            DateTimeZone.Default = originalDateTimeZone;
            // TODO: TimeZone.setDefault(originalTimeZone);
            // TODO: Locale.setDefault(originalLocale);
            originalDateTimeZone = null;
            originalTimeZone = null;
            originalLocale = null;
        }

        [Test]
        public void TestTest()
        {
            Assert.Equals("2002-06-09T00:00:00.000Z", new Instant(TestTimeNow).ToString());
            Assert.Equals("2002-04-05T12:24:00.000Z", new Instant(TestTime1).ToString());
            Assert.Equals("2003-05-06T14:28:00.000Z", new Instant(TestTime2).ToString());
        }

        [Test]
        public void TestGet_DateTimeFieldType()
        {
            var test = new Instant(); // 2002-06-09
            Assert.Equals(1, test.Get(DateTimeFieldType.Era));
            Assert.Equals(20, test.Get(DateTimeFieldType.CenturyOfEra));
            Assert.Equals(2, test.Get(DateTimeFieldType.YearOfCentury));
            Assert.Equals(2002, test.Get(DateTimeFieldType.YearOfEra));
            Assert.Equals(2002, test.Get(DateTimeFieldType.Year));
            Assert.Equals(6, test.Get(DateTimeFieldType.MonthOfYear));
            Assert.Equals(9, test.Get(DateTimeFieldType.DayOfMonth));
            Assert.Equals(2002, test.Get(DateTimeFieldType.Weekyear));
            Assert.Equals(23, test.Get(DateTimeFieldType.WeekOfWeekyear));
            Assert.Equals(7, test.Get(DateTimeFieldType.DayOfWeek));
            Assert.Equals(160, test.Get(DateTimeFieldType.DayOfYear));
            Assert.Equals(0, test.Get(DateTimeFieldType.HalfdayOfDay));
            Assert.Equals(0, test.Get(DateTimeFieldType.HourOfHalfday));
            Assert.Equals(24, test.Get(DateTimeFieldType.ClockhourOfDay));
            Assert.Equals(12, test.Get(DateTimeFieldType.ClockhourOfHalfday));
            Assert.Equals(0, test.Get(DateTimeFieldType.HourOfDay));
            Assert.Equals(0, test.Get(DateTimeFieldType.MinuteOfHour));
            Assert.Equals(0, test.Get(DateTimeFieldType.MinuteOfDay));
            Assert.Equals(0, test.Get(DateTimeFieldType.SecondOfMinute));
            Assert.Equals(0, test.Get(DateTimeFieldType.SecondOfDay));
            Assert.Equals(0, test.Get(DateTimeFieldType.MillisOfSecond));
            Assert.Equals(0, test.Get(DateTimeFieldType.MillisOfDay));
            // This assertion is not relevant in .NET because enums cannot be null
            // I'm keeping it because it may become relevant once DateTimeFieldType is fully shaped
            //try {
            //    test.Get((DateTimeFieldType) null);
            //    fail();
            //} catch (IllegalArgumentException ex) {}
        }

        [Test]
        public void TestGet_DateTimeField()
        {
            var test = new Instant(); // 2002-06-09
            Assert.Equals(1, test.Get(IsoChronology.GetInstance().Era));
            Assert.Equals(20, test.Get(IsoChronology.GetInstance().CenturyOfEra));
            Assert.Equals(2, test.Get(IsoChronology.GetInstance().YearOfCentury));
            Assert.Equals(2002, test.Get(IsoChronology.GetInstance().YearOfEra));
            Assert.Equals(2002, test.Get(IsoChronology.GetInstance().Year));
            Assert.Equals(6, test.Get(IsoChronology.GetInstance().MonthOfYear));
            Assert.Equals(9, test.Get(IsoChronology.GetInstance().DayOfMonth));
            Assert.Equals(2002, test.Get(IsoChronology.GetInstance().Weekyear));
            Assert.Equals(23, test.Get(IsoChronology.GetInstance().WeekOfWeekyear));
            Assert.Equals(7, test.Get(IsoChronology.GetInstance().DayOfWeek));
            Assert.Equals(160, test.Get(IsoChronology.GetInstance().DayOfYear));
            Assert.Equals(0, test.Get(IsoChronology.GetInstance().HalfdayOfDay));
            Assert.Equals(1, test.Get(IsoChronology.GetInstance().HourOfHalfday));
            Assert.Equals(1, test.Get(IsoChronology.GetInstance().ClockhourOfDay));
            Assert.Equals(1, test.Get(IsoChronology.GetInstance().ClockhourOfHalfday));
            Assert.Equals(1, test.Get(IsoChronology.GetInstance().HourOfDay));
            Assert.Equals(0, test.Get(IsoChronology.GetInstance().MinuteOfHour));
            Assert.Equals(60, test.Get(IsoChronology.GetInstance().MinuteOfDay));
            Assert.Equals(0, test.Get(IsoChronology.GetInstance().SecondOfMinute));
            Assert.Equals(60 * 60, test.Get(IsoChronology.GetInstance().SecondOfDay));
            Assert.Equals(0, test.Get(IsoChronology.GetInstance().MillisecondsOfSecond));
            Assert.Equals(60 * 60 * 1000, test.Get(IsoChronology.GetInstance().MillisecondsOfDay));
            Assert.Throws<ArgumentException>(() => test.Get(null));
        }

        [Test]
        public void TestGetMethods()
        {
            var test = new Instant();

            Assert.Equals(IsoChronology.Utc, test.Chronology);
            Assert.Equals(DateTimeZone.Utc, test.Zone);
            Assert.Equals(TestTimeNow, test.Milliseconds);
        }

        [Test]
        public void TestEqualsHashCode()
        {
            var test1 = new Instant(TestTime1);
            var test2 = new Instant(TestTime1);
            Assert.IsTrue(test1.Equals(test2));
            Assert.IsTrue(test2.Equals(test1));
            Assert.IsTrue(test1.Equals(test1));
            Assert.IsTrue(test2.Equals(test2));
            Assert.IsTrue(test1.GetHashCode() == test2.GetHashCode());
            Assert.IsTrue(test1.GetHashCode() == test1.GetHashCode());
            Assert.IsTrue(test2.GetHashCode() == test2.GetHashCode());

            var test3 = new Instant(TestTime2);
            Assert.IsFalse(test1.Equals(test3));
            Assert.IsFalse(test2.Equals(test3));
            Assert.IsFalse(test3.Equals(test1));
            Assert.IsFalse(test3.Equals(test2));
            Assert.IsFalse(test1.GetHashCode() == test3.GetHashCode());
            Assert.IsFalse(test2.GetHashCode() == test3.GetHashCode());

            Assert.IsFalse(test1.Equals("Hello"));
            Assert.IsTrue(test1.Equals(new MockInstant()));
            Assert.IsFalse(test1.Equals(new DateTime(TestTime1)));
        }

        private class MockInstant : AbstractInstant
        {
            public override string ToString()
            {
                return null;
            }

            public override long Milliseconds
            {
                get { return TestTime1; }
            }
            public override IChronology Chronology
            {
                get { return IsoChronology.Utc; }
            }
        }

        [Test]
        public void TestCompareTo()
        {
            var test1 = new Instant(TestTime1);
            var test1a = new Instant(TestTime1);
            Assert.Equals(0, test1.CompareTo(test1a));
            Assert.Equals(0, test1a.CompareTo(test1));
            Assert.Equals(0, test1.CompareTo(test1));
            Assert.Equals(0, test1a.CompareTo(test1a));

            var test2 = new Instant(TestTime2);
            Assert.Equals(-1, test1.CompareTo(test2));
            Assert.Equals(+1, test2.CompareTo(test1));

            var test3 = new DateTime(TestTime2, GregorianChronology.GetInstance(Paris));
            Assert.Equals(-1, test1.CompareTo(test3));
            Assert.Equals(+1, test3.CompareTo(test1));
            Assert.Equals(0, test3.CompareTo(test2));

            Assert.Equals(+1, test2.CompareTo(new MockInstant()));
            Assert.Equals(0, test1.CompareTo(new MockInstant()));

            // Note: JodaTime Tests for NullPointerException here: WRONG!
            Assert.Throws<ArgumentNullException>(() => test1.CompareTo(null));

            // Note: This comment was on the JodaTime code. Not sure what it was meant for
            // We should probably delete it
            // try {
            //     test1.compareTo(new Date());
            //     fail();
            // } catch (ClassCastException ex) {}
        }

        [Test]
        public void TestIsEqual_long()
        {
            Assert.IsFalse(new Instant(TestTime1).IsEqual(TestTime2));
            Assert.IsTrue(new Instant(TestTime1).IsEqual(TestTime1));
            Assert.IsFalse(new Instant(TestTime2).IsEqual(TestTime1));
        }

        [Test]
        public void TestIsEqualNow()
        {
            Assert.IsFalse(new Instant(TestTimeNow - 1).IsEqualNow());
            Assert.IsTrue(new Instant(TestTimeNow).IsEqualNow());
            Assert.IsFalse(new Instant(TestTimeNow + 1).IsEqualNow());
        }

        /// <summary>
        /// Original name: TestIsEqual_RI
        /// </summary>
        [Test]
        public void TestIsEqual_Instant()
        {
            var test1 = new Instant(TestTime1);
            var test1a = new Instant(TestTime1);
            Assert.IsTrue(test1.IsEqual(test1a));
            Assert.IsTrue(test1a.IsEqual(test1));
            Assert.IsTrue(test1.IsEqual(test1));
            Assert.IsTrue(test1a.IsEqual(test1a));

            var test2 = new Instant(TestTime2);
            Assert.IsFalse(test1.IsEqual(test2));
            Assert.IsFalse(test2.IsEqual(test1));

            var test3 = new DateTime(TestTime2, GregorianChronology.GetInstance(Paris));
            Assert.IsFalse(test1.IsEqual(test3));
            Assert.IsFalse(test3.IsEqual(test1));
            Assert.IsTrue(test3.IsEqual(test2));

            Assert.IsFalse(test2.IsEqual(new MockInstant()));
            Assert.IsTrue(test1.IsEqual(new MockInstant()));

            Assert.IsFalse(new Instant(TestTimeNow + 1).IsEqual(null));
            Assert.IsTrue(new Instant(TestTimeNow).IsEqual(null));
            Assert.IsFalse(new Instant(TestTimeNow - 1).IsEqual(null));
        }

        [Test]
        public void TestIsBefore_long()
        {
            Assert.IsTrue(new Instant(TestTime1).IsBefore(TestTime2));
            Assert.IsFalse(new Instant(TestTime1).IsBefore(TestTime1));
            Assert.IsFalse(new Instant(TestTime2).IsBefore(TestTime1));
        }

        [Test]
        public void TestIsBeforeNow()
        {
            Assert.IsTrue(new Instant(TestTimeNow - 1).IsBeforeNow());
            Assert.IsFalse(new Instant(TestTimeNow).IsBeforeNow());
            Assert.IsFalse(new Instant(TestTimeNow + 1).IsBeforeNow());
        }

        /// <summary>
        /// Original name: TestIsBefore_RI
        /// </summary>
        [Test]
        public void TestIsBefore_Instant()
        {
            var test1 = new Instant(TestTime1);
            var test1a = new Instant(TestTime1);
            Assert.IsFalse(test1.IsBefore(test1a));
            Assert.IsFalse(test1a.IsBefore(test1));
            Assert.IsFalse(test1.IsBefore(test1));
            Assert.IsFalse(test1a.IsBefore(test1a));

            var test2 = new Instant(TestTime2);
            Assert.IsTrue(test1.IsBefore(test2));
            Assert.IsFalse(test2.IsBefore(test1));

            var test3 = new DateTime(TestTime2, GregorianChronology.GetInstance(Paris));
            Assert.IsTrue(test1.IsBefore(test3));
            Assert.IsFalse(test3.IsBefore(test1));
            Assert.IsFalse(test3.IsBefore(test2));

            Assert.IsFalse(test2.IsBefore(new MockInstant()));
            Assert.IsFalse(test1.IsBefore(new MockInstant()));

            Assert.IsFalse(new Instant(TestTimeNow + 1).IsBefore(null));
            Assert.IsFalse(new Instant(TestTimeNow).IsBefore(null));
            Assert.IsTrue(new Instant(TestTimeNow - 1).IsBefore(null));
        }

        [Test]
        public void TestIsAfter_long()
        {
            Assert.IsFalse(new Instant(TestTime1).IsAfter(TestTime2));
            Assert.IsFalse(new Instant(TestTime1).IsAfter(TestTime1));
            Assert.IsTrue(new Instant(TestTime2).IsAfter(TestTime1));
        }

        [Test]
        public void TestIsAfterNow()
        {
            Assert.IsFalse(new Instant(TestTimeNow - 1).IsAfterNow());
            Assert.IsFalse(new Instant(TestTimeNow).IsAfterNow());
            Assert.IsTrue(new Instant(TestTimeNow + 1).IsAfterNow());
        }

        /// <summary>
        /// Original name: TestIsAfter_RI
        /// </summary>
        [Test]
        public void TestIsAfter_Instant()
        {
            var test1 = new Instant(TestTime1);
            var test1a = new Instant(TestTime1);
            Assert.IsFalse(test1.IsAfter(test1a));
            Assert.IsFalse(test1a.IsAfter(test1));
            Assert.IsFalse(test1.IsAfter(test1));
            Assert.IsFalse(test1a.IsAfter(test1a));

            var test2 = new Instant(TestTime2);
            Assert.IsFalse(test1.IsAfter(test2));
            Assert.IsTrue(test2.IsAfter(test1));

            var test3 = new DateTime(TestTime2, GregorianChronology.GetInstance(Paris));
            Assert.IsFalse(test1.IsAfter(test3));
            Assert.IsTrue(test3.IsAfter(test1));
            Assert.IsFalse(test3.IsAfter(test2));

            Assert.IsTrue(test2.IsAfter(new MockInstant()));
            Assert.IsFalse(test1.IsAfter(new MockInstant()));

            Assert.IsTrue(new Instant(TestTimeNow + 1).IsAfter(null));
            Assert.IsFalse(new Instant(TestTimeNow).IsAfter(null));
            Assert.IsFalse(new Instant(TestTimeNow - 1).IsAfter(null));
        }

        [Test]
        public void TestSerialization()
        {
            var test = new Instant(TestTimeNow);
            byte[] bytes;
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, test);
                bytes = stream.ToArray();
            }
            using (var stream = new MemoryStream(bytes))
            {
                var result = (Instant) formatter.Deserialize(stream);
                Assert.Equals(test, result);
            }
        }

        [Test]
        public void TestToString()
        {
            var test = new Instant(TestTimeNow);
            Assert.Equals("2002-06-09T00:00:00.000Z", test.ToString());
        }

        [Test]
        public void TestToInstant()
        {
            var test = new Instant(TestTime1);
            var result = test.ToInstant();
            Assert.AreSame(test, result);
        }

        [Test]
        public void TestToDateTime()
        {
            var test = new Instant(TestTime1);
            var result = test.ToDateTime();
            Assert.Equals(TestTime1, result.Milliseconds);
            Assert.Equals(IsoChronology.GetInstance(), result.Chronology);
        }

        // Note: This was Testing a deprecated method. We are not porting deprecated API
        //public void TestToDateTimeISO() {
        //    Instant test = new Instant(TestTime1);
        //    DateTime result = test.toDateTimeISO();
        //    Assert.AreSame(DateTime.class, result.getClass());
        //    Assert.AreSame(IsoChronology.class, result.Chronology.getClass());
        //    assertEquals(test.Milliseconds, result.Milliseconds);
        //    assertEquals(IsoChronology.GetInstance(), result.Chronology);
        //}

        [Test]
        public void TestToDateTime_DateTimeZone()
        {
            var test = new Instant(TestTime1);
            var result = test.ToDateTime(London);
            Assert.Equals(test.Milliseconds, result.Milliseconds);
            Assert.Equals(IsoChronology.GetInstance(London), result.Chronology);

            test = new Instant(TestTime1);
            result = test.ToDateTime(Paris);
            Assert.Equals(test.Milliseconds, result.Milliseconds);
            Assert.Equals(IsoChronology.GetInstance(Paris), result.Chronology);

            test = new Instant(TestTime1);
            result = test.ToDateTime((DateTimeZone) null);
            Assert.Equals(test.Milliseconds, result.Milliseconds);
            Assert.Equals(IsoChronology.GetInstance(), result.Chronology);
        }

        [Test]
        public void TestToDateTime_Chronology()
        {
            var test = new Instant(TestTime1);
            var result = test.ToDateTime(IsoChronology.GetInstance());
            Assert.Equals(test.Milliseconds, result.Milliseconds);
            Assert.Equals(IsoChronology.GetInstance(), result.Chronology);

            test = new Instant(TestTime1);
            result = test.ToDateTime(GregorianChronology.GetInstance(Paris));
            Assert.Equals(test.Milliseconds, result.Milliseconds);
            Assert.Equals(GregorianChronology.GetInstance(Paris), result.Chronology);

            test = new Instant(TestTime1);
            result = test.ToDateTime((IChronology) null);
            Assert.Equals(IsoChronology.GetInstance(), result.Chronology);
        }

        [Test]
        public void TestToMutableDateTime()
        {
            var test = new Instant(TestTime1);
            var result = test.ToMutableDateTime();
            Assert.Equals(test.Milliseconds, result.Milliseconds);
            Assert.Equals(IsoChronology.GetInstance(), result.Chronology);
        }

        // Note: Another method Testing deprecated APIs
        //public void TestToMutableDateTimeISO() {
        //    Instant test = new Instant(TestTime1);
        //    MutableDateTime result = test.ToMutableDateTimeISO();
        //    Assert.AreSame(MutableDateTime.class, result.getClass());
        //    Assert.AreSame(IsoChronology.class, result.Chronology.getClass());
        //    assertEquals(test.Milliseconds, result.Milliseconds);
        //    assertEquals(IsoChronology.GetInstance(), result.Chronology);
        //}

        [Test]
        public void TestToMutableDateTime_DateTimeZone()
        {
            var test = new Instant(TestTime1);
            var result = test.ToMutableDateTime(London);
            Assert.Equals(test.Milliseconds, result.Milliseconds);
            Assert.Equals(IsoChronology.GetInstance(), result.Chronology);

            test = new Instant(TestTime1);
            result = test.ToMutableDateTime(Paris);
            Assert.Equals(test.Milliseconds, result.Milliseconds);
            Assert.Equals(IsoChronology.GetInstance(Paris), result.Chronology);

            test = new Instant(TestTime1);
            result = test.ToMutableDateTime((DateTimeZone) null);
            Assert.Equals(test.Milliseconds, result.Milliseconds);
            Assert.Equals(IsoChronology.GetInstance(), result.Chronology);
        }

        [Test]
        public void TestToMutableDateTime_Chronology()
        {
            var test = new Instant(TestTime1);
            var result = test.ToMutableDateTime(IsoChronology.GetInstance());
            Assert.Equals(test.Milliseconds, result.Milliseconds);
            Assert.Equals(IsoChronology.GetInstance(), result.Chronology);

            test = new Instant(TestTime1);
            result = test.ToMutableDateTime(GregorianChronology.GetInstance(Paris));
            Assert.Equals(test.Milliseconds, result.Milliseconds);
            Assert.Equals(GregorianChronology.GetInstance(Paris), result.Chronology);

            test = new Instant(TestTime1);
            result = test.ToMutableDateTime((IChronology) null);
            Assert.Equals(test.Milliseconds, result.Milliseconds);
            Assert.Equals(IsoChronology.GetInstance(), result.Chronology);
        }

        [Test]
        public void TestWithMillis_long()
        {
            var test = new Instant(TestTime1);
            var result = test.WithMilliseconds(TestTime2);
            Assert.Equals(TestTime2, result.Milliseconds);
            Assert.Equals(test.Chronology, result.Chronology);

            test = new Instant(TestTime1);
            result = test.WithMilliseconds(TestTime1);
            Assert.AreSame(test, result);
        }

        [Test]
        public void TestWithDurationAdded_long_int()
        {
            var test = new Instant(TestTime1);
            var result = test.WithDurationAdded(123456789L, 1);
            var expected = new Instant(TestTime1 + 123456789L);
            Assert.Equals(expected, result);

            result = test.WithDurationAdded(123456789L, 0);
            Assert.AreSame(test, result);

            result = test.WithDurationAdded(123456789L, 2);
            expected = new Instant(TestTime1 + (2L * 123456789L));
            Assert.Equals(expected, result);

            result = test.WithDurationAdded(123456789L, -3);
            expected = new Instant(TestTime1 - (3L * 123456789L));
            Assert.Equals(expected, result);
        }

        /// <summary>
        /// Original name: testWithDurationAdded_RD_int
        /// </summary>
        [Test]
        public void TestWithDurationAdded_Duration_int()
        {
            var test = new Instant(TestTime1);
            var result = test.WithDurationAdded(new Duration(123456789L), 1);
            var expected = new Instant(TestTime1 + 123456789L);
            Assert.Equals(expected, result);

            result = test.WithDurationAdded(null, 1);
            Assert.AreSame(test, result);

            result = test.WithDurationAdded(new Duration(123456789L), 0);
            Assert.AreSame(test, result);

            result = test.WithDurationAdded(new Duration(123456789L), 2);
            expected = new Instant(TestTime1 + (2L * 123456789L));
            Assert.Equals(expected, result);

            result = test.WithDurationAdded(new Duration(123456789L), -3);
            expected = new Instant(TestTime1 - (3L * 123456789L));
            Assert.Equals(expected, result);
        }

        [Test]
        public void TestPlus_long()
        {
            var test = new Instant(TestTime1);
            var result = test.Plus(123456789L);
            var expected = new Instant(TestTime1 + 123456789L);
            Assert.Equals(expected, result);
        }

        /// <summary>
        /// Original name: testPlus_RD
        /// </summary>
        [Test]
        public void TestPlus_Duration()
        {
            var test = new Instant(TestTime1);
            var result = test.Plus(new Duration(123456789L));
            var expected = new Instant(TestTime1 + 123456789L);
            Assert.Equals(expected, result);

            result = test.Plus(null);
            Assert.AreSame(test, result);
        }

        [Test]
        public void TestMinus_long()
        {
            var test = new Instant(TestTime1);
            var result = test.Minus(123456789L);
            var expected = new Instant(TestTime1 - 123456789L);
            Assert.Equals(expected, result);
        }

        /// <summary>
        /// Original name: testMinus_RD
        /// </summary>
        [Test]
        public void TestMinus_Duration()
        {
            var test = new Instant(TestTime1);
            var result = test.Minus(new Duration(123456789L));
            var expected = new Instant(TestTime1 - 123456789L);
            Assert.Equals(expected, result);

            result = test.Minus(null);
            Assert.AreSame(test, result);
        }

        // Note: Not sure if a class being sealed is a nice thing to test here
        //public void TestImmutable() {
        //    assertTrue(Modifier.isFinal(Instant.class.getModifiers()));
        //}
    }
}