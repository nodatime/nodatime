// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.Globalization;

namespace NodaTime
{
    /// <summary>
    /// A compact representation of a year, month and day in a single 32-bit integer. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// The day is represented in bits 0-5. The month is represented in bits 6-9. The
    /// year is represented in bits 10-24. This type is naive: comparisons are performed
    /// assuming that a larger month number always comes after a smaller month number,
    /// etc. This is suitable for most, but not all, calendar systems.
    /// </para>
    /// <para>
    /// The internal representation actually uses 0 for 1 (etc) for each component.
    /// That means the default value is 0001-01-01, which is reasonable for all
    /// supported calendars.
    /// </para>
    /// </remarks>
    internal struct YearMonthDay : IComparable<YearMonthDay>, IEquatable<YearMonthDay>
    {
        private const int DayBits = 6;   // Up to 64 days in a month.
        private const int MonthBits = 4; // Up to 16 months per year.
        private const int YearBits = 15; // 32K range; only need -10K to +10K.

        private const int DayMask = (1 << DayBits) - 1;
        private const int MonthMask = ((1 << MonthBits) - 1) << DayBits;

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
                value = ((year - 1) << (DayBits + MonthBits)) | ((month - 1) << DayBits) | (day - 1);
            }
        }

        internal int Year { get { return unchecked((value >> (DayBits + MonthBits)) + 1); } }
        internal int Month { get { return unchecked(((value & MonthMask) >> DayBits) + 1); } }
        internal int Day { get { return unchecked((value & DayMask) + 1); } }

        public int RawValue { get { return value; } }

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

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:0000}-{1:00}-{2:00}", Year, Month, Day);
        }

        public int CompareTo(YearMonthDay other)
        {
            return value.CompareTo(other.value);
        }

        public bool Equals(YearMonthDay other)
        {
            return value == other.value;
        }

        public override bool Equals(object other)
        {
            return other is YearMonthDay && Equals((YearMonthDay) other);
        }

        public override int GetHashCode()
        {
            return value;
        }

        public static bool operator ==(YearMonthDay lhs, YearMonthDay rhs)
        {
            return lhs.value == rhs.value;
        }

        public static bool operator !=(YearMonthDay lhs, YearMonthDay rhs)
        {
            return lhs.value != rhs.value;
        }

        public static bool operator <(YearMonthDay lhs, YearMonthDay rhs)
        {
            return lhs.value < rhs.value;
        }

        public static bool operator <=(YearMonthDay lhs, YearMonthDay rhs)
        {
            return lhs.value <= rhs.value;
        }

        public static bool operator >(YearMonthDay lhs, YearMonthDay rhs)
        {
            return lhs.value > rhs.value;
        }

        public static bool operator >=(YearMonthDay lhs, YearMonthDay rhs)
        {
            return lhs.value >= rhs.value;
        }
    }
}
