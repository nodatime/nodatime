#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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

namespace NodaTime.Test
{
    // Test is commented out as little of it makes sense at the moment. We may or may not want some of it :)
    /*
    public partial class InstantTest
    {
        [Test]
        public void ToString_Format()
        {
            Assert.AreEqual("2002-06-09T00:00:00.000Z", new Instant(TestTimeNow).ToString());
            Assert.AreEqual("2002-04-05T12:24:00.000Z", new Instant(TestTime1).ToString());
            Assert.AreEqual("2003-05-06T14:28:00.000Z", new Instant(TestTime2).ToString());
        }

        [Test]
        public void DateTimeFieldTypeIndexer()
        {
            var test = new Instant(TestTime1); // 2002-06-09
            Assert.AreEqual(1, test[DateTimeFieldType.Era]);
            Assert.AreEqual(20, test[DateTimeFieldType.CenturyOfEra]);
            Assert.AreEqual(2, test[DateTimeFieldType.YearOfCentury]);
            Assert.AreEqual(2002, test[DateTimeFieldType.YearOfEra]);
            Assert.AreEqual(2002, test[DateTimeFieldType.Year]);
            Assert.AreEqual(6, test[DateTimeFieldType.MonthOfYear]);
            Assert.AreEqual(9, test[DateTimeFieldType.DayOfMonth]);
            Assert.AreEqual(2002, test[DateTimeFieldType.Weekyear]);
            Assert.AreEqual(23, test[DateTimeFieldType.WeekOfWeekyear]);
            Assert.AreEqual(7, test[DateTimeFieldType.DayOfWeek]);
            Assert.AreEqual(160, test[DateTimeFieldType.DayOfYear]);
            Assert.AreEqual(0, test[DateTimeFieldType.HalfdayOfDay]);
            Assert.AreEqual(0, test[DateTimeFieldType.HourOfHalfday]);
            Assert.AreEqual(24, test[DateTimeFieldType.ClockhourOfDay]);
            Assert.AreEqual(12, test[DateTimeFieldType.ClockhourOfHalfday]);
            Assert.AreEqual(0, test[DateTimeFieldType.HourOfDay]);
            Assert.AreEqual(0, test[DateTimeFieldType.MinuteOfHour]);
            Assert.AreEqual(0, test[DateTimeFieldType.MinuteOfDay]);
            Assert.AreEqual(0, test[DateTimeFieldType.SecondOfMinute]);
            Assert.AreEqual(0, test[DateTimeFieldType.SecondOfDay]);
            Assert.AreEqual(0, test[DateTimeFieldType.MillisOfSecond]);
            Assert.AreEqual(0, test[DateTimeFieldType.MillisOfDay]);
        }

        [Test]
        public void DateTimeFieldTypeIndexer_InvalidArgument_ThrowsException()
        {
            var test = new Instant(TestTime1); // 2002-06-09
            DateTimeFieldType invalid = (DateTimeFieldType) (-1);
            // Need to call ToString() as a property access isn't valid on its own
            Assert.Throws<ArgumentOutOfRangeException>(() => test[invalid].ToString());
        }

        [Test]
        public void DateTimeFieldIndexer()
        {
            var test = new Instant(TestTimeNow); // 2002-06-09
            Assert.AreEqual(1, test[IsoChronology.SystemDefault.Era]);
            Assert.AreEqual(20, test[IsoChronology.SystemDefault.CenturyOfEra]);
            Assert.AreEqual(2, test[IsoChronology.SystemDefault.YearOfCentury]);
            Assert.AreEqual(2002, test[IsoChronology.SystemDefault.YearOfEra]);
            Assert.AreEqual(2002, test[IsoChronology.SystemDefault.Year]);
            Assert.AreEqual(6, test[IsoChronology.SystemDefault.MonthOfYear]);
            Assert.AreEqual(9, test[IsoChronology.SystemDefault.DayOfMonth]);
            Assert.AreEqual(2002, test[IsoChronology.SystemDefault.Weekyear]);
            Assert.AreEqual(23, test[IsoChronology.SystemDefault.WeekOfWeekyear]);
            Assert.AreEqual(7, test[IsoChronology.SystemDefault.DayOfWeek]);
            Assert.AreEqual(160, test[IsoChronology.SystemDefault.DayOfYear]);
            Assert.AreEqual(0, test[IsoChronology.SystemDefault.HalfdayOfDay]);
            Assert.AreEqual(1, test[IsoChronology.SystemDefault.HourOfHalfday]);
            Assert.AreEqual(1, test[IsoChronology.SystemDefault.ClockhourOfDay]);
            Assert.AreEqual(1, test[IsoChronology.SystemDefault.ClockhourOfHalfday]);
            Assert.AreEqual(1, test[IsoChronology.SystemDefault.HourOfDay]);
            Assert.AreEqual(0, test[IsoChronology.SystemDefault.MinuteOfHour]);
            Assert.AreEqual(60, test[IsoChronology.SystemDefault.MinuteOfDay]);
            Assert.AreEqual(0, test[IsoChronology.SystemDefault.SecondOfMinute]);
            Assert.AreEqual(60 * 60, test[IsoChronology.SystemDefault.SecondOfDay]);
            Assert.AreEqual(0, test[IsoChronology.SystemDefault.MillisecondsOfSecond]);
            Assert.AreEqual(60 * 60 * 1000, test[IsoChronology.SystemDefault.MillisecondsOfDay]);
            Assert.Throws<ArgumentException>(() => { var ignored = test[null]; });
        }

        [Test]
        public void DateTimeFieldIndexer_NullArgument_ThrowsException()
        {
            var test = new Instant(TestTime1); // 2002-06-09
            DateTimeField invalid = null;
            // Need to call ToString() as a property access isn't valid on its own
            Assert.Throws<ArgumentNullException>(() => test[invalid].ToString());
        }

        [Test]
        public void Now_UsesCurrentClock()
        {
            using (TestClock.ReplaceCurrent(TestTimeNow))
            {
                var test = Instant.Now;

                Assert.AreEqual(IsoChronology.Utc, test.Chronology);
                Assert.AreEqual(DateTimeZone.Utc, test.Zone);
                Assert.AreEqual(TestTimeNow, test.Milliseconds);
            }
        }

        [Test]
        public void EqualsHashCode()
        {
            var test1a = new Instant(TestTime1);
            var test1b = new Instant(TestTime1);
            Assert.IsTrue(test1a.Equals(test1b));
            Assert.IsTrue(test1b.Equals(test1a));
            Assert.IsTrue(test1a.Equals(test1a));
            Assert.IsTrue(test1b.Equals(test1b));
            Assert.IsTrue(test1a.GetHashCode() == test1b.GetHashCode());
            Assert.IsTrue(test1a.GetHashCode() == test1a.GetHashCode());
            Assert.IsTrue(test1b.GetHashCode() == test1b.GetHashCode());

            var test2 = new Instant(TestTime2);
            Assert.IsFalse(test1a.Equals(test2));
            Assert.IsFalse(test1b.Equals(test2));
            Assert.IsFalse(test2.Equals(test1a));
            Assert.IsFalse(test2.Equals(test1b));
            Assert.IsFalse(test1a.GetHashCode() == test2.GetHashCode());
            Assert.IsFalse(test1b.GetHashCode() == test2.GetHashCode());

            // TODO: Work out exactly how we really want to handle equality.
            // I don't really like comparisons between different types.
            Assert.IsFalse(test1a.Equals("Hello"));
            Assert.IsTrue(test1a.Equals(new MockInstant()));
            Assert.IsFalse(test1a.Equals(new DateTime(TestTime1, IsoChronology.GetInstance(London))));
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
        public void CompareTo_Instant()
        {
            var test1a = new Instant(TestTime1);
            var test1b = new Instant(TestTime1);
            Assert.AreEqual(0, test1b.CompareTo(test1a));
            Assert.AreEqual(0, test1a.CompareTo(test1b));
            Assert.AreEqual(0, test1b.CompareTo(test1b));
            Assert.AreEqual(0, test1a.CompareTo(test1a));

            var test2 = new Instant(TestTime2);
            Assert.AreEqual(-1, test1b.CompareTo(test2));
            Assert.AreEqual(+1, test2.CompareTo(test1b));
        }

        [Test]
        public void CompareTo_DateTime()
        {
            var test1 = new Instant(TestTime1);
            var test2a = new Instant(TestTime2);
            var test2b = new DateTime(TestTime2, GregorianChronology.GetInstance(Paris));
            Assert.AreEqual(-1, test1.CompareTo(test2b));
            Assert.AreEqual(+1, test2b.CompareTo(test1));
            Assert.AreEqual(0, test2b.CompareTo(test2a));

            Assert.AreEqual(+1, test2a.CompareTo(new MockInstant()));
            Assert.AreEqual(0, test1.CompareTo(new MockInstant()));
        }

        public void CompareTo_NullArgument_ThrowsException()
        {
            var test = new Instant(TestTime1);
            Assert.Throws<ArgumentNullException>(() => test.CompareTo(null));
        }

        [Test]
        public void IsEqual_Int64()
        {
            Assert.IsTrue(new Instant(TestTime1).IsEqual(TestTime2));
            Assert.IsTrue(new Instant(TestTime1).IsEqual(TestTime1));
            Assert.IsFalse(new Instant(TestTime2).IsEqual(TestTime1));
        }

        [Test]
        public void IsEqualNow()
        {
            using (TestClock.ReplaceCurrent(TestTimeNow))
            {
                Assert.IsFalse(new Instant(TestTimeNow - 1).IsEqualNow());
                Assert.IsTrue(new Instant(TestTimeNow).IsEqualNow());
                Assert.IsFalse(new Instant(TestTimeNow + 1).IsEqualNow());
            }
        }

        /// <summary>
        /// Original name: TestIsEqual_RI
        /// </summary>
        [Test]
        public void IsEqual_Instant()
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
        public void IsBefore_Int64()
        {
            Assert.IsTrue(new Instant(TestTime1).IsBefore(TestTime2));
            Assert.IsFalse(new Instant(TestTime1).IsBefore(TestTime1));
            Assert.IsFalse(new Instant(TestTime2).IsBefore(TestTime1));
        }

        [Test]
        public void IsBeforeNow()
        {
            using (TestClock.ReplaceCurrent(TestTimeNow))
            {
                Assert.IsTrue(new Instant(TestTimeNow - 1).IsBeforeNow());
                Assert.IsFalse(new Instant(TestTimeNow).IsBeforeNow());
                Assert.IsFalse(new Instant(TestTimeNow + 1).IsBeforeNow());
            }
        }

        /// <summary>
        /// Original name: TestIsBefore_RI
        /// </summary>
        [Test]
        public void IsBefore_Instant()
        {
            var test1a = new Instant(TestTime1);
            var test1b = new Instant(TestTime1);
            Assert.IsFalse(test1a.IsBefore(test1b));
            Assert.IsFalse(test1b.IsBefore(test1a));
            Assert.IsFalse(test1a.IsBefore(test1a));
            Assert.IsFalse(test1b.IsBefore(test1b));

            var test2a = new Instant(TestTime2);
            Assert.IsTrue(test1a.IsBefore(test2a));
            Assert.IsFalse(test2a.IsBefore(test1a));

            var test2b = new DateTime(TestTime2, GregorianChronology.GetInstance(Paris));
            Assert.IsTrue(test1a.IsBefore(test2b));
            Assert.IsFalse(test2b.IsBefore(test1a));
            Assert.IsFalse(test2b.IsBefore(test2a));

            Assert.IsFalse(test2a.IsBefore(new MockInstant()));
            Assert.IsFalse(test1a.IsBefore(new MockInstant()));

            Assert.IsFalse(new Instant(TestTimeNow + 1).IsBefore(null));
            Assert.IsFalse(new Instant(TestTimeNow).IsBefore(null));
            Assert.IsTrue(new Instant(TestTimeNow - 1).IsBefore(null));
        }

        [Test]
        public void IsAfter_long()
        {
            Assert.IsFalse(new Instant(TestTime1).IsAfter(TestTime2));
            Assert.IsFalse(new Instant(TestTime1).IsAfter(TestTime1));
            Assert.IsTrue(new Instant(TestTime2).IsAfter(TestTime1));
        }

        [Test]
        public void IsAfterNow()
        {
            using (TestClock.ReplaceCurrent(TestTimeNow))
            {
                Assert.IsFalse(new Instant(TestTimeNow - 1).IsAfterNow());
                Assert.IsFalse(new Instant(TestTimeNow).IsAfterNow());
                Assert.IsTrue(new Instant(TestTimeNow + 1).IsAfterNow());
            }
        }

        /// <summary>
        /// Original name: TestIsAfter_RI
        /// </summary>
        [Test]
        public void IsAfter_Instant()
        {
            var test1a = new Instant(TestTime1);
            var test1b = new Instant(TestTime1);
            Assert.IsFalse(test1a.IsAfter(test1b));
            Assert.IsFalse(test1b.IsAfter(test1a));
            Assert.IsFalse(test1a.IsAfter(test1a));
            Assert.IsFalse(test1b.IsAfter(test1b));

            var test2a = new Instant(TestTime2);
            Assert.IsFalse(test1a.IsAfter(test2a));
            Assert.IsTrue(test2a.IsAfter(test1a));

            var test2b = new DateTime(TestTime2, GregorianChronology.GetInstance(Paris));
            Assert.IsFalse(test1a.IsAfter(test2b));
            Assert.IsTrue(test2b.IsAfter(test1a));
            Assert.IsFalse(test2b.IsAfter(test2a));

            Assert.IsTrue(test2a.IsAfter(new MockInstant()));
            Assert.IsFalse(test1a.IsAfter(new MockInstant()));
        }

        [Test]
        public void IsAfter_NullInstant_ComparesWithNow()
        {
            using (TestClock.ReplaceCurrent(TestTimeNow))
            {
                Assert.IsTrue(new Instant(TestTimeNow + 1).IsAfter(null));
                Assert.IsFalse(new Instant(TestTimeNow).IsAfter(null));
                Assert.IsFalse(new Instant(TestTimeNow - 1).IsAfter(null));
            }
        }

        /// <summary>
        /// TODO: Decide what exactly we're doing with serialization...
        /// </summary>
        [Test]
        public void Serialization_RoundTrips()
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
                Assert.AreEqual(test, result);
            }
        }

        [Test]
        public void ToString_ReturnsIsoFormatInUtc()
        {
            var test = new Instant(TestTimeNow);
            Assert.AreEqual("2002-06-09T00:00:00.000Z", test.ToString());
        }

        [Test]
        public void ToInstant_IsNoOp()
        {
            var test = new Instant(TestTime1);
            var result = test.ToInstant();
            Assert.AreSame(test, result);
        }

        /// <summary>
        /// TODO: Do we even want to support this?
        /// </summary>
        [Test]
        public void ToDateTime_WithoutDateTimeZone_UsesSystemDefault()
        {
            var test = new Instant(TestTime1);
            var result = test.ToDateTime();
            Assert.AreEqual(TestTime1, result.Milliseconds);
            Assert.AreEqual(IsoChronology.SystemDefault, result.Chronology);
        }

        // Note: This was Testing a deprecated method. We are not porting deprecated API
        //public void TestToDateTimeISO() {
        //    Instant test = new Instant(TestTime1);
        //    DateTime result = test.toDateTimeISO();
        //    Assert.AreSame(DateTime.class, result.getClass());
        //    Assert.AreSame(IsoChronology.class, result.Chronology.getClass());
        //    Assert.AreEqual(test.Milliseconds, result.Milliseconds);
        //    Assert.AreEqual(IsoChronology.SystemDefault, result.Chronology);
        //}

        [Test]
        public void ToDateTime_WithDateTimeZone()
        {
            var test = new Instant(TestTime1);
            var result = test.ToDateTime(London);
            Assert.AreEqual(test.Milliseconds, result.Milliseconds);
            Assert.AreEqual(IsoChronology.GetInstance(London), result.Chronology);

            test = new Instant(TestTime1);
            result = test.ToDateTime(Paris);
            Assert.AreEqual(test.Milliseconds, result.Milliseconds);
            Assert.AreEqual(IsoChronology.GetInstance(Paris), result.Chronology);

            test = new Instant(TestTime1);
            result = test.ToDateTime((DateTimeZone) null);
            Assert.AreEqual(test.Milliseconds, result.Milliseconds);
            Assert.AreEqual(IsoChronology.SystemDefault, result.Chronology);
        }

        [Test]
        public void ToDateTime_WithChronology()
        {
            var test = new Instant(TestTime1);
            var result = test.ToDateTime(IsoChronology.SystemDefault);
            Assert.AreEqual(test.Milliseconds, result.Milliseconds);
            Assert.AreEqual(IsoChronology.SystemDefault, result.Chronology);

            test = new Instant(TestTime1);
            result = test.ToDateTime(GregorianChronology.GetInstance(Paris));
            Assert.AreEqual(test.Milliseconds, result.Milliseconds);
            Assert.AreEqual(GregorianChronology.GetInstance(Paris), result.Chronology);
        }

        /// <summary>
        /// TODO: Decide if we want to support this
        /// </summary>
        [Test]
        public void ToDateTime_WithNullChronology_UsesSystemDefault()
        {
            var test = new Instant(TestTime1);
            DateTime result = test.ToDateTime((IChronology) null);
            Assert.AreEqual(IsoChronology.SystemDefault, result.Chronology);
        }

        [Test]
        public void WithMillis_Int64Value()
        {
            var test = new Instant(TestTime1);
            var result = test.WithMilliseconds(TestTime2);
            Assert.AreEqual(TestTime2, result.Milliseconds);
            Assert.AreEqual(test.Chronology, result.Chronology);

            test = new Instant(TestTime1);
            result = test.WithMilliseconds(TestTime1);
            Assert.AreSame(test, result);
        }

        [Test]
        public void WithDurationAdded_Int64AndInt()
        {
            var test = new Instant(TestTime1);
            var result = test.WithDurationAdded(123456789L, 1);
            var expected = new Instant(TestTime1 + 123456789L);
            Assert.AreEqual(expected, result);

            result = test.WithDurationAdded(123456789L, 0);
            Assert.AreSame(test, result);

            result = test.WithDurationAdded(123456789L, 2);
            expected = new Instant(TestTime1 + (2L * 123456789L));
            Assert.AreEqual(expected, result);

            result = test.WithDurationAdded(123456789L, -3);
            expected = new Instant(TestTime1 - (3L * 123456789L));
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Original name: testWithDurationAdded_RD_int
        /// </summary>
        [Test]
        public void WithDurationAdded_DurationAndInt()
        {
            var test = new Instant(TestTime1);
            var result = test.WithDurationAdded(new Duration(123456789L), 1);
            var expected = new Instant(TestTime1 + 123456789L);
            Assert.AreEqual(expected, result);

            result = test.WithDurationAdded(null, 1);
            Assert.AreSame(test, result);

            result = test.WithDurationAdded(new Duration(123456789L), 0);
            Assert.AreSame(test, result);

            result = test.WithDurationAdded(new Duration(123456789L), 2);
            expected = new Instant(TestTime1 + (2L * 123456789L));
            Assert.AreEqual(expected, result);

            result = test.WithDurationAdded(new Duration(123456789L), -3);
            expected = new Instant(TestTime1 - (3L * 123456789L));
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Plus_Int64()
        {
            var test = new Instant(TestTime1);
            var result = test.Plus(123456789L);
            var expected = new Instant(TestTime1 + 123456789L);
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Original name: testPlus_RD
        /// </summary>
        [Test]
        public void Plus_Duration()
        {
            var test = new Instant(TestTime1);
            var result = test.Plus(new Duration(123456789L));
            var expected = new Instant(TestTime1 + 123456789L);
            Assert.AreEqual(expected, result);

            result = test.Plus(null);
            Assert.AreSame(test, result);
        }

        [Test]
        public void Minus_Int64()
        {
            var test = new Instant(TestTime1);
            var result = test.Minus(123456789L);
            var expected = new Instant(TestTime1 - 123456789L);
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Original name: testMinus_RD
        /// </summary>
        [Test]
        public void Minus_Duration()
        {
            var test = new Instant(TestTime1);
            var result = test.Minus(new Duration(123456789L));
            var expected = new Instant(TestTime1 - 123456789L);
            Assert.AreEqual(expected, result);

            result = test.Minus(null);
            Assert.AreSame(test, result);
        }
    }*/
}