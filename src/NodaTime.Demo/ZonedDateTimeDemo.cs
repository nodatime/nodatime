// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;

namespace NodaTime.Demo
{
    internal class ZonedDateTimeDemo
    {
        // TODO: dublin used to be a static field. We could reintroduce that if the snippet
        // processor found every static field used in a snippet and added it at the start of the snippet...

        [Test]
        public void Construction()
        {
            DateTimeZone dublin = DateTimeZoneProviders.Tzdb["Europe/Dublin"];
            ZonedDateTime dt = Snippet.For(dublin.AtStrictly(new LocalDateTime(2010, 6, 9, 15, 15, 0)));

            Assert.AreEqual(15, dt.Hour);
            Assert.AreEqual(2010, dt.Year);

            Instant instant = Instant.FromUtc(2010, 6, 9, 14, 15, 0);
            Assert.AreEqual(instant, dt.ToInstant());
        }

        // TODO: Work out how to have multiple snippets for a single member.

        [Test]
        public void AmbiguousLocalDateTime()
        {
            DateTimeZone dublin = DateTimeZoneProviders.Tzdb["Europe/Dublin"];
            Assert.Throws<AmbiguousTimeException>(() => dublin.AtStrictly(new LocalDateTime(2010, 10, 31, 1, 15, 0)));
        }

        [Test]
        public void SkippedLocalDateTime()
        {
            DateTimeZone dublin = DateTimeZoneProviders.Tzdb["Europe/Dublin"];
            Assert.Throws<SkippedTimeException>(() => dublin.AtStrictly(new LocalDateTime(2010, 3, 28, 1, 15, 0)));
        }

        [Test]
        public void TickOfDay()
        {
            // This is a 25-hour day at the end of daylight saving time
            var dt = new LocalDate(2017, 10, 29);
            var time = new LocalTime(23, 59, 59);
            var dublin = DateTimeZoneProviders.Tzdb["Europe/Dublin"];

            var startOfDay = dublin.AtStartOfDay(dt);
            ZonedDateTime nearEndOfDay = dublin.AtStrictly(dt + time);

            Snippet.For(nearEndOfDay.TickOfDay);
            Assert.AreEqual(time.TickOfDay, nearEndOfDay.TickOfDay);

            Duration duration = nearEndOfDay - startOfDay;
            Assert.AreEqual(Duration.FromHours(25) - Duration.FromSeconds(1), duration);

            Assert.AreNotEqual(duration.TotalTicks, nearEndOfDay.TickOfDay);
        }

        [Test]
        public void NanosecondOfDay()
        {
            // This is a 25-hour day at the end of daylight saving time
            var dt = new LocalDate(2017, 10, 29);
            var time = new LocalTime(23, 59, 59);
            var dublin = DateTimeZoneProviders.Tzdb["Europe/Dublin"];

            var startOfDay = dublin.AtStartOfDay(dt);
            ZonedDateTime nearEndOfDay = dublin.AtStrictly(dt + time);

            Snippet.For(nearEndOfDay.NanosecondOfDay);
            Assert.AreEqual(time.NanosecondOfDay, nearEndOfDay.NanosecondOfDay);

            Duration duration = nearEndOfDay - startOfDay;
            Assert.AreEqual(Duration.FromHours(25) - Duration.FromSeconds(1), duration);

            Assert.AreNotEqual(duration.TotalNanoseconds, nearEndOfDay.NanosecondOfDay);
        }

    }
}