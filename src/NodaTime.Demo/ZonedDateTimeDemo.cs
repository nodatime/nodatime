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
        public void AddDurationAndIsDaylightSavingTime()
        {
            // This is the date when DST ended at 2am in the given time zone.
            var dt = new LocalDateTime(2017, 10, 29, 1, 45, 0);

            DateTimeZone dublin = DateTimeZoneProviders.Tzdb["Europe/Dublin"];

            ZonedDateTime beforeTransition = new ZonedDateTime(dt, dublin, Offset.FromHours(1));

            Assert.AreEqual(true, Snippet.For(beforeTransition.IsDaylightSavingTime()));
            var result = Snippet.For(ZonedDateTime.Add(beforeTransition, Duration.FromHours(1)));
            Assert.AreEqual(new LocalDate(2017, 10, 29), result.Date);
            Assert.AreEqual(new LocalTime(1, 45, 0), result.TimeOfDay);
            Assert.AreEqual(Offset.FromHours(0), result.Offset);
            Assert.AreEqual(false, result.IsDaylightSavingTime());

            var result2 = Snippet.For(beforeTransition + Duration.FromHours(1));
            Assert.AreEqual(result, result2);

            ZonedDateTime afterTransition = new ZonedDateTime(dt, dublin, Offset.FromHours(0));
            result = ZonedDateTime.Add(afterTransition, Duration.FromDays(1));
            Assert.AreEqual(new LocalTime(1, 45, 0), result.TimeOfDay);
            Assert.AreEqual(new LocalDate(2017, 10, 30), result.Date);
        }

        [Test]
        public void PlusDuration()
        {
            // This is the date when DST ended at 2am in the given time zone.
            var dt = new LocalDateTime(2017, 10, 29, 1, 45, 0);

            DateTimeZone dublin = DateTimeZoneProviders.Tzdb["Europe/Dublin"];

            ZonedDateTime beforeTransition = new ZonedDateTime(dt, dublin, Offset.FromHours(1));

            var result = Snippet.For(beforeTransition.Plus(Duration.FromHours(1)));

            Assert.AreEqual(new LocalDate(2017, 10, 29), result.Date);
            Assert.AreEqual(new LocalTime(1, 45, 0), result.TimeOfDay);
            Assert.AreEqual(Offset.FromHours(0), result.Offset);

            ZonedDateTime afterTransition = new ZonedDateTime(dt, dublin, Offset.FromHours(0));
            result = afterTransition.Plus(Duration.FromDays(1));
            Assert.AreEqual(new LocalTime(1, 45, 0), result.TimeOfDay);
            Assert.AreEqual(new LocalDate(2017, 10, 30), result.Date);
        }

        [Test]
        public void PlusHoursMinutesEtc()
        {
            DateTimeZone dublin = DateTimeZoneProviders.Tzdb["Europe/Dublin"];
            var start = Instant.FromUtc(2017, 7, 17, 7, 17);
            ZonedDateTime subject = new ZonedDateTime(start, dublin);
            Assert.AreEqual(new ZonedDateTime(start + Duration.FromNanoseconds(1), dublin),
                Snippet.For(subject.PlusNanoseconds(1)));
            Assert.AreEqual(new ZonedDateTime(start + Duration.FromTicks(1), dublin),
                Snippet.For(subject.PlusTicks(1)));
            Assert.AreEqual(new ZonedDateTime(start + Duration.FromMilliseconds(1), dublin),
                Snippet.For(subject.PlusMilliseconds(1)));
            Assert.AreEqual(new ZonedDateTime(start + Duration.FromSeconds(1), dublin),
                Snippet.For(subject.PlusSeconds(1)));
            Assert.AreEqual(new ZonedDateTime(start + Duration.FromMinutes(1), dublin),
                Snippet.For(subject.PlusMinutes(1)));
            Assert.AreEqual(new ZonedDateTime(start + Duration.FromHours(1), dublin),
                Snippet.For(subject.PlusHours(1)));
        }

        [Test]
        public void SubtractDuration()
        {
            // This is the date when DST ended at 2am in the given time zone.
            var dt = new LocalDateTime(2017, 10, 29, 1, 45, 0);

            DateTimeZone dublin = DateTimeZoneProviders.Tzdb["Europe/Dublin"];

            ZonedDateTime afterDstTransition = new ZonedDateTime(dt, dublin, Offset.FromHours(0));

            var result = Snippet.For(ZonedDateTime.Subtract(afterDstTransition, Duration.FromHours(1)));

            Assert.AreEqual(new LocalDate(2017, 10, 29), result.Date);
            Assert.AreEqual(new LocalTime(1, 45, 0), result.TimeOfDay);
            Assert.AreEqual(Offset.FromHours(1), result.Offset);

            var result2 = Snippet.For(afterDstTransition - Duration.FromHours(1));
            Assert.AreEqual(result, result2);

            ZonedDateTime beforeTransition = new ZonedDateTime(dt, dublin, Offset.FromHours(1));
            result = ZonedDateTime.Subtract(beforeTransition, Duration.FromDays(1));
            Assert.AreEqual(new LocalTime(1, 45, 0), result.TimeOfDay);
            Assert.AreEqual(new LocalDate(2017, 10, 28), result.Date);
        }

        [Test]
        public void SubtractZonedDateTime()
        {
            var zone = DateTimeZone.ForOffset(Offset.FromHours(-5));
            ZonedDateTime subject = new ZonedDateTime(Instant.FromUtc(2017, 7, 17, 7, 17), zone);
            ZonedDateTime other = new ZonedDateTime(Instant.FromUtc(2017, 7, 17, 9, 17), zone);

            var difference = Snippet.For(ZonedDateTime.Subtract(other, subject));
            Assert.AreEqual(Duration.FromHours(2), difference);
        }

        [Test]
        public void MinusDuration()
        {
            // This is the date when DST ended at 2am in the given time zone.
            var dt = new LocalDateTime(2017, 10, 29, 1, 45, 0);

            DateTimeZone dublin = DateTimeZoneProviders.Tzdb["Europe/Dublin"];

            ZonedDateTime afterDstTransition = new ZonedDateTime(dt, dublin, Offset.FromHours(0));

            var result = Snippet.For(afterDstTransition.Minus(Duration.FromHours(1)));

            Assert.AreEqual(new LocalDate(2017, 10, 29), result.Date);
            Assert.AreEqual(new LocalTime(1, 45, 0), result.TimeOfDay);
            Assert.AreEqual(Offset.FromHours(1), result.Offset);

            ZonedDateTime beforeTransition = new ZonedDateTime(dt, dublin, Offset.FromHours(1));
            result = beforeTransition.Minus(Duration.FromDays(1));
            Assert.AreEqual(new LocalTime(1, 45, 0), result.TimeOfDay);
            Assert.AreEqual(new LocalDate(2017, 10, 28), result.Date);
        }

        [Test]
        public void MinusZonedDateTime()
        {
            var zone = DateTimeZone.ForOffset(Offset.FromHours(-5));
            ZonedDateTime subject = new ZonedDateTime(Instant.FromUtc(2017, 7, 17, 7, 17), zone);
            ZonedDateTime other = new ZonedDateTime(Instant.FromUtc(2017, 7, 17, 9, 17), zone);

            var difference = Snippet.For(other.Minus(subject));
            Assert.AreEqual(Duration.FromHours(2), difference);
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