// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using JetBrains.Annotations;
using NodaTime.Annotations;
using NodaTime.Calendars;
using NodaTime.Fields;
using NodaTime.Text;
using NodaTime.TimeZones;
using NodaTime.Utility;

namespace NodaTime
{
    /// <summary>
    /// A date and time in a particular calendar system. A LocalDateTime value does not represent an
    /// instant on the global time line, because it has no associated time zone: "November 12th 2009 7pm, ISO calendar"
    /// occurred at different instants for different people around the world.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This type defaults to using the ISO calendar system unless a different calendar system is
    /// specified.
    /// </para>
    /// <para>
    /// Values can freely be compared for equality: a value in a different calendar system is not equal to
    /// a value in a different calendar system. However, ordering comparisons (either via the <see cref="CompareTo"/> method
    /// or via operators) fail with <see cref="ArgumentException"/>; attempting to compare values in different calendars
    /// almost always indicates a bug in the calling code.
    /// TODO(2.0): Calendar-neutral comparer.
    /// </para>
    /// </remarks>
    /// <threadsafety>This type is an immutable value type. See the thread safety section of the user guide for more information.</threadsafety>
#if !PCL
    [Serializable]
#endif
    public struct LocalDateTime : IEquatable<LocalDateTime>, IComparable<LocalDateTime>, IComparable, IFormattable, IXmlSerializable
#if !PCL
        , ISerializable
#endif
    {
        [ReadWriteForEfficiency] private LocalDate date;
        [ReadWriteForEfficiency] private LocalTime time;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct using the ISO
        /// calendar system.
        /// </summary>
        /// <param name="localInstant">The local instant.</param>
        /// <returns>The resulting date/time.</returns>
        internal LocalDateTime([Trusted] LocalInstant localInstant)
        {
            date = new LocalDate(localInstant.DaysSinceEpoch);
            time = new LocalTime(localInstant.NanosecondOfDay);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct using the ISO calendar system.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year",
        /// so a value of 0 means 1 BC, for example.</param>
        /// <param name="month">The month of year.</param>
        /// <param name="day">The day of month.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <returns>The resulting date/time.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid date/time.</exception>
        public LocalDateTime(int year, int month, int day, int hour, int minute)
            : this(new LocalDate(year, month, day),
                   new LocalTime(hour, minute))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year", so, for
        /// the ISO calendar, a value of 0 means 1 BC, for example.</param>
        /// <param name="month">The month of year.</param>
        /// <param name="day">The day of month.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="calendar">The calendar.</param>
        /// <returns>The resulting date/time.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid date/time.</exception>
        public LocalDateTime(int year, int month, int day, int hour, int minute, [NotNull] CalendarSystem calendar)
            : this(new LocalDate(year, month, day, calendar),
                   new LocalTime(hour, minute))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct using the ISO calendar system.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year",
        /// so a value of 0 means 1 BC, for example.</param>
        /// <param name="month">The month of year.</param>
        /// <param name="day">The day of month.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <returns>The resulting date/time.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid date/time.</exception>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second)
            : this(new LocalDate(year, month, day),
                   new LocalTime(hour, minute, second))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year", so, for
        /// the ISO calendar, a value of 0 means 1 BC, for example.</param>
        /// <param name="month">The month of year.</param>
        /// <param name="day">The day of month.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="calendar">The calendar.</param>
        /// <returns>The resulting date/time.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid date/time.</exception>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, [NotNull] CalendarSystem calendar)
            : this(new LocalDate(year, month, day, calendar),
                   new LocalTime(hour, minute, second))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct using the ISO calendar system.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year",
        /// so a value of 0 means 1 BC, for example.</param>
        /// <param name="month">The month of year.</param>
        /// <param name="day">The day of month.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="millisecond">The millisecond.</param>
        /// <returns>The resulting date/time.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid date/time.</exception>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond)
            : this(new LocalDate(year, month, day),
                   new LocalTime(hour, minute, second, millisecond))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year", so, for
        /// the ISO calendar, a value of 0 means 1 BC, for example.</param>
        /// <param name="month">The month of year.</param>
        /// <param name="day">The day of month.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="millisecond">The millisecond.</param>
        /// <param name="calendar">The calendar.</param>
        /// <returns>The resulting date/time.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid date/time.</exception>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, [NotNull] CalendarSystem calendar)
            : this(year, month, day, hour, minute, second, millisecond, 0, calendar)
        {
        }

        // TODO(2.0): Remove this constructor? It's a pretty odd one at this point.
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year",
        /// so a value of 0 means 1 BC, for example.</param>
        /// <param name="month">The month of year.</param>
        /// <param name="day">The day of month.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="millisecond">The millisecond.</param>
        /// <param name="tickWithinMillisecond">The tick within millisecond.</param>
        /// <returns>The resulting date/time.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid date/time.</exception>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int tickWithinMillisecond)
            : this(new LocalDate(year, month, day),
                   new LocalTime(hour, minute, second, millisecond, tickWithinMillisecond))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year", so, for
        /// the ISO calendar, a value of 0 means 1 BC, for example.</param>
        /// <param name="month">The month of year.</param>
        /// <param name="day">The day of month.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="millisecond">The millisecond.</param>
        /// <param name="tickWithinMillisecond">The tick within millisecond.</param>
        /// <param name="calendar">The calendar.</param>
        /// <returns>The resulting date/time.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid date/time.</exception>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int tickWithinMillisecond, [NotNull] CalendarSystem calendar)
            : this(new LocalDate(year, month, day, calendar),
                   new LocalTime(hour, minute, second, millisecond, tickWithinMillisecond))
        {
        }

        internal LocalDateTime(LocalDate date, LocalTime time)
        {
            this.date = date;
            this.time = time;
        }

        /// <summary>Gets the calendar system associated with this local date and time.</summary>
        /// <value>The calendar system associated with this local date and time.</value>
        public CalendarSystem Calendar => date.Calendar;

        /// <summary>Gets the year of this local date and time.</summary>
        /// <remarks>This returns the "absolute year", so, for the ISO calendar,
        /// a value of 0 means 1 BC, for example.</remarks>
        /// <value>The year of this local date and time.</value>
        public int Year => date.Year;

        /// <summary>Gets the year of this local date and time within its era.</summary>
        /// <value>The year of this local date and time within its era.</value>
        public int YearOfEra => date.YearOfEra;

        /// <summary>Gets the era of this local date and time.</summary>
        /// <value>The era of this local date and time.</value>
        public Era Era => date.Era;

        /// <summary>
        /// Gets the "week year" of this local date and time.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The WeekYear is the year that matches with the <see cref="WeekOfWeekYear"/> field.
        /// In the standard ISO8601 week algorithm, the first week of the year
        /// is that in which at least 4 days are in the year. As a result of this
        /// definition, day 1 of the first week may be in the previous year.
        /// The WeekYear allows you to query the effective year for that day.
        /// </para>
        /// <para>
        /// For example, January 1st 2011 was a Saturday, so only two days of that week
        /// (Saturday and Sunday) were in 2011. Therefore January 1st is part of
        /// week 52 of WeekYear 2010. Conversely, December 31st 2012 is a Monday,
        /// so is part of week 1 of WeekYear 2013.
        /// </para>
        /// </remarks>
        /// <value>The "week year" of this local date and time.</value>
        public int WeekYear => date.WeekYear;

        /// <summary>
        /// Gets the month of this local date and time within the year.
        /// </summary>
        /// <value>The month of this local date and time within the year.</value>
        public int Month => date.Month;

        /// <summary>
        /// Gets the week within the week-year. See <see cref="WeekYear"/> for more details.
        /// </summary>
        /// <value>The week within the week-year. See <see cref="WeekYear"/> for more details.</value>
        public int WeekOfWeekYear => date.WeekOfWeekYear;

        /// <summary>
        /// Gets the day of this local date and time within the year.
        /// </summary>
        /// <value>The day of this local date and time within the year.</value>
        public int DayOfYear => date.DayOfYear;

        /// <summary>
        /// Gets the day of this local date and time within the month.
        /// </summary>
        /// <value>The day of this local date and time within the month.</value>
        public int Day => date.Day;

        /// <summary>
        /// Gets the week day of this local date and time expressed as an <see cref="NodaTime.IsoDayOfWeek"/> value,
        /// for calendars which use ISO days of the week.
        /// </summary>
        /// <exception cref="InvalidOperationException">The underlying calendar doesn't use ISO days of the week.</exception>
        /// <seealso cref="DayOfWeek"/>
        /// <value>The week day of this local date and time expressed as an <c>IsoDayOfWeek</c>.</value>
        public IsoDayOfWeek IsoDayOfWeek => date.IsoDayOfWeek;

        /// <summary>
        /// Gets the week day of this local date and time as a number.
        /// </summary>
        /// <remarks>
        /// For calendars using ISO week days, this gives 1 for Monday to 7 for Sunday.
        /// </remarks>
        /// <seealso cref="IsoDayOfWeek"/>
        /// <value>The week day of this local date and time as a number.</value>
        public int DayOfWeek => date.DayOfWeek;

        /// <summary>
        /// Gets the hour of day of this local date and time, in the range 0 to 23 inclusive.
        /// </summary>
        /// <value>The hour of day of this local date and time, in the range 0 to 23 inclusive.</value>
        public int Hour => time.Hour;

        /// <summary>
        /// Gets the hour of the half-day of this local date and time, in the range 1 to 12 inclusive.
        /// </summary>
        /// <value>The hour of the half-day of this local date and time, in the range 1 to 12 inclusive.</value>
        public int ClockHourOfHalfDay => time.ClockHourOfHalfDay;

        /// <summary>
        /// Gets the minute of this local date and time, in the range 0 to 59 inclusive.
        /// </summary>
        /// <value>The minute of this local date and time, in the range 0 to 59 inclusive.</value>
        public int Minute => time.Minute;

        /// <summary>
        /// Gets the second of this local date and time within the minute, in the range 0 to 59 inclusive.
        /// </summary>
        /// <value>The second of this local date and time within the minute, in the range 0 to 59 inclusive.</value>
        public int Second => time.Second;

        /// <summary>
        /// Gets the millisecond of this local date and time within the second, in the range 0 to 999 inclusive.
        /// </summary>
        /// <value>The millisecond of this local date and time within the second, in the range 0 to 999 inclusive.</value>
        public int Millisecond => time.Millisecond;

        /// <summary>
        /// Gets the tick of this local time within the second, in the range 0 to 9,999,999 inclusive.
        /// </summary>
        /// <value>The tick of this local time within the second, in the range 0 to 9,999,999 inclusive.</value>
        public int TickOfSecond => time.TickOfSecond;

        /// <summary>
        /// Gets the tick of this local date and time within the day, in the range 0 to 863,999,999,999 inclusive.
        /// </summary>
        /// <value>The tick of this local date and time within the day, in the range 0 to 863,999,999,999 inclusive.</value>
        public long TickOfDay => time.TickOfDay;

        /// <summary>
        /// Gets the nanosecond of this local time within the second, in the range 0 to 999,999,999 inclusive.
        /// </summary>
        /// <value>The nanosecond of this local time within the second, in the range 0 to 999,999,999 inclusive.</value>
        public int NanosecondOfSecond => time.NanosecondOfSecond;

        /// <summary>
        /// Gets the nanosecond of this local date and time within the day, in the range 0 to 86,399,999,999,999 inclusive.
        /// </summary>
        /// <value>The nanosecond of this local date and time within the day, in the range 0 to 86,399,999,999,999 inclusive.</value>
        public long NanosecondOfDay => time.NanosecondOfDay;

        /// <summary>
        /// Gets the time portion of this local date and time as a <see cref="LocalTime"/>.
        /// </summary>
        /// <value>The time portion of this local date and time as a <c>LocalTime</c>.</value>
        public LocalTime TimeOfDay => time;

        /// <summary>
        /// Gets the date portion of this local date and time as a <see cref="LocalDate"/> in the same calendar system as this value.
        /// </summary>
        /// <value>The date portion of this local date and time as a <c>LocalDate</c> in the same calendar system as this value.</value>
        public LocalDate Date => date;

        /// <summary>
        /// Constructs a <see cref="DateTime"/> from this value which has a <see cref="DateTime.Kind" />
        /// of <see cref="DateTimeKind.Unspecified"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="DateTimeKind.Unspecified"/> is slightly odd - it can be treated as UTC if you use <see cref="DateTime.ToLocalTime"/>
        /// or as system local time if you use <see cref="DateTime.ToUniversalTime"/>, but it's the only kind which allows
        /// you to construct a <see cref="DateTimeOffset"/> with an arbitrary offset, which makes it as close to
        /// the Noda Time non-system-specific "local" concept as exists in .NET.
        /// </remarks>
        /// <returns>A <see cref="DateTime"/> value for the same date and time as this value.</returns>
        [Pure]
        public DateTime ToDateTimeUnspecified() =>
            new DateTime(
                TickArithmetic.DaysAndTickOfDayToTicks(date.DaysSinceEpoch, time.TickOfDay) + NodaConstants.BclTicksAtUnixEpoch,
                DateTimeKind.Unspecified);
            

        [Pure]
        internal LocalInstant ToLocalInstant() => new LocalInstant(date.DaysSinceEpoch, time.NanosecondOfDay);

        /// <summary>
        /// Converts a <see cref="DateTime" /> of any kind to a LocalDateTime in the ISO calendar. This does not perform
        /// any time zone conversions, so a DateTime with a <see cref="DateTime.Kind"/> of <see cref="DateTimeKind.Utc"/>
        /// will still have the same day/hour/minute etc - it won't be converted into the local system time.
        /// </summary>
        /// <param name="dateTime">Value to convert into a Noda Time local date and time</param>
        /// <returns>A new <see cref="LocalDateTime"/> with the same values as the specified <c>DateTime</c>.</returns>
        public static LocalDateTime FromDateTime(DateTime dateTime)
        {
            long ticks = dateTime.Ticks - NodaConstants.BclTicksAtUnixEpoch;
            long tickOfDay;
            int days = TickArithmetic.TicksToDaysAndTickOfDay(ticks, out tickOfDay);
            return new LocalDateTime(new LocalDate(days), new LocalTime(unchecked(tickOfDay * NodaConstants.NanosecondsPerTick)));
        }

        /// <summary>
        /// Converts a <see cref="DateTime" /> of any kind to a LocalDateTime in the specified calendar. This does not perform
        /// any time zone conversions, so a DateTime with a <see cref="DateTime.Kind"/> of <see cref="DateTimeKind.Utc"/>
        /// will still have the same day/hour/minute etc - it won't be converted into the local system time.
        /// </summary>
        /// <param name="dateTime">Value to convert into a Noda Time local date and time</param>
        /// <param name="calendar">The calendar system to convert into</param>
        /// <returns>A new <see cref="LocalDateTime"/> with the same values as the specified <c>DateTime</c>.</returns>
        public static LocalDateTime FromDateTime(DateTime dateTime, [NotNull] CalendarSystem calendar)
        {
            long ticks = dateTime.Ticks - NodaConstants.BclTicksAtUnixEpoch;
            long tickOfDay;
            int days = TickArithmetic.TicksToDaysAndTickOfDay(ticks, out tickOfDay);
            return new LocalDateTime(new LocalDate(days, calendar), new LocalTime(unchecked(tickOfDay * NodaConstants.NanosecondsPerTick)));
        }

        #region Implementation of IEquatable<LocalDateTime>
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(LocalDateTime other) => date == other.date && time == other.time;
        #endregion

        #region Operators
        /// <summary>
        /// Implements the operator == (equality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator ==(LocalDateTime left, LocalDateTime right) => left.Equals(right);

        /// <summary>
        /// Implements the operator != (inequality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are not equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator !=(LocalDateTime left, LocalDateTime right) => !(left == right);

        /// <summary>
        /// Compares two LocalDateTime values to see if the left one is strictly earlier than the right
        /// one.
        /// </summary>
        /// <remarks>
        /// Only values with the same calendar system can be compared. See the top-level type
        /// documentation for more information about comparisons.
        /// </remarks>
        /// <param name="lhs">First operand of the comparison</param>
        /// <param name="rhs">Second operand of the comparison</param>
        /// <exception cref="ArgumentException">The calendar system of <paramref name="rhs"/> is not the same
        /// as the calendar of <paramref name="lhs"/>.</exception>
        /// <returns>true if the <paramref name="lhs"/> is strictly earlier than <paramref name="rhs"/>, false otherwise.</returns>
        public static bool operator <(LocalDateTime lhs, LocalDateTime rhs)
        {
            Preconditions.CheckArgument(lhs.Calendar.Equals(rhs.Calendar), nameof(rhs), "Only values in the same calendar can be compared");
            return lhs.CompareTo(rhs) < 0;
        }

        /// <summary>
        /// Compares two LocalDateTime values to see if the left one is earlier than or equal to the right
        /// one.
        /// </summary>
        /// <remarks>
        /// Only values with the same calendar system can be compared. See the top-level type
        /// documentation for more information about comparisons.
        /// </remarks>
        /// <param name="lhs">First operand of the comparison</param>
        /// <param name="rhs">Second operand of the comparison</param>
        /// <exception cref="ArgumentException">The calendar system of <paramref name="rhs"/> is not the same
        /// as the calendar of <paramref name="lhs"/>.</exception>
        /// <returns>true if the <paramref name="lhs"/> is earlier than or equal to <paramref name="rhs"/>, false otherwise.</returns>
        public static bool operator <=(LocalDateTime lhs, LocalDateTime rhs)
        {
            Preconditions.CheckArgument(lhs.Calendar.Equals(rhs.Calendar), nameof(rhs), "Only values in the same calendar can be compared");
            return lhs.CompareTo(rhs) <= 0;
        }

        /// <summary>
        /// Compares two LocalDateTime values to see if the left one is strictly later than the right
        /// one.
        /// </summary>
        /// <remarks>
        /// Only values with the same calendar system can be compared. See the top-level type
        /// documentation for more information about comparisons.
        /// </remarks>
        /// <param name="lhs">First operand of the comparison</param>
        /// <param name="rhs">Second operand of the comparison</param>
        /// <exception cref="ArgumentException">The calendar system of <paramref name="rhs"/> is not the same
        /// as the calendar of <paramref name="lhs"/>.</exception>
        /// <returns>true if the <paramref name="lhs"/> is strictly later than <paramref name="rhs"/>, false otherwise.</returns>
        public static bool operator >(LocalDateTime lhs, LocalDateTime rhs)
        {
            Preconditions.CheckArgument(lhs.Calendar.Equals(rhs.Calendar), nameof(rhs), "Only values in the same calendar can be compared");
            return lhs.CompareTo(rhs) > 0;
        }

        /// <summary>
        /// Compares two LocalDateTime values to see if the left one is later than or equal to the right
        /// one.
        /// </summary>
        /// <remarks>
        /// Only values with the same calendar system can be compared. See the top-level type
        /// documentation for more information about comparisons.
        /// </remarks>
        /// <param name="lhs">First operand of the comparison</param>
        /// <param name="rhs">Second operand of the comparison</param>
        /// <exception cref="ArgumentException">The calendar system of <paramref name="rhs"/> is not the same
        /// as the calendar of <paramref name="lhs"/>.</exception>
        /// <returns>true if the <paramref name="lhs"/> is later than or equal to <paramref name="rhs"/>, false otherwise.</returns>
        public static bool operator >=(LocalDateTime lhs, LocalDateTime rhs)
        {
            Preconditions.CheckArgument(lhs.Calendar.Equals(rhs.Calendar), nameof(rhs), "Only values in the same calendar can be compared");
            return lhs.CompareTo(rhs) >= 0;
        }

        /// <summary>
        /// Indicates whether this date/time is earlier, later or the same as another one.
        /// </summary>
        /// <remarks>
        /// Only date/time values within the same calendar systems can be compared with this method. Attempting to compare
        /// values within different calendars will fail with an <see cref="ArgumentException"/>. Ideally, comparisons
        /// is almost always preferable to continuing.
        /// </remarks>
        /// <param name="other">The other local date/time to compare with this value.</param>
        /// <exception cref="ArgumentException">The calendar system of <paramref name="other"/> is not the
        /// same as the calendar system of this value.</exception>
        /// <returns>A value less than zero if this date/time is earlier than <paramref name="other"/>;
        /// zero if this date/time is the same as <paramref name="other"/>; a value greater than zero if this date/time is
        /// later than <paramref name="other"/>.</returns>
        public int CompareTo(LocalDateTime other)
        {
            // This will check calendars...
            int dateComparison = date.CompareTo(other.date);
            if (dateComparison != 0)
            {
                return dateComparison;
            }
            return time.CompareTo(other.time);
        }

        /// <summary>
        /// Implementation of <see cref="IComparable.CompareTo"/> to compare two LocalDateTimes.
        /// </summary>
        /// <remarks>
        /// This uses explicit interface implementation to avoid it being called accidentally. The generic implementation should usually be preferred.
        /// </remarks>
        /// <exception cref="ArgumentException"><paramref name="obj"/> is non-null but does not refer to an instance of <see cref="LocalDateTime"/>,
        /// or refers to a adate/time in a different calendar system.</exception>
        /// <param name="obj">The object to compare this value with.</param>
        /// <returns>The result of comparing this LocalDateTime with another one; see <see cref="CompareTo(NodaTime.LocalDateTime)"/> for general details.
        /// If <paramref name="obj"/> is null, this method returns a value greater than 0.
        /// </returns>
        int IComparable.CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            Preconditions.CheckArgument(obj is LocalDateTime, nameof(obj), "Object must be of type NodaTime.LocalDateTime.");
            return CompareTo((LocalDateTime)obj);
        }

        /// <summary>
        /// Adds a period to a local date/time. Fields are added in the order provided by the period.
        /// This is a convenience operator over the <see cref="Plus"/> method.
        /// </summary>
        /// <param name="localDateTime">Initial local date and time</param>
        /// <param name="period">Period to add</param>
        /// <returns>The resulting local date and time</returns>
        public static LocalDateTime operator +(LocalDateTime localDateTime, [NotNull] Period period) => localDateTime.Plus(period);

        /// <summary>
        /// Add the specified period to the date and time. Friendly alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="localDateTime">Initial local date and time</param>
        /// <param name="period">Period to add</param>
        /// <returns>The resulting local date and time</returns>
        public static LocalDateTime Add(LocalDateTime localDateTime, [NotNull] Period period) => localDateTime.Plus(period);

        /// <summary>
        /// Adds a period to this local date/time. Fields are added in the order provided by the period.
        /// </summary>
        /// <param name="period">Period to add</param>
        /// <returns>The resulting local date and time</returns>
        [Pure]
        public LocalDateTime Plus([NotNull] Period period)
        {
            Preconditions.CheckNotNull(period, nameof(period));
            return period.AddTo(date, time, 1);
        }

        /// <summary>
        /// Subtracts a period from a local date/time. Fields are subtracted in the order provided by the period.
        /// This is a convenience operator over the <see cref="Minus(Period)"/> method.
        /// </summary>
        /// <param name="localDateTime">Initial local date and time</param>
        /// <param name="period">Period to subtract</param>
        /// <returns>The resulting local date and time</returns>
        public static LocalDateTime operator -(LocalDateTime localDateTime, [NotNull] Period period) => localDateTime.Minus(period);

        /// <summary>
        /// Subtracts the specified period from the date and time. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="localDateTime">Initial local date and time</param>
        /// <param name="period">Period to subtract</param>
        /// <returns>The resulting local date and time</returns>
        public static LocalDateTime Subtract(LocalDateTime localDateTime, [NotNull] Period period) => localDateTime.Minus(period);

        /// <summary>
        /// Subtracts a period from a local date/time. Fields are subtracted in the order provided by the period.
        /// </summary>
        /// <param name="period">Period to subtract</param>
        /// <returns>The resulting local date and time</returns>
        [Pure]
        public LocalDateTime Minus([NotNull] Period period)
        {
            Preconditions.CheckNotNull(period, nameof(period));
            return period.AddTo(date, time, -1);
        }

        /// <summary>
        /// Subtracts one date/time from another, returning the result as a <see cref="Period"/>.
        /// </summary>
        /// <remarks>
        /// This is simply a convenience operator for calling <see cref="Period.Between(NodaTime.LocalDateTime,NodaTime.LocalDateTime)"/>.
        /// The calendar systems of the two date/times must be the same.
        /// </remarks>
        /// <param name="lhs">The date/time to subtract from</param>
        /// <param name="rhs">The date/time to subtract</param>
        /// <returns>The result of subtracting one date/time from another.</returns>
        public static Period operator -(LocalDateTime lhs, LocalDateTime rhs) => Period.Between(rhs, lhs);

        /// <summary>
        /// Subtracts one date/time from another, returning the result as a <see cref="Period"/>.
        /// </summary>
        /// <remarks>
        /// This is simply a convenience method for calling <see cref="Period.Between(NodaTime.LocalDateTime,NodaTime.LocalDateTime)"/>.
        /// The calendar systems of the two date/times must be the same.
        /// </remarks>
        /// <param name="lhs">The date/time to subtract from</param>
        /// <param name="rhs">The date/time to subtract</param>
        /// <returns>The result of subtracting one date/time from another.</returns>
        public static Period Subtract(LocalDateTime lhs, LocalDateTime rhs) => lhs - rhs;

        /// <summary>
        /// Subtracts the specified date/time from this date/time, returning the result as a <see cref="Period"/>.
        /// Fluent alternative to <c>operator-()</c>.
        /// </summary>
        /// <remarks>The specified date/time must be in the same calendar system as this.</remarks>
        /// <param name="localDateTime">The date/time to subtract from this</param>
        /// <returns>The difference between the specified date/time and this one</returns>
        [Pure]
        public Period Minus(LocalDateTime localDateTime) => this - localDateTime;
        #endregion

        #region object overrides
        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => obj is LocalDateTime && Equals((LocalDateTime)obj);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </returns>
        public override int GetHashCode() => HashCodeHelper.Hash(date, time, Calendar);
        #endregion
        /// <summary>
        /// Returns this date/time, with the given date adjuster applied to it, maintaing the existing time of day.
        /// </summary>
        /// <remarks>
        /// If the adjuster attempts to construct an
        /// invalid date (such as by trying to set a day-of-month of 30 in February), any exception thrown by
        /// that construction attempt will be propagated through this method.
        /// </remarks>
        /// <param name="adjuster">The adjuster to apply.</param>
        /// <returns>The adjusted date/time.</returns>
        [Pure]
        public LocalDateTime With([NotNull] Func<LocalDate, LocalDate> adjuster) =>
            date.With(adjuster) + time;

        /// <summary>
        /// Returns this date/time, with the given time adjuster applied to it, maintaining the existing date.
        /// </summary>
        /// <remarks>
        /// If the adjuster attempts to construct an invalid time, any exception thrown by
        /// that construction attempt will be propagated through this method.
        /// </remarks>
        /// <param name="adjuster">The adjuster to apply.</param>
        /// <returns>The adjusted date/time.</returns>
        [Pure]
        public LocalDateTime With([NotNull] Func<LocalTime, LocalTime> adjuster) =>
            date + time.With(adjuster);

        /// <summary>
        /// Creates a new LocalDateTime representing the same physical date and time, but in a different calendar.
        /// The returned LocalDateTime is likely to have different date field values to this one.
        /// For example, January 1st 1970 in the Gregorian calendar was December 19th 1969 in the Julian calendar.
        /// </summary>
        /// <param name="calendarSystem">The calendar system to convert this local date to.</param>
        /// <returns>The converted LocalDateTime.</returns>
        [Pure]
        public LocalDateTime WithCalendar([NotNull] CalendarSystem calendarSystem)
        {
            Preconditions.CheckNotNull(calendarSystem, nameof(calendarSystem));
            return new LocalDateTime(date.WithCalendar(calendarSystem), time);
        }

        /// <summary>
        /// Returns a new LocalDateTime representing the current value with the given number of years added.
        /// </summary>
        /// <remarks>
        /// If the resulting date is invalid, lower fields (typically the day of month) are reduced to find a valid value.
        /// For example, adding one year to February 29th 2012 will return February 28th 2013; subtracting one year from
        /// February 29th 2012 will return February 28th 2011.
        /// </remarks>
        /// <param name="years">The number of years to add</param>
        /// <returns>The current value plus the given number of years.</returns>
        [Pure]
        public LocalDateTime PlusYears(int years) => new LocalDateTime(date.PlusYears(years), time);

        /// <summary>
        /// Returns a new LocalDateTime representing the current value with the given number of months added.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method does not try to maintain the year of the current value, so adding four months to a value in 
        /// October will result in a value in the following February.
        /// </para>
        /// <para>
        /// If the resulting date is invalid, the day of month is reduced to find a valid value.
        /// For example, adding one month to January 30th 2011 will return February 28th 2011; subtracting one month from
        /// March 30th 2011 will return February 28th 2011.
        /// </para>
        /// </remarks>
        /// <param name="months">The number of months to add</param>
        /// <returns>The current value plus the given number of months.</returns>
        [Pure]
        public LocalDateTime PlusMonths(int months) => new LocalDateTime(date.PlusMonths(months), time);

        /// <summary>
        /// Returns a new LocalDateTime representing the current value with the given number of days added.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method does not try to maintain the month or year of the current value, so adding 3 days to a value on January 30th
        /// will result in a value on February 2nd.
        /// </para>
        /// </remarks>
        /// <param name="days">The number of days to add</param>
        /// <returns>The current value plus the given number of days.</returns>
        [Pure]
        public LocalDateTime PlusDays(int days) => new LocalDateTime(date.PlusDays(days), time);

        /// <summary>
        /// Returns a new LocalDateTime representing the current value with the given number of weeks added.
        /// </summary>
        /// <param name="weeks">The number of weeks to add</param>
        /// <returns>The current value plus the given number of weeks.</returns>
        [Pure]
        public LocalDateTime PlusWeeks(int weeks) => new LocalDateTime(date.PlusWeeks(weeks), time);

        /// <summary>
        /// Returns a new LocalDateTime representing the current value with the given number of hours added.
        /// </summary>
        /// <param name="hours">The number of hours to add</param>
        /// <returns>The current value plus the given number of hours.</returns>
        [Pure]
        public LocalDateTime PlusHours(long hours) => TimePeriodField.Hours.Add(this, hours);

        /// <summary>
        /// Returns a new LocalDateTime representing the current value with the given number of minutes added.
        /// </summary>
        /// <param name="minutes">The number of minutes to add</param>
        /// <returns>The current value plus the given number of minutes.</returns>
        [Pure]
        public LocalDateTime PlusMinutes(long minutes) => TimePeriodField.Minutes.Add(this, minutes);

        /// <summary>
        /// Returns a new LocalDateTime representing the current value with the given number of seconds added.
        /// </summary>
        /// <param name="seconds">The number of seconds to add</param>
        /// <returns>The current value plus the given number of seconds.</returns>
        [Pure]
        public LocalDateTime PlusSeconds(long seconds) => TimePeriodField.Seconds.Add(this, seconds);

        /// <summary>
        /// Returns a new LocalDateTime representing the current value with the given number of milliseconds added.
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds to add</param>
        /// <returns>The current value plus the given number of milliseconds.</returns>
        [Pure]
        public LocalDateTime PlusMilliseconds(long milliseconds) =>
            TimePeriodField.Milliseconds.Add(this, milliseconds);

        /// <summary>
        /// Returns a new LocalDateTime representing the current value with the given number of ticks added.
        /// </summary>
        /// <param name="ticks">The number of ticks to add</param>
        /// <returns>The current value plus the given number of ticks.</returns>
        [Pure]
        public LocalDateTime PlusTicks(long ticks) => TimePeriodField.Ticks.Add(this, ticks);

        /// <summary>
        /// Returns a new LocalDateTime representing the current value with the given number of nanoseconds added.
        /// </summary>
        /// <param name="nanoseconds">The number of nanoseconds to add</param>
        /// <returns>The current value plus the given number of nanoseconds.</returns>
        [Pure]
        public LocalDateTime PlusNanoseconds(long nanoseconds) => TimePeriodField.Nanoseconds.Add(this, nanoseconds);

        /// <summary>
        /// Returns the next <see cref="LocalDateTime" /> falling on the specified <see cref="IsoDayOfWeek"/>,
        /// at the same time of day as this value.
        /// This is a strict "next" - if this value on already falls on the target
        /// day of the week, the returned value will be a week later.
        /// </summary>
        /// <param name="targetDayOfWeek">The ISO day of the week to return the next date of.</param>
        /// <returns>The next <see cref="LocalDateTime"/> falling on the specified day of the week.</returns>
        /// <exception cref="InvalidOperationException">The underlying calendar doesn't use ISO days of the week.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="targetDayOfWeek"/> is not a valid day of the
        /// week (Monday to Sunday).</exception>
        [Pure]
        public LocalDateTime Next(IsoDayOfWeek targetDayOfWeek) => new LocalDateTime(date.Next(targetDayOfWeek), time);

        /// <summary>
        /// Returns the previous <see cref="LocalDateTime" /> falling on the specified <see cref="IsoDayOfWeek"/>,
        /// at the same time of day as this value.
        /// This is a strict "previous" - if this value on already falls on the target
        /// day of the week, the returned value will be a week earlier.
        /// </summary>
        /// <param name="targetDayOfWeek">The ISO day of the week to return the previous date of.</param>
        /// <returns>The previous <see cref="LocalDateTime"/> falling on the specified day of the week.</returns>
        /// <exception cref="InvalidOperationException">The underlying calendar doesn't use ISO days of the week.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="targetDayOfWeek"/> is not a valid day of the
        /// week (Monday to Sunday).</exception>
        [Pure]
        public LocalDateTime Previous(IsoDayOfWeek targetDayOfWeek) => new LocalDateTime(date.Previous(targetDayOfWeek), time);

        /// <summary>
        /// Returns an <see cref="OffsetDateTime"/> for this local date/time with the given offset.
        /// </summary>
        /// <remarks>This method is purely a convenient alternative to calling the <see cref="OffsetDateTime"/> constructor directly.</remarks>
        /// <param name="offset">The offset to apply.</param>
        /// <returns>The result of this local date/time offset by the given amount.</returns>
        [Pure]
        public OffsetDateTime WithOffset(Offset offset) => new OffsetDateTime(date.YearMonthDayCalendar, time, offset);

        /// <summary>
        /// Returns the mapping of this local date/time within <see cref="DateTimeZone.Utc"/>.
        /// </summary>
        /// <remarks>As UTC is a fixed time zone, there is no chance that this local date/time is ambiguous or skipped.</remarks>
        /// <returns>The result of mapping this local date/time in UTC.</returns>
        [Pure]
        public ZonedDateTime InUtc() =>
            // Use the internal constructors to avoid validation. We know it will be fine.
            new ZonedDateTime(new OffsetDateTime(date.YearMonthDayCalendar, time.NanosecondOfDay), DateTimeZone.Utc);

        /// <summary>
        /// Returns the mapping of this local date/time within the given <see cref="DateTimeZone" />,
        /// with "strict" rules applied such that an exception is thrown if either the mapping is
        /// ambiguous or the time is skipped.
        /// </summary>
        /// <remarks>
        /// See <see cref="InZoneLeniently"/> and <see cref="InZone"/> for alternative ways to map a local time to a
        /// specific instant.
        /// This is solely a convenience method for calling <see cref="DateTimeZone.AtStrictly" />.
        /// </remarks>
        /// <param name="zone">The time zone in which to map this local date/time.</param>
        /// <exception cref="SkippedTimeException">This local date/time is skipped in the given time zone.</exception>
        /// <exception cref="AmbiguousTimeException">This local date/time is ambiguous in the given time zone.</exception>
        /// <returns>The result of mapping this local date/time in the given time zone.</returns>
        [Pure]
        public ZonedDateTime InZoneStrictly([NotNull] DateTimeZone zone)
        {
            Preconditions.CheckNotNull(zone, nameof(zone));
            return zone.AtStrictly(this);
        }

        /// <summary>
        /// Returns the mapping of this local date/time within the given <see cref="DateTimeZone" />,
        /// with "lenient" rules applied such that ambiguous values map to the earlier of the alternatives, and
        /// "skipped" values are shifted forward by the duration of the "gap".
        /// </summary>
        /// <remarks>
        /// See <see cref="InZoneStrictly"/> and <see cref="InZone"/> for alternative ways to map a local time to a
        /// specific instant.
        /// This is solely a convenience method for calling <see cref="DateTimeZone.AtLeniently" />.
        /// <para>Note: The behavior of this method was changed in version 2.0 to fit the most commonly seen real-world
        /// usage pattern.  Previous versions returned the later instance of ambiguous values, and returned the start of
        /// the zone interval after the gap for skipped value.  The previous functionality can still be used if desired,
        /// by using <see cref="InZone(DateTimeZone, ZoneLocalMappingResolver)"/> and passing a resolver that combines the
        /// <see cref="Resolvers.ReturnLater"/> and <see cref="Resolvers.ReturnStartOfIntervalAfter"/> resolvers.</para>
        /// </remarks>
        /// <param name="zone">The time zone in which to map this local date/time.</param>
        /// <returns>The result of mapping this local date/time in the given time zone.</returns>
        [Pure]
        public ZonedDateTime InZoneLeniently([NotNull] DateTimeZone zone)
        {
            Preconditions.CheckNotNull(zone, nameof(zone));
            return zone.AtLeniently(this);
        }

        /// <summary>
        /// Resolves this local date and time into a <see cref="ZonedDateTime"/> in the given time zone, following
        /// the given <see cref="ZoneLocalMappingResolver"/> to handle ambiguity and skipped times.
        /// </summary>
        /// <remarks>
        /// See <see cref="InZoneStrictly"/> and <see cref="InZoneLeniently"/> for alternative ways to map a local time
        /// to a specific instant.
        /// This is a convenience method for calling <see cref="DateTimeZone.ResolveLocal(LocalDateTime, ZoneLocalMappingResolver)"/>.
        /// </remarks>
        /// <param name="zone">The time zone to map this local date and time into</param>
        /// <param name="resolver">The resolver to apply to the mapping.</param>
        /// <returns>The result of resolving the mapping.</returns>
        [Pure]
        public ZonedDateTime InZone([NotNull] DateTimeZone zone, [NotNull] ZoneLocalMappingResolver resolver)
        {
            Preconditions.CheckNotNull(zone, nameof(zone));
            Preconditions.CheckNotNull(resolver, nameof(resolver));
            return zone.ResolveLocal(this, resolver);
        }

        #region Formatting
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// The value of the current instance in the default format pattern ("G"), using the current thread's
        /// culture to obtain a format provider.
        /// </returns>
        public override string ToString() =>
            LocalDateTimePattern.BclSupport.Format(this, null, CultureInfo.CurrentCulture);

        /// <summary>
        /// Formats the value of the current instance using the specified pattern.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> containing the value of the current instance in the specified format.
        /// </returns>
        /// <param name="patternText">The <see cref="T:System.String" /> specifying the pattern to use,
        /// or null to use the default format pattern ("G").
        /// </param>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider" /> to use when formatting the value,
        /// or null to use the current thread's culture to obtain a format provider.
        /// </param>
        /// <filterpriority>2</filterpriority>
        public string ToString(string patternText, IFormatProvider formatProvider) =>
            LocalDateTimePattern.BclSupport.Format(this, patternText, formatProvider);
        #endregion Formatting

        #region XML serialization
        /// <inheritdoc />
        XmlSchema IXmlSerializable.GetSchema() => null;

        /// <inheritdoc />
        void IXmlSerializable.ReadXml([NotNull] XmlReader reader)
        {
            Preconditions.CheckNotNull(reader, nameof(reader));
            var pattern = LocalDateTimePattern.ExtendedIsoPattern;
            if (reader.MoveToAttribute("calendar"))
            {
                string newCalendarId = reader.Value;
                CalendarSystem newCalendar = CalendarSystem.ForId(newCalendarId);
                var newTemplateValue = pattern.TemplateValue.WithCalendar(newCalendar);
                pattern = pattern.WithTemplateValue(newTemplateValue);
                reader.MoveToElement();
            }
            string text = reader.ReadElementContentAsString();
            this = pattern.Parse(text).Value;
        }

        /// <inheritdoc />
        void IXmlSerializable.WriteXml([NotNull] XmlWriter writer)
        {
            Preconditions.CheckNotNull(writer, nameof(writer));
            if (Calendar != CalendarSystem.Iso)
            {
                writer.WriteAttributeString("calendar", Calendar.Id);
            }
            writer.WriteString(LocalDateTimePattern.ExtendedIsoPattern.Format(this));
        }
        #endregion

#if !PCL
        #region Binary serialization
        private const string DaysSerializationName = "days";
        private const string NanosecondOfDaySerializationName = "nanoOfDay";
        private const string CalendarIdSerializationName = "calendar";

        /// <summary>
        /// Private constructor only present for serialization.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to fetch data from.</param>
        /// <param name="context">The source for this deserialization.</param>
        private LocalDateTime([NotNull] SerializationInfo info, StreamingContext context)
            : this(new LocalDate(Preconditions.CheckNotNull(info, nameof(info)).GetInt32(DaysSerializationName),
                                 CalendarSystem.ForId(info.GetString(CalendarIdSerializationName))),
                   LocalTime.FromNanosecondsSinceMidnight(info.GetInt64(NanosecondOfDaySerializationName)))
        {
        }

        /// <summary>
        /// Implementation of <see cref="ISerializable.GetObjectData"/>.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination for this serialization.</param>
        [System.Security.SecurityCritical]
        void ISerializable.GetObjectData([NotNull] SerializationInfo info, StreamingContext context)
        {
            Preconditions.CheckNotNull(info, nameof(info));
            // FIXME(2.0): Revisit the serialization format. (Use the calendar ordinal? Don't bother for ISO?)
            info.AddValue(DaysSerializationName, date.DaysSinceEpoch);
            info.AddValue(NanosecondOfDaySerializationName, time.NanosecondOfDay);
            info.AddValue(CalendarIdSerializationName, Calendar.Id);
        }
        #endregion
#endif
    }
}
