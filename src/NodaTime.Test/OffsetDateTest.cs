// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Calendars;
using NodaTime.Text;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace NodaTime.Test
{
    public class OffsetDateTest
    {
        [Test]
        public void LocalDateProperties()
        {
            LocalDate local = new LocalDate(2012, 6, 19, CalendarSystem.Julian);
            Offset offset = Offset.FromHours(5);

            OffsetDate od = new OffsetDate(local, offset);

            var localDateProperties = typeof(LocalDate).GetTypeInfo()
                .DeclaredProperties
                .ToDictionary(p => p.Name);
            var commonProperties = typeof(OffsetDate).GetTypeInfo()
                .DeclaredProperties
                .Where(p => localDateProperties.ContainsKey(p.Name));
            foreach (var property in commonProperties)
            {
                Assert.AreEqual(localDateProperties[property.Name].GetValue(local, null),
                                property.GetValue(od, null));
            }
        }

        [Test]
        public void ComponentProperties()
        {
            var date = new LocalDate(2012, 1, 2);
            var offset = Offset.FromHours(5);

            var offsetDate = new OffsetDate(date, offset);
            Assert.AreEqual(offset, offsetDate.Offset);
            Assert.AreEqual(date, offsetDate.Date);
        }

        [Test]
        public void Equality()
        {
            LocalDate date1 = new LocalDate(2012, 10, 6);
            LocalDate date2 = new LocalDate(2012, 9, 5);
            Offset offset1 = Offset.FromHours(1);
            Offset offset2 = Offset.FromHours(2);

            OffsetDate equal1 = new OffsetDate(date1, offset1);
            OffsetDate equal2 = new OffsetDate(date1, offset1);
            OffsetDate unequalByOffset = new OffsetDate(date1, offset2);
            OffsetDate unequalByLocal = new OffsetDate(date2, offset1);

            TestHelper.TestEqualsStruct(equal1, equal2, unequalByOffset);
            TestHelper.TestEqualsStruct(equal1, equal2, unequalByLocal);

            TestHelper.TestOperatorEquality(equal1, equal2, unequalByOffset);
            TestHelper.TestOperatorEquality(equal1, equal2, unequalByLocal);
        }

        [Test]
        public void At()
        {
            var date = new LocalDate(2012, 6, 19, CalendarSystem.Julian);
            var offset = Offset.FromHours(5);
            var time = new LocalTime(14, 15, 12).PlusNanoseconds(123456789);

            Assert.AreEqual(new OffsetDate(date, offset).At(time), date.At(time).WithOffset(offset));
        }

        [Test]
        public void WithOffset()
        {
            var date = new LocalDate(2012, 6, 19);
            var initial = new OffsetDate(date, Offset.FromHours(2));
            var actual = initial.WithOffset(Offset.FromHours(5));
            var expected = new OffsetDate(date, Offset.FromHours(5));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WithCalendar()
        {
            var julianDate = new LocalDate(2012, 6, 19, CalendarSystem.Julian);
            var isoDate = julianDate.WithCalendar(CalendarSystem.Iso);
            var offset = Offset.FromHours(5);
            var actual = new OffsetDate(julianDate, offset).WithCalendar(CalendarSystem.Iso);
            var expected = new OffsetDate(isoDate, offset);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WithAdjuster()
        {
            var initial = new OffsetDate(new LocalDate(2016, 6, 19), Offset.FromHours(-5));
            var actual = initial.With(DateAdjusters.StartOfMonth);
            var expected = new OffsetDate(new LocalDate(2016, 6, 1), Offset.FromHours(-5));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToString_WithFormat()
        {
            LocalDate date = new LocalDate(2012, 10, 6);
            Offset offset = Offset.FromHours(1);
            OffsetDate offsetDate = new OffsetDate(date, offset);
            Assert.AreEqual("2012/10/06 01", offsetDate.ToString("yyyy/MM/dd o<-HH>", CultureInfo.InvariantCulture));
        }

        [Test]
        public void ToString_WithNullFormat()
        {
            LocalDate date = new LocalDate(2012, 10, 6);
            Offset offset = Offset.FromHours(1);
            OffsetDate offsetDate = new OffsetDate(date, offset);
            Assert.AreEqual("2012-10-06+01", offsetDate.ToString(null, CultureInfo.InvariantCulture));
        }

        [Test]
        public void ToString_NoFormat()
        {
            LocalDate date = new LocalDate(2012, 10, 6);
            Offset offset = Offset.FromHours(1);
            OffsetDate offsetDate = new OffsetDate(date, offset);
            using (CultureSaver.SetCultures(CultureInfo.InvariantCulture))
            {
                Assert.AreEqual("2012-10-06+01", offsetDate.ToString());
            }
        }

        [Test]
        public void Deconstruction()
        {
            LocalDate date = new LocalDate(2015, 3, 28);
            Offset offset = Offset.FromHours(-2);
            OffsetDate offsetDate = new OffsetDate(date, offset);

            var (actualDate, actualOffset) = offsetDate;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(date, actualDate);
                Assert.AreEqual(offset, actualOffset);
            });
        }

        [Test]
        public void BinarySerialization()
        {
            var date = new LocalDate(2013, 4, 12);
            var offset = Offset.FromHoursAndMinutes(5, 30);
            var value = new OffsetDate(date, offset);
            TestHelper.AssertBinaryRoundtrip(value);
        }

        [Test]
        [TestCase(2013, 4, 12, "2013-04-12+05:30")]
        [TestCase(123, 4, 12, "0123-04-12+05:30")]
        public void XmlSerialization_Iso(int year, int month, int day, string expectedText)
        {
            var date = new LocalDate(year, month, day);
            var offset = Offset.FromHoursAndMinutes(5, 30);
            var value = new OffsetDate(date, offset);
            TestHelper.AssertXmlRoundtrip(value, $"<value>{expectedText}</value>");
        }

        // 1BC is absolute year 0, so 2BC is absolute year -1.
        // https://www.w3.org/TR/xmlschema-2/#dateTime recommends that for xs:date,
        // -0001 is used for 1BC... but that a later version would move to 0000 to be in line
        // with ISO-8601. We stick with ISO-8601.
        [Test]
        [TestCase(1, 4, 12, "0000-04-12+05:30")]
        [TestCase(2, 4, 12, "-0001-04-12+05:30")]
        [TestCase(3, 4, 12, "-0002-04-12+05:30")]
        public void XmlSerialization_Bce(int year, int month, int day, string expectedText)
        {
            var date = new LocalDate(Era.BeforeCommon, year, month, day);
            var offset = Offset.FromHoursAndMinutes(5, 30);
            var value = new OffsetDate(date, offset);
            TestHelper.AssertXmlRoundtrip(value, $"<value>{expectedText}</value>");
        }

        [Test]
        public void XmlSerialization_NonIso()
        {
            var date = new LocalDate(2013, 4, 12, CalendarSystem.Julian);
            var value = new OffsetDate(date, Offset.Zero);
            TestHelper.AssertXmlRoundtrip(value, "<value calendar=\"Julian\">2013-04-12Z</value>");
        }

        [Test]
        [TestCase("<value calendar=\"Rubbish\">2013-06-12</value>", typeof(KeyNotFoundException), Description = "Unknown calendar system")]
        [TestCase("<value>2013-15-12</value>", typeof(UnparsableValueException), Description = "Invalid month")]
        public void XmlSerialization_Invalid(string xml, Type expectedExceptionType)
        {
            TestHelper.AssertXmlInvalid<LocalDateTime>(xml, expectedExceptionType);
        }
    }
}
