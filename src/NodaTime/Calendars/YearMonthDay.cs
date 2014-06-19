// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Calendars
{
    /// <summary>
    /// Just a useful struct to be hand a whole year/month/day value in one go.
    /// This type is a dumb calendar-neutral type; it's just a composition of
    /// three integers.
    /// </summary>
    internal struct YearMonthDay
    {
        private readonly int year;
        internal int Year { get { return year; } }

        private readonly int month;
        internal int Month { get { return month; } }

        private readonly int day;
        internal int Day { get { return day; } }

        internal YearMonthDay(int year, int month, int day)
        {
            this.year = year;
            this.month = month;
            this.day = day;
        }
    }
}