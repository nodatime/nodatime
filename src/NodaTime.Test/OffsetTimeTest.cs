// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Text;
using NUnit.Framework;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace NodaTime.Test
{
    public class OffsetTimeTest
    {
        [Test]
        public void LocalTimeProperties()
        {
            LocalTime local = new LocalTime(5, 23, 45).PlusNanoseconds(987654321);
            Offset offset = Offset.FromHours(5);

            OffsetTime od = new OffsetTime(local, offset);

            var localTimeProperties = typeof(LocalTime).GetTypeInfo()
                .DeclaredProperties
                .ToDictionary(p => p.Name);
            var commonProperties = typeof(OffsetTime).GetTypeInfo()
                .DeclaredProperties
                .Where(p => localTimeProperties.ContainsKey(p.Name));
            foreach (var property in commonProperties)
            {
                Assert.AreEqual(localTimeProperties[property.Name].GetValue(local, null),
                                property.GetValue(od, null));
            }
        }

        [Test]
        public void ComponentProperties()
        {
            var time = new LocalTime(12, 34, 15);
            var offset = Offset.FromHours(5);

            var offsetDate = new OffsetTime(time, offset);
            Assert.AreEqual(offset, offsetDate.Offset);
            Assert.AreEqual(time, offsetDate.TimeOfDay);
        }

        [Test]
        public void Equality()
        {
            LocalTime time1 = new LocalTime(4, 56, 23, 123);
            LocalTime time2 = new LocalTime(6, 23, 12, 987);
            Offset offset1 = Offset.FromHours(1);
            Offset offset2 = Offset.FromHours(2);

            OffsetTime equal1 = new OffsetTime(time1, offset1);
            OffsetTime equal2 = new OffsetTime(time1, offset1);
            OffsetTime unequalByOffset = new OffsetTime(time1, offset2);
            OffsetTime unequalByLocal = new OffsetTime(time2, offset1);

            TestHelper.TestEqualsStruct(equal1, equal2, unequalByOffset);
            TestHelper.TestEqualsStruct(equal1, equal2, unequalByLocal);

            TestHelper.TestOperatorEquality(equal1, equal2, unequalByOffset);
            TestHelper.TestOperatorEquality(equal1, equal2, unequalByLocal);
        }

        [Test]
        public void On()
        {
            var time = new LocalTime(14, 15, 12).PlusNanoseconds(123456789);
            var date = new LocalDate(2012, 6, 19, CalendarSystem.Julian);
            var offset = Offset.FromHours(5);

            Assert.AreEqual(new OffsetTime(time, offset).On(date), time.On(date).WithOffset(offset));
        }

        [Test]
        public void WithOffset()
        {
            var time = new LocalTime(14, 15, 12).PlusNanoseconds(123456789);
            var initial = new OffsetTime(time, Offset.FromHours(2));
            var actual = initial.WithOffset(Offset.FromHours(5));
            var expected = new OffsetTime(time, Offset.FromHours(5));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WithAdjuster()
        {
            var initial = new OffsetTime(new LocalTime(14, 15, 12), Offset.FromHours(-5));
            var actual = initial.With(TimeAdjusters.TruncateToHour);
            var expected = new OffsetTime(new LocalTime(14, 0), Offset.FromHours(-5));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToString_WithFormat()
        {
            LocalTime time = new LocalTime(14, 15, 12, 123);
            Offset offset = Offset.FromHours(1);
            OffsetTime offsetDate = new OffsetTime(time, offset);
            Assert.AreEqual("14:15:12.123 01", offsetDate.ToString("HH:mm:ss.fff o<-HH>", CultureInfo.InvariantCulture));
        }

        [Test]
        public void ToString_WithNullFormat()
        {
            LocalTime time = new LocalTime(14, 15, 12, 123);
            Offset offset = Offset.FromHours(1);
            OffsetTime offsetDate = new OffsetTime(time, offset);
            Assert.AreEqual("14:15:12+01", offsetDate.ToString(null, CultureInfo.InvariantCulture));
        }

        [Test]
        public void ToString_NoFormat()
        {
            LocalTime time = new LocalTime(14, 15, 12, 123);
            Offset offset = Offset.FromHours(1);
            OffsetTime offsetDate = new OffsetTime(time, offset);
            using (CultureSaver.SetCultures(CultureInfo.InvariantCulture))
            {
                Assert.AreEqual("14:15:12+01", offsetDate.ToString());
            }
        }

        [Test]
        public void Deconstruction()
        {
            var time = new LocalTime(15, 33);
            var offset = Offset.FromHours(-2);
            var offsetTime = new OffsetTime(time, offset);

            var (actualTime, actualOffset) = offsetTime;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(time, actualTime);
                Assert.AreEqual(offset, actualOffset);
            });
        }

        [Test]
        public void BinarySerialization()
        {
            var time = new LocalTime(5, 6, 7).PlusNanoseconds(123456789L);
            var offset = Offset.FromHoursAndMinutes(5, 30);
            var value = new OffsetTime(time, offset);
            TestHelper.AssertBinaryRoundtrip(value);
        }

        [Test]
        [TestCase(5, 6, 7, 123456789, 5, 30, "05:06:07.123456789+05:30")]
        [TestCase(5, 6, 7, 123456789, -5, -30, "05:06:07.123456789-05:30")]
        [TestCase(5, 6, 7, 0, 0, 0, "05:06:07Z")]
        public void XmlSerialization(
            int hour, int minute, int second, long nanoseconds,
            int offsetHours, int offsetMinutes,
            string expected)
        {
            var time = new LocalTime(hour, minute, second).PlusNanoseconds(nanoseconds);
            var offset = Offset.FromHoursAndMinutes(offsetHours, offsetMinutes);
            var value = new OffsetTime(time, offset);
            TestHelper.AssertXmlRoundtrip(value, $"<value>{expected}</value>");
        }        

        [Test]
        [TestCase("<value>05:24:00Z</value>", typeof(UnparsableValueException), Description = "Invalid hour")]
        public void XmlSerialization_Invalid(string xml, Type expectedExceptionType)
        {
            TestHelper.AssertXmlInvalid<LocalDateTime>(xml, expectedExceptionType);
        }
    }
}
