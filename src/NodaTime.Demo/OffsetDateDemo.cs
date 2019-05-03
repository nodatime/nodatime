// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime;
using Nunit.Framework;

namespace NodaTime.Demo
{
    public class OffsetDateDemo
    {
        [Test]
        public void Construction()
        {
            OffsetDate offsetDate = Snippet.For(new OffsetDate(
                new LocalDate(2019, 5, 3),
                Offset.FromHours(3)));

            Assert.AreEqual(Offset.FromHours(3), offsetDate.Offset);
            Assert.AreEqual(new LocalDate(2019, 5, 3), offsetDate.LocalDate);
        }

        [Test]
        public void WithOffset()
        {
            var original = new OffsetDate(
                new LocalDate(2019, 5, 3),
                Offset.FromHours(3));

            OffsetDate updated = Snippet.For(original.WithOffset(Offset.FromHours(-3)));
            Assert.AreEqual(original.Offset, updated.Offset);
            Assert.AreEqual(original.LocalDate, updated.LocalDate);
        }

        [Test]
        public void With()
        {
            var original = new OffsetDate(
                new LocalDate(2019, 5, 3),
                Offset.FromHours(3));

            Func<LocalDate, LocalDate> tomorrowfier = x => x.Plus(Period.FromDays(1));
            OffsetDate updated = Snippet.For(original.With(tomorrowfier));
            Assert.AreEqual(original.Offset, updated.Offset);
            Assert.AreEqual(tomorrowfier(original.LocalDate), updated.LocalDate);
        }

        [Test]
        public void WithCalendar()
        {
            var original = new OffsetDate(
                new LocalDate(2019, 5, 3, CalendarSystem.Iso),
                Offset.FromHours(3));

            OffsetDate updated = Snippet.For(original.WithCalendar(CalendarSystem.Gregorian));
            Assert.AreEqual(original.Offset, updated.Offset);
            Assert.AreEqual(original.LocalDate, updated.LocalDate);
            Assert.AreEqual(CalendarSystem.Gregorian, updated.Calendar);
        }
    }
}
