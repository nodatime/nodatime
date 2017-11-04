// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
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
    }
}
