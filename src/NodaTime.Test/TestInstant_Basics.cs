using System;
using System.Globalization;

using NodaTime.Base;
using NodaTime.Chronologies;

using NUnit.Framework;

namespace NodaTime
{
    [TestFixture]
    public class TestInstant_Basics
    {
        //    // Test in 2002/03 as time zones are more well known
        //    // (before the late 90's they were all over the place)

        private static readonly DateTimeZone PARIS = DateTimeZone.ForID("Europe/Paris");
        private static readonly DateTimeZone LONDON = DateTimeZone.ForID("Europe/London");

        private const long y2002days = 365 + 365 + 366 + 365 + 365 + 365 + 366 + 365 + 365 + 365 +
                                       366 + 365 + 365 + 365 + 366 + 365 + 365 + 365 + 366 + 365 +
                                       365 + 365 + 366 + 365 + 365 + 365 + 366 + 365 + 365 + 365 +
                                       366 + 365;
        private const long y2003days = 365 + 365 + 366 + 365 + 365 + 365 + 366 + 365 + 365 + 365 +
                                       366 + 365 + 365 + 365 + 366 + 365 + 365 + 365 + 366 + 365 +
                                       365 + 365 + 366 + 365 + 365 + 365 + 366 + 365 + 365 + 365 +
                                       366 + 365 + 365;

        // 2002-06-09
        private const long Test_TIME_NOW =
            (y2002days + 31L + 28L + 31L + 30L + 31L + 9L - 1L) * DateTimeConstants.MillisecondsPerDay;

        // 2002-04-05
        private const long Test_TIME1 =
            (y2002days + 31L + 28L + 31L + 5L - 1L) * DateTimeConstants.MillisecondsPerDay
            + 12L * DateTimeConstants.MillisecondsPerHour
            + 24L * DateTimeConstants.MillisecondsPerMinute;

        // 2003-05-06
        private const long Test_TIME2 =
            (y2003days + 31L + 28L + 31L + 30L + 6L - 1L) * DateTimeConstants.MillisecondsPerDay
            + 14L * DateTimeConstants.MillisecondsPerHour
            + 28L * DateTimeConstants.MillisecondsPerMinute;

        private DateTimeZone originalDateTimeZone;
        private TimeZone originalTimeZone;
        private CultureInfo originalLocale;

        [SetUp]
        public void SetUp()
        {
            DateTimeUtils.SetCurrentMillisFixed(Test_TIME_NOW);
            originalDateTimeZone = DateTimeZone.Default;
            // TODO: originalTimeZone = TimeZone.getDefault();
            // TODO: originalLocale = Locale.getDefault();
            DateTimeZone.Default = LONDON;
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

        public void TestTest()
        {
            Assert.Equals("2002-06-09T00:00:00.000Z", new Instant(Test_TIME_NOW).ToString());
            Assert.Equals("2002-04-05T12:24:00.000Z", new Instant(Test_TIME1).ToString());
            Assert.Equals("2003-05-06T14:28:00.000Z", new Instant(Test_TIME2).ToString());
        }

        public void TestGet_DateTimeFieldType()
        {
            var Test = new Instant(); // 2002-06-09
            Assert.Equals(1, Test.Get(DateTimeFieldType.Era));
            Assert.Equals(20, Test.Get(DateTimeFieldType.CenturyOfEra));
            Assert.Equals(2, Test.Get(DateTimeFieldType.YearOfCentury));
            Assert.Equals(2002, Test.Get(DateTimeFieldType.YearOfEra));
            Assert.Equals(2002, Test.Get(DateTimeFieldType.Year));
            Assert.Equals(6, Test.Get(DateTimeFieldType.MonthOfYear));
            Assert.Equals(9, Test.Get(DateTimeFieldType.DayOfMonth));
            Assert.Equals(2002, Test.Get(DateTimeFieldType.Weekyear));
            Assert.Equals(23, Test.Get(DateTimeFieldType.WeekOfWeekyear));
            Assert.Equals(7, Test.Get(DateTimeFieldType.DayOfWeek));
            Assert.Equals(160, Test.Get(DateTimeFieldType.DayOfYear));
            Assert.Equals(0, Test.Get(DateTimeFieldType.HalfdayOfDay));
            Assert.Equals(0, Test.Get(DateTimeFieldType.HourOfHalfday));
            Assert.Equals(24, Test.Get(DateTimeFieldType.ClockhourOfDay));
            Assert.Equals(12, Test.Get(DateTimeFieldType.ClockhourOfHalfday));
            Assert.Equals(0, Test.Get(DateTimeFieldType.HourOfDay));
            Assert.Equals(0, Test.Get(DateTimeFieldType.MinuteOfHour));
            Assert.Equals(0, Test.Get(DateTimeFieldType.MinuteOfDay));
            Assert.Equals(0, Test.Get(DateTimeFieldType.SecondOfMinute));
            Assert.Equals(0, Test.Get(DateTimeFieldType.SecondOfDay));
            Assert.Equals(0, Test.Get(DateTimeFieldType.MillisOfSecond));
            Assert.Equals(0, Test.Get(DateTimeFieldType.MillisOfDay));
            // This assertion is not relevant in .NET because enums cannot be null
            // I'm keeping it because it may become relevant once DateTimeFieldType is fully shaped
            //try {
            //    Test.Get((DateTimeFieldType) null);
            //    fail();
            //} catch (IllegalArgumentException ex) {}
        }

        public void TestGet_DateTimeField()
        {
            var Test = new Instant(); // 2002-06-09
            Assert.Equals(1, Test.Get(IsoChronology.GetInstance().Era));
            Assert.Equals(20, Test.Get(IsoChronology.GetInstance().CenturyOfEra));
            Assert.Equals(2, Test.Get(IsoChronology.GetInstance().YearOfCentury));
            Assert.Equals(2002, Test.Get(IsoChronology.GetInstance().YearOfEra));
            Assert.Equals(2002, Test.Get(IsoChronology.GetInstance().Year));
            Assert.Equals(6, Test.Get(IsoChronology.GetInstance().MonthOfYear));
            Assert.Equals(9, Test.Get(IsoChronology.GetInstance().DayOfMonth));
            Assert.Equals(2002, Test.Get(IsoChronology.GetInstance().Weekyear));
            Assert.Equals(23, Test.Get(IsoChronology.GetInstance().WeekOfWeekyear));
            Assert.Equals(7, Test.Get(IsoChronology.GetInstance().DayOfWeek));
            Assert.Equals(160, Test.Get(IsoChronology.GetInstance().DayOfYear));
            Assert.Equals(0, Test.Get(IsoChronology.GetInstance().HalfdayOfDay));
            Assert.Equals(1, Test.Get(IsoChronology.GetInstance().HourOfHalfday));
            Assert.Equals(1, Test.Get(IsoChronology.GetInstance().ClockhourOfDay));
            Assert.Equals(1, Test.Get(IsoChronology.GetInstance().ClockhourOfHalfday));
            Assert.Equals(1, Test.Get(IsoChronology.GetInstance().HourOfDay));
            Assert.Equals(0, Test.Get(IsoChronology.GetInstance().MinuteOfHour));
            Assert.Equals(60, Test.Get(IsoChronology.GetInstance().MinuteOfDay));
            Assert.Equals(0, Test.Get(IsoChronology.GetInstance().SecondOfMinute));
            Assert.Equals(60 * 60, Test.Get(IsoChronology.GetInstance().SecondOfDay));
            Assert.Equals(0, Test.Get(IsoChronology.GetInstance().MillisecondsOfSecond));
            Assert.Equals(60 * 60 * 1000, Test.Get(IsoChronology.GetInstance().MillisecondsOfDay));
            Assert.Throws<ArgumentException>(() => Test.Get(null));
        }

        public void TestGetMethods()
        {
            var Test = new Instant();

            Assert.Equals(IsoChronology.Utc, Test.Chronology);
            Assert.Equals(DateTimeZone.Utc, Test.Zone);
            Assert.Equals(Test_TIME_NOW, Test.Milliseconds);
        }

        public void TestEqualsHashCode()
        {
            var Test1 = new Instant(Test_TIME1);
            var Test2 = new Instant(Test_TIME1);
            Assert.IsTrue(Test1.Equals(Test2));
            Assert.IsTrue(Test2.Equals(Test1));
            Assert.IsTrue(Test1.Equals(Test1));
            Assert.IsTrue(Test2.Equals(Test2));
            Assert.IsTrue(Test1.GetHashCode() == Test2.GetHashCode());
            Assert.IsTrue(Test1.GetHashCode() == Test1.GetHashCode());
            Assert.IsTrue(Test2.GetHashCode() == Test2.GetHashCode());

            var Test3 = new Instant(Test_TIME2);
            Assert.IsFalse(Test1.Equals(Test3));
            Assert.IsFalse(Test2.Equals(Test3));
            Assert.IsFalse(Test3.Equals(Test1));
            Assert.IsFalse(Test3.Equals(Test2));
            Assert.IsFalse(Test1.GetHashCode() == Test3.GetHashCode());
            Assert.IsFalse(Test2.GetHashCode() == Test3.GetHashCode());

            Assert.IsFalse(Test1.Equals("Hello"));
            Assert.IsTrue(Test1.Equals(new MockInstant()));
            Assert.IsFalse(Test1.Equals(new DateTime(Test_TIME1)));
        }

        private class MockInstant : AbstractInstant
        {
            public override string ToString()
            {
                return null;
            }

            public override long Milliseconds
            {
                get { return Test_TIME1; }
            }
            public override IChronology Chronology
            {
                get { return IsoChronology.Utc; }
            }
        }

        public void TestCompareTo()
        {
            var Test1 = new Instant(Test_TIME1);
            var Test1a = new Instant(Test_TIME1);
            Assert.Equals(0, Test1.CompareTo(Test1a));
            Assert.Equals(0, Test1a.CompareTo(Test1));
            Assert.Equals(0, Test1.CompareTo(Test1));
            Assert.Equals(0, Test1a.CompareTo(Test1a));

            var Test2 = new Instant(Test_TIME2);
            Assert.Equals(-1, Test1.CompareTo(Test2));
            Assert.Equals(+1, Test2.CompareTo(Test1));

            var Test3 = new DateTime(Test_TIME2, GregorianChronology.GetInstance(PARIS));
            Assert.Equals(-1, Test1.CompareTo(Test3));
            Assert.Equals(+1, Test3.CompareTo(Test1));
            Assert.Equals(0, Test3.CompareTo(Test2));

            Assert.Equals(+1, Test2.CompareTo(new MockInstant()));
            Assert.Equals(0, Test1.CompareTo(new MockInstant()));

            // Note: JodaTime Tests for NullPointerException here: WRONG!
            Assert.Throws<ArgumentNullException>(() => Test1.CompareTo(null));

            // Note: This comment was on the JodaTime code. Not sure what it was meant for
            // We should probably delete it
            // try {
            //     Test1.compareTo(new Date());
            //     fail();
            // } catch (ClassCastException ex) {}
        }

        public void TestIsEqual_long()
        {
            Assert.IsFalse(new Instant(Test_TIME1).IsEqual(Test_TIME2));
            Assert.IsTrue(new Instant(Test_TIME1).IsEqual(Test_TIME1));
            Assert.IsFalse(new Instant(Test_TIME2).IsEqual(Test_TIME1));
        }

        public void TestIsEqualNow()
        {
            Assert.IsFalse(new Instant(Test_TIME_NOW - 1).IsEqualNow());
            Assert.IsTrue(new Instant(Test_TIME_NOW).IsEqualNow());
            Assert.IsFalse(new Instant(Test_TIME_NOW + 1).IsEqualNow());
        }

        /// <summary>
        /// Original name: TestIsEqual_RI
        /// </summary>
        public void TestIsEqual_Instant()
        {
            var Test1 = new Instant(Test_TIME1);
            var Test1a = new Instant(Test_TIME1);
            Assert.IsTrue(Test1.IsEqual(Test1a));
            Assert.IsTrue(Test1a.IsEqual(Test1));
            Assert.IsTrue(Test1.IsEqual(Test1));
            Assert.IsTrue(Test1a.IsEqual(Test1a));

            var Test2 = new Instant(Test_TIME2);
            Assert.IsFalse(Test1.IsEqual(Test2));
            Assert.IsFalse(Test2.IsEqual(Test1));

            var Test3 = new DateTime(Test_TIME2, GregorianChronology.GetInstance(PARIS));
            Assert.IsFalse(Test1.IsEqual(Test3));
            Assert.IsFalse(Test3.IsEqual(Test1));
            Assert.IsTrue(Test3.IsEqual(Test2));

            Assert.IsFalse(Test2.IsEqual(new MockInstant()));
            Assert.IsTrue(Test1.IsEqual(new MockInstant()));

            Assert.IsFalse(new Instant(Test_TIME_NOW + 1).IsEqual(null));
            Assert.IsTrue(new Instant(Test_TIME_NOW).IsEqual(null));
            Assert.IsFalse(new Instant(Test_TIME_NOW - 1).IsEqual(null));
        }

        public void TestIsBefore_long()
        {
            Assert.IsTrue(new Instant(Test_TIME1).IsBefore(Test_TIME2));
            Assert.IsFalse(new Instant(Test_TIME1).IsBefore(Test_TIME1));
            Assert.IsFalse(new Instant(Test_TIME2).IsBefore(Test_TIME1));
        }

        public void TestIsBeforeNow()
        {
            Assert.IsTrue(new Instant(Test_TIME_NOW - 1).IsBeforeNow());
            Assert.IsFalse(new Instant(Test_TIME_NOW).IsBeforeNow());
            Assert.IsFalse(new Instant(Test_TIME_NOW + 1).IsBeforeNow());
        }

        /// <summary>
        /// Original name: TestIsBefore_RI
        /// </summary>
        public void TestIsBefore_Instant()
        {
            var Test1 = new Instant(Test_TIME1);
            var Test1a = new Instant(Test_TIME1);
            Assert.IsFalse(Test1.IsBefore(Test1a));
            Assert.IsFalse(Test1a.IsBefore(Test1));
            Assert.IsFalse(Test1.IsBefore(Test1));
            Assert.IsFalse(Test1a.IsBefore(Test1a));

            var Test2 = new Instant(Test_TIME2);
            Assert.IsTrue(Test1.IsBefore(Test2));
            Assert.IsFalse(Test2.IsBefore(Test1));

            var Test3 = new DateTime(Test_TIME2, GregorianChronology.GetInstance(PARIS));
            Assert.IsTrue(Test1.IsBefore(Test3));
            Assert.IsFalse(Test3.IsBefore(Test1));
            Assert.IsFalse(Test3.IsBefore(Test2));

            Assert.IsFalse(Test2.IsBefore(new MockInstant()));
            Assert.IsFalse(Test1.IsBefore(new MockInstant()));

            Assert.IsFalse(new Instant(Test_TIME_NOW + 1).IsBefore(null));
            Assert.IsFalse(new Instant(Test_TIME_NOW).IsBefore(null));
            Assert.IsTrue(new Instant(Test_TIME_NOW - 1).IsBefore(null));
        }

        public void TestIsAfter_long()
        {
            Assert.IsFalse(new Instant(Test_TIME1).IsAfter(Test_TIME2));
            Assert.IsFalse(new Instant(Test_TIME1).IsAfter(Test_TIME1));
            Assert.IsTrue(new Instant(Test_TIME2).IsAfter(Test_TIME1));
        }

        public void TestIsAfterNow()
        {
            Assert.IsFalse(new Instant(Test_TIME_NOW - 1).IsAfterNow());
            Assert.IsFalse(new Instant(Test_TIME_NOW).IsAfterNow());
            Assert.IsTrue(new Instant(Test_TIME_NOW + 1).IsAfterNow());
        }

        /// <summary>
        /// Original name: TestIsAfter_RI
        /// </summary>
        public void TestIsAfter_Instant()
        {
            var Test1 = new Instant(Test_TIME1);
            var Test1a = new Instant(Test_TIME1);
            Assert.IsFalse(Test1.IsAfter(Test1a));
            Assert.IsFalse(Test1a.IsAfter(Test1));
            Assert.IsFalse(Test1.IsAfter(Test1));
            Assert.IsFalse(Test1a.IsAfter(Test1a));

            var Test2 = new Instant(Test_TIME2);
            Assert.IsFalse(Test1.IsAfter(Test2));
            Assert.IsTrue(Test2.IsAfter(Test1));

            var Test3 = new DateTime(Test_TIME2, GregorianChronology.GetInstance(PARIS));
            Assert.IsFalse(Test1.IsAfter(Test3));
            Assert.IsTrue(Test3.IsAfter(Test1));
            Assert.IsFalse(Test3.IsAfter(Test2));

            Assert.IsTrue(Test2.IsAfter(new MockInstant()));
            Assert.IsFalse(Test1.IsAfter(new MockInstant()));

            Assert.IsTrue(new Instant(Test_TIME_NOW + 1).IsAfter(null));
            Assert.IsFalse(new Instant(Test_TIME_NOW).IsAfter(null));
            Assert.IsFalse(new Instant(Test_TIME_NOW - 1).IsAfter(null));
        }

        //    public void TestSerialization() throws Exception {
        //        Instant Test = new Instant(Test_TIME_NOW);
        //        
        //        ByteArrayOutputStream baos = new ByteArrayOutputStream();
        //        ObjectOutputStream oos = new ObjectOutputStream(baos);
        //        oos.writeObject(Test);
        //        byte[] bytes = baos.toByteArray();
        //        oos.close();
        //        
        //        ByteArrayInputStream bais = new ByteArrayInputStream(bytes);
        //        ObjectInputStream ois = new ObjectInputStream(bais);
        //        Instant result = (Instant) ois.readObject();
        //        ois.close();
        //        
        //        assertEquals(Test, result);
        //    }

        public void TestToString()
        {
            var Test = new Instant(Test_TIME_NOW);
            Assert.Equals("2002-06-09T00:00:00.000Z", Test.ToString());
        }

        public void TestToInstant()
        {
            var Test = new Instant(Test_TIME1);
            var result = Test.ToInstant();
            Assert.AreSame(Test, result);
        }

        public void TestToDateTime()
        {
            var Test = new Instant(Test_TIME1);
            var result = Test.ToDateTime();
            Assert.Equals(Test_TIME1, result.Milliseconds);
            Assert.Equals(IsoChronology.GetInstance(), result.Chronology);
        }

        // Note: This was Testing a deprecated method. We are not porting deprecated API
        //public void TestToDateTimeISO() {
        //    Instant Test = new Instant(Test_TIME1);
        //    DateTime result = Test.toDateTimeISO();
        //    Assert.AreSame(DateTime.class, result.getClass());
        //    Assert.AreSame(IsoChronology.class, result.Chronology.getClass());
        //    assertEquals(Test.Milliseconds, result.Milliseconds);
        //    assertEquals(IsoChronology.GetInstance(), result.Chronology);
        //}

        public void TestToDateTime_DateTimeZone()
        {
            var Test = new Instant(Test_TIME1);
            var result = Test.ToDateTime(LONDON);
            Assert.Equals(Test.Milliseconds, result.Milliseconds);
            Assert.Equals(IsoChronology.GetInstance(LONDON), result.Chronology);

            Test = new Instant(Test_TIME1);
            result = Test.ToDateTime(PARIS);
            Assert.Equals(Test.Milliseconds, result.Milliseconds);
            Assert.Equals(IsoChronology.GetInstance(PARIS), result.Chronology);

            Test = new Instant(Test_TIME1);
            result = Test.ToDateTime((DateTimeZone) null);
            Assert.Equals(Test.Milliseconds, result.Milliseconds);
            Assert.Equals(IsoChronology.GetInstance(), result.Chronology);
        }

        public void TestToDateTime_Chronology()
        {
            var Test = new Instant(Test_TIME1);
            var result = Test.ToDateTime(IsoChronology.GetInstance());
            Assert.Equals(Test.Milliseconds, result.Milliseconds);
            Assert.Equals(IsoChronology.GetInstance(), result.Chronology);

            Test = new Instant(Test_TIME1);
            result = Test.ToDateTime(GregorianChronology.GetInstance(PARIS));
            Assert.Equals(Test.Milliseconds, result.Milliseconds);
            Assert.Equals(GregorianChronology.GetInstance(PARIS), result.Chronology);

            Test = new Instant(Test_TIME1);
            result = Test.ToDateTime((IChronology) null);
            Assert.Equals(IsoChronology.GetInstance(), result.Chronology);
        }

        public void TestToMutableDateTime()
        {
            var Test = new Instant(Test_TIME1);
            var result = Test.ToMutableDateTime();
            Assert.Equals(Test.Milliseconds, result.Milliseconds);
            Assert.Equals(IsoChronology.GetInstance(), result.Chronology);
        }

        // Note: Another method Testing deprecated APIs
        //public void TestToMutableDateTimeISO() {
        //    Instant Test = new Instant(Test_TIME1);
        //    MutableDateTime result = Test.ToMutableDateTimeISO();
        //    Assert.AreSame(MutableDateTime.class, result.getClass());
        //    Assert.AreSame(IsoChronology.class, result.Chronology.getClass());
        //    assertEquals(Test.Milliseconds, result.Milliseconds);
        //    assertEquals(IsoChronology.GetInstance(), result.Chronology);
        //}

        public void TestToMutableDateTime_DateTimeZone()
        {
            var Test = new Instant(Test_TIME1);
            var result = Test.ToMutableDateTime(LONDON);
            Assert.Equals(Test.Milliseconds, result.Milliseconds);
            Assert.Equals(IsoChronology.GetInstance(), result.Chronology);

            Test = new Instant(Test_TIME1);
            result = Test.ToMutableDateTime(PARIS);
            Assert.Equals(Test.Milliseconds, result.Milliseconds);
            Assert.Equals(IsoChronology.GetInstance(PARIS), result.Chronology);

            Test = new Instant(Test_TIME1);
            result = Test.ToMutableDateTime((DateTimeZone) null);
            Assert.Equals(Test.Milliseconds, result.Milliseconds);
            Assert.Equals(IsoChronology.GetInstance(), result.Chronology);
        }

        public void TestToMutableDateTime_Chronology()
        {
            var Test = new Instant(Test_TIME1);
            var result = Test.ToMutableDateTime(IsoChronology.GetInstance());
            Assert.Equals(Test.Milliseconds, result.Milliseconds);
            Assert.Equals(IsoChronology.GetInstance(), result.Chronology);

            Test = new Instant(Test_TIME1);
            result = Test.ToMutableDateTime(GregorianChronology.GetInstance(PARIS));
            Assert.Equals(Test.Milliseconds, result.Milliseconds);
            Assert.Equals(GregorianChronology.GetInstance(PARIS), result.Chronology);

            Test = new Instant(Test_TIME1);
            result = Test.ToMutableDateTime((IChronology) null);
            Assert.Equals(Test.Milliseconds, result.Milliseconds);
            Assert.Equals(IsoChronology.GetInstance(), result.Chronology);
        }

        public void TestWithMillis_long()
        {
            var Test = new Instant(Test_TIME1);
            var result = Test.WithMilliseconds(Test_TIME2);
            Assert.Equals(Test_TIME2, result.Milliseconds);
            Assert.Equals(Test.Chronology, result.Chronology);

            Test = new Instant(Test_TIME1);
            result = Test.WithMilliseconds(Test_TIME1);
            Assert.AreSame(Test, result);
        }

        public void TestWithDurationAdded_long_int()
        {
            var Test = new Instant(Test_TIME1);
            var result = Test.WithDurationAdded(123456789L, 1);
            var expected = new Instant(Test_TIME1 + 123456789L);
            Assert.Equals(expected, result);

            result = Test.WithDurationAdded(123456789L, 0);
            Assert.AreSame(Test, result);

            result = Test.WithDurationAdded(123456789L, 2);
            expected = new Instant(Test_TIME1 + (2L * 123456789L));
            Assert.Equals(expected, result);

            result = Test.WithDurationAdded(123456789L, -3);
            expected = new Instant(Test_TIME1 - (3L * 123456789L));
            Assert.Equals(expected, result);
        }

        /// <summary>
        /// Original name: testWithDurationAdded_RD_int
        /// </summary>
        public void TestWithDurationAdded_Duration_int()
        {
            var Test = new Instant(Test_TIME1);
            var result = Test.WithDurationAdded(new Duration(123456789L), 1);
            var expected = new Instant(Test_TIME1 + 123456789L);
            Assert.Equals(expected, result);

            result = Test.WithDurationAdded(null, 1);
            Assert.AreSame(Test, result);

            result = Test.WithDurationAdded(new Duration(123456789L), 0);
            Assert.AreSame(Test, result);

            result = Test.WithDurationAdded(new Duration(123456789L), 2);
            expected = new Instant(Test_TIME1 + (2L * 123456789L));
            Assert.Equals(expected, result);

            result = Test.WithDurationAdded(new Duration(123456789L), -3);
            expected = new Instant(Test_TIME1 - (3L * 123456789L));
            Assert.Equals(expected, result);
        }

        public void TestPlus_long()
        {
            var Test = new Instant(Test_TIME1);
            var result = Test.Plus(123456789L);
            var expected = new Instant(Test_TIME1 + 123456789L);
            Assert.Equals(expected, result);
        }

        /// <summary>
        /// Original name: testPlus_RD
        /// </summary>
        public void TestPlus_Duration()
        {
            var Test = new Instant(Test_TIME1);
            var result = Test.Plus(new Duration(123456789L));
            var expected = new Instant(Test_TIME1 + 123456789L);
            Assert.Equals(expected, result);

            result = Test.Plus(null);
            Assert.AreSame(Test, result);
        }

        //-----------------------------------------------------------------------    
        public void TestMinus_long()
        {
            var Test = new Instant(Test_TIME1);
            var result = Test.Minus(123456789L);
            var expected = new Instant(Test_TIME1 - 123456789L);
            Assert.Equals(expected, result);
        }

        /// <summary>
        /// Original name: testMinus_RD
        /// </summary>
        public void TestMinus_Duration()
        {
            var Test = new Instant(Test_TIME1);
            var result = Test.Minus(new Duration(123456789L));
            var expected = new Instant(Test_TIME1 - 123456789L);
            Assert.Equals(expected, result);

            result = Test.Minus(null);
            Assert.AreSame(Test, result);
        }

        // Note: Not sure if a class being sealed is a nice thing to test here
        //public void TestImmutable() {
        //    assertTrue(Modifier.isFinal(Instant.class.getModifiers()));
        //}
    }
}

}