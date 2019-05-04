// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;

namespace NodaTime.Demo
{    
    public class OffsetDateTimeDemo
    {
        [Test]
        public void Construction()
        {
            LocalDateTime localDateTime = new LocalDateTime(1985, 10, 26, 1, 18);
            Offset offset = Offset.FromHours(-5);
            OffsetDateTime offsetDateTime = Snippet.For(new OffsetDateTime(localDateTime, offset));

            Assert.AreEqual(localDateTime, offsetDateTime.LocalDateTime);
            Assert.AreEqual(offset, offsetDateTime.Offset);
        }

        [Test]
        public void WithCalendar()
        {
            LocalDateTime localDateTime = new LocalDateTime(1985, 10, 26, 1, 18, CalendarSystem.Iso);
            Offset offset = Offset.FromHours(-5);
            OffsetDateTime original = new OffsetDateTime(localDateTime, offset);
            OffsetDateTime updated = Snippet.For(original.WithCalendar(CalendarSystem.Julian));

            Assert.AreEqual(
                new LocalDateTime(1985, 10, 13, 1, 18, CalendarSystem.Julian).ToString("r", null),
                updated.LocalDateTime.ToString("r", null));

            Assert.AreEqual(original.Offset, updated.Offset);
        }

        [Test]
        public void WithOffset()
        {
            LocalDateTime localDateTime = new LocalDateTime(1985, 10, 26, 1, 18);
            Offset offset = Offset.FromHours(-3);
            OffsetDateTime original = new OffsetDateTime(localDateTime, offset);
            OffsetDateTime updated = Snippet.For(original.WithOffset(Offset.FromHours(-2)));

            Assert.AreEqual(new LocalDateTime(1985, 10, 26, 2, 18), updated.LocalDateTime);
            Assert.AreEqual(Offset.FromHours(-2), updated.Offset);
        }

        [Test]
        public void WithDateAdjuster()
        {
            LocalDateTime localDateTime = new LocalDateTime(1985, 10, 26, 1, 18);
            Offset offset = Offset.FromHours(-5);
            OffsetDateTime original = new OffsetDateTime(localDateTime, offset);
            var dateAdjuster = DateAdjusters.AddPeriod(Period.FromYears(30));
            OffsetDateTime updated = Snippet.For(original.With(dateAdjuster));

            Assert.AreEqual(
                new LocalDateTime(2015, 10, 26, 1, 18), 
                updated.LocalDateTime);
            Assert.AreEqual(original.Offset, updated.Offset);
        }

        [Test]
        public void WithTimeAdjuster()
        {
            LocalDateTime localDateTime = new LocalDateTime(1985, 10, 26, 1, 18);
            Offset offset = Offset.FromHours(-5);
            OffsetDateTime original = new OffsetDateTime(localDateTime, offset);
            OffsetDateTime updated = Snippet.For(original.With(TimeAdjusters.TruncateToHour));

            Assert.AreEqual(
                new LocalDateTime(1985, 10, 26, 1, 0),
                updated.LocalDateTime);
            Assert.AreEqual(original.Offset, updated.Offset);
        }
    }
}
