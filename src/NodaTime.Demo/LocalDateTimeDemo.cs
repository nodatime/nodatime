// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Globalization;
using NUnit.Framework;
using NodaTime.Text;

namespace NodaTime.Demo
{
    public class LocalDateTimeDemo
    {
        [Test]
        public void SimpleConstruction()
        {
            CalendarSystem calendar = CalendarSystem.Iso;
            LocalDateTime dt = Snippet.For(new LocalDateTime(2010, 6, 16, 16, 20, calendar));
            Assert.AreEqual(20, dt.Minute);
        }

        [Test]
        public void ImplicitIsoCalendar()
        {
            LocalDateTime dt = Snippet.For(new LocalDateTime(2010, 6, 16, 16, 20));
            Assert.AreEqual("2010-06-16T16:20:00", LocalDateTimePattern.GeneralIso.Format(dt));
            Assert.AreEqual(CalendarSystem.Iso, dt.Calendar);
        }

        [Test]
        public void TestToString()
        {
            LocalDateTime dt = new LocalDateTime(2010, 6, 16, 16, 20);
            Assert.AreEqual("2010-06-16T16:20:00", Snippet.For(dt.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture)));
        }
    }
}