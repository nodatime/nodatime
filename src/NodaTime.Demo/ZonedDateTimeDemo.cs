// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Text;
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
            Assert.AreEqual(time.TickOfDay, Snippet.For(nearEndOfDay.TickOfDay));

            Duration duration = nearEndOfDay - startOfDay;
            Assert.AreEqual(Duration.FromHours(25) - Duration.FromSeconds(1), duration);
        }

        [Test]
        public void IsDaylightSavingTime()
        {
            // Europe/Dublin transitions from UTC+1 to UTC+0 at 2am (local) on 2017-10-29
            var dt = new LocalDateTime(2017, 10, 29, 1, 45, 0);
            DateTimeZone dublin = DateTimeZoneProviders.Tzdb["Europe/Dublin"];

            ZonedDateTime beforeTransition = new ZonedDateTime(dt, dublin, Offset.FromHours(1));
            Assert.AreEqual(true, Snippet.For(beforeTransition.IsDaylightSavingTime()));

            // Same local time, different offset - so a different instant, after the transition.
            ZonedDateTime afterTransition = new ZonedDateTime(dt, dublin, Offset.FromHours(0));
            Assert.AreEqual(false, Snippet.For(afterTransition.IsDaylightSavingTime()));
        }

        [Test]
        public void AddDuration()
        {
            // Europe/Dublin transitions from UTC+1 to UTC+0 at 2am (local) on 2017-10-29
            var dt = new LocalDateTime(2017, 10, 29, 1, 45, 0);
            DateTimeZone dublin = DateTimeZoneProviders.Tzdb["Europe/Dublin"];

            ZonedDateTime beforeTransition = new ZonedDateTime(dt, dublin, Offset.FromHours(1));

            var result = Snippet.For(ZonedDateTime.Add(beforeTransition, Duration.FromHours(1)));
            Assert.AreEqual(new LocalDate(2017, 10, 29), result.Date);
            // Adding an hour of elapsed time takes us across the DST transition, so we have
            // the same local time (shown on a clock) but a different offset.
            Assert.AreEqual(new ZonedDateTime(dt, dublin, Offset.FromHours(0)), result);

            // The + operator and Plus instance method are equivalent to the Add static method.
            var result2 = Snippet.For(beforeTransition + Duration.FromHours(1));
            var result3 = Snippet.For(beforeTransition.Plus(Duration.FromHours(1)));
            Assert.AreEqual(result, result2);
            Assert.AreEqual(result, result3);
        }        

        [Test]
        public void PlusHours()
        {
            DateTimeZone dublin = DateTimeZoneProviders.Tzdb["Europe/Dublin"];
            var start = Instant.FromUtc(2017, 7, 20, 5, 30);
            // Dublin is at UTC+1 in July 2017, so this is 6:30am.
            ZonedDateTime zoned = new ZonedDateTime(start, dublin);
            var pattern = ZonedDateTimePattern.ExtendedFormatOnlyIso;
            Assert.AreEqual("2017-07-20T07:30:00 Europe/Dublin (+01)",
                pattern.Format(Snippet.For(zoned.PlusHours(1))));
        }

        [Test]
        public void PlusMinutes()
        {
            DateTimeZone dublin = DateTimeZoneProviders.Tzdb["Europe/Dublin"];
            var start = Instant.FromUtc(2017, 7, 20, 5, 30);
            // Dublin is at UTC+1 in July 2017, so this is 6:30am.
            ZonedDateTime zoned = new ZonedDateTime(start, dublin);
            var pattern = ZonedDateTimePattern.ExtendedFormatOnlyIso;
            Assert.AreEqual("2017-07-20T06:31:00 Europe/Dublin (+01)",
                pattern.Format(Snippet.For(zoned.PlusMinutes(1))));
        }

        [Test]
        public void PlusSeconds()
        {
            DateTimeZone dublin = DateTimeZoneProviders.Tzdb["Europe/Dublin"];
            var start = Instant.FromUtc(2017, 7, 20, 5, 30);
            // Dublin is at UTC+1 in July 2017, so this is 6:30am.
            ZonedDateTime zoned = new ZonedDateTime(start, dublin);
            var pattern = ZonedDateTimePattern.ExtendedFormatOnlyIso;
            Assert.AreEqual("2017-07-20T06:30:01 Europe/Dublin (+01)",
                pattern.Format(Snippet.For(zoned.PlusSeconds(1))));
        }

        [Test]
        public void PlusMilliseconds()
        {
            DateTimeZone dublin = DateTimeZoneProviders.Tzdb["Europe/Dublin"];
            var start = Instant.FromUtc(2017, 7, 20, 5, 30);
            // Dublin is at UTC+1 in July 2017, so this is 6:30am.
            ZonedDateTime zoned = new ZonedDateTime(start, dublin);
            var pattern = ZonedDateTimePattern.ExtendedFormatOnlyIso;
            Assert.AreEqual("2017-07-20T06:30:00.001 Europe/Dublin (+01)",
                pattern.Format(Snippet.For(zoned.PlusMilliseconds(1))));
        }

        [Test]
        public void PlusTicks()
        {
            DateTimeZone dublin = DateTimeZoneProviders.Tzdb["Europe/Dublin"];
            var start = Instant.FromUtc(2017, 7, 20, 5, 30);
            // Dublin is at UTC+1 in July 2017, so this is 6:30am.
            ZonedDateTime zoned = new ZonedDateTime(start, dublin);
            var pattern = ZonedDateTimePattern.ExtendedFormatOnlyIso;
            Assert.AreEqual("2017-07-20T06:30:00.0000001 Europe/Dublin (+01)",
                pattern.Format(Snippet.For(zoned.PlusTicks(1))));
        }

        [Test]
        public void PlusNanoseconds()
        {
            DateTimeZone dublin = DateTimeZoneProviders.Tzdb["Europe/Dublin"];
            var start = Instant.FromUtc(2017, 7, 20, 5, 30);
            // Dublin is at UTC+1 in July 2017, so this is 6:30am.
            ZonedDateTime zoned = new ZonedDateTime(start, dublin);
            var pattern = ZonedDateTimePattern.ExtendedFormatOnlyIso;
            Assert.AreEqual("2017-07-20T06:30:00.000000001 Europe/Dublin (+01)",
                pattern.Format(Snippet.For(zoned.PlusNanoseconds(1))));
        }

        [Test]
        public void SubtractDuration()
        {
            // Europe/Dublin transitions from UTC+1 to UTC+0 at 2am (local) on 2017-10-29
            var dt = new LocalDateTime(2017, 10, 29, 1, 45, 0);
            DateTimeZone dublin = DateTimeZoneProviders.Tzdb["Europe/Dublin"];

            ZonedDateTime afterTransition = new ZonedDateTime(dt, dublin, Offset.FromHours(0));

            var result = Snippet.For(ZonedDateTime.Subtract(afterTransition, Duration.FromHours(1)));
            Assert.AreEqual(new LocalDate(2017, 10, 29), result.Date);
            // Adding an hour of elapsed time takes us across the DST transition, so we have
            // the same local time (shown on a clock) but a different offset.
            Assert.AreEqual(new ZonedDateTime(dt, dublin, Offset.FromHours(1)), result);

            // The + operator and Plus instance method are equivalent to the Add static method.
            var result2 = Snippet.For(afterTransition - Duration.FromHours(1));
            var result3 = Snippet.For(afterTransition.Minus(Duration.FromHours(1)));
            Assert.AreEqual(result, result2);
            Assert.AreEqual(result, result3);            
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

            Assert.AreEqual(time.NanosecondOfDay, Snippet.For(nearEndOfDay.NanosecondOfDay));

            Duration duration = nearEndOfDay - startOfDay;
            Assert.AreEqual(Duration.FromHours(25) - Duration.FromSeconds(1), duration);
        }
    }
}