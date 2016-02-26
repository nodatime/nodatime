// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;

namespace NodaTime.Test.TimeZones
{
    /// <summary>
    /// As of 2002, Jordan switches to DST at the *end* of the last Thursday of March.
    /// This is denoted in the zoneinfo database using lastThu 24:00, which was invalid
    /// in our parser.
    /// </summary>
    public class JordanTest
    {
        private static readonly DateTimeZone Jordan = DateTimeZoneProviders.Tzdb["Asia/Amman"];

        /// <summary>
        /// If all of these transitions are right, we're probably okay... in particular,
        /// checking the 2005 transition occurs on the 1st of April is important.
        /// </summary>
        [Test]
        public void Transitions2000To2010()
        {
            // These were fetched with Joda Time 1.6.2, which definitely uses the new rules.
            var expectedDates = new[]
            {
                new LocalDate(2000, 3, 30), // Thursday morning
                new LocalDate(2001, 3, 29), // Thursday morning
                new LocalDate(2002, 3, 29), // Friday morning from here onwards
                new LocalDate(2003, 3, 28),
                new LocalDate(2004, 3, 26),
                new LocalDate(2005, 4, 1),
                new LocalDate(2006, 3, 31),
                new LocalDate(2007, 3, 30),
                new LocalDate(2008, 3, 28),
                new LocalDate(2009, 3, 27),
                new LocalDate(2010, 3, 26)
            };

            for (int year = 2000; year <= 2010; year++)
            {
                LocalDate summer = new LocalDate(year, 6, 1);
                var intervalPair = Jordan.MapLocal(summer.AtMidnight());
                Assert.AreEqual(1, intervalPair.Count);
                Assert.AreEqual(expectedDates[year - 2000], intervalPair.EarlyInterval.IsoLocalStart.Date);
            }
        }
    }
}