// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.Globalization;
using JetBrains.Annotations;

namespace NodaTime
{
    /// <summary>
    /// A compact representation of a year, month and day in a single 32-bit integer. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// See <see cref="YearMonthDayCalendar"/> for the number of bits per component,
    /// but this doesn't have the calendar component, so bit 0 is part of the day value.
    /// This type is naive: comparisons are performed assuming that a larger month number
    /// always comes after a smaller month number, etc.
    /// This is suitable for most, but not all, calendar systems.
    /// </para>
    /// <para>
    /// The internal representation actually uses 0 for 1 (etc) for each component.
    /// That means the default value is 0001-01-01, which is reasonable for all
    /// supported calendars.
    /// </para>
    /// </remarks>
    internal struct YearMonthDay : IComparable<YearMonthDay>, IEquatable<YearMonthDay>
    {
        private const int DayMask = (1 << YearMonthDayCalendar.DayBits) - 1;
        private const int MonthMask = ((1 << YearMonthDayCalendar.MonthBits) - 1) << YearMonthDayCalendar.DayBits;

        private readonly int value;

        internal YearMonthDay(int rawValue)
        {
            this.value = rawValue;
        }

        /// <summary>
        /// Constructs a new value for the given year, month and day. No validation is performed.
        /// </summary>
        internal YearMonthDay(int year, int month, int day)
        {
            unchecked
            {
                value = ((year - 1) << (YearMonthDayCalendar.DayBits + YearMonthDayCalendar.MonthBits)) |
                        ((month - 1) << YearMonthDayCalendar.DayBits) |
                        (day - 1);
            }
        }

        internal int Year => unchecked((value >> (YearMonthDayCalendar.DayBits + YearMonthDayCalendar.MonthBits)) + 1);
        internal int Month => unchecked(((value & MonthMask) >> YearMonthDayCalendar.DayBits) + 1);
        internal int Day => unchecked((value & DayMask) + 1);

        // Just for testing purposes...
        internal static YearMonthDay Parse(string text)
        {
            // Handle a leading - to negate the year
            if (text.StartsWith("-"))
            {
                var ymd = Parse(text.Substring(1));
                return new YearMonthDay(-ymd.Year, ymd.Month, ymd.Day);
            }

            string[] bits = text.Split('-');
            return new YearMonthDay(
                int.Parse(bits[0], CultureInfo.InvariantCulture),
                int.Parse(bits[1], CultureInfo.InvariantCulture),
                int.Parse(bits[2], CultureInfo.InvariantCulture));
        }

        public override string ToString() =>
            string.Format(CultureInfo.InvariantCulture, "{0:0000}-{1:00}-{2:00}", Year, Month, Day);

        internal YearMonthDayCalendar WithCalendar([CanBeNull] CalendarSystem calendar) =>
            new YearMonthDayCalendar(value, calendar == null ? 0 : calendar.Ordinal);

        internal YearMonthDayCalendar WithCalendarOrdinal(CalendarOrdinal calendarOrdinal) =>
            new YearMonthDayCalendar(value, calendarOrdinal);

        public int CompareTo(YearMonthDay other) => value.CompareTo(other.value);

        public bool Equals(YearMonthDay other)
        {
            return value == other.value;
        }

        public override bool Equals(object other) => other is YearMonthDay && Equals((YearMonthDay) other);

        public override int GetHashCode() => value;

        public static bool operator ==(YearMonthDay lhs, YearMonthDay rhs) => lhs.value == rhs.value;

        public static bool operator !=(YearMonthDay lhs, YearMonthDay rhs) => lhs.value != rhs.value;

        public static bool operator <(YearMonthDay lhs, YearMonthDay rhs) => lhs.value < rhs.value;

        public static bool operator <=(YearMonthDay lhs, YearMonthDay rhs) => lhs.value <= rhs.value;

        public static bool operator >(YearMonthDay lhs, YearMonthDay rhs) => lhs.value > rhs.value;

        public static bool operator >=(YearMonthDay lhs, YearMonthDay rhs) => lhs.value >= rhs.value;
    }
}
