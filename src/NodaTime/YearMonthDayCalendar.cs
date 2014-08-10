// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.Globalization;
using System.Linq.Expressions;
using NodaTime.Annotations;
using NodaTime.Utility;

namespace NodaTime
{
    /// <summary>
    /// A compact representation of a year, month and day and calendar ordinal (integer ID) in a single 32-bit integer. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// The calendar is represented in bits 0-6.
    /// The day is represented in bits 7-12.
    /// The month is represented in bits 13-16.
    /// The year is represented in bits 17-31. (It's convenient to put this at the top as it can be negative.)
    /// 
    /// This type does not implement IComparable[YearMonthDayCalendar] as it turns out it doesn't need to:
    /// comparisons are always done through the calendar system, which uses YearMonthDay instead. We could potentially
    /// optimize by bypassing the calendar and embedding knowledge of calendars which have "odd" month numberings
    /// in here, but it would be a bit of a design smell.
    /// 
    /// Equality is easily tested, however, as it can check for calendar equality.
    /// </para>
    /// <para>
    /// The internal representation actually uses 0 for 1 (etc) for each component.
    /// That means the default value is 0001-01-01, which is reasonable for all
    /// supported calendars.
    /// </para>
    /// </remarks>
    internal struct YearMonthDayCalendar : IEquatable<YearMonthDayCalendar>
    {
        private const int CalendarBits = 7; // Up to 128 calendars.
        private const int DayBits = 6;   // Up to 64 days in a month.
        private const int MonthBits = 4; // Up to 16 months per year.
        private const int YearBits = 15; // 32K range; only need -10K to +10K.

        // Just handy constants to use for shifting and masking.
        private const int CalendarDayBits = CalendarBits + DayBits;
        private const int CalendarDayMonthBits = CalendarDayBits + MonthBits;

        private const int CalendarMask = (1 << CalendarBits) - 1;
        private const int DayMask = ((1 << DayBits) - 1) << CalendarBits;
        private const int MonthMask = ((1 << MonthBits) - 1) << CalendarDayBits;
        private const int YearMask = ((1 << YearBits) - 1) << CalendarDayMonthBits;

        private readonly int value;

        internal YearMonthDayCalendar(int rawValue)
        {
            this.value = rawValue;
        }

        internal YearMonthDayCalendar(int yearMonthDay, CalendarOrdinal calendarOrdinal)
        {
            this.value = (yearMonthDay << CalendarBits) | (int) calendarOrdinal;
        }

        /// <summary>
        /// Constructs a new value for the given year, month, day and calendar. No validation is performed.
        /// </summary>
        internal YearMonthDayCalendar(int year, int month, int day, CalendarOrdinal calendarOrdinal)
        {
            unchecked
            {
                value = ((year - 1) << CalendarDayMonthBits) |
                        ((month - 1) << CalendarDayBits) |
                        ((day - 1) << CalendarBits) |
                        (int) calendarOrdinal;
            }
        }

        internal CalendarOrdinal CalendarOrdinal { get { return (CalendarOrdinal) unchecked(value & CalendarMask); } }
        internal int Year { get { return unchecked(((value & YearMask) >> CalendarDayMonthBits) + 1); } }
        internal int Month { get { return unchecked(((value & MonthMask) >> CalendarDayBits) + 1); } }
        internal int Day { get { return unchecked(((value & DayMask) >> CalendarBits) + 1); } }

        public int RawValue { get { return value; } }

        // Just for testing purposes...
        [VisibleForTesting]
        internal static YearMonthDayCalendar Parse(string text)
        {
            // Handle a leading - to negate the year
            if (text.StartsWith("-"))
            {
                var ymdc = Parse(text.Substring(1));
                return new YearMonthDayCalendar(-ymdc.Year, ymdc.Month, ymdc.Day, ymdc.CalendarOrdinal);
            }

            string[] bits = text.Split('-');
            return new YearMonthDayCalendar(
                int.Parse(bits[0], CultureInfo.InvariantCulture),
                int.Parse(bits[1], CultureInfo.InvariantCulture),
                int.Parse(bits[2], CultureInfo.InvariantCulture),
                (CalendarOrdinal) Enum.Parse(typeof(CalendarOrdinal), bits[3]));
        }

        internal YearMonthDay ToYearMonthDay()
        {
            return new YearMonthDay(value >> CalendarBits);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:0000}-{1:00}-{2:00}-{3}", Year, Month, Day, CalendarOrdinal);
        }

        public static bool operator ==(YearMonthDayCalendar lhs, YearMonthDayCalendar rhs)
        {
            return lhs.value == rhs.value;
        }

        public static bool operator !=(YearMonthDayCalendar lhs, YearMonthDayCalendar rhs)
        {
            return lhs.value != rhs.value;
        }

        public bool Equals(YearMonthDayCalendar other)
        {
            return value == other.value;
        }

        public override bool Equals(object other)
        {
            return other is YearMonthDayCalendar && Equals((YearMonthDayCalendar) other);
        }

        public override int GetHashCode()
        {
            return value;
        }
    }
}
